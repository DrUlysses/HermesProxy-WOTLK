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
		_worldPacket.WritePackedGuid128(Caster);
		_worldPacket.WriteUInt8(Flags);
		_worldPacket.WriteInt32(SpellCooldowns.Count);
		foreach (var cd in SpellCooldowns)
		{
			cd.Write(_worldPacket);
		}
	}
}
