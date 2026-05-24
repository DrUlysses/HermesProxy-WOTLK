using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptySpellHistory : ServerPacket
{
	public EmptySpellHistory()
		: base(Opcode.SMSG_SEND_SPELL_HISTORY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(0);
	}
}
