using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyAccountHeirloomUpdate : ServerPacket
{
	public EmptyAccountHeirloomUpdate()
		: base(Opcode.SMSG_ACCOUNT_HEIRLOOM_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(bit: true);
		_worldPacket.FlushBits();
		_worldPacket.WriteInt32(0);
		_worldPacket.WriteUInt32(0u);
		_worldPacket.WriteUInt32(0u);
	}
}
