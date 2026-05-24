using System;
using Framework.Logging;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

public class ObjectUpdate
{
	public UpdateTypeModern Type;

	public WowGuid128 Guid;

	public GlobalSessionData GlobalSession;

	public CreateObjectData CreateData;

	public ObjectData ObjectData;

	public ItemData ItemData;

	public ContainerData ContainerData;

	public UnitData UnitData;

	public PlayerData PlayerData;

	public ActivePlayerData ActivePlayerData;

	public GameObjectData GameObjectData;

	public DynamicObjectData DynamicObjectData;

	public CorpseData CorpseData;

	public ObjectUpdate(WowGuid128 guid, UpdateTypeModern type, GlobalSessionData globalSession)
	{
		Type = type;
		Guid = guid;
		GlobalSession = globalSession;
		ObjectData = new ObjectData();
		if ((uint)(type - 1) <= 1u)
		{
			CreateData = new CreateObjectData();
		}
		switch (guid.GetObjectType())
		{
		case ObjectType.Item:
		case ObjectType.Container:
			ItemData = new ItemData();
			ContainerData = new ContainerData();
			break;
		case ObjectType.Unit:
			UnitData = new UnitData();
			break;
		case ObjectType.Player:
		case ObjectType.ActivePlayer:
			UnitData = new UnitData();
			PlayerData = new PlayerData();
			ActivePlayerData = new ActivePlayerData();
			break;
		case ObjectType.GameObject:
			GameObjectData = new GameObjectData();
			break;
		case ObjectType.DynamicObject:
			DynamicObjectData = new DynamicObjectData();
			break;
		case ObjectType.Corpse:
			CorpseData = new CorpseData();
			break;
		case ObjectType.AzeriteEmpoweredItem:
		case ObjectType.AzeriteItem:
			break;
		}
	}

	public void InitializePlaceholders()
	{
		if (CreateData == null)
		{
			return;
		}
		if (CreateData.MoveInfo != null)
		{
			if (CreateData.MoveInfo.WalkSpeed == 0f)
			{
				CreateData.MoveInfo.WalkSpeed = 2.5f;
			}
			if (CreateData.MoveInfo.RunSpeed == 0f)
			{
				CreateData.MoveInfo.RunSpeed = 7f;
			}
			if (CreateData.MoveInfo.RunBackSpeed == 0f)
			{
				CreateData.MoveInfo.RunBackSpeed = 4.5f;
			}
			if (CreateData.MoveInfo.SwimSpeed == 0f)
			{
				CreateData.MoveInfo.SwimSpeed = 4.722222f;
			}
			if (CreateData.MoveInfo.SwimBackSpeed == 0f)
			{
				CreateData.MoveInfo.SwimBackSpeed = 2.5f;
			}
			if (CreateData.MoveInfo.FlightSpeed == 0f)
			{
				CreateData.MoveInfo.FlightSpeed = 7f;
			}
			if (CreateData.MoveInfo.FlightBackSpeed == 0f)
			{
				CreateData.MoveInfo.FlightBackSpeed = 4.5f;
			}
			if (CreateData.MoveInfo.TurnRate == 0f)
			{
				CreateData.MoveInfo.TurnRate = 3.141594f;
			}
			if (CreateData.MoveInfo.PitchRate == 0f)
			{
				CreateData.MoveInfo.PitchRate = CreateData.MoveInfo.TurnRate;
			}
			if (CreateData.MoveInfo.Flags.HasAnyFlag(MovementFlagModern.WalkMode) && CreateData.MoveSpline != null)
			{
				CreateData.MoveInfo.Flags &= 4294967039u;
			}
			if (CreateData.MoveInfo.FlagsExtra == 0)
			{
				CreateData.MoveInfo.FlagsExtra = 512u;
			}
		}
		if (CreateData.MoveSpline != null && CreateData.MoveSpline.SplineFlags == SplineFlagModern.None)
		{
			CreateData.MoveSpline.SplineFlags = SplineFlagModern.Unknown5 | SplineFlagModern.Steering | SplineFlagModern.Unknown10;
		}
		if (GameObjectData != null)
		{
			if (!GameObjectData.PercentHealth.HasValue && (GameObjectData.State.HasValue || GameObjectData.TypeID.HasValue || GameObjectData.ArtKit.HasValue))
			{
				GameObjectData.PercentHealth = byte.MaxValue;
			}
			if (!GameObjectData.ParentRotation[3].HasValue)
			{
				GameObjectData.ParentRotation[3] = 1f;
			}
			if (!GameObjectData.StateAnimID.HasValue)
			{
				GameObjectData.StateAnimID = ModernVersion.GetGameObjectStateAnimId();
			}
			if (Guid.GetHighType() == HighGuidType.Transport)
			{
				// Map legacy GOState to 3.4.3 transport states for GAMEOBJECT_TYPE_TRANSPORT (elevators)
				// 3.3.5a: 0=active, 1=ready/stopped
				// 3.4.3:  24=GO_STATE_TRANSPORT_ACTIVE, 25=GO_STATE_TRANSPORT_STOPPED
				if (GameObjectData.TypeID.HasValue && GameObjectData.TypeID.Value == 11)
				{
					sbyte legacyState = GameObjectData.State.GetValueOrDefault();
					if (legacyState == 0)
						GameObjectData.State = 24; // GO_STATE_TRANSPORT_ACTIVE
					else if (legacyState == 1)
						GameObjectData.State = 25; // GO_STATE_TRANSPORT_STOPPED
				}

				uint entry = ObjectData.EntryID.HasValue ? (uint)ObjectData.EntryID.Value : 0;
				uint period = GameData.GetTransportPeriod(entry);
				Log.Print(LogType.Debug, $"[TransportFixup] Entry={entry} Period={period} DynFlagsSet={ObjectData.DynamicFlags.HasValue} DynFlags=0x{ObjectData.DynamicFlags.GetValueOrDefault():X8} Level={GameObjectData.Level} State={GameObjectData.State} TypeID={GameObjectData.TypeID} HasCreateData={CreateData != null} HasMoveInfo={CreateData?.MoveInfo != null}");
				if (period != 0)
				{
					if (!GameObjectData.Level.HasValue)
					{
						GameObjectData.Level = (int)period;
					}
					if (!ObjectData.DynamicFlags.HasValue)
					{
						uint pathTimer = CreateData.MoveInfo.TransportPathTimer;
						uint progress = pathTimer % period;
						uint dynFlags = (uint)(progress / (float)period * 65535f) << 16;
						ObjectData.DynamicFlags = dynFlags;
					}
					GameObjectData.Flags = 1048616u;
				}
				else if (!ObjectData.DynamicFlags.HasValue)
				{
					ObjectData.DynamicFlags = 0u;
				}
			}
		}
		if (CorpseData != null)
		{
			if (!CorpseData.ClassId.HasValue)
			{
				if (CorpseData.Owner != null)
				{
					CorpseData.ClassId = (byte)GlobalSession.GameState.GetUnitClass(CorpseData.Owner);
				}
				else
				{
					CorpseData.ClassId = 1;
				}
			}
			if (!CorpseData.FactionTemplate.HasValue && CorpseData.Owner != null)
			{
				int ownerFaction = GlobalSession.GameState.GetLegacyFieldValueInt32(CorpseData.Owner, UnitField.UNIT_FIELD_FACTIONTEMPLATE);
				if (ownerFaction != 0)
				{
					CorpseData.FactionTemplate = ownerFaction;
				}
				else if (CorpseData.RaceId.HasValue)
				{
					CorpseData.FactionTemplate = (int)GameData.GetFactionForRace(CorpseData.RaceId.Value);
				}
			}
		}
		if (UnitData != null)
		{
			for (int i = 0; i < 6; i++)
			{
				if (!UnitData.ModPowerRegen[i].HasValue)
				{
					UnitData.ModPowerRegen[i] = 1f;
				}
			}
			if (!UnitData.Flags2.HasValue)
			{
				UnitData.Flags2 = 2048u;
			}
			if (!UnitData.DisplayScale.HasValue)
			{
				UnitData.DisplayScale = 1f;
			}
			if (!UnitData.NativeXDisplayScale.HasValue)
			{
				UnitData.NativeXDisplayScale = 1f;
			}
			if (!UnitData.ModCastHaste.HasValue)
			{
				UnitData.ModCastHaste = 1f;
			}
			if (!UnitData.ModHaste.HasValue)
			{
				UnitData.ModHaste = 1f;
			}
			if (!UnitData.ModRangedHaste.HasValue)
			{
				UnitData.ModRangedHaste = 1f;
			}
			if (!UnitData.ModHasteRegen.HasValue)
			{
				UnitData.ModHasteRegen = 1f;
			}
			if (!UnitData.ModTimeRate.HasValue)
			{
				UnitData.ModTimeRate = 1f;
			}
			if (!UnitData.HoverHeight.HasValue)
			{
				UnitData.HoverHeight = 1f;
			}
			if (!UnitData.ScaleDuration.HasValue)
			{
				UnitData.ScaleDuration = 100;
			}
			if (!UnitData.LookAtControllerID.HasValue)
			{
				UnitData.LookAtControllerID = -1;
			}
			if (UnitData.ChannelObject == null && Guid == GlobalSession.GameState.CurrentPlayerGuid)
			{
				UnitData.ChannelObject = WowGuid128.Empty;
			}
		}
		if (PlayerData != null)
		{
			if (PlayerData.WowAccount == null)
			{
				if (CreateData.ThisIsYou)
				{
					PlayerData.WowAccount = WowGuid128.Create(HighGuidType703.WowAccount, GlobalSession.GameAccountInfo.Id);
				}
				else
				{
					PlayerData.WowAccount = WowGuid128.Create(HighGuidType703.WowAccount, Guid.GetCounter());
				}
			}
			if (!PlayerData.VirtualPlayerRealm.HasValue)
			{
				PlayerData.VirtualPlayerRealm = GlobalSession.RealmId.GetAddress();
			}
			if (!PlayerData.HonorLevel.HasValue)
			{
				PlayerData.HonorLevel = 1;
			}
			if (!PlayerData.AvgItemLevel[3].HasValue)
			{
				PlayerData.AvgItemLevel[3] = 1f;
			}
		}
		if (ActivePlayerData == null)
		{
			return;
		}
		if (ActivePlayerData.RestInfo[0] == null)
		{
			ActivePlayerData.RestInfo[0] = new RestInfo();
		}
		if (!ActivePlayerData.RestInfo[0].Threshold.HasValue)
		{
			ActivePlayerData.RestInfo[0].Threshold = 1u;
		}
		if (!ActivePlayerData.RestInfo[0].StateID.HasValue)
		{
			ActivePlayerData.RestInfo[0].StateID = 0u;
		}
		for (int j = 0; j < 7; j++)
		{
			if (!ActivePlayerData.ModDamageDonePercent[j].HasValue)
			{
				ActivePlayerData.ModDamageDonePercent[j] = 1f;
			}
		}
		if (!ActivePlayerData.ModHealingPercent.HasValue)
		{
			ActivePlayerData.ModHealingPercent = 1f;
		}
		if (!ActivePlayerData.ModHealingDonePercent.HasValue)
		{
			ActivePlayerData.ModHealingDonePercent = 1f;
		}
		if (!ActivePlayerData.ModPeriodicHealingDonePercent.HasValue)
		{
			ActivePlayerData.ModPeriodicHealingDonePercent = 1f;
		}
		for (int k = 0; k < 3; k++)
		{
			if (!ActivePlayerData.WeaponDmgMultipliers[k].HasValue)
			{
				ActivePlayerData.WeaponDmgMultipliers[k] = 1f;
			}
			if (!ActivePlayerData.WeaponAtkSpeedMultipliers[k].HasValue)
			{
				ActivePlayerData.WeaponAtkSpeedMultipliers[k] = 1f;
			}
		}
		if (!ActivePlayerData.ModSpellPowerPercent.HasValue)
		{
			ActivePlayerData.ModSpellPowerPercent = 1f;
		}
		if (!ActivePlayerData.NumBackpackSlots.HasValue)
		{
			ActivePlayerData.NumBackpackSlots = 16;
		}
		if (!ActivePlayerData.MultiActionBars.HasValue)
		{
			ActivePlayerData.MultiActionBars = 7;
		}
		if (!ActivePlayerData.MaxLevel.HasValue)
		{
			ActivePlayerData.MaxLevel = LegacyVersion.GetMaxLevel();
		}
		if (!ActivePlayerData.ModPetHaste.HasValue)
		{
			ActivePlayerData.ModPetHaste = 1f;
		}
		if (!ActivePlayerData.HonorNextLevel.HasValue)
		{
			ActivePlayerData.HonorNextLevel = 5500;
		}
		if (!ActivePlayerData.PvPTierMaxFromWins.HasValue)
		{
			ActivePlayerData.PvPTierMaxFromWins = uint.MaxValue;
		}
		if (!ActivePlayerData.PvPLastWeeksTierMaxFromWins.HasValue)
		{
			ActivePlayerData.PvPLastWeeksTierMaxFromWins = uint.MaxValue;
		}
	}
}
