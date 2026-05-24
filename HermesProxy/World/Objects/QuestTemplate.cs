using System.Collections.Generic;

namespace HermesProxy.World.Objects;

public class QuestTemplate
{
	public uint QuestID;

	public int QuestType;

	public int QuestLevel;

	public int QuestScalingFactionGroup;

	public int QuestMaxScalingLevel;

	public uint QuestPackageID;

	public int MinLevel;

	public int QuestSortID;

	public uint QuestInfoID;

	public uint SuggestedGroupNum;

	public uint RewardNextQuest;

	public uint RewardXPDifficulty;

	public float RewardXPMultiplier = 1f;

	public int RewardMoney;

	public uint RewardMoneyDifficulty;

	public float RewardMoneyMultiplier = 1f;

	public uint RewardBonusMoney;

	public readonly uint[] RewardDisplaySpell = new uint[3];

	public uint RewardSpell;

	public uint RewardHonor;

	public float RewardKillHonor;

	public int RewardArtifactXPDifficulty;

	public float RewardArtifactXPMultiplier;

	public int RewardArtifactCategoryID;

	public uint StartItem;

	public uint Flags;

	public uint FlagsEx;

	public uint FlagsEx2;

	public uint POIContinent;

	public float POIx;

	public float POIy;

	public uint POIPriority;

	public long AllowableRaces = -1L;

	public string LogTitle;

	public string LogDescription;

	public string QuestDescription;

	public string AreaDescription;

	public uint RewardTitle;

	public int RewardArenaPoints;

	public uint RewardSkillLineID;

	public uint RewardNumSkillUps;

	public uint PortraitGiver;

	public uint PortraitGiverMount;

	public uint PortraitTurnIn;

	public readonly string PortraitGiverText;

	public readonly string PortraitGiverName;

	public readonly string PortraitTurnInText;

	public readonly string PortraitTurnInName;

	public string QuestCompletionLog;

	public uint RewardFactionFlags;

	public uint AcceptedSoundKitID;

	public uint CompleteSoundKitID;

	public uint AreaGroupID;

	public uint TimeAllowed;

	public int TreasurePickerID;

	public int Expansion;

	public int ManagedWorldStateID;

	public int QuestSessionBonus;

	public int QuestGiverCreatureID;

	public uint PortraitGiverModelSceneID;

	public readonly List<QuestObjective> Objectives = new();

	public readonly uint[] RewardItems = new uint[4];

	public readonly uint[] RewardAmount = new uint[4];

	public readonly int[] ItemDrop = new int[4];

	public readonly int[] ItemDropQuantity = new int[4];

	public readonly QuestInfoChoiceItem[] UnfilteredChoiceItems = new QuestInfoChoiceItem[6];

	public readonly uint[] RewardFactionID = new uint[5];

	public readonly int[] RewardFactionValue = new int[5];

	public readonly int[] RewardFactionOverride = new int[5];

	public readonly int[] RewardFactionCapIn = new int[5];

	public readonly uint[] RewardCurrencyID = new uint[4];

	public readonly uint[] RewardCurrencyQty = new uint[4];

	public bool ReadyForTranslation;

	public QuestTemplate()
	{
		LogTitle = "";
		LogDescription = "";
		QuestDescription = "";
		AreaDescription = "";
		PortraitGiverText = "";
		PortraitGiverName = "";
		PortraitTurnInText = "";
		PortraitTurnInName = "";
		QuestCompletionLog = "";
	}
}
