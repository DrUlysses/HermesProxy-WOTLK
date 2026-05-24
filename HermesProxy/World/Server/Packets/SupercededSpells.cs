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
		base._worldPacket.WriteInt32(this.SpellID.Count);
		base._worldPacket.WriteInt32(this.Superceded.Count);
		base._worldPacket.WriteInt32(this.FavoriteSpellID.Count);
		foreach (uint spellId in this.SpellID)
		{
			base._worldPacket.WriteUInt32(spellId);
		}
		foreach (uint spellId2 in this.Superceded)
		{
			base._worldPacket.WriteUInt32(spellId2);
		}
		foreach (int spellId3 in this.FavoriteSpellID)
		{
			base._worldPacket.WriteInt32(spellId3);
		}
	}
}
