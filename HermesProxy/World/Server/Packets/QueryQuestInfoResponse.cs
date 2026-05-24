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
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.FlushBits();
		if (!this.Allow)
		{
			return;
		}
		base._worldPacket.WriteUInt32(this.Info.QuestID);
		base._worldPacket.WriteInt32(this.Info.QuestType);
		base._worldPacket.WriteInt32(this.Info.QuestLevel);
		base._worldPacket.WriteInt32(this.Info.QuestScalingFactionGroup);
		base._worldPacket.WriteInt32(this.Info.QuestMaxScalingLevel);
		base._worldPacket.WriteUInt32(this.Info.QuestPackageID);
		base._worldPacket.WriteInt32(this.Info.MinLevel);
		base._worldPacket.WriteInt32(this.Info.QuestSortID);
		base._worldPacket.WriteUInt32(this.Info.QuestInfoID);
		base._worldPacket.WriteUInt32(this.Info.SuggestedGroupNum);
		base._worldPacket.WriteUInt32(this.Info.RewardNextQuest);
		base._worldPacket.WriteUInt32(this.Info.RewardXPDifficulty);
		base._worldPacket.WriteFloat(this.Info.RewardXPMultiplier);
		base._worldPacket.WriteInt32(this.Info.RewardMoney);
		base._worldPacket.WriteUInt32(this.Info.RewardMoneyDifficulty);
		base._worldPacket.WriteFloat(this.Info.RewardMoneyMultiplier);
		base._worldPacket.WriteUInt32(this.Info.RewardBonusMoney);
		for (uint i = 0u; i < 3; i++)
		{
			base._worldPacket.WriteUInt32(this.Info.RewardDisplaySpell[i]);
		}
		base._worldPacket.WriteUInt32(this.Info.RewardSpell);
		base._worldPacket.WriteUInt32(this.Info.RewardHonor);
		base._worldPacket.WriteFloat(this.Info.RewardKillHonor);
		base._worldPacket.WriteInt32(this.Info.RewardArtifactXPDifficulty);
		base._worldPacket.WriteFloat(this.Info.RewardArtifactXPMultiplier);
		base._worldPacket.WriteInt32(this.Info.RewardArtifactCategoryID);
		base._worldPacket.WriteUInt32(this.Info.StartItem);
		base._worldPacket.WriteUInt32(this.Info.Flags);
		base._worldPacket.WriteUInt32(this.Info.FlagsEx);
		base._worldPacket.WriteUInt32(this.Info.FlagsEx2);
		for (uint i2 = 0u; i2 < 4; i2++)
		{
			base._worldPacket.WriteUInt32(this.Info.RewardItems[i2]);
			base._worldPacket.WriteUInt32(this.Info.RewardAmount[i2]);
			base._worldPacket.WriteInt32(this.Info.ItemDrop[i2]);
			base._worldPacket.WriteInt32(this.Info.ItemDropQuantity[i2]);
		}
		for (uint i3 = 0u; i3 < 6; i3++)
		{
			base._worldPacket.WriteUInt32(this.Info.UnfilteredChoiceItems[i3].ItemID);
			base._worldPacket.WriteUInt32(this.Info.UnfilteredChoiceItems[i3].Quantity);
			base._worldPacket.WriteUInt32(this.Info.UnfilteredChoiceItems[i3].DisplayID);
		}
		base._worldPacket.WriteUInt32(this.Info.POIContinent);
		base._worldPacket.WriteFloat(this.Info.POIx);
		base._worldPacket.WriteFloat(this.Info.POIy);
		base._worldPacket.WriteUInt32(this.Info.POIPriority);
		base._worldPacket.WriteUInt32(this.Info.RewardTitle);
		base._worldPacket.WriteInt32(this.Info.RewardArenaPoints);
		base._worldPacket.WriteUInt32(this.Info.RewardSkillLineID);
		base._worldPacket.WriteUInt32(this.Info.RewardNumSkillUps);
		base._worldPacket.WriteInt32((int)this.Info.PortraitGiver);
		base._worldPacket.WriteInt32((int)this.Info.PortraitGiverMount);
		base._worldPacket.WriteInt32((int)this.Info.PortraitGiverModelSceneID);
		base._worldPacket.WriteInt32((int)this.Info.PortraitTurnIn);
		for (uint i4 = 0u; i4 < 5; i4++)
		{
			base._worldPacket.WriteUInt32(this.Info.RewardFactionID[i4]);
			base._worldPacket.WriteInt32(this.Info.RewardFactionValue[i4]);
			base._worldPacket.WriteInt32(this.Info.RewardFactionOverride[i4]);
			base._worldPacket.WriteInt32(this.Info.RewardFactionCapIn[i4]);
		}
		base._worldPacket.WriteUInt32(this.Info.RewardFactionFlags);
		for (uint i5 = 0u; i5 < 4; i5++)
		{
			base._worldPacket.WriteUInt32(this.Info.RewardCurrencyID[i5]);
			base._worldPacket.WriteUInt32(this.Info.RewardCurrencyQty[i5]);
		}
		base._worldPacket.WriteUInt32(this.Info.AcceptedSoundKitID);
		base._worldPacket.WriteUInt32(this.Info.CompleteSoundKitID);
		base._worldPacket.WriteInt32((int)this.Info.AreaGroupID);
		base._worldPacket.WriteInt64(this.Info.TimeAllowed);
		base._worldPacket.WriteInt32(this.Info.Objectives.Count);
		base._worldPacket.WriteUInt64((ulong)this.Info.AllowableRaces);
		base._worldPacket.WriteInt32(this.Info.TreasurePickerID);
		base._worldPacket.WriteInt32(this.Info.Expansion);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteInt32(this.Info.ManagedWorldStateID);
			base._worldPacket.WriteInt32(this.Info.QuestSessionBonus);
			base._worldPacket.WriteInt32(this.Info.QuestGiverCreatureID);
		}
		base._worldPacket.WriteBits(this.Info.LogTitle.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.Info.LogDescription.GetByteCount(), 12);
		base._worldPacket.WriteBits(this.Info.QuestDescription.GetByteCount(), 12);
		base._worldPacket.WriteBits(this.Info.AreaDescription.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.Info.PortraitGiverText.GetByteCount(), 10);
		base._worldPacket.WriteBits(this.Info.PortraitGiverName.GetByteCount(), 8);
		base._worldPacket.WriteBits(this.Info.PortraitTurnInText.GetByteCount(), 10);
		base._worldPacket.WriteBits(this.Info.PortraitTurnInName.GetByteCount(), 8);
		base._worldPacket.WriteBits(this.Info.QuestCompletionLog.GetByteCount(), 11);
		base._worldPacket.WriteBit(this.Info.ReadyForTranslation);
		base._worldPacket.FlushBits();
		foreach (QuestObjective questObjective in this.Info.Objectives)
		{
			base._worldPacket.WriteUInt32(questObjective.Id);
			base._worldPacket.WriteUInt8((byte)questObjective.Type);
			base._worldPacket.WriteInt8(questObjective.StorageIndex);
			base._worldPacket.WriteInt32(questObjective.ObjectID);
			base._worldPacket.WriteInt32(questObjective.Amount);
			base._worldPacket.WriteUInt32((uint)questObjective.Flags);
			base._worldPacket.WriteUInt32(questObjective.Flags2);
			base._worldPacket.WriteFloat(questObjective.ProgressBarWeight);
			base._worldPacket.WriteInt32(questObjective.VisualEffects.Length);
			int[] visualEffects = questObjective.VisualEffects;
			foreach (int visualEffect in visualEffects)
			{
				base._worldPacket.WriteInt32(visualEffect);
			}
			base._worldPacket.WriteBits(questObjective.Description.GetByteCount(), 8);
			base._worldPacket.FlushBits();
			base._worldPacket.WriteString(questObjective.Description);
		}
		base._worldPacket.WriteString(this.Info.LogTitle);
		base._worldPacket.WriteString(this.Info.LogDescription);
		base._worldPacket.WriteString(this.Info.QuestDescription);
		base._worldPacket.WriteString(this.Info.AreaDescription);
		base._worldPacket.WriteString(this.Info.PortraitGiverText);
		base._worldPacket.WriteString(this.Info.PortraitGiverName);
		base._worldPacket.WriteString(this.Info.PortraitTurnInText);
		base._worldPacket.WriteString(this.Info.PortraitTurnInName);
		base._worldPacket.WriteString(this.Info.QuestCompletionLog);
	}
}
