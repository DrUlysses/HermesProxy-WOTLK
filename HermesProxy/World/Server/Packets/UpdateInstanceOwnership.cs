using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class UpdateInstanceOwnership : ServerPacket
{
	public uint IOwnInstance;

	public UpdateInstanceOwnership()
		: base(Opcode.SMSG_UPDATE_INSTANCE_OWNERSHIP)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(IOwnInstance);
	}
}
