using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BattlefieldStatusActive : ServerPacket
{
	public BattlefieldStatusHeader Hdr = new BattlefieldStatusHeader();

	public uint Mapid;

	public uint ShutdownTimer;

	public uint StartTimer;

	public byte ArenaFaction;

	public bool LeftEarly;

	public BattlefieldStatusActive()
		: base(Opcode.SMSG_BATTLEFIELD_STATUS_ACTIVE)
	{
	}

	protected override void Write()
	{
		this.Hdr.Write(base._worldPacket);
		base._worldPacket.WriteUInt32(this.Mapid);
		base._worldPacket.WriteUInt32(this.ShutdownTimer);
		base._worldPacket.WriteUInt32(this.StartTimer);
		base._worldPacket.WriteBit(this.ArenaFaction != 0);
		base._worldPacket.WriteBit(this.LeftEarly);
		base._worldPacket.FlushBits();
	}
}
