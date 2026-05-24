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
		for (var i = 0; i < 5; i++)
		{
			Rewards.FactionCapIn[i] = 7;
		}
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(QuestGiverGUID);
		_worldPacket.WritePackedGuid128(InformUnit);
		_worldPacket.WriteInt32((int)QuestID);
		_worldPacket.WriteInt32(QuestPackageID);
		_worldPacket.WriteInt32((int)PortraitGiver);
		_worldPacket.WriteUInt32(PortraitGiverMount);
		_worldPacket.WriteUInt32(PortraitGiverModelSceneID);
		_worldPacket.WriteInt32((int)PortraitTurnIn);
		_worldPacket.WriteUInt32(QuestFlags[0]);
		_worldPacket.WriteUInt32(QuestFlags[1]);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteUInt32((QuestFlags.Length > 2) ? QuestFlags[2] : 0u);
		}
		_worldPacket.WriteInt32((int)SuggestedPartyMembers);
		_worldPacket.WriteUInt32((uint)LearnSpells.Count);
		_worldPacket.WriteUInt32((uint)DescEmotes.Length);
		_worldPacket.WriteUInt32((uint)Objectives.Count);
		_worldPacket.WriteInt32(QuestStartItemID);
		_worldPacket.WriteInt32(QuestSessionBonus);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteInt32(QuestGiverCreatureID);
			_worldPacket.WriteUInt32(0u);
		}
		foreach (var spell in LearnSpells)
		{
			_worldPacket.WriteInt32((int)spell);
		}
		var descEmotes = DescEmotes;
		for (var i = 0; i < descEmotes.Length; i++)
		{
			var emote = descEmotes[i];
			_worldPacket.WriteInt32((int)emote.Type);
			_worldPacket.WriteUInt32(emote.Delay);
		}
		foreach (var obj in Objectives)
		{
			_worldPacket.WriteInt32((int)obj.Id);
			_worldPacket.WriteInt32(obj.ObjectID);
			_worldPacket.WriteInt32(obj.Amount);
			_worldPacket.WriteUInt8(obj.Type);
		}
		_worldPacket.WriteBits(QuestTitle.GetByteCount(), 9);
		_worldPacket.WriteBits(DescriptionText.GetByteCount(), 12);
		_worldPacket.WriteBits(LogDescription.GetByteCount(), 12);
		_worldPacket.WriteBits(PortraitGiverText.GetByteCount(), 10);
		_worldPacket.WriteBits(PortraitGiverName.GetByteCount(), 8);
		_worldPacket.WriteBits(PortraitTurnInText.GetByteCount(), 10);
		_worldPacket.WriteBits(PortraitTurnInName.GetByteCount(), 8);
		_worldPacket.WriteBit(AutoLaunched);
		_worldPacket.WriteBit(bit: false);
		_worldPacket.WriteBit(StartCheat);
		_worldPacket.WriteBit(DisplayPopup);
		_worldPacket.FlushBits();
		Rewards.Write(_worldPacket);
		_worldPacket.WriteString(QuestTitle);
		_worldPacket.WriteString(DescriptionText);
		_worldPacket.WriteString(LogDescription);
		_worldPacket.WriteString(PortraitGiverText);
		_worldPacket.WriteString(PortraitGiverName);
		_worldPacket.WriteString(PortraitTurnInText);
		_worldPacket.WriteString(PortraitTurnInName);
	}
}
