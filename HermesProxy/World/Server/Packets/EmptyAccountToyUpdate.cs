using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyAccountToyUpdate : ServerPacket
{
	public EmptyAccountToyUpdate()
		: base(Opcode.SMSG_ACCOUNT_TOY_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(bit: true);
		_worldPacket.FlushBits();
		_worldPacket.WriteInt32(0);
		_worldPacket.WriteInt32(0);
		_worldPacket.WriteInt32(0);
	}
}
