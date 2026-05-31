using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Framework;
using Framework.Logging;
using HermesProxy.Enums;
using HermesProxy.World.Enums;
using HermesProxy.World.Enums.V2_5_2_39570;
using ActivePlayerDynamicField = HermesProxy.World.Enums.ActivePlayerDynamicField;
using ActivePlayerField = HermesProxy.World.Enums.ActivePlayerField;
using AreaTriggerDynamicField = HermesProxy.World.Enums.AreaTriggerDynamicField;
using AreaTriggerField = HermesProxy.World.Enums.AreaTriggerField;
using ContainerDynamicField = HermesProxy.World.Enums.ContainerDynamicField;
using ContainerField = HermesProxy.World.Enums.ContainerField;
using ConversationDynamicField = HermesProxy.World.Enums.ConversationDynamicField;
using ConversationField = HermesProxy.World.Enums.ConversationField;
using CorpseDynamicField = HermesProxy.World.Enums.CorpseDynamicField;
using CorpseField = HermesProxy.World.Enums.CorpseField;
using DynamicObjectDynamicField = HermesProxy.World.Enums.DynamicObjectDynamicField;
using DynamicObjectField = HermesProxy.World.Enums.DynamicObjectField;
using GameObjectDynamicField = HermesProxy.World.Enums.GameObjectDynamicField;
using GameObjectField = HermesProxy.World.Enums.GameObjectField;
using ItemDynamicField = HermesProxy.World.Enums.ItemDynamicField;
using ItemField = HermesProxy.World.Enums.ItemField;
using ObjectDynamicField = HermesProxy.World.Enums.ObjectDynamicField;
using ObjectField = HermesProxy.World.Enums.ObjectField;
using Opcode = HermesProxy.World.Enums.Opcode;
using PlayerDynamicField = HermesProxy.World.Enums.PlayerDynamicField;
using PlayerField = HermesProxy.World.Enums.PlayerField;
using SceneObjectDynamicField = HermesProxy.World.Enums.SceneObjectDynamicField;
using SceneObjectField = HermesProxy.World.Enums.SceneObjectField;
using UnitDynamicField = HermesProxy.World.Enums.UnitDynamicField;
using UnitField = HermesProxy.World.Enums.UnitField;

namespace HermesProxy;

public static class ModernVersion
{
	private static readonly Dictionary<uint, Opcode> CurrentToUniversalOpcodeDictionary;

	private static readonly Dictionary<Opcode, uint> UniversalToCurrentOpcodeDictionary;

	private static readonly Dictionary<Type, SortedList<int, UpdateFieldInfo>> UpdateFieldDictionary;

	private static readonly Dictionary<Type, Dictionary<string, int>> UpdateFieldNameDictionary;

	public static byte ExpansionVersion { get; }

	public static byte MajorVersion { get; }

	public static byte MinorVersion { get; }

	private static ClientVersionBuild Build { get; }

	public static int BuildInt => (int)Build;

	private static string VersionString => Build.ToString();

	static ModernVersion()
	{
		CurrentToUniversalOpcodeDictionary = new Dictionary<uint, Opcode>();
		UniversalToCurrentOpcodeDictionary = new Dictionary<Opcode, uint>();
		Build = Settings.ClientBuild;
		ExpansionVersion = GetExpansionVersion();
		MajorVersion = GetMajorPatchVersion();
		MinorVersion = GetMinorPatchVersion();
		UpdateFieldDictionary = new Dictionary<Type, SortedList<int, UpdateFieldInfo>>();
		UpdateFieldNameDictionary = new Dictionary<Type, Dictionary<string, int>>();
		if (!LoadUFDictionariesInto(UpdateFieldDictionary, UpdateFieldNameDictionary))
		{
			Log.Print(LogType.Error, "Could not load update fields for current modern version.", ".cctor", "VersionChecker.cs");
		}
		if (!LoadOpcodeDictionaries())
		{
			Log.Print(LogType.Error, "Could not load opcodes for current modern version.", ".cctor", "VersionChecker.cs");
		}
	}

	private static bool LoadOpcodeDictionaries()
	{
		var enumType = Opcodes.GetOpcodesEnumForVersion(Build);
		if (enumType == null)
		{
			return false;
		}
		foreach (var oldOpcodeName in Enum.GetNames(enumType))
		{
			var item = Enum.Parse(enumType, oldOpcodeName);
			var opcodeValue = (uint)item;
			if (opcodeValue == 0 && oldOpcodeName != "MSG_NULL_ACTION")
			{
				continue;
			}
			var universalOpcode = Opcodes.GetUniversalOpcode(oldOpcodeName);
			if (universalOpcode == Opcode.UNKNOWN_SMSG && oldOpcodeName != "MSG_NULL_ACTION")
			{
				Log.Print(LogType.Error, "Opcode " + oldOpcodeName + " is missing from the universal opcode enum!", "VersionChecker.cs");
				continue;
			}
			if (!CurrentToUniversalOpcodeDictionary.ContainsKey(opcodeValue))
			{
				CurrentToUniversalOpcodeDictionary.Add(opcodeValue, universalOpcode);
			}
			if (!UniversalToCurrentOpcodeDictionary.ContainsKey(universalOpcode))
			{
				UniversalToCurrentOpcodeDictionary.Add(universalOpcode, opcodeValue);
			}
		}
		if (CurrentToUniversalOpcodeDictionary.Count < 1)
		{
			return false;
		}
		Log.Print(LogType.Server, $"Loaded {CurrentToUniversalOpcodeDictionary.Count} modern opcodes ({UniversalToCurrentOpcodeDictionary.Count} universal mappings).", "VersionChecker.cs");
		return true;
	}

	public static Opcode GetUniversalOpcode(uint opcode)
	{
		if (CurrentToUniversalOpcodeDictionary.TryGetValue(opcode, out var universalOpcode))
		{
			return universalOpcode;
		}
		return Opcode.UNKNOWN_SMSG;
	}

	public static uint GetCurrentOpcode(Opcode universalOpcode)
	{
		if (UniversalToCurrentOpcodeDictionary.TryGetValue(universalOpcode, out var opcode))
		{
			return opcode;
		}
		return 0u;
	}

	public static ClientVersionBuild GetUpdateFieldsDefiningBuild()
	{
		return GetUpdateFieldsDefiningBuild(Build);
	}

	public static ClientVersionBuild GetUpdateFieldsDefiningBuild(ClientVersionBuild version)
	{
		switch (version)
		{
		case ClientVersionBuild.V1_14_0_39802:
		case ClientVersionBuild.V1_14_0_39958:
		case ClientVersionBuild.V1_14_0_40140:
		case ClientVersionBuild.V1_14_0_40179:
		case ClientVersionBuild.V1_14_0_40237:
		case ClientVersionBuild.V1_14_0_40347:
		case ClientVersionBuild.V1_14_0_40441:
		case ClientVersionBuild.V1_14_0_40618:
			return ClientVersionBuild.V1_14_0_40237;
		case ClientVersionBuild.V1_14_1_40487:
		case ClientVersionBuild.V1_14_1_40594:
		case ClientVersionBuild.V1_14_1_40666:
		case ClientVersionBuild.V1_14_1_40688:
		case ClientVersionBuild.V1_14_1_40800:
		case ClientVersionBuild.V1_14_1_40818:
		case ClientVersionBuild.V1_14_1_40926:
		case ClientVersionBuild.V1_14_1_40962:
		case ClientVersionBuild.V1_14_1_41009:
		case ClientVersionBuild.V1_14_1_41030:
		case ClientVersionBuild.V1_14_1_41077:
		case ClientVersionBuild.V1_14_1_41137:
		case ClientVersionBuild.V1_14_1_41243:
		case ClientVersionBuild.V1_14_1_41511:
		case ClientVersionBuild.V1_14_1_41794:
		case ClientVersionBuild.V1_14_2_41858:
		case ClientVersionBuild.V1_14_2_41959:
		case ClientVersionBuild.V1_14_1_42032:
		case ClientVersionBuild.V1_14_2_42065:
		case ClientVersionBuild.V1_14_2_42082:
		case ClientVersionBuild.V1_14_2_42214:
		case ClientVersionBuild.V1_14_2_42597:
			return ClientVersionBuild.V1_14_1_40688;
		case ClientVersionBuild.V2_5_2_39570:
		case ClientVersionBuild.V2_5_2_39618:
		case ClientVersionBuild.V2_5_2_39926:
		case ClientVersionBuild.V2_5_2_40011:
		case ClientVersionBuild.V2_5_2_40045:
		case ClientVersionBuild.V2_5_2_40203:
		case ClientVersionBuild.V2_5_2_40260:
		case ClientVersionBuild.V2_5_2_40422:
		case ClientVersionBuild.V2_5_2_40488:
		case ClientVersionBuild.V2_5_2_40617:
		case ClientVersionBuild.V2_5_2_40892:
		case ClientVersionBuild.V2_5_2_41446:
		case ClientVersionBuild.V2_5_2_41510:
			return ClientVersionBuild.V2_5_2_39570;
		case ClientVersionBuild.V2_5_3_41402:
		case ClientVersionBuild.V2_5_3_41531:
		case ClientVersionBuild.V2_5_3_41750:
		case ClientVersionBuild.V2_5_3_41812:
		case ClientVersionBuild.V2_5_3_42083:
		case ClientVersionBuild.V2_5_3_42328:
		case ClientVersionBuild.V2_5_3_42598:
			return ClientVersionBuild.V2_5_3_41750;
		case ClientVersionBuild.V3_4_3_54261:
			return ClientVersionBuild.V3_4_3_54261;
		default:
			return ClientVersionBuild.Zero;
		}
	}

	private static bool LoadUFDictionariesInto(Dictionary<Type, SortedList<int, UpdateFieldInfo>> dicts, Dictionary<Type, Dictionary<string, int>> nameToValueDict)
	{
		var enumTypes = new Type[28]
		{
			typeof(ObjectField),
			typeof(ItemField),
			typeof(ContainerField),
			typeof(AzeriteEmpoweredItemField),
			typeof(AzeriteItemField),
			typeof(UnitField),
			typeof(PlayerField),
			typeof(ActivePlayerField),
			typeof(GameObjectField),
			typeof(DynamicObjectField),
			typeof(CorpseField),
			typeof(AreaTriggerField),
			typeof(SceneObjectField),
			typeof(ConversationField),
			typeof(ObjectDynamicField),
			typeof(ItemDynamicField),
			typeof(ContainerDynamicField),
			typeof(AzeriteEmpoweredItemDynamicField),
			typeof(AzeriteItemDynamicField),
			typeof(UnitDynamicField),
			typeof(PlayerDynamicField),
			typeof(ActivePlayerDynamicField),
			typeof(GameObjectDynamicField),
			typeof(DynamicObjectDynamicField),
			typeof(CorpseDynamicField),
			typeof(AreaTriggerDynamicField),
			typeof(SceneObjectDynamicField),
			typeof(ConversationDynamicField)
		};
		var ufDefiningBuild = GetUpdateFieldsDefiningBuild(Build);
		var loaded = false;
		var array = enumTypes;
		foreach (var enumType in array)
		{
			var vTypeString = "HermesProxy.World.Enums." + ufDefiningBuild + "." + enumType.Name;
			var vEnumType = Assembly.GetExecutingAssembly().GetType(vTypeString);
			if (vEnumType == null)
			{
				vTypeString = "HermesProxy.World.Enums." + ufDefiningBuild + "." + enumType.Name;
				vEnumType = Assembly.GetExecutingAssembly().GetType(vTypeString);
				if (vEnumType == null)
				{
					continue;
				}
			}
			var vValues = Enum.GetValues(vEnumType);
			var vNames = Enum.GetNames(vEnumType);
			var result = new SortedList<int, UpdateFieldInfo>(vValues.Length);
			var namesResult = new Dictionary<string, int>(vNames.Length);
			for (var j = 0; j < vValues.Length; j++)
			{
				var format = (from attribute in enumType.GetMember(vNames[j]).SelectMany(member => member.GetCustomAttributes(typeof(UpdateFieldAttribute), inherit: false))
					where ((UpdateFieldAttribute)attribute).Version <= Build
					orderby ((UpdateFieldAttribute)attribute).Version descending
					select ((UpdateFieldAttribute)attribute).UFAttribute).DefaultIfEmpty(UpdateFieldType.Default).First();
				result.Add((int)vValues.GetValue(j), new UpdateFieldInfo
				{
					Value = (int)vValues.GetValue(j),
					Name = vNames[j],
					Size = 0,
					Format = format
				});
				namesResult.Add(vNames[j], (int)vValues.GetValue(j));
			}
			for (var i2 = 0; i2 < result.Count - 1; i2++)
			{
				result.Values[i2].Size = result.Keys[i2 + 1] - result.Keys[i2];
			}
			dicts.Add(enumType, result);
			nameToValueDict.Add(enumType, namesResult);
			loaded = true;
		}
		return loaded;
	}

	public static int GetUpdateField<T>(T field)
	{
		if (UpdateFieldNameDictionary.TryGetValue(typeof(T), out var byNamesDict) && byNamesDict.TryGetValue(field.ToString(), out var fieldValue))
		{
			return fieldValue;
		}
		return -1;
	}

	public static string GetUpdateFieldName<T>(int field)
	{
		if (UpdateFieldDictionary.TryGetValue(typeof(T), out var infoDict) && infoDict.Count != 0)
		{
			var index = infoDict.BinarySearch(field);
			if (index >= 0)
			{
				return infoDict.Values[index].Name;
			}
			index = ~index - 1;
			var start = infoDict.Keys[index];
			return infoDict.Values[index].Name + " + " + (field - start);
		}
		return field.ToString(CultureInfo.InvariantCulture);
	}

	public static UpdateFieldInfo GetUpdateFieldInfo<T>(int field)
	{
		if (UpdateFieldDictionary.TryGetValue(typeof(T), out var infoDict) && infoDict.Count != 0)
		{
			var index = infoDict.BinarySearch(field);
			if (index >= 0)
			{
				return infoDict.Values[index];
			}
			return infoDict.Values[~index - 1];
		}
		return null;
	}

	public static Type GetResponseCodesEnum()
	{
		switch (Opcodes.GetOpcodesDefiningBuild(Build))
		{
		case ClientVersionBuild.V2_5_2_39570:
			return typeof(ResponseCodes);
		case ClientVersionBuild.V1_14_1_40688:
		case ClientVersionBuild.V2_5_3_41750:
			return typeof(World.Enums.V1_14_1_40688.ResponseCodes);
		case ClientVersionBuild.V3_4_3_54261:
			return typeof(World.Enums.V3_4_3_54261.ResponseCodes);
		default:
			return null;
		}
	}

	private static byte GetExpansionVersion()
	{
		var str = VersionString;
		str = str.Replace("V", "");
		str = str.Substring(0, str.IndexOf("_", StringComparison.Ordinal));
		return (byte)uint.Parse(str);
	}

	private static byte GetMajorPatchVersion()
	{
		var str = VersionString;
		str = str.Substring(str.IndexOf('_') + 1);
		str = str.Substring(0, str.IndexOf("_", StringComparison.Ordinal));
		return (byte)uint.Parse(str);
	}

	private static byte GetMinorPatchVersion()
	{
		var str = VersionString;
		str = str.Substring(str.IndexOf('_') + 1);
		str = str.Substring(str.IndexOf('_') + 1);
		str = str.Substring(0, str.IndexOf("_", StringComparison.Ordinal));
		str = new string(str.TakeWhile(char.IsDigit).ToArray());
		return (byte)uint.Parse(str);
	}

	public static bool AddedInVersion(byte expansion, byte major, byte minor)
	{
		if (ExpansionVersion < expansion)
		{
			return false;
		}
		if (ExpansionVersion > expansion)
		{
			return true;
		}
		if (MajorVersion < major)
		{
			return false;
		}
		if (MajorVersion > major)
		{
			return true;
		}
		return MinorVersion >= minor;
	}

	public static bool AddedInVersion(byte retailExpansion, byte retailMajor, byte retailMinor, byte classicEraExpansion, byte classicEraMajor, byte classicEraMinor, byte classicExpansion, byte classicMajor, byte classicMinor)
	{
		if (ExpansionVersion == 1)
		{
			return AddedInVersion(classicEraExpansion, classicEraMajor, classicEraMinor);
		}
		if (ExpansionVersion == 2 || ExpansionVersion == 3)
		{
			return AddedInVersion(classicExpansion, classicMajor, classicMinor);
		}
		return AddedInVersion(retailExpansion, retailMajor, retailMinor);
	}

	public static bool RemovedInVersion(byte retailExpansion, byte retailMajor, byte retailMinor, byte classicEraExpansion, byte classicEraMajor, byte classicEraMinor, byte classicExpansion, byte classicMajor, byte classicMinor)
	{
		return !AddedInVersion(retailExpansion, retailMajor, retailMinor, classicEraExpansion, classicEraMajor, classicEraMinor, classicExpansion, classicMajor, classicMinor);
	}

	public static bool AddedInClassicVersion(byte classicEraExpansion, byte classicEraMajor, byte classicEraMinor, byte classicExpansion, byte classicMajor, byte classicMinor)
	{
		if (ExpansionVersion == 1)
		{
			return AddedInVersion(classicEraExpansion, classicEraMajor, classicEraMinor);
		}
		if (ExpansionVersion == 2 || ExpansionVersion == 3)
		{
			return AddedInVersion(classicExpansion, classicMajor, classicMinor);
		}
		return false;
	}

	public static bool RemovedInClassicVersion(byte classicEraExpansion, byte classicEraMajor, byte classicEraMinor, byte classicExpansion, byte classicMajor, byte classicMinor)
	{
		return !AddedInClassicVersion(classicEraExpansion, classicEraMajor, classicEraMinor, classicExpansion, classicMajor, classicMinor);
	}

	public static bool IsVersion(byte expansion, byte major, byte minor)
	{
		return ExpansionVersion == expansion && MajorVersion == major && MinorVersion == minor;
	}

	public static bool InVersion(ClientVersionBuild build1, ClientVersionBuild build2)
	{
		return AddedInVersion(build1) && RemovedInVersion(build2);
	}

	public static bool AddedInVersion(ClientVersionBuild build)
	{
		return Build >= build;
	}

	public static bool RemovedInVersion(ClientVersionBuild build)
	{
		return Build < build;
	}

	public static bool IsClassicVersionBuild()
	{
		return (ExpansionVersion == 1 && MajorVersion >= 13) || (ExpansionVersion == 2 && MajorVersion >= 5) || (ExpansionVersion == 3 && MajorVersion >= 4);
	}

	public static int GetAccountDataCount()
	{
		if (ExpansionVersion == 1 && MajorVersion >= 14)
		{
			if (AddedInVersion(1, 14, 1))
			{
				return 13;
			}
			return 10;
		}
		if (ExpansionVersion == 2 && MajorVersion >= 5)
		{
			if (AddedInVersion(2, 5, 3))
			{
				return 13;
			}
		}
		else
		{
			if (ExpansionVersion == 3 && MajorVersion >= 4)
			{
				return 15;
			}
			if (!IsClassicVersionBuild())
			{
				if (AddedInVersion(9, 2, 0))
				{
					return 13;
				}
				if (AddedInVersion(9, 1, 5))
				{
					return 12;
				}
			}
		}
		return 8;
	}

	public static int GetPowerCountForClientVersion()
	{
		if (IsClassicVersionBuild())
		{
			if (AddedInClassicVersion(1, 14, 1, 2, 5, 3))
			{
				return 7;
			}
			return 6;
		}
		if (RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return 5;
		}
		if (RemovedInVersion(ClientVersionBuild.V4_0_6_13596))
		{
			return 7;
		}
		if (RemovedInVersion(ClientVersionBuild.V6_0_2_19033))
		{
			return 5;
		}
		if (RemovedInVersion(ClientVersionBuild.V9_1_5_40772))
		{
			return 6;
		}
		return 7;
	}

	public static uint GetGameObjectStateAnimId()
	{
		if (IsVersion(1, 14, 0) || IsVersion(2, 5, 2))
		{
			return 1556u;
		}
		if (IsVersion(1, 14, 1))
		{
			return 1618u;
		}
		if (IsVersion(1, 14, 2) || IsVersion(2, 5, 3))
		{
			return 1672u;
		}
		if (IsVersion(3, 4, 3))
		{
			return 1772u;
		}
		return 0u;
	}

	/// <summary>
	/// Converts modern 3.4.3 inventory slot indices to legacy 3.3.5a slot indices.
	/// TC343 slot layout → AzerothCore slot layout:
	///   Equipment:  0-18  → 0-18   (no change)
	///   Bags:       30-33 → 19-22  (offset 11)
	///   Backpack:   35-50 → 23-38  (offset 12)
	///   Bank items: 59-86 → 39-66  (offset 20)
	///   Bank bags:  87-93 → 67-73  (offset 20)
	///   Buyback:    94-105 → 74-85 (offset 20)
	///   Keyring:    106-137 → 86-117 (offset 20)
	/// </summary>
	public static byte AdjustInventorySlot(byte slot)
	{
		if (slot >= 30 && slot <= 33)
		{
			// Bag slots: modern 30-33 → legacy 19-22
			return (byte)(slot - 11);
		}
		if (slot >= 35 && slot <= 58)
		{
			// Backpack items: modern 35-58 → legacy 23-38 (16 slots base, 24 max)
			return (byte)(slot - 12);
		}
		if (slot >= 59 && slot <= 137)
		{
			// Bank items (59-86), bank bags (87-93), buyback (94-105), keyring (106-137)
			// All shift by 20: modern → legacy
			return (byte)(slot - 20);
		}
		return slot;
	}

	public static void ConvertAuraFlags(ushort oldFlags, byte slot, out AuraFlagsModern newFlags, out uint activeFlags)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			activeFlags = 0u;
			newFlags = AuraFlagsModern.None;
			if (slot >= 32)
			{
				newFlags |= AuraFlagsModern.Negative;
			}
			else
			{
				newFlags |= AuraFlagsModern.Positive;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsVanilla.Cancelable))
			{
				newFlags |= AuraFlagsModern.Cancelable;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsVanilla.EffectIndex0))
			{
				activeFlags |= 1u;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsVanilla.EffectIndex1))
			{
				activeFlags |= 2u;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsVanilla.EffectIndex2))
			{
				activeFlags |= 4u;
			}
			return;
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			activeFlags = 1u;
			newFlags = AuraFlagsModern.None;
			if (oldFlags.HasAnyFlag(AuraFlagsTBC.NotCancelable))
			{
				newFlags |= AuraFlagsModern.Negative;
			}
			else if (oldFlags.HasAnyFlag(AuraFlagsTBC.Cancelable))
			{
				newFlags |= AuraFlagsModern.Cancelable | AuraFlagsModern.Positive;
			}
			else if (slot >= 40)
			{
				newFlags |= AuraFlagsModern.Negative;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsTBC.EffectIndex0))
			{
				activeFlags |= 1u;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsTBC.EffectIndex1))
			{
				activeFlags |= 2u;
			}
			if (oldFlags.HasAnyFlag(AuraFlagsTBC.EffectIndex2))
			{
				activeFlags |= 4u;
			}
			return;
		}
		activeFlags = 0u;
		newFlags = AuraFlagsModern.None;
		if (oldFlags.HasAnyFlag(AuraFlagsWotLK.Negative))
		{
			newFlags |= AuraFlagsModern.Negative;
		}
		else if (oldFlags.HasAnyFlag(AuraFlagsWotLK.Positive))
		{
			newFlags |= AuraFlagsModern.Cancelable | AuraFlagsModern.Positive;
		}
		if (oldFlags.HasAnyFlag(AuraFlagsWotLK.NoCaster))
		{
			newFlags |= AuraFlagsModern.NoCaster;
		}
		if (oldFlags.HasAnyFlag(AuraFlagsWotLK.Duration))
		{
			newFlags |= AuraFlagsModern.Duration;
		}
		if (oldFlags.HasAnyFlag(AuraFlagsWotLK.EffectIndex0))
		{
			activeFlags |= 1u;
		}
		if (oldFlags.HasAnyFlag(AuraFlagsWotLK.EffectIndex1))
		{
			activeFlags |= 2u;
		}
		if (oldFlags.HasAnyFlag(AuraFlagsWotLK.EffectIndex2))
		{
			activeFlags |= 4u;
		}
	}

	public static uint GetArenaTeamSizeFromIndex(uint index)
	{
		return index switch
		{
			0u => 2u, 
			1u => 3u, 
			2u => 5u, 
			_ => 0u, 
		};
	}

	public static uint GetArenaTeamIndexFromSize(uint size)
	{
		return size switch
		{
			2u => 0u, 
			3u => 1u, 
			5u => 2u, 
			_ => 0u, 
		};
	}

	public static byte ConvertResponseCodesValue(byte legacyValue)
	{
		var legacyName = Enum.ToObject(LegacyVersion.GetResponseCodesEnum(), legacyValue).ToString();
		return (byte)Enum.Parse(GetResponseCodesEnum(), legacyName);
	}

	public static byte ConvertSocketColor(byte legacyValue)
	{
		var typeFromHandle = typeof(SocketColorModern);
		var socketColorLegacy = (SocketColorLegacy)legacyValue;
		return (byte)Enum.Parse(typeFromHandle, socketColorLegacy.ToString());
	}
}
