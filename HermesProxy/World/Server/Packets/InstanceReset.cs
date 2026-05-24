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
		base._worldPacket.WriteUInt32(this.MapID);
	}
}
