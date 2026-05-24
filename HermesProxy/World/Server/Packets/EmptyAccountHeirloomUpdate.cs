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
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WriteUInt32(0u);
	}
}
