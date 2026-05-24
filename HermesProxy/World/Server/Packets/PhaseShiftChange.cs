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
		_worldPacket.WritePackedGuid128(Client);
		_worldPacket.WriteUInt32(PhaseShiftFlags);
		_worldPacket.WriteUInt32(0u);
		_worldPacket.WritePackedGuid128(WowGuid128.Empty);
		_worldPacket.WriteUInt32(0u);
		_worldPacket.WriteUInt32(0u);
		_worldPacket.WriteUInt32(0u);
	}
}
