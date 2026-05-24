using System.Collections.Generic;

namespace HermesProxy.World.Objects;

public class ActivePlayerData
{
	public readonly WowGuid128[] InvSlots = new WowGuid128[23];

	public readonly WowGuid128[] PackSlots = new WowGuid128[24];

	public readonly WowGuid128[] BankSlots = new WowGuid128[28];

	public readonly WowGuid128[] BankBagSlots = new WowGuid128[7];

	public readonly WowGuid128[] BuyBackSlots = new WowGuid128[12];

	public readonly WowGuid128[] KeyringSlots = new WowGuid128[32];

	public WowGuid128 FarsightObject;

	public WowGuid128 ComboTarget;

	public WowGuid128 SummonedBattlePetGUID;

	public readonly uint?[] KnownTitles = new uint?[12];

	public ulong? Coinage;

	public int? XP;

	public int? NextLevelXP;

	public int? TrialXP;

	public readonly SkillInfo Skill = new();

	public int? CharacterPoints;

	public int? MaxTalentTiers;

	public uint? TrackCreatureMask;

	public readonly uint?[] TrackResourceMask = new uint?[2];

	public float? MainhandExpertise;

	public float? OffhandExpertise;

	public float? RangedExpertise;

	public float? CombatRatingExpertise;

	public float? BlockPercentage;

	public float? DodgePercentage;

	public float? DodgePercentageFromAttribute;

	public float? ParryPercentage;

	public float? ParryPercentageFromAttribute;

	public float? CritPercentage;

	public float? RangedCritPercentage;

	public float? OffhandCritPercentage;

	public readonly float?[] SpellCritPercentage = new float?[7];

	public int? ShieldBlock;

	public float? Mastery;

	public float? Speed;

	public float? Avoidance;

	public float? Sturdiness;

	public int? Versatility;

	public float? VersatilityBonus;

	public float? PvpPowerDamage;

	public float? PvpPowerHealing;

	public readonly ulong?[] ExploredZones = new ulong?[240];

	public readonly RestInfo[] RestInfo = new RestInfo[2];

	public readonly int?[] ModDamageDonePos = new int?[7];

	public readonly int?[] ModDamageDoneNeg = new int?[7];

	public readonly float?[] ModDamageDonePercent = new float?[7];

	public int? ModHealingDonePos;

	public float? ModHealingPercent;

	public float? ModHealingDonePercent;

	public float? ModPeriodicHealingDonePercent;

	public readonly float?[] WeaponDmgMultipliers = new float?[3];

	public readonly float?[] WeaponAtkSpeedMultipliers = new float?[3];

	public float? ModSpellPowerPercent;

	public float? ModResiliencePercent;

	public float? OverrideSpellPowerByAPPercent;

	public float? OverrideAPBySpellPowerPercent;

	public int? ModTargetResistance;

	public int? ModTargetPhysicalResistance;

	public uint? LocalFlags;

	public byte? GrantableLevels;

	public byte? MultiActionBars;

	public byte? LifetimeMaxRank;

	public byte? NumRespecs;

	public uint? AmmoID;

	public uint? PvpMedals;

	public readonly uint?[] BuybackPrice = new uint?[12];

	public readonly uint?[] BuybackTimestamp = new uint?[12];

	public ushort? TodayHonorableKills;

	public ushort? TodayDishonorableKills;

	public ushort? YesterdayHonorableKills;

	public ushort? YesterdayDishonorableKills;

	public ushort? LastWeekHonorableKills;

	public ushort? LastWeekDishonorableKills;

	public ushort? ThisWeekHonorableKills;

	public ushort? ThisWeekDishonorableKills;

	public uint? ThisWeekContribution;

	public uint? LifetimeHonorableKills;

	public uint? LifetimeDishonorableKills;

	public uint? YesterdayContribution;

	public uint? LastWeekContribution;

	public uint? LastWeekRank;

	public int? WatchedFactionIndex;

	public int? MaxLevel;

	public int? ScalingPlayerLevelDelta;

	public int? MaxCreatureScalingLevel;

	public int? PetSpellPower;

	public float? UiHitModifier;

	public float? UiSpellHitModifier;

	public int? HomeRealmTimeOffset;

	public float? ModPetHaste;

	public byte? LocalRegenFlags;

	public byte? AuraVision;

	public byte? NumBackpackSlots;

	public int? OverrideSpellsID;

	public int? LfgBonusFactionID;

	public uint? LootSpecID;

	public uint? OverrideZonePVPType;

	public int? Honor;

	public int? HonorNextLevel;

	public uint? PvPTierMaxFromWins;

	public uint? PvPLastWeeksTierMaxFromWins;

	public bool? InsertItemsLeftToRight;

	public byte? PvPRankProgress;

	public List<uint> SelfResSpells;

	public bool HasDailyQuestsUpdate;

	public int?[] CombatRatings { get; } = new int?[32];

	public PVPInfo[] PvpInfo { get; } = new PVPInfo[6];

	public uint?[] NoReagentCostMask { get; } = new uint?[4];

	public int?[] ProfessionSkillLine { get; } = new int?[2];

	public uint?[] BagSlotFlags { get; } = new uint?[4];

	public uint?[] BankBagSlotFlags { get; } = new uint?[7];

	public ulong?[] QuestCompleted { get; } = new ulong?[875];
}
