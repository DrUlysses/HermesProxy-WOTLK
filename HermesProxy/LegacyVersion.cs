using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Framework;
using Framework.Logging;
using HermesProxy.Enums;
using HermesProxy.World.Enums;
using HermesProxy.World.Enums.V1_12_1_5875;
using ContainerField = HermesProxy.World.Enums.ContainerField;
using CorpseField = HermesProxy.World.Enums.CorpseField;
using DynamicObjectField = HermesProxy.World.Enums.DynamicObjectField;
using GameObjectField = HermesProxy.World.Enums.GameObjectField;
using ItemField = HermesProxy.World.Enums.ItemField;
using ObjectField = HermesProxy.World.Enums.ObjectField;
using Opcode = HermesProxy.World.Enums.Opcode;
using PlayerField = HermesProxy.World.Enums.PlayerField;
using UnitField = HermesProxy.World.Enums.UnitField;

namespace HermesProxy;

public static class LegacyVersion
{
	private static readonly Dictionary<uint, Opcode> CurrentToUniversalOpcodeDictionary;

	private static readonly Dictionary<Opcode, uint> UniversalToCurrentOpcodeDictionary;

	private static readonly Dictionary<Type, SortedList<int, UpdateFieldInfo>> UpdateFieldDictionary;

	private static readonly Dictionary<Type, Dictionary<string, int>> UpdateFieldNameDictionary;

	public static byte ExpansionVersion { get; private set; }

	public static byte MajorVersion { get; private set; }

	public static byte MinorVersion { get; private set; }

	public static ClientVersionBuild Build { get; private set; }

	public static int BuildInt => (int)Build;

	public static string VersionString => Build.ToString();

	static LegacyVersion()
	{
		CurrentToUniversalOpcodeDictionary = new Dictionary<uint, Opcode>();
		UniversalToCurrentOpcodeDictionary = new Dictionary<Opcode, uint>();
		Build = Settings.ServerBuild;
		ExpansionVersion = GetExpansionVersion();
		MajorVersion = GetMajorPatchVersion();
		MinorVersion = GetMinorPatchVersion();
		UpdateFieldDictionary = new Dictionary<Type, SortedList<int, UpdateFieldInfo>>();
		UpdateFieldNameDictionary = new Dictionary<Type, Dictionary<string, int>>();
		if (!LoadUFDictionariesInto(UpdateFieldDictionary, UpdateFieldNameDictionary))
		{
			Log.Print(LogType.Error, "Could not load update fields for current legacy version.", ".cctor", "VersionChecker.cs");
		}
		if (!LoadOpcodeDictionaries())
		{
			Log.Print(LogType.Error, "Could not load opcodes for current legacy version.", ".cctor", "VersionChecker.cs");
		}
	}

	private static bool LoadOpcodeDictionaries()
	{
		var enumType = Opcodes.GetOpcodesEnumForVersion(Build);
		if (enumType == null)
		{
			return false;
		}
		foreach (var item in Enum.GetValues(enumType))
		{
			var oldOpcodeName = Enum.GetName(enumType, item);
			var universalOpcode = Opcodes.GetUniversalOpcode(oldOpcodeName);
			if (universalOpcode == Opcode.MSG_NULL_ACTION && oldOpcodeName != "MSG_NULL_ACTION")
			{
				Log.Print(LogType.Error, "Opcode " + oldOpcodeName + " is missing from the universal opcode enum!", "VersionChecker.cs");
				continue;
			}
			CurrentToUniversalOpcodeDictionary.Add((uint)item, universalOpcode);
			UniversalToCurrentOpcodeDictionary.Add(universalOpcode, (uint)item);
		}
		if (CurrentToUniversalOpcodeDictionary.Count < 1)
		{
			return false;
		}
		Log.Print(LogType.Server, $"Loaded {CurrentToUniversalOpcodeDictionary.Count} legacy opcodes.", "VersionChecker.cs");
		return true;
	}

	public static Opcode GetUniversalOpcode(uint opcode)
	{
		if (CurrentToUniversalOpcodeDictionary.TryGetValue(opcode, out var universalOpcode))
		{
			return universalOpcode;
		}
		Log.Print(LogType.Warn, $"Unknown legacy opcode 0x{opcode:X4} received from server!", "VersionChecker.cs");
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
		case ClientVersionBuild.V1_12_1_5875:
		case ClientVersionBuild.V1_12_2_6005:
		case ClientVersionBuild.V1_12_3_6141:
			return ClientVersionBuild.V1_12_1_5875;
		case ClientVersionBuild.V2_4_3_8606:
			return ClientVersionBuild.V2_4_3_8606;
		case ClientVersionBuild.V3_3_5a_12340:
			return ClientVersionBuild.V3_3_5a_12340;
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
		case ClientVersionBuild.V1_12_1_5875:
			return typeof(ResponseCodes);
		case ClientVersionBuild.V2_4_3_8606:
			return typeof(World.Enums.V2_4_3_8606.ResponseCodes);
		case ClientVersionBuild.V3_3_5a_12340:
			return typeof(World.Enums.V3_3_5a_12340.ResponseCodes);
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

	public static int GetPowersCount()
	{
		if (RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return 5;
		}
		return 7;
	}

	public static byte GetMaxLevel()
	{
		if (RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			return 60;
		}
		if (RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return 70;
		}
		return 80;
	}

	public static HitInfo ConvertHitInfoFlags(uint hitInfo)
	{
		if (RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return ((HitInfoVanilla)hitInfo).CastFlags<HitInfo>();
		}
		return (HitInfo)hitInfo;
	}

	public static uint ConvertSpellCastResult(uint result)
	{
		if (AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var typeFromHandle = typeof(SpellCastResultClassic);
			var spellCastResultWotLK = (SpellCastResultWotLK)result;
			return (uint)Enum.Parse(typeFromHandle, spellCastResultWotLK.ToString());
		}
		if (AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var typeFromHandle2 = typeof(SpellCastResultClassic);
			var spellCastResultTBC = (SpellCastResultTBC)result;
			return (uint)Enum.Parse(typeFromHandle2, spellCastResultTBC.ToString());
		}
		var typeFromHandle3 = typeof(SpellCastResultClassic);
		var spellCastResultVanilla = (SpellCastResultVanilla)result;
		return (uint)Enum.Parse(typeFromHandle3, spellCastResultVanilla.ToString());
	}

	public static QuestGiverStatusModern ConvertQuestGiverStatus(byte status)
	{
		if (AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var typeFromHandle = typeof(QuestGiverStatusModern);
			var questGiverStatusWotLK = (QuestGiverStatusWotLK)status;
			return (QuestGiverStatusModern)Enum.Parse(typeFromHandle, questGiverStatusWotLK.ToString());
		}
		if (AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var typeFromHandle2 = typeof(QuestGiverStatusModern);
			var questGiverStatusTBC = (QuestGiverStatusTBC)status;
			return (QuestGiverStatusModern)Enum.Parse(typeFromHandle2, questGiverStatusTBC.ToString());
		}
		var typeFromHandle3 = typeof(QuestGiverStatusModern);
		var questGiverStatusVanilla = (QuestGiverStatusVanilla)status;
		return (QuestGiverStatusModern)Enum.Parse(typeFromHandle3, questGiverStatusVanilla.ToString());
	}

	public static InventoryResult ConvertInventoryResult(uint result)
	{
		if (RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var typeFromHandle = typeof(InventoryResult);
			var inventoryResultVanilla = (InventoryResultVanilla)result;
			return (InventoryResult)Enum.Parse(typeFromHandle, inventoryResultVanilla.ToString());
		}
		if (RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var typeFromHandle2 = typeof(InventoryResult);
			var inventoryResultTBC = (InventoryResultTBC)result;
			return (InventoryResult)Enum.Parse(typeFromHandle2, inventoryResultTBC.ToString());
		}
		return (InventoryResult)result;
	}

	public static int GetQuestLogSize()
	{
		return AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 25 : 20;
	}

	public static int GetAuraSlotsCount()
	{
		return AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 56 : 48;
	}
}
