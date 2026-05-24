using System;
using System.Collections.Generic;
using Framework.GameMath;
using HermesProxy.World.Enums;
using HermesProxy.World.Server.Packets;
using ActivePlayerField = HermesProxy.World.Enums.V1_14_1_40688.ActivePlayerField;
using ContainerField = HermesProxy.World.Enums.V1_14_1_40688.ContainerField;
using CorpseField = HermesProxy.World.Enums.V1_14_1_40688.CorpseField;
using DynamicObjectField = HermesProxy.World.Enums.V1_14_1_40688.DynamicObjectField;
using GameObjectField = HermesProxy.World.Enums.V1_14_1_40688.GameObjectField;
using ItemField = HermesProxy.World.Enums.V1_14_1_40688.ItemField;
using ObjectField = HermesProxy.World.Enums.V1_14_1_40688.ObjectField;
using PlayerField = HermesProxy.World.Enums.V1_14_1_40688.PlayerField;
using UnitDynamicField = HermesProxy.World.Enums.V1_14_1_40688.UnitDynamicField;
using UnitField = HermesProxy.World.Enums.V1_14_1_40688.UnitField;

namespace HermesProxy.World.Objects.Version.V1_14_1_40688;

public class ObjectUpdateBuilder
{
	protected bool m_alreadyWritten;

	protected ObjectUpdate m_updateData;

	protected UpdateFieldsArray m_fields;

	protected DynamicUpdateFieldsArray m_dynamicFields;

	protected ObjectTypeBCC m_objectType;

	protected ObjectTypeMask m_objectTypeMask;

	protected CreateObjectBits m_createBits;

	protected GameSessionData m_gameState;

	public ObjectUpdateBuilder(ObjectUpdate updateData, GameSessionData gameState)
	{
		m_alreadyWritten = false;
		m_updateData = updateData;
		m_gameState = gameState;
		ObjectType objectType = updateData.Guid.GetObjectType();
		if (updateData.CreateData != null)
		{
			objectType = updateData.CreateData.ObjectType;
			if (updateData.CreateData.ThisIsYou)
			{
				objectType = ObjectType.ActivePlayer;
			}
		}
		if (objectType == ObjectType.Player && m_gameState.CurrentPlayerGuid == updateData.Guid)
		{
			objectType = ObjectType.ActivePlayer;
		}
		m_objectType = ObjectTypeConverter.ConvertToBCC(objectType);
		m_objectTypeMask = ObjectTypeMask.Object;
		uint fieldsSize;
		uint dynamicFieldsSize;
		switch (m_objectType)
		{
		case ObjectTypeBCC.Item:
			fieldsSize = 80u;
			dynamicFieldsSize = 4u;
			m_objectTypeMask |= ObjectTypeMask.Item;
			break;
		case ObjectTypeBCC.Container:
			fieldsSize = 225u;
			dynamicFieldsSize = 4u;
			m_objectTypeMask |= ObjectTypeMask.Item;
			m_objectTypeMask |= ObjectTypeMask.Container;
			break;
		case ObjectTypeBCC.Unit:
			fieldsSize = 218u;
			dynamicFieldsSize = 3u;
			m_objectTypeMask |= ObjectTypeMask.Unit;
			break;
		case ObjectTypeBCC.Player:
			fieldsSize = 760u;
			dynamicFieldsSize = 4u;
			m_objectTypeMask |= ObjectTypeMask.Unit;
			m_objectTypeMask |= ObjectTypeMask.Player;
			break;
		case ObjectTypeBCC.ActivePlayer:
			fieldsSize = 4682u;
			dynamicFieldsSize = 18u;
			m_objectTypeMask |= ObjectTypeMask.Unit;
			m_objectTypeMask |= ObjectTypeMask.Player;
			m_objectTypeMask |= ObjectTypeMask.ActivePlayer;
			break;
		case ObjectTypeBCC.GameObject:
			fieldsSize = 33u;
			dynamicFieldsSize = 1u;
			m_objectTypeMask |= ObjectTypeMask.GameObject;
			break;
		case ObjectTypeBCC.DynamicObject:
			fieldsSize = 16u;
			dynamicFieldsSize = 0u;
			m_objectTypeMask |= ObjectTypeMask.DynamicObject;
			break;
		case ObjectTypeBCC.Corpse:
			fieldsSize = 113u;
			dynamicFieldsSize = 0u;
			m_objectTypeMask |= ObjectTypeMask.Corpse;
			break;
		default:
			throw new ArgumentOutOfRangeException("Unsupported object type!");
		}
		m_dynamicFields = new DynamicUpdateFieldsArray(dynamicFieldsSize, m_updateData.Type);
		m_gameState.ObjectCacheMutex.WaitOne();
		if (m_updateData.CreateData == null && m_gameState.ObjectCacheModern.TryGetValue(updateData.Guid, out m_fields) && m_fields != null)
		{
			m_fields.UpdateMask.Clear();
		}
		else
		{
			m_fields = new UpdateFieldsArray(fieldsSize);
			m_gameState.ObjectCacheModern.Remove(updateData.Guid);
			m_gameState.ObjectCacheModern.Add(updateData.Guid, m_fields);
		}
		m_gameState.ObjectCacheMutex.ReleaseMutex();
	}

	public void WriteToPacket(WorldPacket packet)
	{
		packet.WriteUInt8((byte)m_updateData.Type);
		packet.WritePackedGuid128(m_updateData.Guid);
		if (m_updateData.Type != UpdateTypeModern.Values)
		{
			packet.WriteUInt8((byte)m_objectType);
			packet.WriteInt32((int)m_objectTypeMask);
			SetCreateObjectBits();
			BuildMovementUpdate(packet);
		}
		BuildValuesUpdate(packet);
		BuildDynamicValuesUpdate(packet);
	}

	public void SetCreateObjectBits()
	{
		m_createBits.Clear();
		m_createBits.PlayHoverAnim = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && m_updateData.CreateData.MoveInfo.Hover;
		m_createBits.MovementUpdate = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && m_objectTypeMask.HasAnyFlag(ObjectTypeMask.Unit);
		m_createBits.MovementTransport = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && m_updateData.CreateData.MoveInfo.TransportGuid != null && m_objectType == ObjectTypeBCC.GameObject;
		m_createBits.Stationary = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && !m_objectTypeMask.HasAnyFlag(ObjectTypeMask.Unit);
		m_createBits.ServerTime = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && m_updateData.Guid.GetHighType() == HighGuidType.Transport;
		m_createBits.CombatVictim = m_updateData.CreateData != null && m_updateData.CreateData.AutoAttackVictim != null;
		m_createBits.Vehicle = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && m_updateData.CreateData.MoveInfo.VehicleId != 0;
		m_createBits.Rotation = ((m_updateData.CreateData != null) & (m_updateData.CreateData.MoveInfo != null)) && m_objectType == ObjectTypeBCC.GameObject;
		m_createBits.ThisIsYou = (m_createBits.ActivePlayer = m_objectType == ObjectTypeBCC.ActivePlayer);
	}

	public void BuildValuesUpdate(WorldPacket packet)
	{
		WriteValuesToArray();
		m_fields.WriteToPacket(packet);
	}

	public void BuildDynamicValuesUpdate(WorldPacket packet)
	{
		m_dynamicFields.WriteToPacket(packet);
	}

	public void BuildMovementUpdate(WorldPacket data)
	{
		int PauseTimesCount = 0;
		data.WriteBit(m_createBits.NoBirthAnim);
		data.WriteBit(m_createBits.EnablePortals);
		data.WriteBit(m_createBits.PlayHoverAnim);
		data.WriteBit(m_createBits.MovementUpdate);
		data.WriteBit(m_createBits.MovementTransport);
		data.WriteBit(m_createBits.Stationary);
		data.WriteBit(m_createBits.CombatVictim);
		data.WriteBit(m_createBits.ServerTime);
		data.WriteBit(m_createBits.Vehicle);
		data.WriteBit(m_createBits.AnimKit);
		data.WriteBit(m_createBits.Rotation);
		data.WriteBit(m_createBits.AreaTrigger);
		data.WriteBit(m_createBits.GameObject);
		data.WriteBit(m_createBits.SmoothPhasing);
		data.WriteBit(m_createBits.ThisIsYou);
		data.WriteBit(m_createBits.SceneObject);
		data.WriteBit(m_createBits.ActivePlayer);
		data.WriteBit(m_createBits.Conversation);
		data.FlushBits();
		if (m_createBits.MovementUpdate)
		{
			MovementInfo moveInfo = m_updateData.CreateData.MoveInfo;
			bool hasSpline = m_updateData.CreateData.MoveSpline != null;
			moveInfo.WriteMovementInfoModern(data, m_updateData.Guid);
			data.WriteFloat(moveInfo.WalkSpeed);
			data.WriteFloat(moveInfo.RunSpeed);
			data.WriteFloat(moveInfo.RunBackSpeed);
			data.WriteFloat(moveInfo.SwimSpeed);
			data.WriteFloat(moveInfo.SwimBackSpeed);
			data.WriteFloat(moveInfo.FlightSpeed);
			data.WriteFloat(moveInfo.FlightBackSpeed);
			data.WriteFloat(moveInfo.TurnRate);
			data.WriteFloat(moveInfo.PitchRate);
			data.WriteUInt32(0u);
			data.WriteFloat(1f);
			data.WriteBit(hasSpline);
			data.FlushBits();
			if (hasSpline)
			{
				WriteCreateObjectSplineDataBlock(m_updateData.CreateData.MoveSpline, data);
			}
		}
		data.WriteInt32(PauseTimesCount);
		if (m_createBits.Stationary)
		{
			data.WriteFloat(m_updateData.CreateData.MoveInfo.Position.X);
			data.WriteFloat(m_updateData.CreateData.MoveInfo.Position.Y);
			data.WriteFloat(m_updateData.CreateData.MoveInfo.Position.Z);
			data.WriteFloat(m_updateData.CreateData.MoveInfo.Orientation);
		}
		if (m_createBits.CombatVictim)
		{
			data.WritePackedGuid128(m_updateData.CreateData.AutoAttackVictim);
		}
		if (m_createBits.ServerTime)
		{
			if (m_updateData.CreateData.MoveInfo.TransportPathTimer != 0)
			{
				data.WriteUInt32(m_updateData.CreateData.MoveInfo.TransportPathTimer);
			}
			else
			{
				data.WriteUInt32((uint)Time.UnixTime);
			}
		}
		if (m_createBits.Vehicle)
		{
			data.WriteUInt32(m_updateData.CreateData.MoveInfo.VehicleId);
			data.WriteFloat(m_updateData.CreateData.MoveInfo.VehicleOrientation);
		}
		if (m_createBits.AnimKit)
		{
			data.WriteUInt16(0);
			data.WriteUInt16(0);
			data.WriteUInt16(0);
		}
		if (m_createBits.Rotation)
		{
			data.WriteInt64(m_updateData.CreateData.MoveInfo.Rotation.GetPackedRotation());
		}
		for (int i = 0; i < PauseTimesCount; i++)
		{
			data.WriteUInt32(0u);
		}
		if (m_createBits.MovementTransport)
		{
			m_updateData.CreateData.MoveInfo.WriteTransportInfoModern(data);
		}
		if (m_createBits.GameObject)
		{
			bool bit8 = false;
			uint Int1 = 0u;
			data.WriteUInt32(0u);
			data.WriteBit(bit8);
			data.FlushBits();
			if (bit8)
			{
				data.WriteUInt32(Int1);
			}
		}
		if (!m_createBits.ActivePlayer)
		{
			return;
		}
		bool hasSceneInstanceIDs = false;
		bool hasRuneState = false;
		bool hasActionButtons = m_gameState.ActionButtons.Count != 0;
		data.WriteBit(hasSceneInstanceIDs);
		data.WriteBit(hasRuneState);
		data.WriteBit(hasActionButtons);
		data.FlushBits();
		if (hasSceneInstanceIDs)
		{
			int sceneInstanceIDs = 0;
			data.WriteInt32(sceneInstanceIDs);
			for (int j = 0; j < sceneInstanceIDs; j++)
			{
				data.WriteInt32(0);
			}
		}
		if (hasRuneState)
		{
			byte RechargingRuneMask = 0;
			byte UsableRuneMask = 0;
			data.WriteUInt8(RechargingRuneMask);
			data.WriteUInt8(UsableRuneMask);
			uint runeCount = 0u;
			data.WriteUInt32(runeCount);
			for (int k = 0; k < runeCount; k++)
			{
				data.WriteUInt8(0);
			}
		}
		if (hasActionButtons)
		{
			for (int l = 0; l < 132; l++)
			{
				data.WriteInt32(m_gameState.ActionButtons[l]);
			}
		}
	}

	public static void WriteCreateObjectSplineDataBlock(ServerSideMovement moveSpline, WorldPacket data)
	{
		data.WriteUInt32(moveSpline.SplineId);
		if (!moveSpline.SplineFlags.HasAnyFlag(SplineFlagModern.Cyclic))
		{
			data.WriteVector3(moveSpline.EndPosition);
		}
		else
		{
			data.WriteVector3(Vector3.Zero);
		}
		bool hasSplineMove = data.WriteBit(moveSpline.SplineCount != 0);
		data.FlushBits();
		if (!hasSplineMove)
		{
			return;
		}
		data.WriteUInt32((uint)moveSpline.SplineFlags);
		data.WriteUInt32(moveSpline.SplineTime);
		data.WriteUInt32(moveSpline.SplineTimeFull);
		data.WriteFloat(1f);
		data.WriteFloat(1f);
		data.WriteBits((byte)moveSpline.SplineType, 2);
		bool hasFadeObjectTime = data.WriteBit(bit: false);
		data.WriteBits(moveSpline.SplineCount, 16);
		data.WriteBit(bit: false);
		data.WriteBit(bit: false);
		data.WriteBit(bit: false);
		data.WriteBit(bit: false);
		data.FlushBits();
		switch (moveSpline.SplineType)
		{
		case SplineTypeModern.FacingSpot:
			data.WriteVector3(moveSpline.FinalFacingSpot);
			break;
		case SplineTypeModern.FacingTarget:
			data.WritePackedGuid128(moveSpline.FinalFacingGuid);
			break;
		case SplineTypeModern.FacingAngle:
			data.WriteFloat(moveSpline.FinalOrientation);
			break;
		}
		if (hasFadeObjectTime)
		{
			data.WriteInt32(0);
		}
		foreach (Vector3 vec in moveSpline.SplinePoints)
		{
			data.WriteVector3(vec);
		}
	}

	public void WriteValuesToArray()
	{
		if (m_alreadyWritten)
		{
			return;
		}
		ObjectData objectData = m_updateData.ObjectData;
		if (objectData.Guid != null)
		{
			m_fields.SetUpdateField(ObjectField.OBJECT_FIELD_GUID, objectData.Guid);
		}
		if (objectData.EntryID.HasValue)
		{
			m_fields.SetUpdateField(ObjectField.OBJECT_FIELD_ENTRY, objectData.EntryID.Value);
		}
		if (objectData.DynamicFlags.HasValue)
		{
			m_fields.SetUpdateField(ObjectField.OBJECT_DYNAMIC_FLAGS, objectData.DynamicFlags.Value);
		}
		if (objectData.Scale.HasValue)
		{
			m_fields.SetUpdateField(ObjectField.OBJECT_FIELD_SCALE_X, objectData.Scale.Value);
		}
		ItemData itemData = m_updateData.ItemData;
		if (itemData != null)
		{
			if (itemData.Owner != null)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_OWNER, itemData.Owner);
			}
			if (itemData.ContainedIn != null)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_CONTAINED, itemData.ContainedIn);
			}
			if (itemData.Creator != null)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_CREATOR, itemData.Creator);
			}
			if (itemData.GiftCreator != null)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_GIFTCREATOR, itemData.GiftCreator);
			}
			if (itemData.StackCount.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_STACK_COUNT, itemData.StackCount.Value);
			}
			if (itemData.Duration.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_DURATION, itemData.Duration.Value);
			}
			for (int i = 0; i < 5; i++)
			{
				int startIndex = 25;
				if (itemData.SpellCharges[i].HasValue)
				{
					m_fields.SetUpdateField(startIndex + i, itemData.SpellCharges[i].Value);
				}
			}
			if (itemData.Flags.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_FLAGS, itemData.Flags.Value);
			}
			for (int j = 0; j < 13; j++)
			{
				int startIndex2 = 31;
				int sizePerEntry = 3;
				if (itemData.Enchantment[j] != null)
				{
					if (itemData.Enchantment[j].ID.HasValue)
					{
						m_fields.SetUpdateField(startIndex2 + j * sizePerEntry, itemData.Enchantment[j].ID.Value);
					}
					if (itemData.Enchantment[j].Duration.HasValue)
					{
						m_fields.SetUpdateField(startIndex2 + j * sizePerEntry + 1, itemData.Enchantment[j].Duration.Value);
					}
					if (itemData.Enchantment[j].Charges.HasValue)
					{
						m_fields.SetUpdateField(startIndex2 + j * sizePerEntry + 2, itemData.Enchantment[j].Charges.Value);
					}
					if (itemData.Enchantment[j].Inactive.HasValue)
					{
						m_fields.SetUpdateField(startIndex2 + j * sizePerEntry + 2, itemData.Enchantment[j].Inactive.Value, 1);
					}
				}
			}
			if (itemData.PropertySeed.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_PROPERTY_SEED, itemData.PropertySeed.Value);
			}
			if (itemData.RandomProperty.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_RANDOM_PROPERTIES_ID, itemData.RandomProperty.Value);
			}
			if (itemData.Durability.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_DURABILITY, itemData.Durability.Value);
			}
			if (itemData.MaxDurability.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_MAXDURABILITY, itemData.MaxDurability.Value);
			}
			if (itemData.CreatePlayedTime.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_CREATE_PLAYED_TIME, itemData.CreatePlayedTime.Value);
			}
			if (itemData.ModifiersMask.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_MODIFIERS_MASK, itemData.ModifiersMask.Value);
			}
			if (itemData.Context.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_CONTEXT, itemData.Context.Value);
			}
			if (itemData.ArtifactXP.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_ARTIFACT_XP, itemData.ArtifactXP.Value);
			}
			if (itemData.ItemAppearanceModID.HasValue)
			{
				m_fields.SetUpdateField(ItemField.ITEM_FIELD_APPEARANCE_MOD_ID, itemData.ItemAppearanceModID.Value);
			}
			if (itemData.HasGemsUpdate)
			{
				uint[] fields = new uint[30];
				uint[] gems = m_gameState.GetGemsForItem(m_updateData.Guid);
				fields[0] = gems[0];
				fields[10] = gems[1];
				fields[20] = gems[2];
				m_dynamicFields.SetUpdateField(3, fields, DynamicFieldChangeType.ValueAndSizeChanged);
			}
		}
		ContainerData containerData = m_updateData.ContainerData;
		if (containerData != null)
		{
			for (int k = 0; k < 36; k++)
			{
				int startIndex3 = 80;
				int sizePerEntry2 = 4;
				if (containerData.Slots[k] != null)
				{
					m_fields.SetUpdateField(startIndex3 + k * sizePerEntry2, containerData.Slots[k]);
				}
			}
			if (containerData.NumSlots.HasValue)
			{
				m_fields.SetUpdateField(ContainerField.CONTAINER_FIELD_NUM_SLOTS, containerData.NumSlots.Value);
			}
		}
		UnitData unitData = m_updateData.UnitData;
		if (unitData != null)
		{
			if (unitData.Charm != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_CHARM, unitData.Charm);
			}
			if (unitData.Summon != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SUMMON, unitData.Summon);
			}
			if (unitData.Critter != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_CRITTER, unitData.Critter);
			}
			if (unitData.CharmedBy != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_CHARMEDBY, unitData.CharmedBy);
			}
			if (unitData.SummonedBy != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SUMMONEDBY, unitData.SummonedBy);
			}
			if (unitData.CreatedBy != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_CREATEDBY, unitData.CreatedBy);
			}
			if (unitData.DemonCreator != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_DEMON_CREATOR, unitData.DemonCreator);
			}
			if (unitData.LookAtControllerTarget != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_LOOK_AT_CONTROLLER_TARGET, unitData.LookAtControllerTarget);
			}
			if (unitData.Target != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_TARGET, unitData.Target);
			}
			if (unitData.BattlePetCompanionGUID != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_BATTLE_PET_COMPANION_GUID, unitData.BattlePetCompanionGUID);
			}
			if (unitData.BattlePetDBID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_BATTLE_PET_DB_ID, unitData.BattlePetDBID.Value);
			}
			if (unitData.ChannelData != null)
			{
				int startIndex4 = 49;
				m_fields.SetUpdateField(startIndex4, unitData.ChannelData.SpellID);
				m_fields.SetUpdateField(startIndex4 + 1, unitData.ChannelData.SpellXSpellVisualID);
			}
			if (unitData.SummonedByHomeRealm.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SUMMONED_BY_HOME_REALM, unitData.SummonedByHomeRealm.Value);
			}
			if (unitData.RaceId.HasValue || unitData.ClassId.HasValue || unitData.PlayerClassId.HasValue || unitData.SexId.HasValue)
			{
				if (unitData.RaceId.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_0, unitData.RaceId.Value);
				}
				if (unitData.ClassId.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_0, unitData.ClassId.Value, 1);
				}
				if (unitData.PlayerClassId.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_0, unitData.PlayerClassId.Value, 2);
				}
				if (unitData.SexId.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_0, unitData.SexId.Value, 3);
				}
			}
			if (unitData.DisplayPower.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_DISPLAY_POWER, unitData.DisplayPower.Value);
			}
			if (unitData.OverrideDisplayPowerID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_OVERRIDE_DISPLAY_POWER_ID, unitData.OverrideDisplayPowerID.Value);
			}
			if (unitData.Health.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_HEALTH, (ulong)unitData.Health.Value);
			}
			for (int l = 0; l < 7; l++)
			{
				int startIndex5 = 57;
				if (unitData.Power[l].HasValue)
				{
					m_fields.SetUpdateField(startIndex5 + l, unitData.Power[l].Value);
				}
			}
			if (unitData.MaxHealth.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MAXHEALTH, (ulong)unitData.MaxHealth.Value);
			}
			for (int m = 0; m < 7; m++)
			{
				int startIndex6 = 66;
				if (unitData.MaxPower[m].HasValue)
				{
					m_fields.SetUpdateField(startIndex6 + m, unitData.MaxPower[m].Value);
				}
			}
			for (int n = 0; n < 7; n++)
			{
				int startIndex7 = 73;
				if (unitData.ModPowerRegen[n].HasValue)
				{
					m_fields.SetUpdateField(startIndex7 + n, unitData.ModPowerRegen[n].Value);
				}
			}
			if (unitData.Level.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_LEVEL, unitData.Level.Value);
			}
			if (unitData.EffectiveLevel.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_EFFECTIVE_LEVEL, unitData.EffectiveLevel.Value);
			}
			if (unitData.ContentTuningID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_CONTENT_TUNING_ID, unitData.ContentTuningID.Value);
			}
			if (unitData.ScalingLevelMin.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALING_LEVEL_MIN, unitData.ScalingLevelMin.Value);
			}
			if (unitData.ScalingLevelMax.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALING_LEVEL_MAX, unitData.ScalingLevelMax.Value);
			}
			if (unitData.ScalingLevelDelta.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALING_LEVEL_DELTA, unitData.ScalingLevelDelta.Value);
			}
			if (unitData.ScalingFactionGroup.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALING_FACTION_GROUP, unitData.ScalingFactionGroup.Value);
			}
			if (unitData.ScalingHealthItemLevelCurveID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALING_HEALTH_ITEM_LEVEL_CURVE_ID, unitData.ScalingHealthItemLevelCurveID.Value);
			}
			if (unitData.ScalingDamageItemLevelCurveID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALING_DAMAGE_ITEM_LEVEL_CURVE_ID, unitData.ScalingDamageItemLevelCurveID.Value);
			}
			if (unitData.FactionTemplate.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_FACTIONTEMPLATE, unitData.FactionTemplate.Value);
			}
			for (int num = 0; num < 3; num++)
			{
				int startIndex8 = 90;
				int sizePerEntry3 = 2;
				if (unitData.VirtualItems[num] != null)
				{
					m_fields.SetUpdateField(startIndex8 + num * sizePerEntry3, unitData.VirtualItems[num].ItemID);
					m_fields.SetUpdateField(startIndex8 + num * sizePerEntry3 + 1, unitData.VirtualItems[num].ItemAppearanceModID);
					m_fields.SetUpdateField(startIndex8 + num * sizePerEntry3 + 1, unitData.VirtualItems[num].ItemVisual, 1);
				}
			}
			if (unitData.Flags.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_FLAGS, unitData.Flags.Value);
			}
			if (unitData.Flags2.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_FLAGS_2, unitData.Flags2.Value);
			}
			if (unitData.Flags3.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_FLAGS_3, unitData.Flags3.Value);
			}
			if (unitData.AuraState.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_AURASTATE, unitData.AuraState.Value);
			}
			for (int num2 = 0; num2 < 2; num2++)
			{
				int startIndex9 = 100;
				if (unitData.AttackRoundBaseTime[num2].HasValue)
				{
					m_fields.SetUpdateField(startIndex9 + num2, unitData.AttackRoundBaseTime[num2].Value);
				}
			}
			if (unitData.RangedAttackRoundBaseTime.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_RANGEDATTACKTIME, unitData.RangedAttackRoundBaseTime.Value);
			}
			if (unitData.BoundingRadius.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_BOUNDINGRADIUS, unitData.BoundingRadius.Value);
			}
			if (unitData.CombatReach.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_COMBATREACH, unitData.CombatReach.Value);
			}
			if (unitData.DisplayID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_DISPLAYID, unitData.DisplayID.Value);
			}
			if (unitData.DisplayScale.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_DISPLAY_SCALE, unitData.DisplayScale.Value);
			}
			if (unitData.NativeDisplayID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_NATIVEDISPLAYID, unitData.NativeDisplayID.Value);
			}
			if (unitData.NativeXDisplayScale.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_NATIVE_X_DISPLAY_SCALE, unitData.NativeXDisplayScale.Value);
			}
			if (unitData.MountDisplayID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MOUNTDISPLAYID, unitData.MountDisplayID.Value);
			}
			if (unitData.MinDamage.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MINDAMAGE, unitData.MinDamage.Value);
			}
			if (unitData.MaxDamage.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MAXDAMAGE, unitData.MaxDamage.Value);
			}
			if (unitData.MinOffHandDamage.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MINOFFHANDDAMAGE, unitData.MinOffHandDamage.Value);
			}
			if (unitData.MaxOffHandDamage.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MAXOFFHANDDAMAGE, unitData.MaxOffHandDamage.Value);
			}
			if (unitData.StandState.HasValue || unitData.PetLoyaltyIndex.HasValue || unitData.VisFlags.HasValue || unitData.AnimTier.HasValue)
			{
				if (unitData.StandState.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_1, unitData.StandState.Value);
				}
				if (unitData.PetLoyaltyIndex.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_1, unitData.PetLoyaltyIndex.Value, 1);
				}
				if (unitData.VisFlags.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_1, unitData.VisFlags.Value, 2);
				}
				if (unitData.AnimTier.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_1, unitData.AnimTier.Value, 3);
				}
			}
			if (unitData.PetNumber.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_PETNUMBER, unitData.PetNumber.Value);
			}
			if (unitData.PetNameTimestamp.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_PET_NAME_TIMESTAMP, unitData.PetNameTimestamp.Value);
			}
			if (unitData.PetExperience.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_PETEXPERIENCE, unitData.PetExperience.Value);
			}
			if (unitData.PetNextLevelExperience.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_PETNEXTLEVELXP, unitData.PetNextLevelExperience.Value);
			}
			if (unitData.ModCastSpeed.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_MOD_CAST_SPEED, unitData.ModCastSpeed.Value);
			}
			if (unitData.ModCastHaste.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_MOD_CAST_HASTE, unitData.ModCastHaste.Value);
			}
			if (unitData.ModHaste.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MOD_HASTE, unitData.ModHaste.Value);
			}
			if (unitData.ModRangedHaste.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MOD_RANGED_HASTE, unitData.ModRangedHaste.Value);
			}
			if (unitData.ModHasteRegen.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MOD_HASTE_REGEN, unitData.ModHasteRegen.Value);
			}
			if (unitData.ModTimeRate.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MOD_TIME_RATE, unitData.ModTimeRate.Value);
			}
			if (unitData.CreatedBySpell.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_CREATED_BY_SPELL, unitData.CreatedBySpell.Value);
			}
			for (int num3 = 0; num3 < 2; num3++)
			{
				int startIndex10 = 126;
				if (unitData.NpcFlags[num3].HasValue)
				{
					m_fields.SetUpdateField(startIndex10 + num3, unitData.NpcFlags[num3].Value);
				}
			}
			if (unitData.EmoteState.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_NPC_EMOTESTATE, unitData.EmoteState.Value);
			}
			if (unitData.TrainingPointsUsed.HasValue && unitData.TrainingPointsTotal.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_TRAINING_POINTS_TOTAL, unitData.TrainingPointsUsed.Value);
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_TRAINING_POINTS_TOTAL, unitData.TrainingPointsTotal.Value, 1);
			}
			for (int num4 = 0; num4 < 5; num4++)
			{
				int startIndex11 = 130;
				if (unitData.Stats[num4].HasValue)
				{
					m_fields.SetUpdateField(startIndex11 + num4, unitData.Stats[num4].Value);
				}
			}
			for (int num5 = 0; num5 < 5; num5++)
			{
				int startIndex12 = 135;
				if (unitData.StatPosBuff[num5].HasValue)
				{
					m_fields.SetUpdateField(startIndex12 + num5, unitData.StatPosBuff[num5].Value);
				}
			}
			for (int num6 = 0; num6 < 5; num6++)
			{
				int startIndex13 = 140;
				if (unitData.StatNegBuff[num6].HasValue)
				{
					m_fields.SetUpdateField(startIndex13 + num6, unitData.StatNegBuff[num6].Value);
				}
			}
			for (int num7 = 0; num7 < 7; num7++)
			{
				int startIndex14 = 145;
				if (unitData.Resistances[num7].HasValue)
				{
					m_fields.SetUpdateField(startIndex14 + num7, unitData.Resistances[num7].Value);
				}
			}
			for (int num8 = 0; num8 < 7; num8++)
			{
				int startIndex15 = 152;
				if (unitData.ResistanceBuffModsPositive[num8].HasValue)
				{
					m_fields.SetUpdateField(startIndex15 + num8, unitData.ResistanceBuffModsPositive[num8].Value);
				}
			}
			for (int num9 = 0; num9 < 7; num9++)
			{
				int startIndex16 = 159;
				if (unitData.ResistanceBuffModsNegative[num9].HasValue)
				{
					m_fields.SetUpdateField(startIndex16 + num9, unitData.ResistanceBuffModsNegative[num9].Value);
				}
			}
			if (unitData.BaseMana.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_BASE_MANA, unitData.BaseMana.Value);
			}
			if (unitData.BaseHealth.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_BASE_HEALTH, unitData.BaseHealth.Value);
			}
			if (unitData.SheatheState.HasValue || unitData.PvpFlags.HasValue || unitData.PetFlags.HasValue || unitData.ShapeshiftForm.HasValue)
			{
				if (unitData.SheatheState.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_2, unitData.SheatheState.Value);
				}
				if (unitData.PvpFlags.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_2, unitData.PvpFlags.Value, 1);
				}
				if (unitData.PetFlags.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_2, unitData.PetFlags.Value, 2);
				}
				if (unitData.ShapeshiftForm.HasValue)
				{
					m_fields.SetUpdateField(UnitField.UNIT_FIELD_BYTES_2, unitData.ShapeshiftForm.Value, 3);
				}
			}
			if (unitData.AttackPower.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_ATTACK_POWER, unitData.AttackPower.Value);
			}
			if (unitData.AttackPowerModPos.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_ATTACK_POWER_MOD_POS, unitData.AttackPowerModPos.Value);
			}
			if (unitData.AttackPowerModNeg.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_ATTACK_POWER_MOD_NEG, unitData.AttackPowerModNeg.Value);
			}
			if (unitData.AttackPowerMultiplier.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_ATTACK_POWER_MULTIPLIER, unitData.AttackPowerMultiplier.Value);
			}
			if (unitData.RangedAttackPower.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_RANGED_ATTACK_POWER, unitData.RangedAttackPower.Value);
			}
			if (unitData.RangedAttackPowerModPos.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_RANGED_ATTACK_POWER_MOD_POS, unitData.RangedAttackPowerModPos.Value);
			}
			if (unitData.RangedAttackPowerModNeg.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_RANGED_ATTACK_POWER_MOD_NEG, unitData.RangedAttackPowerModNeg.Value);
			}
			if (unitData.RangedAttackPowerMultiplier.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER, unitData.RangedAttackPowerMultiplier.Value);
			}
			if (unitData.AttackSpeedAura.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_ATTACK_SPEED_AURA, unitData.AttackSpeedAura.Value);
			}
			if (unitData.Lifesteal.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_LIFESTEAL, unitData.Lifesteal.Value);
			}
			if (unitData.MinRangedDamage.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MINRANGEDDAMAGE, unitData.MinRangedDamage.Value);
			}
			if (unitData.MaxRangedDamage.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MAXRANGEDDAMAGE, unitData.MaxRangedDamage.Value);
			}
			for (int num10 = 0; num10 < 7; num10++)
			{
				int startIndex17 = 181;
				if (unitData.PowerCostModifier[num10].HasValue)
				{
					m_fields.SetUpdateField(startIndex17 + num10, unitData.PowerCostModifier[num10].Value);
				}
			}
			for (int num11 = 0; num11 < 7; num11++)
			{
				int startIndex18 = 188;
				if (unitData.PowerCostMultiplier[num11].HasValue)
				{
					m_fields.SetUpdateField(startIndex18 + num11, unitData.PowerCostMultiplier[num11].Value);
				}
			}
			if (unitData.MaxHealthModifier.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MAXHEALTHMODIFIER, unitData.MaxHealthModifier.Value);
			}
			if (unitData.HoverHeight.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_HOVERHEIGHT, unitData.HoverHeight.Value);
			}
			if (unitData.MinItemLevelCutoff.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MIN_ITEM_LEVEL_CUTOFF, unitData.MinItemLevelCutoff.Value);
			}
			if (unitData.MinItemLevel.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MIN_ITEM_LEVEL, unitData.MinItemLevel.Value);
			}
			if (unitData.MaxItemLevel.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_MAXITEMLEVEL, unitData.MaxItemLevel.Value);
			}
			if (unitData.WildBattlePetLevel.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_WILD_BATTLEPET_LEVEL, unitData.WildBattlePetLevel.Value);
			}
			if (unitData.BattlePetCompanionNameTimestamp.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_BATTLEPET_COMPANION_NAME_TIMESTAMP, unitData.BattlePetCompanionNameTimestamp.Value);
			}
			if (unitData.InteractSpellID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_INTERACT_SPELLID, unitData.InteractSpellID.Value);
			}
			if (unitData.StateSpellVisualID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_STATE_SPELL_VISUAL_ID, unitData.StateSpellVisualID.Value);
			}
			if (unitData.StateAnimID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_STATE_ANIM_ID, unitData.StateAnimID.Value);
			}
			if (unitData.StateAnimKitID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_STATE_ANIM_KIT_ID, unitData.StateAnimKitID.Value);
			}
			if (unitData.StateWorldEffectsID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_STATE_WORLD_EFFECT_ID, unitData.StateWorldEffectsID.Value);
			}
			if (unitData.ScaleDuration.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_SCALE_DURATION, unitData.ScaleDuration.Value);
			}
			if (unitData.LooksLikeMountID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_LOOKS_LIKE_MOUNT_ID, unitData.LooksLikeMountID.Value);
			}
			if (unitData.LooksLikeCreatureID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_LOOKS_LIKE_CREATURE_ID, unitData.LooksLikeCreatureID.Value);
			}
			if (unitData.LookAtControllerID.HasValue)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_LOOK_AT_CONTROLLER_ID, unitData.LookAtControllerID.Value);
			}
			if (unitData.GuildGUID != null)
			{
				m_fields.SetUpdateField(UnitField.UNIT_FIELD_GUILD_GUID, unitData.GuildGUID);
			}
			if (unitData.ChannelObject != null)
			{
				m_dynamicFields.SetUpdateField(UnitDynamicField.UNIT_DYNAMIC_FIELD_CHANNEL_OBJECTS, unitData.ChannelObject, DynamicFieldChangeType.ValueAndSizeChanged);
			}
		}
		PlayerData playerData = m_updateData.PlayerData;
		if (playerData != null)
		{
			if (playerData.DuelArbiter != null)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_DUEL_ARBITER, playerData.DuelArbiter);
			}
			if (playerData.WowAccount != null)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_WOW_ACCOUNT, playerData.WowAccount);
			}
			if (playerData.LootTargetGUID != null)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_LOOT_TARGET_GUID, playerData.LootTargetGUID);
			}
			if (playerData.PlayerFlags.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FLAGS, playerData.PlayerFlags.Value);
			}
			if (playerData.PlayerFlagsEx.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FLAGS_EX, playerData.PlayerFlagsEx.Value);
			}
			if (playerData.GuildRankID.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_GUILDRANK, playerData.GuildRankID.Value);
			}
			if (playerData.GuildDeleteDate.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_GUILDDELETE_DATE, playerData.GuildDeleteDate.Value);
			}
			if (playerData.GuildLevel.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_GUILDLEVEL, playerData.GuildLevel.Value);
			}
			if (playerData.PartyType.HasValue || playerData.NumBankSlots.HasValue || playerData.NativeSex.HasValue || playerData.Inebriation.HasValue)
			{
				if (playerData.PartyType.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES, playerData.PartyType.Value);
				}
				if (playerData.NumBankSlots.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES, playerData.NumBankSlots.Value, 1);
				}
				if (playerData.NativeSex.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES, playerData.NativeSex.Value, 2);
				}
				if (playerData.Inebriation.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES, playerData.Inebriation.Value, 3);
				}
			}
			if (playerData.PvpTitle.HasValue || playerData.ArenaFaction.HasValue || playerData.PvPRank.HasValue)
			{
				if (playerData.PvpTitle.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES_2, playerData.PvpTitle.Value);
				}
				if (playerData.ArenaFaction.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES_2, playerData.ArenaFaction.Value, 1);
				}
				if (playerData.PvPRank.HasValue)
				{
					m_fields.SetUpdateField(PlayerField.PLAYER_BYTES_2, playerData.PvPRank.Value, 2);
				}
			}
			if (playerData.DuelTeam.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_DUEL_TEAM, playerData.DuelTeam.Value);
			}
			if (playerData.GuildTimeStamp.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_GUILD_TIMESTAMP, playerData.GuildTimeStamp.Value);
			}
			for (int num12 = 0; num12 < 25; num12++)
			{
				int startIndex19 = 239;
				int sizePerEntry4 = 16;
				if (playerData.QuestLog[num12] == null)
				{
					continue;
				}
				if (playerData.QuestLog[num12].QuestID.HasValue)
				{
					m_fields.SetUpdateField(startIndex19 + num12 * sizePerEntry4, playerData.QuestLog[num12].QuestID.Value);
				}
				if (playerData.QuestLog[num12].StateFlags.HasValue)
				{
					m_fields.SetUpdateField(startIndex19 + num12 * sizePerEntry4 + 1, playerData.QuestLog[num12].StateFlags.Value);
				}
				for (int num13 = 0; num13 < 24; num13++)
				{
					if (playerData.QuestLog[num12].ObjectiveProgress[num13].HasValue)
					{
						m_fields.SetUpdateField(startIndex19 + num12 * sizePerEntry4 + 2 + num13 / 2, (ushort)playerData.QuestLog[num12].ObjectiveProgress[num13].Value, (byte)(num13 & 1));
					}
				}
				if (playerData.QuestLog[num12].EndTime.HasValue)
				{
					m_fields.SetUpdateField(startIndex19 + num12 * sizePerEntry4 + 2 + 12, playerData.QuestLog[num12].EndTime.Value);
				}
				if (playerData.QuestLog[num12].AcceptTime.HasValue)
				{
					m_fields.SetUpdateField(startIndex19 + num12 * sizePerEntry4 + 3 + 12, playerData.QuestLog[num12].AcceptTime.Value);
				}
			}
			for (int num14 = 0; num14 < 19; num14++)
			{
				int startIndex20 = 639;
				int sizePerEntry5 = 2;
				if (playerData.VisibleItems[num14] != null)
				{
					m_fields.SetUpdateField(startIndex20 + num14 * sizePerEntry5, playerData.VisibleItems[num14].ItemID);
					m_fields.SetUpdateField(startIndex20 + num14 * sizePerEntry5 + 1, playerData.VisibleItems[num14].ItemAppearanceModID);
					m_fields.SetUpdateField(startIndex20 + num14 * sizePerEntry5 + 1, playerData.VisibleItems[num14].ItemVisual, 1);
				}
			}
			if (playerData.ChosenTitle.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_CHOSEN_TITLE, playerData.ChosenTitle.Value);
			}
			if (playerData.FakeInebriation.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FAKE_INEBRIATION, playerData.FakeInebriation.Value);
			}
			if (playerData.VirtualPlayerRealm.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FIELD_VIRTUAL_PLAYER_REALM, playerData.VirtualPlayerRealm.Value);
			}
			if (playerData.CurrentSpecID.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FIELD_CURRENT_SPEC_ID, playerData.CurrentSpecID.Value);
			}
			if (playerData.TaxiMountAnimKitID.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FIELD_TAXI_MOUNT_ANIM_KIT_ID, playerData.TaxiMountAnimKitID.Value);
			}
			for (int num15 = 0; num15 < 6; num15++)
			{
				int startIndex21 = 682;
				if (playerData.AvgItemLevel[num15].HasValue)
				{
					m_fields.SetUpdateField(startIndex21 + num15, playerData.AvgItemLevel[num15].Value);
				}
			}
			if (playerData.CurrentBattlePetBreedQuality.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FIELD_CURRENT_BATTLE_PET_BREED_QUALITY, playerData.CurrentBattlePetBreedQuality.Value);
			}
			if (playerData.HonorLevel.HasValue)
			{
				m_fields.SetUpdateField(PlayerField.PLAYER_FIELD_HONOR_LEVEL, playerData.HonorLevel.Value);
			}
			for (int num16 = 0; num16 < 35; num16++)
			{
				int startIndex22 = 690;
				int sizePerEntry6 = 2;
				if (playerData.Customizations[num16] != null)
				{
					m_fields.SetUpdateField(startIndex22 + num16 * sizePerEntry6, playerData.Customizations[num16].ChrCustomizationOptionID);
					m_fields.SetUpdateField(startIndex22 + num16 * sizePerEntry6 + 1, playerData.Customizations[num16].ChrCustomizationChoiceID);
				}
			}
		}
		ActivePlayerData activeData = m_updateData.ActivePlayerData;
		if (activeData != null && m_objectType == ObjectTypeBCC.ActivePlayer)
		{
			for (int num17 = 0; num17 < 23; num17++)
			{
				int startIndex23 = 760;
				int sizePerEntry7 = 4;
				if (activeData.InvSlots[num17] != null)
				{
					m_fields.SetUpdateField(startIndex23 + num17 * sizePerEntry7, activeData.InvSlots[num17]);
				}
			}
			for (int num18 = 0; num18 < 24; num18++)
			{
				int startIndex24 = 852;
				int sizePerEntry8 = 4;
				if (activeData.PackSlots[num18] != null)
				{
					m_fields.SetUpdateField(startIndex24 + num18 * sizePerEntry8, activeData.PackSlots[num18]);
				}
			}
			for (int num19 = 0; num19 < 28; num19++)
			{
				int startIndex25 = 948;
				int sizePerEntry9 = 4;
				if (activeData.BankSlots[num19] != null)
				{
					m_fields.SetUpdateField(startIndex25 + num19 * sizePerEntry9, activeData.BankSlots[num19]);
				}
			}
			for (int num20 = 0; num20 < 7; num20++)
			{
				int startIndex26 = 1060;
				int sizePerEntry10 = 4;
				if (activeData.BankBagSlots[num20] != null)
				{
					m_fields.SetUpdateField(startIndex26 + num20 * sizePerEntry10, activeData.BankBagSlots[num20]);
				}
			}
			for (int num21 = 0; num21 < 12; num21++)
			{
				int startIndex27 = 1088;
				int sizePerEntry11 = 4;
				if (activeData.BuyBackSlots[num21] != null)
				{
					m_fields.SetUpdateField(startIndex27 + num21 * sizePerEntry11, activeData.BuyBackSlots[num21]);
				}
			}
			for (int num22 = 0; num22 < 32; num22++)
			{
				int startIndex28 = 1136;
				int sizePerEntry12 = 4;
				if (activeData.KeyringSlots[num22] != null)
				{
					m_fields.SetUpdateField(startIndex28 + num22 * sizePerEntry12, activeData.KeyringSlots[num22]);
				}
			}
			if (activeData.FarsightObject != null)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_FARSIGHT, activeData.FarsightObject);
			}
			if (activeData.ComboTarget != null)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_COMBO_TARGET, activeData.ComboTarget);
			}
			if (activeData.SummonedBattlePetGUID != null)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_SUMMONED_BATTLE_PET_ID, activeData.SummonedBattlePetGUID);
			}
			for (int num23 = 0; num23 < 12; num23++)
			{
				int startIndex29 = 1288;
				if (activeData.KnownTitles[num23].HasValue)
				{
					m_fields.SetUpdateField(startIndex29 + num23, activeData.KnownTitles[num23].Value);
				}
			}
			if (activeData.Coinage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_COINAGE, activeData.Coinage.Value);
			}
			if (activeData.XP.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_XP, activeData.XP.Value);
			}
			if (activeData.NextLevelXP.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_NEXT_LEVEL_XP, activeData.NextLevelXP.Value);
			}
			if (activeData.TrialXP.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_TRIAL_XP, activeData.TrialXP.Value);
			}
			for (int num24 = 0; num24 < 256; num24++)
			{
				if (activeData.Skill.SkillLineID[num24].HasValue)
				{
					int startIndex30 = 1305;
					m_fields.SetUpdateField(startIndex30 + num24 / 2, activeData.Skill.SkillLineID[num24].Value, (byte)(num24 & 1));
				}
				if (activeData.Skill.SkillStep[num24].HasValue)
				{
					int startIndex31 = 1433;
					m_fields.SetUpdateField(startIndex31 + num24 / 2, activeData.Skill.SkillStep[num24].Value, (byte)(num24 & 1));
				}
				if (activeData.Skill.SkillRank[num24].HasValue)
				{
					int startIndex32 = 1561;
					m_fields.SetUpdateField(startIndex32 + num24 / 2, activeData.Skill.SkillRank[num24].Value, (byte)(num24 & 1));
				}
				if (activeData.Skill.SkillStartingRank[num24].HasValue)
				{
					int startIndex33 = 1689;
					m_fields.SetUpdateField(startIndex33 + num24 / 2, activeData.Skill.SkillStartingRank[num24].Value, (byte)(num24 & 1));
				}
				if (activeData.Skill.SkillMaxRank[num24].HasValue)
				{
					int startIndex34 = 1817;
					m_fields.SetUpdateField(startIndex34 + num24 / 2, activeData.Skill.SkillMaxRank[num24].Value, (byte)(num24 & 1));
				}
				if (activeData.Skill.SkillTempBonus[num24].HasValue)
				{
					int startIndex35 = 1945;
					m_fields.SetUpdateField(startIndex35 + num24 / 2, (ushort)activeData.Skill.SkillTempBonus[num24].Value, (byte)(num24 & 1));
				}
				if (activeData.Skill.SkillPermBonus[num24].HasValue)
				{
					int startIndex36 = 2073;
					m_fields.SetUpdateField(startIndex36 + num24 / 2, activeData.Skill.SkillPermBonus[num24].Value, (byte)(num24 & 1));
				}
			}
			if (activeData.CharacterPoints.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_CHARACTER_POINTS, activeData.CharacterPoints.Value);
			}
			if (activeData.MaxTalentTiers.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MAX_TALENT_TIERS, activeData.MaxTalentTiers.Value);
			}
			if (activeData.TrackCreatureMask.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_TRACK_CREATURES, activeData.TrackCreatureMask.Value);
			}
			for (int num25 = 0; num25 < 2; num25++)
			{
				int startIndex37 = 2204;
				if (activeData.TrackResourceMask[num25].HasValue)
				{
					m_fields.SetUpdateField(startIndex37 + num25, activeData.TrackResourceMask[num25].Value);
				}
			}
			if (activeData.MainhandExpertise.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_EXPERTISE, activeData.MainhandExpertise.Value);
			}
			if (activeData.OffhandExpertise.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_OFFHAND_EXPERTISE, activeData.OffhandExpertise.Value);
			}
			if (activeData.RangedExpertise.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_RANGED_EXPERTISE, activeData.RangedExpertise.Value);
			}
			if (activeData.CombatRatingExpertise.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_COMBAT_RATING_EXPERTISE, activeData.CombatRatingExpertise.Value);
			}
			if (activeData.BlockPercentage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BLOCK_PERCENTAGE, activeData.BlockPercentage.Value);
			}
			if (activeData.DodgePercentage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_DODGE_PERCENTAGE, activeData.DodgePercentage.Value);
			}
			if (activeData.DodgePercentageFromAttribute.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_DODGE_PERCENTAGE_FROM_ATTRIBUTE, activeData.DodgePercentageFromAttribute.Value);
			}
			if (activeData.ParryPercentage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PARRY_PERCENTAGE, activeData.ParryPercentage.Value);
			}
			if (activeData.ParryPercentageFromAttribute.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PARRY_PERCENTAGE_FROM_ATTRIBUTE, activeData.ParryPercentageFromAttribute.Value);
			}
			if (activeData.CritPercentage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_CRIT_PERCENTAGE, activeData.CritPercentage.Value);
			}
			if (activeData.RangedCritPercentage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_RANGED_CRIT_PERCENTAGE, activeData.RangedCritPercentage.Value);
			}
			if (activeData.OffhandCritPercentage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_OFFHAND_CRIT_PERCENTAGE, activeData.OffhandCritPercentage.Value);
			}
			for (int num26 = 0; num26 < 7; num26++)
			{
				int startIndex38 = 2218;
				if (activeData.SpellCritPercentage[num26].HasValue)
				{
					m_fields.SetUpdateField(startIndex38 + num26, activeData.SpellCritPercentage[num26].Value);
				}
			}
			if (activeData.ShieldBlock.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_SHIELD_BLOCK, activeData.ShieldBlock.Value);
			}
			if (activeData.Mastery.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MASTERY, activeData.Mastery.Value);
			}
			if (activeData.Speed.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_SPEED, activeData.Speed.Value);
			}
			if (activeData.Avoidance.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_AVOIDANCE, activeData.Avoidance.Value);
			}
			if (activeData.Sturdiness.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_STURDINESS, activeData.Sturdiness.Value);
			}
			if (activeData.Versatility.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_VERSATILITY, activeData.Versatility.Value);
			}
			if (activeData.VersatilityBonus.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_VERSATILITY_BONUS, activeData.VersatilityBonus.Value);
			}
			if (activeData.PvpPowerDamage.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PVP_POWER_DAMAGE, activeData.PvpPowerDamage.Value);
			}
			if (activeData.PvpPowerHealing.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PVP_POWER_HEALING, activeData.PvpPowerHealing.Value);
			}
			for (int num27 = 0; num27 < 240; num27++)
			{
				int startIndex39 = 2234;
				if (activeData.ExploredZones[num27].HasValue)
				{
					m_fields.SetUpdateField(startIndex39 + num27 * 2, activeData.ExploredZones[num27].Value);
				}
			}
			for (int num28 = 0; num28 < 2; num28++)
			{
				int startIndex40 = 2714;
				int sizePerEntry13 = 2;
				if (activeData.RestInfo[num28] != null)
				{
					if (activeData.RestInfo[num28].StateID.HasValue)
					{
						m_fields.SetUpdateField(startIndex40 + num28 * sizePerEntry13, activeData.RestInfo[num28].StateID.Value);
					}
					if (activeData.RestInfo[num28].Threshold.HasValue)
					{
						m_fields.SetUpdateField(startIndex40 + num28 * sizePerEntry13 + 1, activeData.RestInfo[num28].Threshold.Value);
					}
				}
			}
			for (int num29 = 0; num29 < 7; num29++)
			{
				int startIndex41 = 2718;
				if (activeData.ModDamageDonePos[num29].HasValue)
				{
					m_fields.SetUpdateField(startIndex41 + num29, activeData.ModDamageDonePos[num29].Value);
				}
			}
			for (int num30 = 0; num30 < 7; num30++)
			{
				int startIndex42 = 2725;
				if (activeData.ModDamageDoneNeg[num30].HasValue)
				{
					m_fields.SetUpdateField(startIndex42 + num30, activeData.ModDamageDoneNeg[num30].Value);
				}
			}
			for (int num31 = 0; num31 < 7; num31++)
			{
				int startIndex43 = 2732;
				if (activeData.ModDamageDonePercent[num31].HasValue)
				{
					m_fields.SetUpdateField(startIndex43 + num31, activeData.ModDamageDonePercent[num31].Value);
				}
			}
			if (activeData.ModHealingDonePos.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_HEALING_DONE_POS, activeData.ModHealingDonePos.Value);
			}
			if (activeData.ModHealingPercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_HEALING_PCT, activeData.ModHealingPercent.Value);
			}
			if (activeData.ModHealingDonePercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_HEALING_DONE_PCT, activeData.ModHealingDonePercent.Value);
			}
			if (activeData.ModPeriodicHealingDonePercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_PERIODIC_HEALING_DONE_PERCENT, activeData.ModPeriodicHealingDonePercent.Value);
			}
			for (int num32 = 0; num32 < 3; num32++)
			{
				int startIndex44 = 2743;
				if (activeData.WeaponDmgMultipliers[num32].HasValue)
				{
					m_fields.SetUpdateField(startIndex44 + num32, activeData.WeaponDmgMultipliers[num32].Value);
				}
			}
			for (int num33 = 0; num33 < 3; num33++)
			{
				int startIndex45 = 2746;
				if (activeData.WeaponAtkSpeedMultipliers[num33].HasValue)
				{
					m_fields.SetUpdateField(startIndex45 + num33, activeData.WeaponAtkSpeedMultipliers[num33].Value);
				}
			}
			if (activeData.ModSpellPowerPercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_SPELL_POWER_PCT, activeData.ModSpellPowerPercent.Value);
			}
			if (activeData.ModResiliencePercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_RESILIENCE_PERCENT, activeData.ModResiliencePercent.Value);
			}
			if (activeData.OverrideSpellPowerByAPPercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_OVERRIDE_SPELL_POWER_BY_AP_PCT, activeData.OverrideSpellPowerByAPPercent.Value);
			}
			if (activeData.OverrideAPBySpellPowerPercent.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_OVERRIDE_AP_BY_SPELL_POWER_PERCENT, activeData.OverrideAPBySpellPowerPercent.Value);
			}
			if (activeData.ModTargetResistance.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_TARGET_RESISTANCE, activeData.ModTargetResistance.Value);
			}
			if (activeData.ModTargetPhysicalResistance.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE, activeData.ModTargetPhysicalResistance.Value);
			}
			if (activeData.LocalFlags.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LOCAL_FLAGS, activeData.LocalFlags.Value);
			}
			if (activeData.GrantableLevels.HasValue || activeData.MultiActionBars.HasValue || activeData.LifetimeMaxRank.HasValue || activeData.NumRespecs.HasValue)
			{
				if (activeData.GrantableLevels.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES, activeData.GrantableLevels.Value);
				}
				if (activeData.MultiActionBars.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES, activeData.MultiActionBars.Value, 1);
				}
				if (activeData.LifetimeMaxRank.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES, activeData.LifetimeMaxRank.Value, 2);
				}
				if (activeData.NumRespecs.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES, activeData.NumRespecs.Value, 3);
				}
			}
			if (activeData.AmmoID.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_AMMO_ID, activeData.AmmoID.Value);
			}
			if (activeData.PvpMedals.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PVP_MEDALS, activeData.PvpMedals.Value);
			}
			for (int num34 = 0; num34 < 12; num34++)
			{
				int startIndex46 = 2759;
				if (activeData.BuybackPrice[num34].HasValue)
				{
					m_fields.SetUpdateField(startIndex46 + num34, activeData.BuybackPrice[num34].Value);
				}
			}
			for (int num35 = 0; num35 < 12; num35++)
			{
				int startIndex47 = 2771;
				if (activeData.BuybackTimestamp[num35].HasValue)
				{
					m_fields.SetUpdateField(startIndex47 + num35, activeData.BuybackTimestamp[num35].Value);
				}
			}
			if (activeData.TodayHonorableKills.HasValue && activeData.TodayDishonorableKills.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_2, activeData.TodayHonorableKills.Value);
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_2, activeData.TodayDishonorableKills.Value, 1);
			}
			if (activeData.YesterdayHonorableKills.HasValue && activeData.YesterdayDishonorableKills.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_3, activeData.YesterdayHonorableKills.Value);
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_3, activeData.YesterdayDishonorableKills.Value, 1);
			}
			if (activeData.LastWeekHonorableKills.HasValue && activeData.LastWeekDishonorableKills.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_4, activeData.LastWeekHonorableKills.Value);
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_4, activeData.LastWeekDishonorableKills.Value, 1);
			}
			if (activeData.ThisWeekHonorableKills.HasValue && activeData.ThisWeekDishonorableKills.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_5, activeData.ThisWeekHonorableKills.Value);
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_5, activeData.ThisWeekDishonorableKills.Value, 1);
			}
			if (activeData.ThisWeekContribution.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_THIS_WEEK_CONTRIBUTION, activeData.ThisWeekContribution.Value);
			}
			if (activeData.LifetimeHonorableKills.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LIFETIME_HONORABLE_KILLS, activeData.LifetimeHonorableKills.Value);
			}
			if (activeData.LifetimeDishonorableKills.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS, activeData.LifetimeDishonorableKills.Value);
			}
			if (activeData.YesterdayContribution.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_YESTERDAY_CONTRIBUTION, activeData.YesterdayContribution.Value);
			}
			if (activeData.LastWeekContribution.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LAST_WEEK_CONTRIBUTION, activeData.LastWeekContribution.Value);
			}
			if (activeData.LastWeekRank.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LAST_WEEK_RANK, activeData.LastWeekRank.Value);
			}
			if (activeData.WatchedFactionIndex.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_WATCHED_FACTION_INDEX, activeData.WatchedFactionIndex.Value);
			}
			for (int num36 = 0; num36 < 32; num36++)
			{
				int startIndex48 = 2794;
				if (activeData.CombatRatings[num36].HasValue)
				{
					m_fields.SetUpdateField(startIndex48 + num36, activeData.CombatRatings[num36].Value);
				}
			}
			for (int num37 = 0; num37 < 6; num37++)
			{
				int startIndex49 = 2826;
				int sizePerEntry14 = 12;
				if (activeData.PvpInfo[num37] != null)
				{
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14, activeData.PvpInfo[num37].WeeklyPlayed);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 1, activeData.PvpInfo[num37].WeeklyWon);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 2, activeData.PvpInfo[num37].SeasonPlayed);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 3, activeData.PvpInfo[num37].SeasonWon);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 4, activeData.PvpInfo[num37].Rating);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 5, activeData.PvpInfo[num37].WeeklyBestRating);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 6, activeData.PvpInfo[num37].SeasonBestRating);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 7, activeData.PvpInfo[num37].PvpTierID);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 8, activeData.PvpInfo[num37].WeeklyBestWinPvpTierID);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 9, activeData.PvpInfo[num37].Field_28);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 10, activeData.PvpInfo[num37].Field_2C);
					m_fields.SetUpdateField(startIndex49 + num37 * sizePerEntry14 + 11, activeData.PvpInfo[num37].Disqualified ? 1u : 0u);
				}
			}
			if (activeData.MaxLevel.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MAX_LEVEL, activeData.MaxLevel.Value);
			}
			if (activeData.ScalingPlayerLevelDelta.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_SCALING_PLAYER_LEVEL_DELTA, activeData.ScalingPlayerLevelDelta.Value);
			}
			if (activeData.MaxCreatureScalingLevel.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MAX_CREATURE_SCALING_LEVEL, activeData.MaxCreatureScalingLevel.Value);
			}
			for (int num38 = 0; num38 < 4; num38++)
			{
				int startIndex50 = 2901;
				if (activeData.NoReagentCostMask[num38].HasValue)
				{
					m_fields.SetUpdateField(startIndex50 + num38, activeData.NoReagentCostMask[num38].Value);
				}
			}
			if (activeData.PetSpellPower.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PET_SPELL_POWER, activeData.PetSpellPower.Value);
			}
			for (int num39 = 0; num39 < 2; num39++)
			{
				int startIndex51 = 2906;
				if (activeData.ProfessionSkillLine[num39].HasValue)
				{
					m_fields.SetUpdateField(startIndex51 + num39, activeData.ProfessionSkillLine[num39].Value);
				}
			}
			if (activeData.UiHitModifier.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_UI_HIT_MODIFIER, activeData.UiHitModifier.Value);
			}
			if (activeData.UiSpellHitModifier.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_UI_SPELL_HIT_MODIFIER, activeData.UiSpellHitModifier.Value);
			}
			if (activeData.HomeRealmTimeOffset.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_HOME_REALM_TIME_OFFSET, activeData.HomeRealmTimeOffset.Value);
			}
			if (activeData.ModPetHaste.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_MOD_PET_HASTE, activeData.ModPetHaste.Value);
			}
			if (activeData.LocalRegenFlags.HasValue || activeData.AuraVision.HasValue || activeData.NumBackpackSlots.HasValue)
			{
				if (activeData.LocalRegenFlags.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_6, activeData.LocalRegenFlags.Value);
				}
				if (activeData.AuraVision.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_6, activeData.AuraVision.Value, 1);
				}
				if (activeData.NumBackpackSlots.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_6, activeData.NumBackpackSlots.Value, 2);
				}
			}
			if (activeData.OverrideSpellsID.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_OVERRIDE_SPELLS_ID, activeData.OverrideSpellsID.Value);
			}
			if (activeData.LfgBonusFactionID.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LFG_BONUS_FACTION_ID, activeData.LfgBonusFactionID.Value);
			}
			if (activeData.LootSpecID.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_LOOT_SPEC_ID, activeData.LootSpecID.Value);
			}
			if (activeData.OverrideZonePVPType.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_OVERRIDE_ZONE_PVP_TYPE, activeData.OverrideZonePVPType.Value);
			}
			for (int num40 = 0; num40 < 4; num40++)
			{
				int startIndex52 = 2917;
				if (activeData.BagSlotFlags[num40].HasValue)
				{
					m_fields.SetUpdateField(startIndex52 + num40, activeData.BagSlotFlags[num40].Value);
				}
			}
			for (int num41 = 0; num41 < 7; num41++)
			{
				int startIndex53 = 2921;
				if (activeData.BankBagSlotFlags[num41].HasValue)
				{
					m_fields.SetUpdateField(startIndex53 + num41, activeData.BankBagSlotFlags[num41].Value);
				}
			}
			for (int num42 = 0; num42 < 875; num42++)
			{
				int startIndex54 = 2927;
				if (activeData.QuestCompleted[num42].HasValue)
				{
					m_fields.SetUpdateField(startIndex54 + num42 * 2, activeData.QuestCompleted[num42].Value);
				}
			}
			if (activeData.Honor.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_HONOR, activeData.Honor.Value);
			}
			if (activeData.HonorNextLevel.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_HONOR_NEXT_LEVEL, activeData.HonorNextLevel.Value);
			}
			if (activeData.PvPTierMaxFromWins.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PVP_TIER_MAX_FROM_WINS, activeData.PvPTierMaxFromWins.Value);
			}
			if (activeData.PvPLastWeeksTierMaxFromWins.HasValue)
			{
				m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_PVP_LAST_WEEKS_TIER_MAX_FROM_WINS, activeData.PvPLastWeeksTierMaxFromWins.Value);
			}
			if (activeData.InsertItemsLeftToRight.HasValue || activeData.PvPRankProgress.HasValue)
			{
				if (activeData.InsertItemsLeftToRight.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_7, (byte)((activeData.InsertItemsLeftToRight == true) ? 1u : 0u));
				}
				if (activeData.PvPRankProgress.HasValue)
				{
					m_fields.SetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_BYTES_7, activeData.PvPRankProgress.Value, 1);
				}
			}
			if (activeData.SelfResSpells != null)
			{
				uint[] fields2 = new uint[activeData.SelfResSpells.Count];
				for (int num43 = 0; num43 < activeData.SelfResSpells.Count; num43++)
				{
					fields2[num43] = activeData.SelfResSpells[num43];
				}
				m_dynamicFields.SetUpdateField(14, fields2, DynamicFieldChangeType.ValueAndSizeChanged);
			}
			if (activeData.HasDailyQuestsUpdate)
			{
				uint[] fields3 = new uint[m_gameState.DailyQuestsDone.Count];
				int counter = 0;
				foreach (KeyValuePair<uint, uint> itr in m_gameState.DailyQuestsDone)
				{
					fields3[counter++] = itr.Value;
				}
				m_dynamicFields.SetUpdateField(7, fields3, DynamicFieldChangeType.ValueAndSizeChanged);
			}
		}
		GameObjectData goData = m_updateData.GameObjectData;
		if (goData != null)
		{
			if (goData.CreatedBy != null)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_FIELD_CREATED_BY, goData.CreatedBy);
			}
			if (goData.DisplayID.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_DISPLAYID, goData.DisplayID.Value);
			}
			if (goData.Flags.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_FLAGS, goData.Flags.Value);
			}
			for (int num44 = 0; num44 < 4; num44++)
			{
				int startIndex55 = 17;
				if (goData.ParentRotation[num44].HasValue)
				{
					m_fields.SetUpdateField(startIndex55 + num44, goData.ParentRotation[num44].Value);
				}
			}
			if (goData.FactionTemplate.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_FACTION, goData.FactionTemplate.Value);
			}
			if (goData.Level.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_LEVEL, goData.Level.Value);
			}
			if (goData.State.HasValue || goData.TypeID.HasValue || goData.ArtKit.HasValue || goData.PercentHealth.HasValue)
			{
				if (goData.State.HasValue)
				{
					m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_BYTES_1, (byte)goData.State.Value);
				}
				if (goData.TypeID.HasValue)
				{
					m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_BYTES_1, (byte)goData.TypeID.Value, 1);
				}
				if (goData.ArtKit.HasValue)
				{
					m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_BYTES_1, goData.ArtKit.Value, 2);
				}
				if (goData.PercentHealth.HasValue)
				{
					m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_BYTES_1, goData.PercentHealth.Value, 3);
				}
			}
			if (goData.SpellVisualID.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_SPELL_VISUAL_ID, goData.SpellVisualID.Value);
			}
			if (goData.StateSpellVisualID.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_STATE_SPELL_VISUAL_ID, goData.StateSpellVisualID.Value);
			}
			if (goData.StateAnimID.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_STATE_ANIM_ID, goData.StateAnimID.Value);
			}
			if (goData.StateAnimKitID.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_STATE_ANIM_KIT_ID, goData.StateAnimKitID.Value);
			}
			for (int num45 = 0; num45 < 4; num45++)
			{
				int startIndex56 = 28;
				if (goData.StateWorldEffectIDs[num45].HasValue)
				{
					m_fields.SetUpdateField(startIndex56 + num45, goData.StateWorldEffectIDs[num45].Value);
				}
			}
			if (goData.CustomParam.HasValue)
			{
				m_fields.SetUpdateField(GameObjectField.GAMEOBJECT_FIELD_CUSTOM_PARAM, goData.CustomParam.Value);
			}
		}
		DynamicObjectData dynData = m_updateData.DynamicObjectData;
		if (dynData != null)
		{
			if (dynData.Caster != null)
			{
				m_fields.SetUpdateField(DynamicObjectField.DYNAMICOBJECT_CASTER, dynData.Caster);
			}
			if (dynData.Type.HasValue)
			{
				m_fields.SetUpdateField(DynamicObjectField.DYNAMICOBJECT_TYPE, dynData.Type.Value);
			}
			if (dynData.SpellXSpellVisualID.HasValue)
			{
				m_fields.SetUpdateField(DynamicObjectField.DYNAMICOBJECT_SPELL_X_SPELL_VISUAL_ID, dynData.SpellXSpellVisualID.Value);
			}
			if (dynData.SpellID.HasValue)
			{
				m_fields.SetUpdateField(DynamicObjectField.DYNAMICOBJECT_SPELLID, dynData.SpellID.Value);
			}
			if (dynData.Radius.HasValue)
			{
				m_fields.SetUpdateField(DynamicObjectField.DYNAMICOBJECT_RADIUS, dynData.Radius.Value);
			}
			if (dynData.CastTime.HasValue)
			{
				m_fields.SetUpdateField(DynamicObjectField.DYNAMICOBJECT_CASTTIME, dynData.CastTime.Value);
			}
		}
		CorpseData corpseData = m_updateData.CorpseData;
		if (corpseData != null)
		{
			if (corpseData.Owner != null)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_OWNER, corpseData.Owner);
			}
			if (corpseData.PartyGUID != null)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_PARTY_GUID, corpseData.PartyGUID);
			}
			if (corpseData.GuildGUID != null)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_GUILD_GUID, corpseData.GuildGUID);
			}
			if (corpseData.DisplayID.HasValue)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_DISPLAY_ID, corpseData.DisplayID.Value);
			}
			for (int num46 = 0; num46 < 19; num46++)
			{
				int startIndex57 = 20;
				if (corpseData.Items[num46].HasValue)
				{
					m_fields.SetUpdateField(startIndex57 + num46, corpseData.Items[num46].Value);
				}
			}
			if (corpseData.RaceId.HasValue || corpseData.SexId.HasValue || corpseData.ClassId.HasValue)
			{
				if (corpseData.RaceId.HasValue)
				{
					m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_BYTES_1, corpseData.RaceId.Value);
				}
				if (corpseData.SexId.HasValue)
				{
					m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_BYTES_1, corpseData.SexId.Value, 1);
				}
				if (corpseData.ClassId.HasValue)
				{
					m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_BYTES_1, corpseData.ClassId.Value, 2);
				}
			}
			if (corpseData.Flags.HasValue)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_FLAGS, corpseData.Flags.Value);
			}
			if (corpseData.DynamicFlags.HasValue)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_DYNAMIC_FLAGS, corpseData.DynamicFlags.Value);
			}
			if (corpseData.FactionTemplate.HasValue)
			{
				m_fields.SetUpdateField(CorpseField.CORPSE_FIELD_FACTION_TEMPLATE, corpseData.FactionTemplate.Value);
			}
			for (int num47 = 0; num47 < 35; num47++)
			{
				int startIndex58 = 43;
				int sizePerEntry15 = 2;
				if (corpseData.Customizations[num47] != null)
				{
					m_fields.SetUpdateField(startIndex58 + num47 * sizePerEntry15, corpseData.Customizations[num47].ChrCustomizationOptionID);
					m_fields.SetUpdateField(startIndex58 + num47 * sizePerEntry15 + 1, corpseData.Customizations[num47].ChrCustomizationChoiceID);
				}
			}
		}
		m_alreadyWritten = true;
	}
}
