using System;
using Framework.Constants;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

public class QueryQuestInfoResponse : ServerPacket
{
	public bool Allow;

	public QuestTemplate Info;

	public uint QuestID;

	public QueryQuestInfoResponse()
		: base(Opcode.SMSG_QUERY_QUEST_INFO_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(QuestID);
		_worldPacket.WriteBit(Allow);
		_worldPacket.FlushBits();
		if (!Allow)
		{
			return;
		}
		_worldPacket.WriteUInt32(Info.QuestID);
		_worldPacket.WriteInt32(Info.QuestType);
		_worldPacket.WriteInt32(Info.QuestLevel);
		_worldPacket.WriteInt32(Info.QuestScalingFactionGroup);
		_worldPacket.WriteInt32(Info.QuestMaxScalingLevel);
		_worldPacket.WriteUInt32(Info.QuestPackageID);
		_worldPacket.WriteInt32(Info.MinLevel);
		_worldPacket.WriteInt32(Info.QuestSortID);
		_worldPacket.WriteUInt32(Info.QuestInfoID);
		_worldPacket.WriteUInt32(Info.SuggestedGroupNum);
		_worldPacket.WriteUInt32(Info.RewardNextQuest);
		_worldPacket.WriteUInt32(Info.RewardXPDifficulty);
		_worldPacket.WriteFloat(Info.RewardXPMultiplier);
		_worldPacket.WriteInt32(Info.RewardMoney);
		_worldPacket.WriteUInt32(Info.RewardMoneyDifficulty);
		_worldPacket.WriteFloat(Info.RewardMoneyMultiplier);
		_worldPacket.WriteUInt32(Info.RewardBonusMoney);
		for (var i = 0u; i < 3; i++)
		{
			_worldPacket.WriteUInt32(Info.RewardDisplaySpell[i]);
		}
		_worldPacket.WriteUInt32(Info.RewardSpell);
		_worldPacket.WriteUInt32(Info.RewardHonor);
		_worldPacket.WriteFloat(Info.RewardKillHonor);
		_worldPacket.WriteInt32(Info.RewardArtifactXPDifficulty);
		_worldPacket.WriteFloat(Info.RewardArtifactXPMultiplier);
		_worldPacket.WriteInt32(Info.RewardArtifactCategoryID);
		_worldPacket.WriteUInt32(Info.StartItem);
		_worldPacket.WriteUInt32(Info.Flags);
		_worldPacket.WriteUInt32(Info.FlagsEx);
		_worldPacket.WriteUInt32(Info.FlagsEx2);
		for (var i2 = 0u; i2 < 4; i2++)
		{
			_worldPacket.WriteUInt32(Info.RewardItems[i2]);
			_worldPacket.WriteUInt32(Info.RewardAmount[i2]);
			_worldPacket.WriteInt32(Info.ItemDrop[i2]);
			_worldPacket.WriteInt32(Info.ItemDropQuantity[i2]);
		}
		for (var i3 = 0u; i3 < 6; i3++)
		{
			_worldPacket.WriteUInt32(Info.UnfilteredChoiceItems[i3].ItemID);
			_worldPacket.WriteUInt32(Info.UnfilteredChoiceItems[i3].Quantity);
			_worldPacket.WriteUInt32(Info.UnfilteredChoiceItems[i3].DisplayID);
		}
		_worldPacket.WriteUInt32(Info.POIContinent);
		_worldPacket.WriteFloat(Info.POIx);
		_worldPacket.WriteFloat(Info.POIy);
		_worldPacket.WriteUInt32(Info.POIPriority);
		_worldPacket.WriteUInt32(Info.RewardTitle);
		_worldPacket.WriteInt32(Info.RewardArenaPoints);
		_worldPacket.WriteUInt32(Info.RewardSkillLineID);
		_worldPacket.WriteUInt32(Info.RewardNumSkillUps);
		_worldPacket.WriteInt32((int)Info.PortraitGiver);
		_worldPacket.WriteInt32((int)Info.PortraitGiverMount);
		_worldPacket.WriteInt32((int)Info.PortraitGiverModelSceneID);
		_worldPacket.WriteInt32((int)Info.PortraitTurnIn);
		for (var i4 = 0u; i4 < 5; i4++)
		{
			_worldPacket.WriteUInt32(Info.RewardFactionID[i4]);
			_worldPacket.WriteInt32(Info.RewardFactionValue[i4]);
			_worldPacket.WriteInt32(Info.RewardFactionOverride[i4]);
			_worldPacket.WriteInt32(Info.RewardFactionCapIn[i4]);
		}
		_worldPacket.WriteUInt32(Info.RewardFactionFlags);
		for (var i5 = 0u; i5 < 4; i5++)
		{
			_worldPacket.WriteUInt32(Info.RewardCurrencyID[i5]);
			_worldPacket.WriteUInt32(Info.RewardCurrencyQty[i5]);
		}
		_worldPacket.WriteUInt32(Info.AcceptedSoundKitID);
		_worldPacket.WriteUInt32(Info.CompleteSoundKitID);
		_worldPacket.WriteInt32((int)Info.AreaGroupID);
		_worldPacket.WriteInt64(Info.TimeAllowed);
		_worldPacket.WriteInt32(Info.Objectives.Count);
		_worldPacket.WriteUInt64((ulong)Info.AllowableRaces);
		_worldPacket.WriteInt32(Info.TreasurePickerID);
		_worldPacket.WriteInt32(Info.Expansion);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteInt32(Info.ManagedWorldStateID);
			_worldPacket.WriteInt32(Info.QuestSessionBonus);
			_worldPacket.WriteInt32(Info.QuestGiverCreatureID);
		}
		_worldPacket.WriteBits(Info.LogTitle.GetByteCount(), 9);
		_worldPacket.WriteBits(Info.LogDescription.GetByteCount(), 12);
		_worldPacket.WriteBits(Info.QuestDescription.GetByteCount(), 12);
		_worldPacket.WriteBits(Info.AreaDescription.GetByteCount(), 9);
		_worldPacket.WriteBits(Info.PortraitGiverText.GetByteCount(), 10);
		_worldPacket.WriteBits(Info.PortraitGiverName.GetByteCount(), 8);
		_worldPacket.WriteBits(Info.PortraitTurnInText.GetByteCount(), 10);
		_worldPacket.WriteBits(Info.PortraitTurnInName.GetByteCount(), 8);
		_worldPacket.WriteBits(Info.QuestCompletionLog.GetByteCount(), 11);
		_worldPacket.WriteBit(Info.ReadyForTranslation);
		_worldPacket.FlushBits();
		foreach (var questObjective in Info.Objectives)
		{
			_worldPacket.WriteUInt32(questObjective.Id);
			_worldPacket.WriteUInt8((byte)questObjective.Type);
			_worldPacket.WriteInt8(questObjective.StorageIndex);
			_worldPacket.WriteInt32(questObjective.ObjectID);
			_worldPacket.WriteInt32(questObjective.Amount);
			_worldPacket.WriteUInt32((uint)questObjective.Flags);
			_worldPacket.WriteUInt32(questObjective.Flags2);
			_worldPacket.WriteFloat(questObjective.ProgressBarWeight);
			_worldPacket.WriteInt32(questObjective.VisualEffects.Length);
			var visualEffects = questObjective.VisualEffects;
			foreach (var visualEffect in visualEffects)
			{
				_worldPacket.WriteInt32(visualEffect);
			}
			_worldPacket.WriteBits(questObjective.Description.GetByteCount(), 8);
			_worldPacket.FlushBits();
			_worldPacket.WriteString(questObjective.Description);
		}
		_worldPacket.WriteString(Info.LogTitle);
		_worldPacket.WriteString(Info.LogDescription);
		_worldPacket.WriteString(Info.QuestDescription);
		_worldPacket.WriteString(Info.AreaDescription);
		_worldPacket.WriteString(Info.PortraitGiverText);
		_worldPacket.WriteString(Info.PortraitGiverName);
		_worldPacket.WriteString(Info.PortraitTurnInText);
		_worldPacket.WriteString(Info.PortraitTurnInName);
		_worldPacket.WriteString(Info.QuestCompletionLog);
	}
}
