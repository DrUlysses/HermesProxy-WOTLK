using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BattlefieldStatusQueued : ServerPacket
{
	public BattlefieldStatusHeader Hdr = new BattlefieldStatusHeader();

	public uint AverageWaitTime;

	public uint WaitTime;

	public int Unk254;

	public bool AsGroup;

	public bool EligibleForMatchmaking = true;

	public bool SuspendedQueue;

	public BattlefieldStatusQueued()
		: base(Opcode.SMSG_BATTLEFIELD_STATUS_QUEUED)
	{
	}

	protected override void Write()
	{
		this.Hdr.Write(base._worldPacket);
		base._worldPacket.WriteUInt32(this.AverageWaitTime);
		base._worldPacket.WriteUInt32(this.WaitTime);
		if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 3, 2, 5, 4))
		{
			base._worldPacket.WriteInt32(this.Unk254);
		}
		base._worldPacket.WriteBit(this.AsGroup);
		base._worldPacket.WriteBit(this.EligibleForMatchmaking);
		base._worldPacket.WriteBit(this.SuspendedQueue);
		base._worldPacket.FlushBits();
	}
}
