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
using HermesProxy.World.Enums.V2_4_3_8606;

namespace HermesProxy;

public static class LegacyVersion
{
	private static readonly Dictionary<uint, HermesProxy.World.Enums.Opcode> CurrentToUniversalOpcodeDictionary;

	private static readonly Dictionary<HermesProxy.World.Enums.Opcode, uint> UniversalToCurrentOpcodeDictionary;

	private static readonly Dictionary<Type, SortedList<int, UpdateFieldInfo>> UpdateFieldDictionary;

	private static readonly Dictionary<Type, Dictionary<string, int>> UpdateFieldNameDictionary;

	public static byte ExpansionVersion { get; private set; }

	public static byte MajorVersion { get; private set; }

	public static byte MinorVersion { get; private set; }

	public static ClientVersionBuild Build { get; private set; }

	public static int BuildInt => (int)LegacyVersion.Build;

	public static string VersionString => LegacyVersion.Build.ToString();

	static LegacyVersion()
	{
		LegacyVersion.CurrentToUniversalOpcodeDictionary = new Dictionary<uint, HermesProxy.World.Enums.Opcode>();
		LegacyVersion.UniversalToCurrentOpcodeDictionary = new Dictionary<HermesProxy.World.Enums.Opcode, uint>();
		LegacyVersion.Build = Settings.ServerBuild;
		LegacyVersion.ExpansionVersion = LegacyVersion.GetExpansionVersion();
		LegacyVersion.MajorVersion = LegacyVersion.GetMajorPatchVersion();
		LegacyVersion.MinorVersion = LegacyVersion.GetMinorPatchVersion();
		LegacyVersion.UpdateFieldDictionary = new Dictionary<Type, SortedList<int, UpdateFieldInfo>>();
		LegacyVersion.UpdateFieldNameDictionary = new Dictionary<Type, Dictionary<string, int>>();
		if (!LegacyVersion.LoadUFDictionariesInto(LegacyVersion.UpdateFieldDictionary, LegacyVersion.UpdateFieldNameDictionary))
		{
			Log.Print(LogType.Error, "Could not load update fields for current legacy version.", ".cctor", "F:\\Ampps\\HermesProxy-master\\HermesProxy\\VersionChecker.cs");
		}
		if (!LegacyVersion.LoadOpcodeDictionaries())
		{
			Log.Print(LogType.Error, "Could not load opcodes for current legacy version.", ".cctor", "F:\\Ampps\\HermesProxy-master\\HermesProxy\\VersionChecker.cs");
		}
	}

	private static bool LoadOpcodeDictionaries()
	{
		Type enumType = Opcodes.GetOpcodesEnumForVersion(LegacyVersion.Build);
		if (enumType == null)
		{
			return false;
		}
		foreach (object item in Enum.GetValues(enumType))
		{
			string oldOpcodeName = Enum.GetName(enumType, item);
			HermesProxy.World.Enums.Opcode universalOpcode = Opcodes.GetUniversalOpcode(oldOpcodeName);
			if (universalOpcode == HermesProxy.World.Enums.Opcode.MSG_NULL_ACTION && oldOpcodeName != "MSG_NULL_ACTION")
			{
				Log.Print(LogType.Error, "Opcode " + oldOpcodeName + " is missing from the universal opcode enum!", "LoadOpcodeDictionaries", "F:\\Ampps\\HermesProxy-master\\HermesProxy\\VersionChecker.cs");
				continue;
			}
			LegacyVersion.CurrentToUniversalOpcodeDictionary.Add((uint)item, universalOpcode);
			LegacyVersion.UniversalToCurrentOpcodeDictionary.Add(universalOpcode, (uint)item);
		}
		if (LegacyVersion.CurrentToUniversalOpcodeDictionary.Count < 1)
		{
			return false;
		}
		Log.Print(LogType.Server, $"Loaded {LegacyVersion.CurrentToUniversalOpcodeDictionary.Count} legacy opcodes.", "LoadOpcodeDictionaries", "F:\\Ampps\\HermesProxy-master\\HermesProxy\\VersionChecker.cs");
		return true;
	}

	public static HermesProxy.World.Enums.Opcode GetUniversalOpcode(uint opcode)
	{
		if (LegacyVersion.CurrentToUniversalOpcodeDictionary.TryGetValue(opcode, out var universalOpcode))
		{
			return universalOpcode;
		}
		return HermesProxy.World.Enums.Opcode.MSG_NULL_ACTION;
	}

	public static uint GetCurrentOpcode(HermesProxy.World.Enums.Opcode universalOpcode)
	{
		if (LegacyVersion.UniversalToCurrentOpcodeDictionary.TryGetValue(universalOpcode, out var opcode))
		{
			return opcode;
		}
		return 0u;
	}

	public static ClientVersionBuild GetUpdateFieldsDefiningBuild()
	{
		return LegacyVersion.GetUpdateFieldsDefiningBuild(LegacyVersion.Build);
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
		Type[] enumTypes = new Type[28]
		{
			typeof(HermesProxy.World.Enums.ObjectField),
			typeof(HermesProxy.World.Enums.ItemField),
			typeof(HermesProxy.World.Enums.ContainerField),
			typeof(AzeriteEmpoweredItemField),
			typeof(AzeriteItemField),
			typeof(HermesProxy.World.Enums.UnitField),
			typeof(HermesProxy.World.Enums.PlayerField),
			typeof(ActivePlayerField),
			typeof(HermesProxy.World.Enums.GameObjectField),
			typeof(HermesProxy.World.Enums.DynamicObjectField),
			typeof(HermesProxy.World.Enums.CorpseField),
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
		ClientVersionBuild ufDefiningBuild = LegacyVersion.GetUpdateFieldsDefiningBuild(LegacyVersion.Build);
		bool loaded = false;
		Type[] array = enumTypes;
		foreach (Type enumType in array)
		{
			string vTypeString = "HermesProxy.World.Enums." + ufDefiningBuild.ToString() + "." + enumType.Name;
			Type vEnumType = Assembly.GetExecutingAssembly().GetType(vTypeString);
			if (vEnumType == null)
			{
				vTypeString = "HermesProxy.World.Enums." + ufDefiningBuild.ToString() + "." + enumType.Name;
				vEnumType = Assembly.GetExecutingAssembly().GetType(vTypeString);
				if (vEnumType == null)
				{
					continue;
				}
			}
			Array vValues = Enum.GetValues(vEnumType);
			string[] vNames = Enum.GetNames(vEnumType);
			SortedList<int, UpdateFieldInfo> result = new SortedList<int, UpdateFieldInfo>(vValues.Length);
			Dictionary<string, int> namesResult = new Dictionary<string, int>(vNames.Length);
			for (int j = 0; j < vValues.Length; j++)
			{
				UpdateFieldType format = (from attribute in enumType.GetMember(vNames[j]).SelectMany((MemberInfo member) => member.GetCustomAttributes(typeof(UpdateFieldAttribute), inherit: false))
					where ((UpdateFieldAttribute)attribute).Version <= LegacyVersion.Build
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
			for (int i2 = 0; i2 < result.Count - 1; i2++)
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
		if (LegacyVersion.UpdateFieldNameDictionary.TryGetValue(typeof(T), out var byNamesDict) && byNamesDict.TryGetValue(field.ToString(), out var fieldValue))
		{
			return fieldValue;
		}
		return -1;
	}

	public static string GetUpdateFieldName<T>(int field)
	{
		if (LegacyVersion.UpdateFieldDictionary.TryGetValue(typeof(T), out var infoDict) && infoDict.Count != 0)
		{
			int index = infoDict.BinarySearch(field);
			if (index >= 0)
			{
				return infoDict.Values[index].Name;
			}
			index = ~index - 1;
			int start = infoDict.Keys[index];
			return infoDict.Values[index].Name + " + " + (field - start);
		}
		return field.ToString(CultureInfo.InvariantCulture);
	}

	public static UpdateFieldInfo GetUpdateFieldInfo<T>(int field)
	{
		if (LegacyVersion.UpdateFieldDictionary.TryGetValue(typeof(T), out var infoDict) && infoDict.Count != 0)
		{
			int index = infoDict.BinarySearch(field);
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
		switch (Opcodes.GetOpcodesDefiningBuild(LegacyVersion.Build))
		{
		case ClientVersionBuild.V1_12_1_5875:
			return typeof(HermesProxy.World.Enums.V1_12_1_5875.ResponseCodes);
		case ClientVersionBuild.V2_4_3_8606:
			return typeof(HermesProxy.World.Enums.V2_4_3_8606.ResponseCodes);
		case ClientVersionBuild.V3_3_5a_12340:
			return typeof(HermesProxy.World.Enums.V3_3_5a_12340.ResponseCodes);
		default:
			return null;
		}
	}

	private static byte GetExpansionVersion()
	{
		string str = LegacyVersion.VersionString;
		str = str.Replace("V", "");
		str = str.Substring(0, str.IndexOf("_"));
		return (byte)uint.Parse(str);
	}

	private static byte GetMajorPatchVersion()
	{
		string str = LegacyVersion.VersionString;
		str = str.Substring(str.IndexOf('_') + 1);
		str = str.Substring(0, str.IndexOf("_"));
		return (byte)uint.Parse(str);
	}

	private static byte GetMinorPatchVersion()
	{
		string str = LegacyVersion.VersionString;
		str = str.Substring(str.IndexOf('_') + 1);
		str = str.Substring(str.IndexOf('_') + 1);
		str = str.Substring(0, str.IndexOf("_"));
		str = new string(str.TakeWhile(char.IsDigit).ToArray());
		return (byte)uint.Parse(str);
	}

	public static bool InVersion(ClientVersionBuild build1, ClientVersionBuild build2)
	{
		return LegacyVersion.AddedInVersion(build1) && LegacyVersion.RemovedInVersion(build2);
	}

	public static bool AddedInVersion(ClientVersionBuild build)
	{
		return LegacyVersion.Build >= build;
	}

	public static bool RemovedInVersion(ClientVersionBuild build)
	{
		return LegacyVersion.Build < build;
	}

	public static int GetPowersCount()
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return 5;
		}
		return 7;
	}

	public static byte GetMaxLevel()
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			return 60;
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return 70;
		}
		return 80;
	}

	public static HitInfo ConvertHitInfoFlags(uint hitInfo)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			return ((HitInfoVanilla)hitInfo).CastFlags<HitInfo>();
		}
		return (HitInfo)hitInfo;
	}

	public static uint ConvertSpellCastResult(uint result)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			Type typeFromHandle = typeof(SpellCastResultClassic);
			SpellCastResultWotLK spellCastResultWotLK = (SpellCastResultWotLK)result;
			return (uint)Enum.Parse(typeFromHandle, spellCastResultWotLK.ToString());
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			Type typeFromHandle2 = typeof(SpellCastResultClassic);
			SpellCastResultTBC spellCastResultTBC = (SpellCastResultTBC)result;
			return (uint)Enum.Parse(typeFromHandle2, spellCastResultTBC.ToString());
		}
		Type typeFromHandle3 = typeof(SpellCastResultClassic);
		SpellCastResultVanilla spellCastResultVanilla = (SpellCastResultVanilla)result;
		return (uint)Enum.Parse(typeFromHandle3, spellCastResultVanilla.ToString());
	}

	public static QuestGiverStatusModern ConvertQuestGiverStatus(byte status)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			Type typeFromHandle = typeof(QuestGiverStatusModern);
			QuestGiverStatusWotLK questGiverStatusWotLK = (QuestGiverStatusWotLK)status;
			return (QuestGiverStatusModern)Enum.Parse(typeFromHandle, questGiverStatusWotLK.ToString());
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			Type typeFromHandle2 = typeof(QuestGiverStatusModern);
			QuestGiverStatusTBC questGiverStatusTBC = (QuestGiverStatusTBC)status;
			return (QuestGiverStatusModern)Enum.Parse(typeFromHandle2, questGiverStatusTBC.ToString());
		}
		Type typeFromHandle3 = typeof(QuestGiverStatusModern);
		QuestGiverStatusVanilla questGiverStatusVanilla = (QuestGiverStatusVanilla)status;
		return (QuestGiverStatusModern)Enum.Parse(typeFromHandle3, questGiverStatusVanilla.ToString());
	}

	public static InventoryResult ConvertInventoryResult(uint result)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			Type typeFromHandle = typeof(InventoryResult);
			InventoryResultVanilla inventoryResultVanilla = (InventoryResultVanilla)result;
			return (InventoryResult)Enum.Parse(typeFromHandle, inventoryResultVanilla.ToString());
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			Type typeFromHandle2 = typeof(InventoryResult);
			InventoryResultTBC inventoryResultTBC = (InventoryResultTBC)result;
			return (InventoryResult)Enum.Parse(typeFromHandle2, inventoryResultTBC.ToString());
		}
		return (InventoryResult)result;
	}

	public static int GetQuestLogSize()
	{
		return LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 25 : 20;
	}

	public static int GetAuraSlotsCount()
	{
		return LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 56 : 48;
	}
}
