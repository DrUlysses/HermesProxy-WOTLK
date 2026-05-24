using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class InstanceReset : ServerPacket
{
	public uint MapID;

	public InstanceReset()
		: base(Opcode.SMSG_INSTANCE_RESET)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(MapID);
	}
}
