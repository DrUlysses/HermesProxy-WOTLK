using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverRequestItems : ServerPacket
{
	public WowGuid128 QuestGiverGUID;

	public uint QuestGiverCreatureID;

	public uint QuestID;

	public uint CompEmoteDelay;

	public uint CompEmoteType;

	public bool AutoLaunched;

	public uint SuggestPartyMembers;

	public int MoneyToGet;

	public readonly List<QuestObjectiveCollect> Collect = new();

	public readonly List<QuestCurrency> Currency = new();

	public uint StatusFlags;

	public readonly uint[] QuestFlags = new uint[3];

	public string QuestTitle = "";

	public string CompletionText = "";

	public QuestGiverRequestItems()
		: base(Opcode.SMSG_QUEST_GIVER_REQUEST_ITEMS)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(QuestGiverGUID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteInt32((int)QuestGiverCreatureID);
		}
		_worldPacket.WriteInt32((int)QuestID);
		_worldPacket.WriteInt32((int)CompEmoteDelay);
		_worldPacket.WriteInt32((int)CompEmoteType);
		_worldPacket.WriteUInt32(QuestFlags[0]);
		_worldPacket.WriteUInt32(QuestFlags[1]);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteUInt32(QuestFlags.Length > 2 ? QuestFlags[2] : 0u);
		}
		_worldPacket.WriteInt32((int)SuggestPartyMembers);
		_worldPacket.WriteInt32(MoneyToGet);
		_worldPacket.WriteInt32(Collect.Count);
		_worldPacket.WriteInt32(Currency.Count);
		_worldPacket.WriteInt32((int)StatusFlags);
		foreach (var obj in Collect)
		{
			_worldPacket.WriteInt32((int)obj.ObjectID);
			_worldPacket.WriteInt32((int)obj.Amount);
			_worldPacket.WriteUInt32(obj.Flags);
		}
		foreach (var cur in Currency)
		{
			_worldPacket.WriteInt32((int)cur.CurrencyID);
			_worldPacket.WriteInt32(cur.Amount);
		}
		_worldPacket.WriteBit(AutoLaunched);
		_worldPacket.FlushBits();
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteInt32((int)QuestGiverCreatureID);
			_worldPacket.WriteUInt32(0u);
		}
		_worldPacket.WriteBits(QuestTitle.GetByteCount(), 9);
		_worldPacket.WriteBits(CompletionText.GetByteCount(), 12);
		_worldPacket.FlushBits();
		_worldPacket.WriteString(QuestTitle);
		_worldPacket.WriteString(CompletionText);
	}
}
