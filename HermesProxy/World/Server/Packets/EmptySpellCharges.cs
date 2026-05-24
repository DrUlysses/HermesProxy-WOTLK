using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptySpellCharges : ServerPacket
{
	public EmptySpellCharges()
		: base(Opcode.SMSG_SEND_SPELL_CHARGES, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(0);
	}
}
