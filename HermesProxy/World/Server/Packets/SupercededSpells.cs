using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SupercededSpells : ServerPacket
{
	public readonly List<uint> SpellID = new();

	public readonly List<uint> Superceded = new();

	public readonly List<int> FavoriteSpellID = new();

	public SupercededSpells()
		: base(Opcode.SMSG_SUPERCEDED_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(SpellID.Count);
		_worldPacket.WriteInt32(Superceded.Count);
		_worldPacket.WriteInt32(FavoriteSpellID.Count);
		foreach (var spellId in SpellID)
		{
			_worldPacket.WriteUInt32(spellId);
		}
		foreach (var spellId2 in Superceded)
		{
			_worldPacket.WriteUInt32(spellId2);
		}
		foreach (var spellId3 in FavoriteSpellID)
		{
			_worldPacket.WriteInt32(spellId3);
		}
	}
}
