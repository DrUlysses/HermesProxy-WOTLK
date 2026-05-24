using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LearnedSpellInfo
{
	public int SpellID;
	public bool IsFavorite;
	public int? Superceded;
}

public class LearnedSpells : ServerPacket
{
	public readonly List<LearnedSpellInfo> ClientLearnedSpellData = new();

	public uint SpecializationID;

	public bool SuppressMessaging;

	public LearnedSpells()
		: base(Opcode.SMSG_LEARNED_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(ClientLearnedSpellData.Count);
		_worldPacket.WriteUInt32(SpecializationID);
		_worldPacket.WriteBit(SuppressMessaging);
		_worldPacket.FlushBits();
		foreach (var info in ClientLearnedSpellData)
		{
			_worldPacket.WriteInt32(info.SpellID);
			_worldPacket.WriteBit(info.IsFavorite);
			_worldPacket.WriteBit(false); // field_8
			_worldPacket.WriteBit(info.Superceded.HasValue); // Superceded
			_worldPacket.WriteBit(false); // TraitDefinitionID
			_worldPacket.FlushBits();
			if (info.Superceded.HasValue)
				_worldPacket.WriteInt32(info.Superceded.Value);
		}
	}
}
