using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellCooldownPkt : ServerPacket
{
	public List<SpellCooldownStruct> SpellCooldowns = new List<SpellCooldownStruct>();

	public WowGuid128 Caster;

	public byte Flags;

	public SpellCooldownPkt()
		: base(Opcode.SMSG_SPELL_COOLDOWN, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Caster);
		base._worldPacket.WriteUInt8(this.Flags);
		base._worldPacket.WriteInt32(this.SpellCooldowns.Count);
		foreach (SpellCooldownStruct cd in this.SpellCooldowns)
		{
			cd.Write(base._worldPacket);
		}
	}
}
