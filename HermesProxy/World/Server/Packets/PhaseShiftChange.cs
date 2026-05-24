using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PhaseShiftChange : ServerPacket
{
	public WowGuid128 Client;

	public uint PhaseShiftFlags = 8u;

	public PhaseShiftChange()
		: base(Opcode.SMSG_PHASE_SHIFT_CHANGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Client);
		base._worldPacket.WriteUInt32(this.PhaseShiftFlags);
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WritePackedGuid128(WowGuid128.Empty);
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WriteUInt32(0u);
	}
}
