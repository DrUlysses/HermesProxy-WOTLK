using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SetSpellModifier : ServerPacket
{
	public List<SpellModifierInfo> Modifiers = new List<SpellModifierInfo>();

	public SetSpellModifier(Opcode opcode)
		: base(opcode, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Modifiers.Count);
		foreach (SpellModifierInfo spellMod in this.Modifiers)
		{
			spellMod.Write(base._worldPacket);
		}
	}
}
