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
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteInt32(0);
	}
}
