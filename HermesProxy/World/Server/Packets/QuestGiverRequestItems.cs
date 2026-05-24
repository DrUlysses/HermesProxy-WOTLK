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

	public List<QuestObjectiveCollect> Collect = new List<QuestObjectiveCollect>();

	public List<QuestCurrency> Currency = new List<QuestCurrency>();

	public uint StatusFlags;

	public uint[] QuestFlags = new uint[3];

	public string QuestTitle = "";

	public string CompletionText = "";

	public QuestGiverRequestItems()
		: base(Opcode.SMSG_QUEST_GIVER_REQUEST_ITEMS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.QuestGiverGUID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteInt32((int)this.QuestGiverCreatureID);
		}
		base._worldPacket.WriteInt32((int)this.QuestID);
		base._worldPacket.WriteInt32((int)this.CompEmoteDelay);
		base._worldPacket.WriteInt32((int)this.CompEmoteType);
		base._worldPacket.WriteUInt32(this.QuestFlags[0]);
		base._worldPacket.WriteUInt32(this.QuestFlags[1]);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteUInt32((this.QuestFlags.Length > 2) ? this.QuestFlags[2] : 0u);
		}
		base._worldPacket.WriteInt32((int)this.SuggestPartyMembers);
		base._worldPacket.WriteInt32(this.MoneyToGet);
		base._worldPacket.WriteInt32(this.Collect.Count);
		base._worldPacket.WriteInt32(this.Currency.Count);
		base._worldPacket.WriteInt32((int)this.StatusFlags);
		foreach (QuestObjectiveCollect obj in this.Collect)
		{
			base._worldPacket.WriteInt32((int)obj.ObjectID);
			base._worldPacket.WriteInt32((int)obj.Amount);
			base._worldPacket.WriteUInt32(obj.Flags);
		}
		foreach (QuestCurrency cur in this.Currency)
		{
			base._worldPacket.WriteInt32((int)cur.CurrencyID);
			base._worldPacket.WriteInt32(cur.Amount);
		}
		base._worldPacket.WriteBit(this.AutoLaunched);
		base._worldPacket.FlushBits();
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteInt32((int)this.QuestGiverCreatureID);
			base._worldPacket.WriteUInt32(0u);
		}
		base._worldPacket.WriteBits(this.QuestTitle.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.CompletionText.GetByteCount(), 12);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.QuestTitle);
		base._worldPacket.WriteString(this.CompletionText);
	}
}
