using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SupercededSpells : ServerPacket
{
	public List<uint> SpellID = new List<uint>();

	public List<uint> Superceded = new List<uint>();

	public List<int> FavoriteSpellID = new List<int>();

	public SupercededSpells()
		: base(Opcode.SMSG_SUPERCEDED_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(SpellID.Count);
		_worldPacket.WriteInt32(Superceded.Count);
		_worldPacket.WriteInt32(FavoriteSpellID.Count);
		foreach (uint spellId in SpellID)
		{
			_worldPacket.WriteUInt32(spellId);
		}
		foreach (uint spellId2 in Superceded)
		{
			_worldPacket.WriteUInt32(spellId2);
		}
		foreach (int spellId3 in FavoriteSpellID)
		{
			_worldPacket.WriteInt32(spellId3);
		}
	}
}
