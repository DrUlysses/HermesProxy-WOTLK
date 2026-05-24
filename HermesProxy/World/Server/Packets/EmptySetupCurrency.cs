using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptySetupCurrency : ServerPacket
{
	public EmptySetupCurrency()
		: base(Opcode.SMSG_SETUP_CURRENCY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(0);
	}
}
