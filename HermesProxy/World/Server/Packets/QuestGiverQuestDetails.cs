using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverQuestDetails : ServerPacket
{
	public WowGuid128 QuestGiverGUID;

	public WowGuid128 InformUnit;

	public uint QuestID;

	public int QuestPackageID;

	public uint[] QuestFlags = new uint[3];

	public uint SuggestedPartyMembers;

	public QuestRewards Rewards = new QuestRewards();

	public List<QuestObjectiveSimple> Objectives = new List<QuestObjectiveSimple>();

	public QuestDescEmote[] DescEmotes = new QuestDescEmote[4];

	public List<uint> LearnSpells = new List<uint>();

	public uint PortraitTurnIn;

	public uint PortraitGiver;

	public uint PortraitGiverMount;

	public uint PortraitGiverModelSceneID;

	public int QuestStartItemID;

	public int QuestSessionBonus;

	public int QuestGiverCreatureID;

	public string PortraitGiverText = "";

	public string PortraitGiverName = "";

	public string PortraitTurnInText = "";

	public string PortraitTurnInName = "";

	public string QuestTitle = "";

	public string DescriptionText = "";

	public string LogDescription = "";

	public bool DisplayPopup;

	public bool StartCheat;

	public bool AutoLaunched;

	public QuestGiverQuestDetails()
		: base(Opcode.SMSG_QUEST_GIVER_QUEST_DETAILS)
	{
		for (int i = 0; i < 5; i++)
		{
			this.Rewards.FactionCapIn[i] = 7;
		}
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.QuestGiverGUID);
		base._worldPacket.WritePackedGuid128(this.InformUnit);
		base._worldPacket.WriteInt32((int)this.QuestID);
		base._worldPacket.WriteInt32(this.QuestPackageID);
		base._worldPacket.WriteInt32((int)this.PortraitGiver);
		base._worldPacket.WriteUInt32(this.PortraitGiverMount);
		base._worldPacket.WriteUInt32(this.PortraitGiverModelSceneID);
		base._worldPacket.WriteInt32((int)this.PortraitTurnIn);
		base._worldPacket.WriteUInt32(this.QuestFlags[0]);
		base._worldPacket.WriteUInt32(this.QuestFlags[1]);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteUInt32((this.QuestFlags.Length > 2) ? this.QuestFlags[2] : 0u);
		}
		base._worldPacket.WriteInt32((int)this.SuggestedPartyMembers);
		base._worldPacket.WriteUInt32((uint)this.LearnSpells.Count);
		base._worldPacket.WriteUInt32((uint)this.DescEmotes.Length);
		base._worldPacket.WriteUInt32((uint)this.Objectives.Count);
		base._worldPacket.WriteInt32(this.QuestStartItemID);
		base._worldPacket.WriteInt32(this.QuestSessionBonus);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteInt32(this.QuestGiverCreatureID);
			base._worldPacket.WriteUInt32(0u);
		}
		foreach (uint spell in this.LearnSpells)
		{
			base._worldPacket.WriteInt32((int)spell);
		}
		QuestDescEmote[] descEmotes = this.DescEmotes;
		for (int i = 0; i < descEmotes.Length; i++)
		{
			QuestDescEmote emote = descEmotes[i];
			base._worldPacket.WriteInt32((int)emote.Type);
			base._worldPacket.WriteUInt32(emote.Delay);
		}
		foreach (QuestObjectiveSimple obj in this.Objectives)
		{
			base._worldPacket.WriteInt32((int)obj.Id);
			base._worldPacket.WriteInt32(obj.ObjectID);
			base._worldPacket.WriteInt32(obj.Amount);
			base._worldPacket.WriteUInt8(obj.Type);
		}
		base._worldPacket.WriteBits(this.QuestTitle.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.DescriptionText.GetByteCount(), 12);
		base._worldPacket.WriteBits(this.LogDescription.GetByteCount(), 12);
		base._worldPacket.WriteBits(this.PortraitGiverText.GetByteCount(), 10);
		base._worldPacket.WriteBits(this.PortraitGiverName.GetByteCount(), 8);
		base._worldPacket.WriteBits(this.PortraitTurnInText.GetByteCount(), 10);
		base._worldPacket.WriteBits(this.PortraitTurnInName.GetByteCount(), 8);
		base._worldPacket.WriteBit(this.AutoLaunched);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(this.StartCheat);
		base._worldPacket.WriteBit(this.DisplayPopup);
		base._worldPacket.FlushBits();
		this.Rewards.Write(base._worldPacket);
		base._worldPacket.WriteString(this.QuestTitle);
		base._worldPacket.WriteString(this.DescriptionText);
		base._worldPacket.WriteString(this.LogDescription);
		base._worldPacket.WriteString(this.PortraitGiverText);
		base._worldPacket.WriteString(this.PortraitGiverName);
		base._worldPacket.WriteString(this.PortraitTurnInText);
		base._worldPacket.WriteString(this.PortraitTurnInName);
	}
}
