using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyAllAccountCriteria : ServerPacket
{
	public EmptyAllAccountCriteria()
		: base(Opcode.SMSG_ALL_ACCOUNT_CRITERIA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(0);
	}
}
