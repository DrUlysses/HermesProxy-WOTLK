using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellStart : ServerPacket
{
	public SpellCastData Cast;

	public SpellStart()
		: base(Opcode.SMSG_SPELL_START, ConnectionType.Instance)
	{
		Cast = new SpellCastData();
	}

	protected override void Write()
	{
		Cast.Write(_worldPacket);
	}
}
