using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework.IO;
using Framework.Logging;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;
using HermesProxy.World.Server.Packets;
using Microsoft.VisualBasic.FileIO;

namespace HermesProxy.World;

public static class GameData
{
    public static readonly Dictionary<uint, Dictionary<string, byte[]>> BuildAuthSeeds = new();

    public static readonly SortedDictionary<uint, BroadcastText> BroadcastTextStore = new();

    public static readonly Dictionary<uint, uint> ItemDisplayIdStore = new();

    public static readonly Dictionary<uint, uint> ItemDisplayIdToFileDataIdStore = new();

    public static readonly Dictionary<uint, ItemSpellsData> ItemSpellsDataStore = new();

    public static readonly Dictionary<uint, ItemRecord> ItemRecordsStore = new();

    public static readonly Dictionary<uint, ItemSparseRecord> ItemSparseRecordsStore = new();

    public static readonly Dictionary<uint, ItemAppearance> ItemAppearanceStore = new();

    public static readonly Dictionary<uint, ItemModifiedAppearance> ItemModifiedAppearanceStore = new();

    public static readonly Dictionary<uint, ItemEffect> ItemEffectStore = new();

    public static readonly Dictionary<uint, Battleground> Battlegrounds = new();

    public static readonly Dictionary<uint, ChatChannel> ChatChannels = new();

    public static readonly Dictionary<uint, Dictionary<uint, byte>> ItemEffects = new();

    public static readonly Dictionary<uint, uint> ItemEnchantVisuals = new();

    public static readonly Dictionary<uint, uint> SpellVisuals = new();

    public static readonly Dictionary<uint, uint> LearnSpells = new();

    public static readonly Dictionary<uint, uint> TotemSpells = new();

    public static readonly Dictionary<uint, uint> Gems = new();

    public static readonly Dictionary<uint, CreatureDisplayInfo> CreatureDisplayInfos = new();

    public static readonly Dictionary<uint, CreatureModelCollisionHeight> CreatureModelCollisionHeights = new();

    public static readonly Dictionary<uint, uint> TransportPeriods = new();

    // Entries from TransportAnimation DB2 — elevators the 3.4.3 client knows about
    public static readonly HashSet<uint> TransportAnimationEntries = new();

    public static readonly Dictionary<uint, string> AreaNames = new();

    public static readonly Dictionary<uint, uint> RaceFaction = new();

    public static readonly HashSet<uint> DispellSpells = new();

    public static readonly Dictionary<uint, List<float>> SpellEffectPoints = new();

    public static readonly HashSet<uint> StackableAuras = new();

    public static readonly HashSet<uint> MountAuras = new();

    public static readonly HashSet<uint> MountSpells = new();

    public static readonly HashSet<uint> NextMeleeSpells = new();

    public static readonly HashSet<uint> AutoRepeatSpells = new();

    public static readonly HashSet<uint> AuraSpells = new();

    public static readonly Dictionary<uint, TaxiPath> TaxiPaths = new();

    public static readonly int[,] TaxiNodesGraph = new int[250, 250];

    public static readonly Dictionary<uint, uint> QuestBits = new();

    public static readonly Dictionary<uint, ItemTemplate> ItemTemplates = new();

    public static readonly Dictionary<uint, CreatureTemplate> CreatureTemplates = new();

    public static readonly Dictionary<uint, QuestTemplate> QuestTemplates = new();

    public static readonly Dictionary<uint, string> ItemNames = new();

    public const uint HotfixAreaTriggerBegin = 100000u;

    public const uint HotfixSkillLineBegin = 110000u;

    public const uint HotfixSkillRaceClassInfoBegin = 120000u;

    public const uint HotfixSkillLineAbilityBegin = 130000u;

    public const uint HotfixSpellBegin = 140000u;

    public const uint HotfixSpellNameBegin = 150000u;

    public const uint HotfixSpellLevelsBegin = 160000u;

    public const uint HotfixSpellAuraOptionsBegin = 170000u;

    public const uint HotfixSpellMiscBegin = 180000u;

    public const uint HotfixSpellEffectBegin = 190000u;

    public const uint HotfixSpellXSpellVisualBegin = 200000u;

    public const uint HotfixItemBegin = 210000u;

    public const uint HotfixItemSparseBegin = 220000u;

    public const uint HotfixItemAppearanceBegin = 230000u;

    public const uint HotfixItemModifiedAppearanceBegin = 240000u;

    public const uint HotfixItemEffectBegin = 250000u;

    public const uint HotfixItemDisplayInfoBegin = 260000u;

    public const uint HotfixCreatureDisplayInfoBegin = 270000u;

    public const uint HotfixCreatureDisplayInfoExtraBegin = 280000u;

    public const uint HotfixCreatureDisplayInfoOptionBegin = 290000u;

    public static readonly Dictionary<uint, HotfixRecord> Hotfixes = new();

    public static void StoreItemName(uint entry, string name)
    {
        ItemNames[entry] = name;
    }

    public static string GetItemName(uint entry)
    {
        if (ItemNames.TryGetValue(entry, out var data))
        {
            return data;
        }

        var template = GetItemTemplate(entry);
        return template != null ? template.Name[0] : "";
    }

    public static void StoreItemTemplate(uint entry, ItemTemplate template)
    {
        ItemTemplates[entry] = template;
    }

    public static ItemTemplate? GetItemTemplate(uint entry)
    {
        return ItemTemplates.TryGetValue(entry, out var data) ? data : null;
    }

    public static void StoreQuestTemplate(uint entry, QuestTemplate template)
    {
        if (QuestTemplates.ContainsKey(entry))
        {
            QuestTemplates[entry] = template;
        }
        else
        {
            QuestTemplates.Add(entry, template);
        }
    }

    public static QuestTemplate? GetQuestTemplate(uint entry)
    {
        return QuestTemplates.TryGetValue(entry, out var data) ? data : null;
    }

    public static QuestObjective? GetQuestObjectiveForItem(uint entry)
    {
        return QuestTemplates.SelectMany(questTemplate => questTemplate.Value.Objectives)
            .FirstOrDefault(objective => objective.ObjectID == entry && objective.Type == QuestObjectiveType.Item);
    }

    public static uint? GetUniqueQuestBit(uint questId)
    {
        if (!QuestBits.TryGetValue(questId, out var result))
        {
            return null;
        }

        return result;
    }

    public static void StoreCreatureTemplate(uint entry, CreatureTemplate template)
    {
        CreatureTemplates[entry] = template;
    }

    public static CreatureTemplate? GetCreatureTemplate(uint entry)
    {
        return CreatureTemplates.TryGetValue(entry, out var data) ? data : null;
    }

    public static uint GetItemDisplayId(uint entry)
    {
        return ItemDisplayIdStore.TryGetValue(entry, out var displayId) ? displayId : 0u;
    }

    public static uint GetItemIdWithDisplayId(uint displayId)
    {
        return (from item in ItemDisplayIdStore where item.Value == displayId select item.Key).FirstOrDefault();
    }

    public static ItemAppearance? GetItemAppearanceByDisplayId(uint displayId)
    {
        return (
            from item in ItemAppearanceStore
            where item.Value.ItemDisplayInfoID == (int)displayId
            select item.Value
        ).FirstOrDefault();
    }

    public static ItemAppearance GetItemAppearanceByItemId(uint itemId)
    {
        var modAppearance = GetItemModifiedAppearanceByItemId(itemId);
        if (modAppearance == null)
        {
            return null;
        }

        if (ItemAppearanceStore.TryGetValue((uint)modAppearance.ItemAppearanceID, out var data))
        {
            return data;
        }

        return null;
    }

    public static uint GetItemIconFileDataIdByDisplayId(uint displayId)
    {
        if (ItemDisplayIdToFileDataIdStore.TryGetValue(displayId, out var fileDataId))
        {
            return fileDataId;
        }

        return 0u;
    }

    public static ItemModifiedAppearance GetItemModifiedAppearanceByDisplayId(uint displayId)
    {
        var appearance = GetItemAppearanceByDisplayId(displayId);
        if (appearance != null)
        {
            foreach (var item in ItemModifiedAppearanceStore)
            {
                if (item.Value.ItemAppearanceID == appearance.Id)
                {
                    return item.Value;
                }
            }
        }

        return null;
    }

    public static ItemModifiedAppearance? GetItemModifiedAppearanceByItemId(uint itemId)
    {
        return (
            from item in ItemModifiedAppearanceStore
            where item.Value.ItemID == (int)itemId
            select item.Value
        ).FirstOrDefault();
    }

    public static ItemEffect? GetItemEffectByItemId(uint itemId, byte slot)
    {
        return (
            from item in ItemEffectStore
            where item.Value.ParentItemID == itemId && item.Value.LegacySlotIndex == slot
            select item.Value
        ).FirstOrDefault();
    }

    public static uint GetFirstFreeId(IDictionary dict, uint after = 0u)
    {
        return (
            from object? item in dict
            let type = item?.GetType()
            let key = type.GetProperty("Key")
            select key.GetValue(item,
                null)
            into keyObj
            where after == 0 || (uint)keyObj > after
            select (uint)keyObj
        ).FirstOrDefault();
    }

    public static void SaveItemEffectSlot(uint itemId, uint spellId, byte slot)
    {
        if (ItemEffects.ContainsKey(itemId))
        {
            if (ItemEffects[itemId].ContainsKey(spellId))
            {
                ItemEffects[itemId][spellId] = slot;
            }
            else
            {
                ItemEffects[itemId].Add(spellId, slot);
            }
        }
        else
        {
            var dict = new Dictionary<uint, byte>();
            dict.Add(spellId, slot);
            ItemEffects.Add(itemId, dict);
        }
    }

    public static byte GetItemEffectSlot(uint itemId, uint spellId)
    {
        if (ItemEffects.ContainsKey(itemId) && ItemEffects[itemId].ContainsKey(spellId))
        {
            return ItemEffects[itemId][spellId];
        }

        return 0;
    }

    public static uint GetItemEnchantVisual(uint enchantId)
    {
        if (ItemEnchantVisuals.TryGetValue(enchantId, out var visualId))
        {
            return visualId;
        }

        return 0u;
    }

    public static uint GetSpellVisual(uint spellId)
    {
        if (SpellVisuals.TryGetValue(spellId, out var visual))
        {
            return visual;
        }

        return 0u;
    }

    public static uint GetSpellIdFromVisual(uint visualId)
    {
        foreach (var kvp in SpellVisuals)
        {
            if (kvp.Value == visualId)
                return kvp.Key;
        }

        return 0u;
    }

    public static int GetTotemSlotForSpell(uint spellId)
    {
        if (TotemSpells.TryGetValue(spellId, out var slot))
        {
            return (int)slot;
        }

        return -1;
    }

    public static uint GetRealSpell(uint learnSpellId)
    {
        if (LearnSpells.TryGetValue(learnSpellId, out var realSpellId))
        {
            return realSpellId;
        }

        return learnSpellId;
    }

    public static uint GetGemFromEnchantId(uint enchantId)
    {
        if (Gems.TryGetValue(enchantId, out var itemId))
        {
            return itemId;
        }

        return 0u;
    }

    public static uint GetEnchantIdFromGem(uint itemId)
    {
        foreach (var itr in Gems)
        {
            if (itr.Value == itemId)
            {
                return itr.Key;
            }
        }

        return 0u;
    }

    public static float GetUnitCompleteDisplayScale(uint displayId)
    {
        var displayData = GetDisplayInfo(displayId);
        if (displayData.ModelId == 0)
        {
            return 1f;
        }

        var modelData = GetModelData(displayId);
        return displayData.DisplayScale * modelData.ModelScale;
    }

    public static CreatureDisplayInfo GetDisplayInfo(uint displayId)
    {
        if (CreatureDisplayInfos.TryGetValue(displayId, out var info))
        {
            return info;
        }

        return new CreatureDisplayInfo(0u, 1f);
    }

    public static CreatureModelCollisionHeight GetModelData(uint modelId)
    {
        if (CreatureModelCollisionHeights.TryGetValue(modelId, out var info))
        {
            return info;
        }

        return new CreatureModelCollisionHeight(1f, 0f, 0f);
    }

    public static uint GetTransportPeriod(uint entry)
    {
        if (TransportPeriods.TryGetValue(entry, out var period))
        {
            return period;
        }

        return 0u;
    }

    public static string GetAreaName(uint id)
    {
        if (AreaNames.TryGetValue(id, out var name))
        {
            return name;
        }

        return "";
    }

    public static uint GetFactionForRace(uint race)
    {
        if (RaceFaction.TryGetValue(race, out var faction))
        {
            return faction;
        }

        return 1u;
    }

    public static uint GetBattlegroundIdFromMapId(uint mapId)
    {
        foreach (var bg in Battlegrounds)
        {
            if (bg.Value.MapIds.Contains(mapId))
            {
                return bg.Key;
            }
        }

        return 0u;
    }

    public static uint GetMapIdFromBattlegroundId(uint bgId)
    {
        if (Battlegrounds.TryGetValue(bgId, out var bg))
        {
            return bg.MapIds[0];
        }

        return 0u;
    }

    public static uint GetChatChannelIdFromName(string name)
    {
        foreach (var channel in ChatChannels)
        {
            if (name.Contains(channel.Value.Name))
            {
                return channel.Key;
            }
        }

        return 0u;
    }

    public static List<ChatChannel> GetChatChannelsWithFlags(ChannelFlags flags)
    {
        var channels = new List<ChatChannel>();
        foreach (var channel in ChatChannels)
        {
            if ((channel.Value.Flags & flags) == flags)
            {
                channels.Add(channel.Value);
            }
        }

        return channels;
    }

    public static bool IsAllianceRace(Race raceId)
    {
        switch (raceId)
        {
            case Race.Human:
            case Race.Dwarf:
            case Race.NightElf:
            case Race.Gnome:
            case Race.Draenei:
            case Race.Worgen:
                return true;
            default:
                return false;
        }
    }

    public static bool IsHordeRace(Race raceId)
    {
        switch (raceId)
        {
            case Race.Orc:
            case Race.Undead:
            case Race.Tauren:
            case Race.Troll:
            case Race.Goblin:
            case Race.BloodElf:
                return true;
            default:
                return false;
        }
    }

    public static int GetFactionByRace(Race race)
    {
        if (IsAllianceRace(race))
        {
            return 1;
        }

        if (IsHordeRace(race))
        {
            return 2;
        }

        return 0;
    }

    public static BroadcastText? GetBroadcastText(uint entry)
    {
        return BroadcastTextStore.TryGetValue(entry, out var data) ? data : null;
    }

    public static uint GetBroadcastTextId(string maleText, string femaleText, uint language, ushort[] emoteDelays,
        ushort[] emotes)
    {
        foreach (
            var itr in BroadcastTextStore.Where(itr =>
                ((!string.IsNullOrEmpty(maleText) && itr.Value.MaleText == maleText) ||
                 (!string.IsNullOrEmpty(femaleText) && itr.Value.FemaleText == femaleText)) &&
                itr.Value.Language == language && itr.Value.EmoteDelays.SequenceEqual(emoteDelays) &&
                itr.Value.Emotes.SequenceEqual(emotes))
        )
        {
            return itr.Key;
        }

        var broadcastText = new BroadcastText
        {
            Entry = BroadcastTextStore.Keys.Last() + 1,
            MaleText = maleText,
            FemaleText = femaleText,
            Language = language,
            EmoteDelays = emoteDelays,
            Emotes = emotes
        };
        BroadcastTextStore.Add(broadcastText.Entry, broadcastText);
        return broadcastText.Entry;
    }

    public static void LoadEverything()
    {
        Log.Print(LogType.Storage, "Loading data files...", "GameData.cs");
        LoadBuildAuthSeeds();
        LoadBroadcastTexts();
        LoadItemDisplayIds();
        LoadItemRecords();
        LoadItemSparseRecords();
        LoadItemAppearance();
        LoadItemModifiedAppearance();
        LoadItemEffect();
        LoadItemSpellsData();
        LoadItemDisplayIdToFileDataId();
        LoadBattlegrounds();
        LoadChatChannels();
        LoadItemEnchantVisuals();
        LoadSpellVisuals();
        LoadLearnSpells();
        LoadTotemSpells();
        LoadGems();
        LoadCreatureDisplayInfo();
        LoadCreatureModelCollisionHeights();
        LoadTransports();
        LoadAreaNames();
        LoadRaceFaction();
        LoadDispellSpells();
        LoadSpellEffectPoints();
        LoadStackableAuras();
        LoadMountAuras();
        LoadMountSpells();
        LoadMeleeSpells();
        LoadAutoRepeatSpells();
        LoadAuraSpells();
        LoadTaxiPaths();
        LoadTaxiPathNodesGraph();
        LoadQuestBits();
        LoadHotfixes();
        Log.Print(LogType.Storage, "Finished loading data.", "GameData.cs");
    }

    public static void LoadBuildAuthSeeds()
    {
        var path = Path.Combine("CSV", "BuildAuthSeeds.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var build = uint.Parse(fields[0]);
            var platform = fields[1];
            var seed = fields[2].ParseAsByteArray();
            if (!BuildAuthSeeds.TryGetValue(build, out var seeds))
            {
                seeds = new Dictionary<string, byte[]>();
                BuildAuthSeeds.Add(build, seeds);
            }

            seeds.Add(platform, seed);
        }
    }

    public static void LoadBroadcastTexts()
    {
        var path = Path.Combine("CSV", $"BroadcastTexts{LegacyVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var broadcastText = new BroadcastText
            {
                Entry = uint.Parse(fields[0]),
                MaleText = fields[1].TrimEnd().Replace("\0", "").Replace("~", "\n"),
                FemaleText = fields[2].TrimEnd().Replace("\0", "").Replace("~", "\n"),
                Language = uint.Parse(fields[3]),
                Emotes =
                {
                    [0] = ushort.Parse(fields[4]),
                    [1] = ushort.Parse(fields[5]),
                    [2] = ushort.Parse(fields[6])
                },
                EmoteDelays =
                {
                    [0] = ushort.Parse(fields[7]),
                    [1] = ushort.Parse(fields[8]),
                    [2] = ushort.Parse(fields[9])
                }
            };
            BroadcastTextStore.Add(broadcastText.Entry, broadcastText);
        }
    }

    public static void LoadItemDisplayIds()
    {
        var path = Path.Combine("CSV", $"ItemIdToDisplayId{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var entry = uint.Parse(fields[0]);
            var displayId = uint.Parse(fields[1]);
            ItemDisplayIdStore.Add(entry, displayId);
        }
    }

    public static void LoadItemRecords()
    {
        var path = Path.Combine("CSV", $"Item{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var row = new ItemRecord
            {
                Id = int.Parse(fields[0]),
                ClassId = byte.Parse(fields[1]),
                SubclassId = byte.Parse(fields[2]),
                Material = byte.Parse(fields[3]),
                InventoryType = sbyte.Parse(fields[4]),
                RequiredLevel = int.Parse(fields[5]),
                SheatheType = byte.Parse(fields[6]),
                RandomProperty = ushort.Parse(fields[7]),
                ItemRandomSuffixGroupId = ushort.Parse(fields[8]),
                SoundOverrideSubclassId = sbyte.Parse(fields[9]),
                ScalingStatDistributionId = ushort.Parse(fields[10]),
                IconFileDataId = int.Parse(fields[11]),
                ItemGroupSoundsId = byte.Parse(fields[12]),
                ContentTuningId = int.Parse(fields[13]),
                MaxDurability = uint.Parse(fields[14]),
                AmmoType = byte.Parse(fields[15]),
                DamageType =
                {
                    [0] = byte.Parse(fields[16]),
                    [1] = byte.Parse(fields[17]),
                    [2] = byte.Parse(fields[18]),
                    [3] = byte.Parse(fields[19]),
                    [4] = byte.Parse(fields[20])
                },
                Resistances =
                {
                    [0] = short.Parse(fields[21]),
                    [1] = short.Parse(fields[22]),
                    [2] = short.Parse(fields[23]),
                    [3] = short.Parse(fields[24]),
                    [4] = short.Parse(fields[25]),
                    [5] = short.Parse(fields[26]),
                    [6] = short.Parse(fields[27])
                },
                MinDamage =
                {
                    [0] = ushort.Parse(fields[28]),
                    [1] = ushort.Parse(fields[29]),
                    [2] = ushort.Parse(fields[30]),
                    [3] = ushort.Parse(fields[31]),
                    [4] = ushort.Parse(fields[32])
                },
                MaxDamage =
                {
                    [0] = ushort.Parse(fields[33]),
                    [1] = ushort.Parse(fields[34]),
                    [2] = ushort.Parse(fields[35]),
                    [3] = ushort.Parse(fields[36]),
                    [4] = ushort.Parse(fields[37])
                }
            };
            ItemRecordsStore.Add((uint)row.Id, row);
        }
    }

    public static void LoadItemSparseRecords()
    {
        var path = Path.Combine("CSV", $"ItemSparse{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var row = new ItemSparseRecord
            {
                Id = int.Parse(fields[0]),
                AllowableRace = long.Parse(fields[1]),
                Description = fields[2],
                Name4 = fields[3],
                Name3 = fields[4],
                Name2 = fields[5],
                Name1 = fields[6],
                DmgVariance = float.Parse(fields[7]),
                DurationInInventory = uint.Parse(fields[8]),
                QualityModifier = float.Parse(fields[9]),
                BagFamily = uint.Parse(fields[10]),
                RangeMod = float.Parse(fields[11]),
                StatPercentageOfSocket =
                {
                    [0] = float.Parse(fields[12]),
                    [1] = float.Parse(fields[13]),
                    [2] = float.Parse(fields[14]),
                    [3] = float.Parse(fields[15]),
                    [4] = float.Parse(fields[16]),
                    [5] = float.Parse(fields[17]),
                    [6] = float.Parse(fields[18]),
                    [7] = float.Parse(fields[19]),
                    [8] = float.Parse(fields[20]),
                    [9] = float.Parse(fields[21])
                },
                StatPercentEditor =
                {
                    [0] = int.Parse(fields[22]),
                    [1] = int.Parse(fields[23]),
                    [2] = int.Parse(fields[24]),
                    [3] = int.Parse(fields[25]),
                    [4] = int.Parse(fields[26]),
                    [5] = int.Parse(fields[27]),
                    [6] = int.Parse(fields[28]),
                    [7] = int.Parse(fields[29]),
                    [8] = int.Parse(fields[30]),
                    [9] = int.Parse(fields[31])
                },
                Stackable = int.Parse(fields[32]),
                MaxCount = int.Parse(fields[33]),
                RequiredAbility = uint.Parse(fields[34]),
                SellPrice = uint.Parse(fields[35]),
                BuyPrice = uint.Parse(fields[36]),
                VendorStackCount = uint.Parse(fields[37]),
                PriceVariance = float.Parse(fields[38]),
                PriceRandomValue = float.Parse(fields[39]),
                Flags =
                {
                    [0] = uint.Parse(fields[40]),
                    [1] = uint.Parse(fields[41]),
                    [2] = uint.Parse(fields[42]),
                    [3] = uint.Parse(fields[43])
                },
                OppositeFactionItemId = int.Parse(fields[44]),
                MaxDurability = uint.Parse(fields[45]),
                ItemNameDescriptionId = ushort.Parse(fields[46]),
                RequiredTransmogHoliday = ushort.Parse(fields[47]),
                RequiredHoliday = ushort.Parse(fields[48]),
                LimitCategory = ushort.Parse(fields[49]),
                GemProperties = ushort.Parse(fields[50]),
                SocketMatchEnchantmentId = ushort.Parse(fields[51]),
                TotemCategoryId = ushort.Parse(fields[52]),
                InstanceBound = ushort.Parse(fields[53]),
                ZoneBound =
                {
                    [0] = ushort.Parse(fields[54]),
                    [1] = ushort.Parse(fields[55])
                },
                ItemSet = ushort.Parse(fields[56]),
                LockId = ushort.Parse(fields[57]),
                StartQuestId = ushort.Parse(fields[58]),
                PageText = ushort.Parse(fields[59]),
                Delay = ushort.Parse(fields[60]),
                RequiredReputationId = ushort.Parse(fields[61]),
                RequiredSkillRank = ushort.Parse(fields[62]),
                RequiredSkill = ushort.Parse(fields[63]),
                ItemLevel = ushort.Parse(fields[64]),
                AllowableClass = short.Parse(fields[65]),
                ItemRandomSuffixGroupId = ushort.Parse(fields[66]),
                RandomProperty = ushort.Parse(fields[67]),
                MinDamage =
                {
                    [0] = ushort.Parse(fields[68]),
                    [1] = ushort.Parse(fields[69]),
                    [2] = ushort.Parse(fields[70]),
                    [3] = ushort.Parse(fields[71]),
                    [4] = ushort.Parse(fields[72])
                },
                MaxDamage =
                {
                    [0] = ushort.Parse(fields[73]),
                    [1] = ushort.Parse(fields[74]),
                    [2] = ushort.Parse(fields[75]),
                    [3] = ushort.Parse(fields[76]),
                    [4] = ushort.Parse(fields[77])
                },
                Resistances =
                {
                    [0] = short.Parse(fields[78]),
                    [1] = short.Parse(fields[79]),
                    [2] = short.Parse(fields[80]),
                    [3] = short.Parse(fields[81]),
                    [4] = short.Parse(fields[82]),
                    [5] = short.Parse(fields[83]),
                    [6] = short.Parse(fields[84])
                },
                ScalingStatDistributionId = ushort.Parse(fields[85]),
                ExpansionId = byte.Parse(fields[86]),
                ArtifactId = byte.Parse(fields[87]),
                SpellWeight = byte.Parse(fields[88]),
                SpellWeightCategory = byte.Parse(fields[89]),
                SocketType =
                {
                    [0] = byte.Parse(fields[90]),
                    [1] = byte.Parse(fields[91]),
                    [2] = byte.Parse(fields[92])
                },
                SheatheType = byte.Parse(fields[93]),
                Material = byte.Parse(fields[94]),
                PageMaterial = byte.Parse(fields[95]),
                PageLanguage = byte.Parse(fields[96]),
                Bonding = byte.Parse(fields[97]),
                DamageType = byte.Parse(fields[98]),
                StatType =
                {
                    [0] = sbyte.Parse(fields[99]),
                    [1] = sbyte.Parse(fields[100]),
                    [2] = sbyte.Parse(fields[101]),
                    [3] = sbyte.Parse(fields[102]),
                    [4] = sbyte.Parse(fields[103]),
                    [5] = sbyte.Parse(fields[104]),
                    [6] = sbyte.Parse(fields[105]),
                    [7] = sbyte.Parse(fields[106]),
                    [8] = sbyte.Parse(fields[107]),
                    [9] = sbyte.Parse(fields[108])
                },
                ContainerSlots = byte.Parse(fields[109]),
                RequiredReputationRank = byte.Parse(fields[110]),
                RequiredCityRank = byte.Parse(fields[111]),
                RequiredHonorRank = byte.Parse(fields[112]),
                InventoryType = byte.Parse(fields[113]),
                OverallQualityId = byte.Parse(fields[114]),
                AmmoType = byte.Parse(fields[115]),
                StatModifierBonusAmount =
                {
                    [0] = sbyte.Parse(fields[116]),
                    [1] = sbyte.Parse(fields[117]),
                    [2] = sbyte.Parse(fields[118]),
                    [3] = sbyte.Parse(fields[119]),
                    [4] = sbyte.Parse(fields[120]),
                    [5] = sbyte.Parse(fields[121]),
                    [6] = sbyte.Parse(fields[122]),
                    [7] = sbyte.Parse(fields[123]),
                    [8] = sbyte.Parse(fields[124]),
                    [9] = sbyte.Parse(fields[125])
                },
                RequiredLevel = sbyte.Parse(fields[126])
            };
            ItemSparseRecordsStore.Add((uint)row.Id, row);
        }
    }

    public static void LoadItemAppearance()
    {
        var path = Path.Combine("CSV", $"ItemAppearance{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var appearance = new ItemAppearance
            {
                Id = int.Parse(fields[0]),
                DisplayType = byte.Parse(fields[1]),
                ItemDisplayInfoID = int.Parse(fields[2]),
                DefaultIconFileDataID = int.Parse(fields[3]),
                UiOrder = int.Parse(fields[4])
            };
            ItemAppearanceStore.Add((uint)appearance.Id, appearance);
        }
    }

    public static void LoadItemModifiedAppearance()
    {
        var path = Path.Combine("CSV", $"ItemModifiedAppearance{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var modifiedAppearance = new ItemModifiedAppearance
            {
                Id = int.Parse(fields[0]),
                ItemID = int.Parse(fields[1]),
                ItemAppearanceModifierID = int.Parse(fields[2]),
                ItemAppearanceID = int.Parse(fields[3]),
                OrderIndex = int.Parse(fields[4]),
                TransmogSourceTypeEnum = int.Parse(fields[5])
            };
            ItemModifiedAppearanceStore.Add((uint)modifiedAppearance.Id, modifiedAppearance);
        }
    }

    public static void LoadItemEffect()
    {
        var path = Path.Combine("CSV", $"ItemEffect{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var effect = new ItemEffect
            {
                Id = int.Parse(fields[0]),
                LegacySlotIndex = byte.Parse(fields[1]),
                TriggerType = sbyte.Parse(fields[2]),
                Charges = short.Parse(fields[3]),
                CoolDownMSec = int.Parse(fields[4]),
                CategoryCoolDownMSec = int.Parse(fields[5]),
                SpellCategoryID = ushort.Parse(fields[6]),
                SpellID = int.Parse(fields[7]),
                ChrSpecializationID = ushort.Parse(fields[8]),
                ParentItemID = int.Parse(fields[9])
            };
            ItemEffectStore.Add((uint)effect.Id, effect);
        }
    }

    public static void LoadItemSpellsData()
    {
        var path = Path.Combine("CSV", $"ItemSpellsData{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var data = new ItemSpellsData
            {
                Id = int.Parse(fields[0]),
                Category = int.Parse(fields[1]),
                RecoveryTime = int.Parse(fields[2]),
                CategoryRecoveryTime = int.Parse(fields[3])
            };
            ItemSpellsDataStore.Add((uint)data.Id, data);
        }
    }

    public static void LoadItemDisplayIdToFileDataId()
    {
        var path = Path.Combine("CSV", $"ItemDisplayIdToFileDataId{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var displayId = uint.Parse(fields[0]);
            var fileDataId = uint.Parse(fields[1]);
            ItemDisplayIdToFileDataIdStore.Add(displayId, fileDataId);
        }
    }

    public static void LoadBattlegrounds()
    {
        var path = Path.Combine("CSV", "Battlegrounds.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var bg = new Battleground();
            var bgId = uint.Parse(fields[0]);
            bg.IsArena = byte.Parse(fields[1]) != 0;
            for (var i = 0; i < 6; i++)
            {
                var mapId = uint.Parse(fields[2 + i]);
                if (mapId != 0)
                {
                    bg.MapIds.Add(mapId);
                }
            }

            Battlegrounds.Add(bgId, bg);
        }
    }

    public static void LoadChatChannels()
    {
        var path = Path.Combine("CSV", "ChatChannels.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var channel = new ChatChannel
            {
                Id = uint.Parse(fields[0]),
                Flags = (ChannelFlags)uint.Parse(fields[1]),
                Name = fields[2]
            };
            ChatChannels.Add(channel.Id, channel);
        }
    }

    public static void LoadItemEnchantVisuals()
    {
        var path = Path.Combine("CSV", $"ItemEnchantVisuals{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var enchantId = uint.Parse(fields[0]);
            var visualId = uint.Parse(fields[1]);
            ItemEnchantVisuals.Add(enchantId, visualId);
        }
    }

    public static void LoadSpellVisuals()
    {
        var path = Path.Combine("CSV", $"SpellVisuals{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            var visualId = uint.Parse(fields[1]);
            SpellVisuals.Add(spellId, visualId);
        }
    }

    public static void LoadLearnSpells()
    {
        var path = Path.Combine("CSV", "LearnSpells.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var learnSpellId = uint.Parse(fields[0]);
            var realSpellId = uint.Parse(fields[1]);
            if (!LearnSpells.ContainsKey(learnSpellId))
            {
                LearnSpells.Add(learnSpellId, realSpellId);
            }
        }
    }

    public static void LoadTotemSpells()
    {
        if (LegacyVersion.ExpansionVersion > 1)
        {
            return;
        }

        var path = Path.Combine("CSV", "TotemSpells.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            var totemSlot = uint.Parse(fields[1]);
            TotemSpells.Add(spellId, totemSlot);
        }
    }

    public static void LoadGems()
    {
        if (ModernVersion.ExpansionVersion <= 1)
        {
            return;
        }

        var path = Path.Combine("CSV", $"Gems{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var enchantId = uint.Parse(fields[0]);
            var itemId = uint.Parse(fields[1]);
            Gems.Add(enchantId, itemId);
        }
    }

    public static void LoadCreatureDisplayInfo()
    {
        var path = Path.Combine("CSV", "CreatureDisplayInfo.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var displayId = uint.Parse(fields[0]);
            var modelId = uint.Parse(fields[1]);
            var scale = float.Parse(fields[2]);
            CreatureDisplayInfos.Add(displayId, new CreatureDisplayInfo(modelId, scale));
        }
    }

    public static void LoadCreatureModelCollisionHeights()
    {
        var path = Path.Combine("CSV", $"CreatureModelCollisionHeightsModern{LegacyVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var modelId = uint.Parse(fields[0]);
            var modelScale = float.Parse(fields[1]);
            var collisionHeight = float.Parse(fields[2]);
            var collisionHeightMounted = float.Parse(fields[3]);
            CreatureModelCollisionHeights.Add(modelId,
                new CreatureModelCollisionHeight(modelScale, collisionHeight, collisionHeightMounted));
        }
    }

    public static void LoadTransports()
    {
        var path = Path.Combine("CSV", $"Transports{LegacyVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var entry = uint.Parse(fields[0]);
            var period = uint.Parse(fields[1]);
            TransportPeriods.Add(entry, period);
        }

        // Load TransportAnimation DB2 entries (elevators the client knows about)
        var animPath = Path.Combine("CSV", "TransportAnimation.3.4.3.54261.csv");
        if (File.Exists(animPath))
        {
            using var animParser = new TextFieldParser(animPath);
            animParser.CommentTokens = new string[1] { "#" };
            animParser.SetDelimiters(",");
            animParser.HasFieldsEnclosedInQuotes = false;
            animParser.ReadLine();
            while (!animParser.EndOfData)
            {
                var fields = animParser.ReadFields();
                var transportId = uint.Parse(fields[6]); // TransportID column
                TransportAnimationEntries.Add(transportId);
            }

            Log.Print(LogType.Network, $"Loaded {TransportAnimationEntries.Count} TransportAnimation entries");
        }
    }

    public static void LoadAreaNames()
    {
        var path = Path.Combine("CSV", "AreaNames.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var name = fields[1];
            AreaNames.Add(id, name);
        }
    }

    public static void LoadRaceFaction()
    {
        var path = Path.Combine("CSV", "RaceFaction.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var faction = uint.Parse(fields[1]);
            RaceFaction.Add(id, faction);
        }
    }

    public static void LoadDispellSpells()
    {
        if (LegacyVersion.ExpansionVersion > 1)
        {
            return;
        }

        var path = Path.Combine("CSV", "DispellSpells.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            DispellSpells.Add(spellId);
        }
    }

    public static void LoadSpellEffectPoints()
    {
        var path = Path.Combine("CSV", $"SpellEffectPoints{LegacyVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            var basePointsEff1 = int.Parse(fields[2]);
            if (basePointsEff1 != 0)
            {
                basePointsEff1++;
            }

            var basePointsEff2 = int.Parse(fields[3]);
            if (basePointsEff2 != 0)
            {
                basePointsEff2++;
            }

            var basePointsEff3 = int.Parse(fields[4]);
            if (basePointsEff3 != 0)
            {
                basePointsEff3++;
            }

            SpellEffectPoints.Add(spellId, new List<float> { basePointsEff1, basePointsEff2, basePointsEff3 });
        }
    }

    public static void LoadStackableAuras()
    {
        if (LegacyVersion.ExpansionVersion > 2)
        {
            return;
        }

        var path = Path.Combine("CSV", $"StackableAuras{LegacyVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            StackableAuras.Add(spellId);
        }
    }

    public static void LoadMountAuras()
    {
        if (LegacyVersion.ExpansionVersion > 1)
        {
            return;
        }

        var path = Path.Combine("CSV", "MountAuras.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            MountAuras.Add(spellId);
        }
    }

    public static void LoadMountSpells()
    {
        var path = Path.Combine("CSV", $"MountSpells{ModernVersion.ExpansionVersion}.csv");
        if (!File.Exists(path))
            return;
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0].Trim());
            MountSpells.Add(spellId);
        }

        Log.Print(LogType.Storage, $"Loaded {MountSpells.Count} mount spells.", "");
    }

    public static void LoadMeleeSpells()
    {
        var path = Path.Combine("CSV", $"MeleeSpells{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            NextMeleeSpells.Add(spellId);
        }
    }

    public static void LoadAutoRepeatSpells()
    {
        var path = Path.Combine("CSV", $"AutoRepeatSpells{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            AutoRepeatSpells.Add(spellId);
        }
    }

    public static void LoadAuraSpells()
    {
        var path = Path.Combine("CSV", $"AuraSpells{LegacyVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var spellId = uint.Parse(fields[0]);
            AuraSpells.Add(spellId);
        }
    }

    public static void LoadTaxiPaths()
    {
        var path = Path.Combine("CSV", $"TaxiPath{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var taxiPath = new TaxiPath
            {
                Id = uint.Parse(fields[0]),
                From = uint.Parse(fields[1]),
                To = uint.Parse(fields[2]),
                Cost = int.Parse(fields[3])
            };
            TaxiPaths.Add(counter, taxiPath);
            counter++;
        }
    }

    public static void LoadTaxiPathNodesGraph()
    {
        var TaxiNodes = new Dictionary<uint, TaxiNode>();
        var pathNodes = Path.Combine("CSV", $"TaxiNodes{ModernVersion.ExpansionVersion}.csv");
        using (var csvParser = new TextFieldParser(pathNodes))
        {
            csvParser.CommentTokens = new string[1] { "#" };
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = false;
            csvParser.ReadLine();
            while (!csvParser.EndOfData)
            {
                var fields = csvParser.ReadFields();
                var taxiNode = new TaxiNode
                {
                    Id = uint.Parse(fields[0]),
                    mapId = uint.Parse(fields[1]),
                    x = float.Parse(fields[2]),
                    y = float.Parse(fields[3]),
                    z = float.Parse(fields[4])
                };
                TaxiNodes.Add(taxiNode.Id, taxiNode);
            }
        }

        var TaxiPathNodes = new Dictionary<uint, TaxiPathNode>();
        var pathPathNodes = Path.Combine("CSV", $"TaxiPathNode{ModernVersion.ExpansionVersion}.csv");
        using (var csvParser2 = new TextFieldParser(pathPathNodes))
        {
            csvParser2.CommentTokens = new string[1] { "#" };
            csvParser2.SetDelimiters(",");
            csvParser2.HasFieldsEnclosedInQuotes = true;
            csvParser2.ReadLine();
            while (!csvParser2.EndOfData)
            {
                var fields2 = csvParser2.ReadFields();
                var taxiPathNode = new TaxiPathNode
                {
                    Id = uint.Parse(fields2[0]),
                    pathId = uint.Parse(fields2[1]),
                    nodeIndex = uint.Parse(fields2[2]),
                    mapId = uint.Parse(fields2[3]),
                    x = float.Parse(fields2[4]),
                    y = float.Parse(fields2[5]),
                    z = float.Parse(fields2[6]),
                    flags = uint.Parse(fields2[7]),
                    delay = uint.Parse(fields2[8])
                };
                TaxiPathNodes.Add(taxiPathNode.Id, taxiPathNode);
            }
        }

        for (var i = 0u; i < TaxiPaths.Count; i++)
        {
            if (!TaxiPaths.ContainsKey(i))
            {
                continue;
            }

            var dist = 0f;
            var taxiPath = TaxiPaths[i];
            var nodeFrom = TaxiNodes[TaxiPaths[i].From];
            var nodeTo = TaxiNodes[TaxiPaths[i].To];
            if ((nodeFrom.x == 0f && nodeFrom.x == 0f && nodeFrom.z == 0f) ||
                (nodeTo.x == 0f && nodeTo.x == 0f && nodeTo.z == 0f))
            {
                continue;
            }

            var pathNodeList = new HashSet<uint>();
            foreach (var item in TaxiPathNodes)
            {
                var pNode = item.Value;
                if (pNode.pathId == taxiPath.Id)
                {
                    pathNodeList.Add(pNode.Id);
                }
            }

            IEnumerable<uint> query = pathNodeList.OrderBy(node => TaxiPathNodes[node].nodeIndex);
            var curNode = 0u;
            foreach (var itr in query)
            {
                var pNode2 = TaxiPathNodes[itr];
                if (pNode2.nodeIndex == 0)
                {
                    dist += (float)Math.Sqrt(
                        Math.Pow(nodeFrom.x - pNode2.x, 2.0) + Math.Pow(nodeFrom.y - pNode2.y, 2.0));
                }
                else if (curNode == 0)
                {
                    curNode = pNode2.Id;
                }
                else if (curNode != 0)
                {
                    var prevNode = TaxiPathNodes[curNode];
                    curNode = pNode2.Id;
                    if (prevNode.mapId == pNode2.mapId)
                    {
                        dist += (float)Math.Sqrt(Math.Pow(prevNode.x - pNode2.x, 2.0) +
                                                 Math.Pow(prevNode.y - pNode2.y, 2.0));
                    }
                }
            }

            if (curNode != 0)
            {
                var lastNode = TaxiPathNodes[curNode];
                dist += (float)Math.Sqrt(Math.Pow(nodeTo.x - lastNode.x, 2.0) + Math.Pow(nodeTo.y - lastNode.y, 2.0));
            }

            TaxiNodesGraph[TaxiPaths[i].From, TaxiPaths[i].To] = dist > 0f ? (int)dist : 0;
        }
    }

    public static void LoadQuestBits()
    {
        var path = Path.Combine("CSV", $"QuestV2_{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        while (!csvParser.EndOfData)
        {
            var fields = csvParser.ReadFields();
            var questId = uint.Parse(fields[0]);
            if (!fields[1].StartsWith("-"))
            {
                var uniqueBitFlag = uint.Parse(fields[1]);
                QuestBits.Add(questId, uniqueBitFlag);
            }
        }
    }

    public static void LoadHotfixes()
    {
        LoadAreaTriggerHotfixes();
        LoadSkillLineHotfixes();
        LoadSkillRaceClassInfoHotfixes();
        LoadSkillLineAbilityHotfixes();
        LoadSpellHotfixes();
        LoadSpellNameHotfixes();
        LoadSpellLevelsHotfixes();
        LoadSpellAuraOptionsHotfixes();
        LoadSpellMiscHotfixes();
        LoadSpellEffectHotfixes();
        LoadSpellXSpellVisualHotfixes();
        LoadItemSparseHotfixes();
        LoadItemHotfixes();
        LoadItemEffectHotfixes();
        LoadItemDisplayInfoHotfixes();
        LoadCreatureDisplayInfoHotfixes();
        LoadCreatureDisplayInfoExtraHotfixes();
        LoadCreatureDisplayInfoOptionHotfixes();
    }

    public static void LoadAreaTriggerHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"AreaTrigger{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var at = new AreaTrigger
            {
                Message = fields[0],
                PositionX = float.Parse(fields[1]),
                PositionY = float.Parse(fields[2]),
                PositionZ = float.Parse(fields[3]),
                Id = uint.Parse(fields[4]),
                MapId = ushort.Parse(fields[5]),
                PhaseUseFlags = byte.Parse(fields[6]),
                PhaseId = ushort.Parse(fields[7]),
                PhaseGroupId = ushort.Parse(fields[8]),
                Radius = float.Parse(fields[9]),
                BoxLength = float.Parse(fields[10]),
                BoxWidth = float.Parse(fields[11]),
                BoxHeight = float.Parse(fields[12]),
                BoxYaw = float.Parse(fields[13]),
                ShapeType = byte.Parse(fields[14]),
                ShapeId = ushort.Parse(fields[15]),
                ActionSetId = ushort.Parse(fields[16]),
                Flags = byte.Parse(fields[17])
            };
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.AreaTrigger,
                HotfixId = 100000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = at.Id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteCString(at.Message);
            record.HotfixContent.WriteFloat(at.PositionX);
            record.HotfixContent.WriteFloat(at.PositionY);
            record.HotfixContent.WriteFloat(at.PositionZ);
            record.HotfixContent.WriteUInt32(at.Id);
            record.HotfixContent.WriteUInt16(at.MapId);
            record.HotfixContent.WriteUInt8(at.PhaseUseFlags);
            record.HotfixContent.WriteUInt16(at.PhaseId);
            record.HotfixContent.WriteUInt16(at.PhaseGroupId);
            record.HotfixContent.WriteFloat(at.Radius);
            record.HotfixContent.WriteFloat(at.BoxLength);
            record.HotfixContent.WriteFloat(at.BoxWidth);
            record.HotfixContent.WriteFloat(at.BoxHeight);
            record.HotfixContent.WriteFloat(at.BoxYaw);
            record.HotfixContent.WriteUInt8(at.ShapeType);
            record.HotfixContent.WriteUInt16(at.ShapeId);
            record.HotfixContent.WriteUInt16(at.ActionSetId);
            record.HotfixContent.WriteUInt8(at.Flags);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSkillLineHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SkillLine{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var displayName = fields[0];
            var alternateVerb = fields[1];
            var description = fields[2];
            var hordeDisplayName = fields[3];
            var neutralDisplayName = fields[4];
            var id = uint.Parse(fields[5]);
            var categoryID = byte.Parse(fields[6]);
            var spellIconFileID = uint.Parse(fields[7]);
            var canLink = byte.Parse(fields[8]);
            var parentSkillLineID = uint.Parse(fields[9]);
            var parentTierIndex = uint.Parse(fields[10]);
            var flags = ushort.Parse(fields[11]);
            var spellBookSpellID = uint.Parse(fields[12]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SkillLine,
                HotfixId = 110000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteCString(displayName);
            record.HotfixContent.WriteCString(alternateVerb);
            record.HotfixContent.WriteCString(description);
            record.HotfixContent.WriteCString(hordeDisplayName);
            record.HotfixContent.WriteCString(neutralDisplayName);
            record.HotfixContent.WriteUInt32(id);
            record.HotfixContent.WriteUInt8(categoryID);
            record.HotfixContent.WriteUInt32(spellIconFileID);
            record.HotfixContent.WriteUInt8(canLink);
            record.HotfixContent.WriteUInt32(parentSkillLineID);
            record.HotfixContent.WriteUInt32(parentTierIndex);
            record.HotfixContent.WriteUInt16(flags);
            record.HotfixContent.WriteUInt32(spellBookSpellID);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSkillRaceClassInfoHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SkillRaceClassInfo{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var raceMask = ulong.Parse(fields[1]);
            var skillId = ushort.Parse(fields[2]);
            var classMask = uint.Parse(fields[3]);
            var flags = ushort.Parse(fields[4]);
            var availability = byte.Parse(fields[5]);
            var minLevel = byte.Parse(fields[6]);
            var skillTierId = ushort.Parse(fields[7]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SkillRaceClassInfo,
                HotfixId = 120000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt64(raceMask);
            record.HotfixContent.WriteUInt16(skillId);
            record.HotfixContent.WriteUInt32(classMask);
            record.HotfixContent.WriteUInt16(flags);
            record.HotfixContent.WriteUInt8(availability);
            record.HotfixContent.WriteUInt8(minLevel);
            record.HotfixContent.WriteUInt16(skillTierId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSkillLineAbilityHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SkillLineAbility{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var raceMask = ulong.Parse(fields[0]);
            var id = uint.Parse(fields[1]);
            var skillId = ushort.Parse(fields[2]);
            var spellId = uint.Parse(fields[3]);
            var minSkillLineRank = ushort.Parse(fields[4]);
            var classMask = uint.Parse(fields[5]);
            var supercedesSpellId = uint.Parse(fields[6]);
            var acquireMethod = byte.Parse(fields[7]);
            var trivialSkillLineRankHigh = ushort.Parse(fields[8]);
            var trivialSkillLineRankLow = ushort.Parse(fields[9]);
            var flags = byte.Parse(fields[10]);
            var numSkillUps = byte.Parse(fields[11]);
            var uniqueBit = ushort.Parse(fields[12]);
            var tradeSkillCategoryId = ushort.Parse(fields[13]);
            var skillUpSkillLineId = ushort.Parse(fields[14]);
            var characterPoints1 = uint.Parse(fields[15]);
            var characterPoints2 = uint.Parse(fields[16]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SkillLineAbility,
                HotfixId = 130000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt64(raceMask);
            record.HotfixContent.WriteUInt32(id);
            record.HotfixContent.WriteUInt16(skillId);
            record.HotfixContent.WriteUInt32(spellId);
            record.HotfixContent.WriteUInt16(minSkillLineRank);
            record.HotfixContent.WriteUInt32(classMask);
            record.HotfixContent.WriteUInt32(supercedesSpellId);
            record.HotfixContent.WriteUInt8(acquireMethod);
            record.HotfixContent.WriteUInt16(trivialSkillLineRankHigh);
            record.HotfixContent.WriteUInt16(trivialSkillLineRankLow);
            record.HotfixContent.WriteUInt8(flags);
            record.HotfixContent.WriteUInt8(numSkillUps);
            record.HotfixContent.WriteUInt16(uniqueBit);
            record.HotfixContent.WriteUInt16(tradeSkillCategoryId);
            record.HotfixContent.WriteUInt16(skillUpSkillLineId);
            record.HotfixContent.WriteUInt32(characterPoints1);
            record.HotfixContent.WriteUInt32(characterPoints2);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"Spell{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var nameSubText = fields[1];
            var description = fields[2];
            var auraDescription = fields[3];
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.Spell,
                HotfixId = 140000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteCString(nameSubText);
            record.HotfixContent.WriteCString(description);
            record.HotfixContent.WriteCString(auraDescription);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellNameHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SpellName{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var name = fields[1];
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SpellName,
                HotfixId = 150000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteCString(name);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellLevelsHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SpellLevels{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var difficultyId = byte.Parse(fields[1]);
            var baseLevel = ushort.Parse(fields[2]);
            var maxLevel = ushort.Parse(fields[3]);
            var spellLevel = ushort.Parse(fields[4]);
            var maxPassiveAuraLevel = byte.Parse(fields[5]);
            var spellId = uint.Parse(fields[6]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SpellLevels,
                HotfixId = 160000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt8(difficultyId);
            record.HotfixContent.WriteUInt16(baseLevel);
            record.HotfixContent.WriteUInt16(maxLevel);
            record.HotfixContent.WriteUInt16(spellLevel);
            record.HotfixContent.WriteUInt8(maxPassiveAuraLevel);
            record.HotfixContent.WriteUInt32(spellId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellAuraOptionsHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SpellAuraOptions{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var difficultyId = byte.Parse(fields[1]);
            var cumulatievAura = uint.Parse(fields[2]);
            var procCategoryRecovery = uint.Parse(fields[3]);
            var procChance = byte.Parse(fields[4]);
            var procCharges = uint.Parse(fields[5]);
            var spellProcsPerMinuteId = ushort.Parse(fields[6]);
            var procTypeMask0 = uint.Parse(fields[7]);
            var procTypeMask1 = uint.Parse(fields[8]);
            var spellId = uint.Parse(fields[9]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SpellAuraOptions,
                HotfixId = 170000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt8(difficultyId);
            record.HotfixContent.WriteUInt32(cumulatievAura);
            record.HotfixContent.WriteUInt32(procCategoryRecovery);
            record.HotfixContent.WriteUInt8(procChance);
            record.HotfixContent.WriteUInt32(procCharges);
            record.HotfixContent.WriteUInt16(spellProcsPerMinuteId);
            record.HotfixContent.WriteUInt32(procTypeMask0);
            record.HotfixContent.WriteUInt32(procTypeMask1);
            record.HotfixContent.WriteUInt32(spellId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellMiscHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SpellMisc{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var difficultyId = byte.Parse(fields[1]);
            var castingTimeIndex = ushort.Parse(fields[2]);
            var durationIndex = ushort.Parse(fields[3]);
            var rangeIndex = ushort.Parse(fields[4]);
            var schoolMask = byte.Parse(fields[5]);
            var speed = float.Parse(fields[6]);
            var launchDelay = float.Parse(fields[7]);
            var minDuration = float.Parse(fields[8]);
            var spellIconFileDataId = uint.Parse(fields[9]);
            var activeIconFileDataId = uint.Parse(fields[10]);
            var attributes1 = uint.Parse(fields[11]);
            var attributes2 = uint.Parse(fields[12]);
            var attributes3 = uint.Parse(fields[13]);
            var attributes4 = uint.Parse(fields[14]);
            var attributes5 = uint.Parse(fields[15]);
            var attributes6 = uint.Parse(fields[16]);
            var attributes7 = uint.Parse(fields[17]);
            var attributes8 = uint.Parse(fields[18]);
            var attributes9 = uint.Parse(fields[19]);
            var attributes10 = uint.Parse(fields[20]);
            var attributes11 = uint.Parse(fields[21]);
            var attributes12 = uint.Parse(fields[22]);
            var attributes13 = uint.Parse(fields[23]);
            var attributes14 = uint.Parse(fields[24]);
            var spellId = uint.Parse(fields[25]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SpellMisc,
                HotfixId = 180000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt8(difficultyId);
            record.HotfixContent.WriteUInt16(castingTimeIndex);
            record.HotfixContent.WriteUInt16(durationIndex);
            record.HotfixContent.WriteUInt16(rangeIndex);
            record.HotfixContent.WriteUInt8(schoolMask);
            record.HotfixContent.WriteFloat(speed);
            record.HotfixContent.WriteFloat(launchDelay);
            record.HotfixContent.WriteFloat(minDuration);
            record.HotfixContent.WriteUInt32(spellIconFileDataId);
            record.HotfixContent.WriteUInt32(activeIconFileDataId);
            record.HotfixContent.WriteUInt32(attributes1);
            record.HotfixContent.WriteUInt32(attributes2);
            record.HotfixContent.WriteUInt32(attributes3);
            record.HotfixContent.WriteUInt32(attributes4);
            record.HotfixContent.WriteUInt32(attributes5);
            record.HotfixContent.WriteUInt32(attributes6);
            record.HotfixContent.WriteUInt32(attributes7);
            record.HotfixContent.WriteUInt32(attributes8);
            record.HotfixContent.WriteUInt32(attributes9);
            record.HotfixContent.WriteUInt32(attributes10);
            record.HotfixContent.WriteUInt32(attributes11);
            record.HotfixContent.WriteUInt32(attributes12);
            record.HotfixContent.WriteUInt32(attributes13);
            record.HotfixContent.WriteUInt32(attributes14);
            record.HotfixContent.WriteUInt32(spellId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellEffectHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SpellEffect{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var difficultyId = uint.Parse(fields[1]);
            var effectIndex = uint.Parse(fields[2]);
            var effect = uint.Parse(fields[3]);
            var effectAmplitude = float.Parse(fields[4]);
            var effectAttributes = uint.Parse(fields[5]);
            var effectAura = short.Parse(fields[6]);
            var effectAuraPeriod = int.Parse(fields[7]);
            var effectBasePoints = int.Parse(fields[8]);
            var effectBonusCoefficient = float.Parse(fields[9]);
            var effectChainAmplitude = float.Parse(fields[10]);
            var effectChainTargets = int.Parse(fields[11]);
            var effectDieSides = int.Parse(fields[12]);
            var effectItemType = int.Parse(fields[13]);
            var effectMechanic = int.Parse(fields[14]);
            var effectPointsPerResource = float.Parse(fields[15]);
            var effectPosFacing = float.Parse(fields[16]);
            var effectRealPointsPerLevel = float.Parse(fields[17]);
            var EffectTriggerSpell = int.Parse(fields[18]);
            var bonusCoefficientFromAP = float.Parse(fields[19]);
            var pvpMultiplier = float.Parse(fields[20]);
            var coefficient = float.Parse(fields[21]);
            var variance = float.Parse(fields[22]);
            var resourceCoefficient = float.Parse(fields[23]);
            var groupSizeBasePointsCoefficient = float.Parse(fields[24]);
            var effectMiscValue1 = int.Parse(fields[25]);
            var effectMiscValue2 = int.Parse(fields[26]);
            var effectRadiusIndex1 = uint.Parse(fields[27]);
            var effectRadiusIndex2 = uint.Parse(fields[28]);
            var effectSpellClassMask1 = int.Parse(fields[29]);
            var effectSpellClassMask2 = int.Parse(fields[30]);
            var effectSpellClassMask3 = int.Parse(fields[31]);
            var effectSpellClassMask4 = int.Parse(fields[32]);
            var implicitTarget1 = short.Parse(fields[33]);
            var implicitTarget2 = short.Parse(fields[34]);
            var spellId = uint.Parse(fields[35]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SpellEffect,
                HotfixId = 190000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt32(difficultyId);
            record.HotfixContent.WriteUInt32(effectIndex);
            record.HotfixContent.WriteUInt32(effect);
            record.HotfixContent.WriteFloat(effectAmplitude);
            record.HotfixContent.WriteUInt32(effectAttributes);
            record.HotfixContent.WriteInt16(effectAura);
            record.HotfixContent.WriteInt32(effectAuraPeriod);
            record.HotfixContent.WriteInt32(effectBasePoints);
            record.HotfixContent.WriteFloat(effectBonusCoefficient);
            record.HotfixContent.WriteFloat(effectChainAmplitude);
            record.HotfixContent.WriteInt32(effectChainTargets);
            record.HotfixContent.WriteInt32(effectDieSides);
            record.HotfixContent.WriteInt32(effectItemType);
            record.HotfixContent.WriteInt32(effectMechanic);
            record.HotfixContent.WriteFloat(effectPointsPerResource);
            record.HotfixContent.WriteFloat(effectPosFacing);
            record.HotfixContent.WriteFloat(effectRealPointsPerLevel);
            record.HotfixContent.WriteInt32(EffectTriggerSpell);
            record.HotfixContent.WriteFloat(bonusCoefficientFromAP);
            record.HotfixContent.WriteFloat(pvpMultiplier);
            record.HotfixContent.WriteFloat(coefficient);
            record.HotfixContent.WriteFloat(variance);
            record.HotfixContent.WriteFloat(resourceCoefficient);
            record.HotfixContent.WriteFloat(groupSizeBasePointsCoefficient);
            record.HotfixContent.WriteInt32(effectMiscValue1);
            record.HotfixContent.WriteInt32(effectMiscValue2);
            record.HotfixContent.WriteUInt32(effectRadiusIndex1);
            record.HotfixContent.WriteUInt32(effectRadiusIndex2);
            record.HotfixContent.WriteInt32(effectSpellClassMask1);
            record.HotfixContent.WriteInt32(effectSpellClassMask2);
            record.HotfixContent.WriteInt32(effectSpellClassMask3);
            record.HotfixContent.WriteInt32(effectSpellClassMask4);
            record.HotfixContent.WriteInt16(implicitTarget1);
            record.HotfixContent.WriteInt16(implicitTarget2);
            record.HotfixContent.WriteUInt32(spellId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadSpellXSpellVisualHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"SpellXSpellVisual{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var difficultyId = byte.Parse(fields[1]);
            var spellVisualId = uint.Parse(fields[2]);
            var probability = float.Parse(fields[3]);
            var flags = byte.Parse(fields[4]);
            var priority = byte.Parse(fields[5]);
            var spellIconFileId = int.Parse(fields[6]);
            var activeIconFileId = int.Parse(fields[7]);
            var viewerUnitConditionId = ushort.Parse(fields[8]);
            var viewerPlayerConditionId = uint.Parse(fields[9]);
            var casterUnitConditionId = ushort.Parse(fields[10]);
            var casterPlayerConditionId = uint.Parse(fields[11]);
            var spellId = uint.Parse(fields[12]);
            if (SpellVisuals.ContainsKey(spellId))
            {
                SpellVisuals[spellId] = id;
            }
            else
            {
                SpellVisuals.Add(spellId, id);
            }

            var record = new HotfixRecord
            {
                TableHash = DB2Hash.SpellXSpellVisual,
                HotfixId = 200000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt32(id);
            record.HotfixContent.WriteUInt8(difficultyId);
            record.HotfixContent.WriteUInt32(spellVisualId);
            record.HotfixContent.WriteFloat(probability);
            record.HotfixContent.WriteUInt8(flags);
            record.HotfixContent.WriteUInt8(priority);
            record.HotfixContent.WriteInt32(spellIconFileId);
            record.HotfixContent.WriteInt32(activeIconFileId);
            record.HotfixContent.WriteUInt16(viewerUnitConditionId);
            record.HotfixContent.WriteUInt32(viewerPlayerConditionId);
            record.HotfixContent.WriteUInt16(casterUnitConditionId);
            record.HotfixContent.WriteUInt32(casterPlayerConditionId);
            record.HotfixContent.WriteUInt32(spellId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadItemSparseHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"ItemSparse{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = true;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var allowableRace = long.Parse(fields[1]);
            var description = fields[2];
            var name4 = fields[3];
            var name5 = fields[4];
            var name6 = fields[5];
            var name7 = fields[6];
            var dmgVariance = float.Parse(fields[7]);
            var durationInInventory = uint.Parse(fields[8]);
            var qualityModifier = float.Parse(fields[9]);
            var bagFamily = uint.Parse(fields[10]);
            var rangeMod = float.Parse(fields[11]);
            var statPercentageOfSocket1 = float.Parse(fields[12]);
            var statPercentageOfSocket2 = float.Parse(fields[13]);
            var statPercentageOfSocket3 = float.Parse(fields[14]);
            var statPercentageOfSocket4 = float.Parse(fields[15]);
            var statPercentageOfSocket5 = float.Parse(fields[16]);
            var statPercentageOfSocket6 = float.Parse(fields[17]);
            var statPercentageOfSocket7 = float.Parse(fields[18]);
            var statPercentageOfSocket8 = float.Parse(fields[19]);
            var statPercentageOfSocket9 = float.Parse(fields[20]);
            var statPercentageOfSocket10 = float.Parse(fields[21]);
            var statPercentEditor1 = int.Parse(fields[22]);
            var statPercentEditor2 = int.Parse(fields[23]);
            var statPercentEditor3 = int.Parse(fields[24]);
            var statPercentEditor4 = int.Parse(fields[25]);
            var statPercentEditor5 = int.Parse(fields[26]);
            var statPercentEditor6 = int.Parse(fields[27]);
            var statPercentEditor7 = int.Parse(fields[28]);
            var statPercentEditor8 = int.Parse(fields[29]);
            var statPercentEditor9 = int.Parse(fields[30]);
            var statPercentEditor10 = int.Parse(fields[31]);
            var stackable = int.Parse(fields[32]);
            var maxCount = int.Parse(fields[33]);
            var requiredAbility = uint.Parse(fields[34]);
            var sellPrice = uint.Parse(fields[35]);
            var buyPrice = uint.Parse(fields[36]);
            var vendorStackCount = uint.Parse(fields[37]);
            var priceVariance = float.Parse(fields[38]);
            var priceRandomValue = float.Parse(fields[39]);
            var flags1 = int.Parse(fields[40]);
            var flags2 = int.Parse(fields[41]);
            var flags3 = int.Parse(fields[42]);
            var flags4 = int.Parse(fields[43]);
            var oppositeFactionItemId = int.Parse(fields[44]);
            var maxDurability = uint.Parse(fields[45]);
            var itemNameDescriptionId = ushort.Parse(fields[46]);
            var requiredTransmogHoliday = ushort.Parse(fields[47]);
            var requiredHoliday = ushort.Parse(fields[48]);
            var limitCategory = ushort.Parse(fields[49]);
            var gemProperties = ushort.Parse(fields[50]);
            var socketMatchEnchantmentId = ushort.Parse(fields[51]);
            var totemCategoryId = ushort.Parse(fields[52]);
            var instanceBound = ushort.Parse(fields[53]);
            var zoneBound1 = ushort.Parse(fields[54]);
            var zoneBound2 = ushort.Parse(fields[55]);
            var itemSet = ushort.Parse(fields[56]);
            var lockId = ushort.Parse(fields[57]);
            var startQuestId = ushort.Parse(fields[58]);
            var pageText = ushort.Parse(fields[59]);
            var delay = ushort.Parse(fields[60]);
            var requiredReputationId = ushort.Parse(fields[61]);
            var requiredSkillRank = ushort.Parse(fields[62]);
            var requiredSkill = ushort.Parse(fields[63]);
            var itemLevel = ushort.Parse(fields[64]);
            var allowableClass = short.Parse(fields[65]);
            var itemRandomSuffixGroupId = ushort.Parse(fields[66]);
            var randomProperty = ushort.Parse(fields[67]);
            var damageMin1 = ushort.Parse(fields[68]);
            var damageMin2 = ushort.Parse(fields[69]);
            var damageMin3 = ushort.Parse(fields[70]);
            var damageMin4 = ushort.Parse(fields[71]);
            var damageMin5 = ushort.Parse(fields[72]);
            var damageMax1 = ushort.Parse(fields[73]);
            var damageMax2 = ushort.Parse(fields[74]);
            var damageMax3 = ushort.Parse(fields[75]);
            var damageMax4 = ushort.Parse(fields[76]);
            var damageMax5 = ushort.Parse(fields[77]);
            var armor = short.Parse(fields[78]);
            var holyResistance = short.Parse(fields[79]);
            var fireResistance = short.Parse(fields[80]);
            var natureResistance = short.Parse(fields[81]);
            var frostResistance = short.Parse(fields[82]);
            var shadowResistance = short.Parse(fields[83]);
            var arcaneResistance = short.Parse(fields[84]);
            var scalingStatDistributionId = ushort.Parse(fields[85]);
            var expansionId = byte.Parse(fields[86]);
            var artifactId = byte.Parse(fields[87]);
            var spellWeight = byte.Parse(fields[88]);
            var spellWeightCategory = byte.Parse(fields[89]);
            var socketType1 = byte.Parse(fields[90]);
            var socketType2 = byte.Parse(fields[91]);
            var socketType3 = byte.Parse(fields[92]);
            var sheatheType = byte.Parse(fields[93]);
            var material = byte.Parse(fields[94]);
            var pageMaterial = byte.Parse(fields[95]);
            var pageLanguage = byte.Parse(fields[96]);
            var bonding = byte.Parse(fields[97]);
            var damageType = byte.Parse(fields[98]);
            var statType1 = sbyte.Parse(fields[99]);
            var statType2 = sbyte.Parse(fields[100]);
            var statType3 = sbyte.Parse(fields[101]);
            var statType4 = sbyte.Parse(fields[102]);
            var statType5 = sbyte.Parse(fields[103]);
            var statType6 = sbyte.Parse(fields[104]);
            var statType7 = sbyte.Parse(fields[105]);
            var statType8 = sbyte.Parse(fields[106]);
            var statType9 = sbyte.Parse(fields[107]);
            var statType10 = sbyte.Parse(fields[108]);
            var containerSlots = byte.Parse(fields[109]);
            var requiredReputationRank = byte.Parse(fields[110]);
            var requiredCityRank = byte.Parse(fields[111]);
            var requiredHonorRank = byte.Parse(fields[112]);
            var inventoryType = byte.Parse(fields[113]);
            var overallQualityId = byte.Parse(fields[114]);
            var ammoType = byte.Parse(fields[115]);
            var statValue1 = sbyte.Parse(fields[116]);
            var statValue2 = sbyte.Parse(fields[117]);
            var statValue3 = sbyte.Parse(fields[118]);
            var statValue4 = sbyte.Parse(fields[119]);
            var statValue5 = sbyte.Parse(fields[120]);
            var statValue6 = sbyte.Parse(fields[121]);
            var statValue7 = sbyte.Parse(fields[122]);
            var statValue8 = sbyte.Parse(fields[123]);
            var statValue9 = sbyte.Parse(fields[124]);
            var statValue10 = sbyte.Parse(fields[125]);
            var requiredLevel = sbyte.Parse(fields[126]);
            var record = new HotfixRecord
            {
                Status = HotfixStatus.Valid,
                TableHash = DB2Hash.ItemSparse,
                HotfixId = 220000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.HotfixContent.WriteInt64(allowableRace);
            record.HotfixContent.WriteCString(description);
            record.HotfixContent.WriteCString(name4);
            record.HotfixContent.WriteCString(name5);
            record.HotfixContent.WriteCString(name6);
            record.HotfixContent.WriteCString(name7);
            record.HotfixContent.WriteFloat(dmgVariance);
            record.HotfixContent.WriteUInt32(durationInInventory);
            record.HotfixContent.WriteFloat(qualityModifier);
            record.HotfixContent.WriteUInt32(bagFamily);
            record.HotfixContent.WriteInt32(0); // StartQuestID
            record.HotfixContent.WriteFloat(rangeMod);
            record.HotfixContent.WriteFloat(statPercentageOfSocket1);
            record.HotfixContent.WriteFloat(statPercentageOfSocket2);
            record.HotfixContent.WriteFloat(statPercentageOfSocket3);
            record.HotfixContent.WriteFloat(statPercentageOfSocket4);
            record.HotfixContent.WriteFloat(statPercentageOfSocket5);
            record.HotfixContent.WriteFloat(statPercentageOfSocket6);
            record.HotfixContent.WriteFloat(statPercentageOfSocket7);
            record.HotfixContent.WriteFloat(statPercentageOfSocket8);
            record.HotfixContent.WriteFloat(statPercentageOfSocket9);
            record.HotfixContent.WriteFloat(statPercentageOfSocket10);
            record.HotfixContent.WriteInt32(statPercentEditor1);
            record.HotfixContent.WriteInt32(statPercentEditor2);
            record.HotfixContent.WriteInt32(statPercentEditor3);
            record.HotfixContent.WriteInt32(statPercentEditor4);
            record.HotfixContent.WriteInt32(statPercentEditor5);
            record.HotfixContent.WriteInt32(statPercentEditor6);
            record.HotfixContent.WriteInt32(statPercentEditor7);
            record.HotfixContent.WriteInt32(statPercentEditor8);
            record.HotfixContent.WriteInt32(statPercentEditor9);
            record.HotfixContent.WriteInt32(statPercentEditor10);
            record.HotfixContent.WriteInt32(stackable);
            record.HotfixContent.WriteInt32(maxCount);
            record.HotfixContent.WriteInt32(0); // MinReputation
            record.HotfixContent.WriteUInt32(requiredAbility);
            record.HotfixContent.WriteUInt32(sellPrice);
            record.HotfixContent.WriteUInt32(buyPrice);
            record.HotfixContent.WriteUInt32(vendorStackCount);
            record.HotfixContent.WriteFloat(priceVariance);
            record.HotfixContent.WriteFloat(priceRandomValue);
            record.HotfixContent.WriteInt32(flags1);
            record.HotfixContent.WriteInt32(flags2);
            record.HotfixContent.WriteInt32(flags3);
            record.HotfixContent.WriteInt32(flags4);
            record.HotfixContent.WriteInt32(oppositeFactionItemId);
            record.HotfixContent.WriteInt32(0); // ModifiedCraftingReagentItemID
            record.HotfixContent.WriteInt32(0); // ContentTuningID
            record.HotfixContent.WriteInt32(0); // PlayerLevelToItemLevelCurveID
            record.HotfixContent.WriteUInt32(maxDurability);
            record.HotfixContent.WriteUInt16(itemNameDescriptionId);
            record.HotfixContent.WriteUInt16(requiredTransmogHoliday);
            record.HotfixContent.WriteUInt16(requiredHoliday);
            record.HotfixContent.WriteUInt16(limitCategory);
            record.HotfixContent.WriteUInt16(gemProperties);
            record.HotfixContent.WriteUInt16(socketMatchEnchantmentId);
            record.HotfixContent.WriteUInt16(totemCategoryId);
            record.HotfixContent.WriteUInt16(instanceBound);
            record.HotfixContent.WriteUInt16(zoneBound1);
            record.HotfixContent.WriteUInt16(zoneBound2);
            record.HotfixContent.WriteUInt16(itemSet);
            record.HotfixContent.WriteUInt16(lockId);
            record.HotfixContent.WriteUInt16(pageText);
            record.HotfixContent.WriteUInt16(delay);
            record.HotfixContent.WriteUInt16(requiredReputationId);
            record.HotfixContent.WriteUInt16(requiredSkillRank);
            record.HotfixContent.WriteUInt16(requiredSkill);
            record.HotfixContent.WriteUInt16(itemLevel);
            record.HotfixContent.WriteInt16(allowableClass);
            record.HotfixContent.WriteUInt16(itemRandomSuffixGroupId);
            record.HotfixContent.WriteUInt16(randomProperty);
            record.HotfixContent.WriteUInt16(damageMin1);
            record.HotfixContent.WriteUInt16(damageMin2);
            record.HotfixContent.WriteUInt16(damageMin3);
            record.HotfixContent.WriteUInt16(damageMin4);
            record.HotfixContent.WriteUInt16(damageMin5);
            record.HotfixContent.WriteUInt16(damageMax1);
            record.HotfixContent.WriteUInt16(damageMax2);
            record.HotfixContent.WriteUInt16(damageMax3);
            record.HotfixContent.WriteUInt16(damageMax4);
            record.HotfixContent.WriteUInt16(damageMax5);
            record.HotfixContent.WriteInt16(armor);
            record.HotfixContent.WriteInt16(holyResistance);
            record.HotfixContent.WriteInt16(fireResistance);
            record.HotfixContent.WriteInt16(natureResistance);
            record.HotfixContent.WriteInt16(frostResistance);
            record.HotfixContent.WriteInt16(shadowResistance);
            record.HotfixContent.WriteInt16(arcaneResistance);
            record.HotfixContent.WriteUInt16(scalingStatDistributionId);
            // StatModifierBonusAmount[10] - use CSV statValue fields as int16
            record.HotfixContent.WriteInt16(statValue1);
            record.HotfixContent.WriteInt16(statValue2);
            record.HotfixContent.WriteInt16(statValue3);
            record.HotfixContent.WriteInt16(statValue4);
            record.HotfixContent.WriteInt16(statValue5);
            record.HotfixContent.WriteInt16(statValue6);
            record.HotfixContent.WriteInt16(statValue7);
            record.HotfixContent.WriteInt16(statValue8);
            record.HotfixContent.WriteInt16(statValue9);
            record.HotfixContent.WriteInt16(statValue10);
            record.HotfixContent.WriteUInt8(expansionId);
            record.HotfixContent.WriteUInt8(artifactId);
            record.HotfixContent.WriteUInt8(spellWeight);
            record.HotfixContent.WriteUInt8(spellWeightCategory);
            record.HotfixContent.WriteUInt8(socketType1);
            record.HotfixContent.WriteUInt8(socketType2);
            record.HotfixContent.WriteUInt8(socketType3);
            record.HotfixContent.WriteUInt8(sheatheType);
            record.HotfixContent.WriteUInt8(material);
            record.HotfixContent.WriteUInt8(pageMaterial);
            record.HotfixContent.WriteUInt8(pageLanguage);
            record.HotfixContent.WriteUInt8(bonding);
            record.HotfixContent.WriteUInt8(damageType);
            record.HotfixContent.WriteInt8(statType1);
            record.HotfixContent.WriteInt8(statType2);
            record.HotfixContent.WriteInt8(statType3);
            record.HotfixContent.WriteInt8(statType4);
            record.HotfixContent.WriteInt8(statType5);
            record.HotfixContent.WriteInt8(statType6);
            record.HotfixContent.WriteInt8(statType7);
            record.HotfixContent.WriteInt8(statType8);
            record.HotfixContent.WriteInt8(statType9);
            record.HotfixContent.WriteInt8(statType10);
            record.HotfixContent.WriteUInt8(containerSlots);
            record.HotfixContent.WriteUInt8(requiredReputationRank);
            record.HotfixContent.WriteUInt8(requiredCityRank);
            record.HotfixContent.WriteUInt8(requiredHonorRank);
            record.HotfixContent.WriteUInt8(inventoryType);
            record.HotfixContent.WriteUInt8(overallQualityId);
            record.HotfixContent.WriteUInt8(ammoType);
            record.HotfixContent.WriteInt8(requiredLevel);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void WriteItemSparseHotfix(ItemTemplate item, ByteBuffer buffer)
    {
        var StatValues = new short[10];
        for (var i = 0; i < item.StatsCount; i++)
        {
            StatValues[i] = (short)Math.Clamp(item.StatValues[i], short.MinValue, short.MaxValue);
        }

        buffer.WriteInt64(item.AllowedRaces);
        buffer.WriteCString(item.Description);
        buffer.WriteCString(item.Name[3]);
        buffer.WriteCString(item.Name[2]);
        buffer.WriteCString(item.Name[1]);
        buffer.WriteCString(item.Name[0]);
        buffer.WriteFloat(1f); // DmgVariance
        buffer.WriteUInt32(item.Duration); // DurationInInventory
        buffer.WriteFloat(0f); // QualityModifier
        buffer.WriteUInt32(item.BagFamily); // BagFamily
        buffer.WriteInt32((int)item.StartQuestId); // StartQuestID
        buffer.WriteFloat(item.RangedMod); // ItemRange
        for (var i = 0; i < 10; i++)
            buffer.WriteFloat(0f); // StatPercentageOfSocket[10]
        for (var i = 0; i < 10; i++)
            buffer.WriteInt32(0); // StatPercentEditor[10]
        buffer.WriteInt32(item.MaxStackSize); // Stackable
        buffer.WriteInt32(item.MaxCount); // MaxCount
        buffer.WriteInt32((int)item.RequiredRepValue); // MinReputation
        buffer.WriteUInt32(item.RequiredSpell); // RequiredAbility
        buffer.WriteUInt32(item.SellPrice); // SellPrice
        buffer.WriteUInt32(item.BuyPrice); // BuyPrice
        buffer.WriteUInt32(item.BuyCount); // VendorStackCount
        buffer.WriteFloat(1f); // PriceVariance
        buffer.WriteFloat(1f); // PriceRandomValue
        buffer.WriteUInt32(item.Flags); // Flags[0]
        buffer.WriteUInt32(item.FlagsExtra); // Flags[1]
        buffer.WriteInt32(0); // Flags[2]
        buffer.WriteInt32(0); // Flags[3]
        buffer.WriteInt32(0); // FactionRelated (OppositeFactionItemId)
        buffer.WriteInt32(0); // ModifiedCraftingReagentItemID
        buffer.WriteInt32(0); // ContentTuningID
        buffer.WriteInt32(0); // PlayerLevelToItemLevelCurveID
        buffer.WriteUInt32(item.MaxDurability); // MaxDurability
        buffer.WriteUInt16(0); // ItemNameDescriptionID
        buffer.WriteUInt16(0); // RequiredTransmogHoliday
        buffer.WriteUInt16((ushort)item.HolidayID); // RequiredHoliday
        buffer.WriteUInt16((ushort)item.ItemLimitCategory); // LimitCategory
        buffer.WriteUInt16((ushort)item.GemProperties); // GemProperties
        buffer.WriteUInt16((ushort)item.SocketBonus); // SocketMatchEnchantmentId
        buffer.WriteUInt16((ushort)item.TotemCategory); // TotemCategoryID
        buffer.WriteUInt16((ushort)item.MapID); // InstanceBound
        buffer.WriteUInt16((ushort)item.AreaID); // ZoneBound[0]
        buffer.WriteUInt16(0); // ZoneBound[1]
        buffer.WriteUInt16((ushort)item.ItemSet); // ItemSet
        buffer.WriteUInt16((ushort)item.LockId); // LockID
        buffer.WriteUInt16((ushort)item.PageText); // PageID
        buffer.WriteUInt16((ushort)item.Delay); // ItemDelay
        buffer.WriteUInt16((ushort)item.RequiredRepFaction); // MinFactionID
        buffer.WriteUInt16((ushort)item.RequiredSkillLevel); // RequiredSkillRank
        buffer.WriteUInt16((ushort)item.RequiredSkillId); // RequiredSkill
        buffer.WriteUInt16((ushort)item.ItemLevel); // ItemLevel
        buffer.WriteInt16((short)item.AllowedClasses); // AllowableClass
        buffer.WriteUInt16((ushort)item.RandomSuffix); // ItemRandomSuffixGroupID
        buffer.WriteUInt16((ushort)item.RandomProperty); // RandomSelect
        buffer.WriteUInt16((ushort)item.DamageMins[0]); // MinDamage[0]
        buffer.WriteUInt16((ushort)item.DamageMins[1]);
        buffer.WriteUInt16((ushort)item.DamageMins[2]);
        buffer.WriteUInt16((ushort)item.DamageMins[3]);
        buffer.WriteUInt16((ushort)item.DamageMins[4]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[0]); // MaxDamage[0]
        buffer.WriteUInt16((ushort)item.DamageMaxs[1]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[2]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[3]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[4]);
        buffer.WriteInt16((short)item.Armor); // Resistances[0]
        buffer.WriteInt16((short)item.HolyResistance); // Resistances[1]
        buffer.WriteInt16((short)item.FireResistance);
        buffer.WriteInt16((short)item.NatureResistance);
        buffer.WriteInt16((short)item.FrostResistance);
        buffer.WriteInt16((short)item.ShadowResistance);
        buffer.WriteInt16((short)item.ArcaneResistance); // Resistances[6]
        buffer.WriteUInt16((ushort)item.ScalingStatDistribution); // ScalingStatDistributionID
        for (var i = 0; i < 10; i++)
            buffer.WriteInt16(StatValues[i]); // StatModifierBonusAmount[10]
        buffer.WriteUInt8(254); // ExpansionID
        buffer.WriteUInt8(0); // ArtifactID
        buffer.WriteUInt8(0); // SpellWeight
        buffer.WriteUInt8(0); // SpellWeightCategory
        buffer.WriteUInt8((byte)item.ItemSocketColors[0]); // SocketType[0]
        buffer.WriteUInt8((byte)item.ItemSocketColors[1]);
        buffer.WriteUInt8((byte)item.ItemSocketColors[2]);
        buffer.WriteUInt8((byte)item.SheathType); // SheatheType
        buffer.WriteUInt8((byte)item.Material); // Material
        buffer.WriteUInt8((byte)item.PageMaterial); // PageMaterialID
        buffer.WriteUInt8((byte)item.Language); // LanguageID
        buffer.WriteUInt8((byte)item.Bonding); // Bonding
        buffer.WriteUInt8((byte)item.DamageTypes[0]); // DamageDamageType
        buffer.WriteInt8((sbyte)item.StatTypes[0]); // StatModifierBonusStat[0]
        buffer.WriteInt8((sbyte)item.StatTypes[1]);
        buffer.WriteInt8((sbyte)item.StatTypes[2]);
        buffer.WriteInt8((sbyte)item.StatTypes[3]);
        buffer.WriteInt8((sbyte)item.StatTypes[4]);
        buffer.WriteInt8((sbyte)item.StatTypes[5]);
        buffer.WriteInt8((sbyte)item.StatTypes[6]);
        buffer.WriteInt8((sbyte)item.StatTypes[7]);
        buffer.WriteInt8((sbyte)item.StatTypes[8]);
        buffer.WriteInt8((sbyte)item.StatTypes[9]);
        buffer.WriteUInt8((byte)item.ContainerSlots); // ContainerSlots
        buffer.WriteUInt8((byte)item.RequiredRepValue); // RequiredPVPMedal
        buffer.WriteUInt8((byte)item.RequiredCityRank); // RequiredPVPRank
        buffer.WriteUInt8((byte)item.RequiredHonorRank); // (unused)
        buffer.WriteInt8((sbyte)item.InventoryType); // InventoryType
        buffer.WriteInt8((sbyte)item.Quality); // OverallQualityID
        buffer.WriteUInt8((byte)item.AmmoType); // AmmunitionType
        buffer.WriteInt8((sbyte)item.RequiredLevel); // RequiredLevel
    }

    public static void WriteItemSparseHotfix(ItemSparseRecord row, ByteBuffer buffer)
    {
        buffer.WriteInt64(row.AllowableRace);
        buffer.WriteCString(row.Description);
        buffer.WriteCString(row.Name4);
        buffer.WriteCString(row.Name3);
        buffer.WriteCString(row.Name2);
        buffer.WriteCString(row.Name1);
        buffer.WriteFloat(row.DmgVariance);
        buffer.WriteUInt32(row.DurationInInventory);
        buffer.WriteFloat(row.QualityModifier);
        buffer.WriteUInt32(row.BagFamily);
        buffer.WriteInt32(row.StartQuestID);
        buffer.WriteFloat(row.RangeMod);
        buffer.WriteFloat(row.StatPercentageOfSocket[0]);
        buffer.WriteFloat(row.StatPercentageOfSocket[1]);
        buffer.WriteFloat(row.StatPercentageOfSocket[2]);
        buffer.WriteFloat(row.StatPercentageOfSocket[3]);
        buffer.WriteFloat(row.StatPercentageOfSocket[4]);
        buffer.WriteFloat(row.StatPercentageOfSocket[5]);
        buffer.WriteFloat(row.StatPercentageOfSocket[6]);
        buffer.WriteFloat(row.StatPercentageOfSocket[7]);
        buffer.WriteFloat(row.StatPercentageOfSocket[8]);
        buffer.WriteFloat(row.StatPercentageOfSocket[9]);
        buffer.WriteInt32(row.StatPercentEditor[0]);
        buffer.WriteInt32(row.StatPercentEditor[1]);
        buffer.WriteInt32(row.StatPercentEditor[2]);
        buffer.WriteInt32(row.StatPercentEditor[3]);
        buffer.WriteInt32(row.StatPercentEditor[4]);
        buffer.WriteInt32(row.StatPercentEditor[5]);
        buffer.WriteInt32(row.StatPercentEditor[6]);
        buffer.WriteInt32(row.StatPercentEditor[7]);
        buffer.WriteInt32(row.StatPercentEditor[8]);
        buffer.WriteInt32(row.StatPercentEditor[9]);
        buffer.WriteInt32(row.Stackable);
        buffer.WriteInt32(row.MaxCount);
        buffer.WriteInt32(row.MinReputation);
        buffer.WriteUInt32(row.RequiredAbility);
        buffer.WriteUInt32(row.SellPrice);
        buffer.WriteUInt32(row.BuyPrice);
        buffer.WriteUInt32(row.VendorStackCount);
        buffer.WriteFloat(row.PriceVariance);
        buffer.WriteFloat(row.PriceRandomValue);
        buffer.WriteUInt32(row.Flags[0]);
        buffer.WriteUInt32(row.Flags[1]);
        buffer.WriteUInt32(row.Flags[2]);
        buffer.WriteUInt32(row.Flags[3]);
        buffer.WriteInt32(row.OppositeFactionItemId);
        buffer.WriteInt32(row.ModifiedCraftingReagentItemID);
        buffer.WriteInt32(row.ContentTuningID);
        buffer.WriteInt32(row.PlayerLevelToItemLevelCurveID);
        buffer.WriteUInt32(row.MaxDurability);
        buffer.WriteUInt16(row.ItemNameDescriptionId);
        buffer.WriteUInt16(row.RequiredTransmogHoliday);
        buffer.WriteUInt16(row.RequiredHoliday);
        buffer.WriteUInt16(row.LimitCategory);
        buffer.WriteUInt16(row.GemProperties);
        buffer.WriteUInt16(row.SocketMatchEnchantmentId);
        buffer.WriteUInt16(row.TotemCategoryId);
        buffer.WriteUInt16(row.InstanceBound);
        buffer.WriteUInt16(row.ZoneBound[0]);
        buffer.WriteUInt16(row.ZoneBound[1]);
        buffer.WriteUInt16(row.ItemSet);
        buffer.WriteUInt16(row.LockId);
        buffer.WriteUInt16(row.PageText);
        buffer.WriteUInt16(row.Delay);
        buffer.WriteUInt16(row.RequiredReputationId);
        buffer.WriteUInt16(row.RequiredSkillRank);
        buffer.WriteUInt16(row.RequiredSkill);
        buffer.WriteUInt16(row.ItemLevel);
        buffer.WriteInt16(row.AllowableClass);
        buffer.WriteUInt16(row.ItemRandomSuffixGroupId);
        buffer.WriteUInt16(row.RandomProperty);
        buffer.WriteUInt16(row.MinDamage[0]);
        buffer.WriteUInt16(row.MinDamage[1]);
        buffer.WriteUInt16(row.MinDamage[2]);
        buffer.WriteUInt16(row.MinDamage[3]);
        buffer.WriteUInt16(row.MinDamage[4]);
        buffer.WriteUInt16(row.MaxDamage[0]);
        buffer.WriteUInt16(row.MaxDamage[1]);
        buffer.WriteUInt16(row.MaxDamage[2]);
        buffer.WriteUInt16(row.MaxDamage[3]);
        buffer.WriteUInt16(row.MaxDamage[4]);
        buffer.WriteInt16(row.Resistances[0]);
        buffer.WriteInt16(row.Resistances[1]);
        buffer.WriteInt16(row.Resistances[2]);
        buffer.WriteInt16(row.Resistances[3]);
        buffer.WriteInt16(row.Resistances[4]);
        buffer.WriteInt16(row.Resistances[5]);
        buffer.WriteInt16(row.Resistances[6]);
        buffer.WriteUInt16(row.ScalingStatDistributionId);
        for (var i = 0; i < 10; i++)
            buffer.WriteInt16(row.StatModifierBonusAmount[i]);
        buffer.WriteUInt8(row.ExpansionId);
        buffer.WriteUInt8(row.ArtifactId);
        buffer.WriteUInt8(row.SpellWeight);
        buffer.WriteUInt8(row.SpellWeightCategory);
        buffer.WriteUInt8(row.SocketType[0]);
        buffer.WriteUInt8(row.SocketType[1]);
        buffer.WriteUInt8(row.SocketType[2]);
        buffer.WriteUInt8(row.SheatheType);
        buffer.WriteUInt8(row.Material);
        buffer.WriteUInt8(row.PageMaterial);
        buffer.WriteUInt8(row.PageLanguage);
        buffer.WriteUInt8(row.Bonding);
        buffer.WriteUInt8(row.DamageType);
        buffer.WriteInt8(row.StatType[0]);
        buffer.WriteInt8(row.StatType[1]);
        buffer.WriteInt8(row.StatType[2]);
        buffer.WriteInt8(row.StatType[3]);
        buffer.WriteInt8(row.StatType[4]);
        buffer.WriteInt8(row.StatType[5]);
        buffer.WriteInt8(row.StatType[6]);
        buffer.WriteInt8(row.StatType[7]);
        buffer.WriteInt8(row.StatType[8]);
        buffer.WriteInt8(row.StatType[9]);
        buffer.WriteUInt8(row.ContainerSlots);
        buffer.WriteUInt8(row.RequiredReputationRank);
        buffer.WriteUInt8(row.RequiredCityRank);
        buffer.WriteUInt8(row.RequiredHonorRank);
        buffer.WriteUInt8(row.InventoryType);
        buffer.WriteUInt8(row.OverallQualityId);
        buffer.WriteUInt8(row.AmmoType);
        buffer.WriteInt8(row.RequiredLevel);
    }

    public static void LoadItemHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"Item{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var ClassID = byte.Parse(fields[1]);
            var SubclassID = byte.Parse(fields[2]);
            var Material = byte.Parse(fields[3]);
            var InventoryType = sbyte.Parse(fields[4]);
            var RequiredLevel = uint.Parse(fields[5]);
            var SheatheType = byte.Parse(fields[6]);
            var RandomSelect = ushort.Parse(fields[7]);
            var ItemRandomSuffixGroupID = ushort.Parse(fields[8]);
            var Sound_override_subclassID = sbyte.Parse(fields[9]);
            var ScalingStatDistributionID = ushort.Parse(fields[10]);
            var IconFileDataID = int.Parse(fields[11]);
            var ItemGroupSoundsID = byte.Parse(fields[12]);
            var ContentTuningID = int.Parse(fields[13]);
            var MaxDurability = uint.Parse(fields[14]);
            var AmmunitionType = byte.Parse(fields[15]);
            var DamageType1 = byte.Parse(fields[16]);
            var DamageType2 = byte.Parse(fields[17]);
            var DamageType3 = byte.Parse(fields[18]);
            var DamageType4 = byte.Parse(fields[19]);
            var DamageType5 = byte.Parse(fields[20]);
            var Resistances1 = short.Parse(fields[21]);
            var Resistances2 = short.Parse(fields[22]);
            var Resistances3 = short.Parse(fields[23]);
            var Resistances4 = short.Parse(fields[24]);
            var Resistances5 = short.Parse(fields[25]);
            var Resistances6 = short.Parse(fields[26]);
            var Resistances7 = short.Parse(fields[27]);
            var MinDamage1 = ushort.Parse(fields[28]);
            var MinDamage2 = ushort.Parse(fields[29]);
            var MinDamage3 = ushort.Parse(fields[30]);
            var MinDamage4 = ushort.Parse(fields[31]);
            var MinDamage5 = ushort.Parse(fields[32]);
            var MaxDamage1 = ushort.Parse(fields[33]);
            var MaxDamage2 = ushort.Parse(fields[34]);
            var MaxDamage3 = ushort.Parse(fields[35]);
            var MaxDamage4 = ushort.Parse(fields[36]);
            var MaxDamage5 = ushort.Parse(fields[37]);
            var record = new HotfixRecord
            {
                Status = HotfixStatus.Valid,
                TableHash = DB2Hash.Item,
                HotfixId = 210000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.HotfixContent.WriteUInt8(ClassID);
            record.HotfixContent.WriteUInt8(SubclassID);
            record.HotfixContent.WriteUInt8(Material);
            record.HotfixContent.WriteInt8(InventoryType);
            record.HotfixContent.WriteUInt32(RequiredLevel);
            record.HotfixContent.WriteUInt8(SheatheType);
            record.HotfixContent.WriteUInt16(RandomSelect);
            record.HotfixContent.WriteUInt16(ItemRandomSuffixGroupID);
            record.HotfixContent.WriteInt8(Sound_override_subclassID);
            record.HotfixContent.WriteUInt16(ScalingStatDistributionID);
            record.HotfixContent.WriteInt32(IconFileDataID);
            record.HotfixContent.WriteUInt8(ItemGroupSoundsID);
            record.HotfixContent.WriteInt32(ContentTuningID);
            record.HotfixContent.WriteUInt32(MaxDurability);
            record.HotfixContent.WriteUInt8(AmmunitionType);
            record.HotfixContent.WriteUInt8(DamageType1);
            record.HotfixContent.WriteUInt8(DamageType2);
            record.HotfixContent.WriteUInt8(DamageType3);
            record.HotfixContent.WriteUInt8(DamageType4);
            record.HotfixContent.WriteUInt8(DamageType5);
            record.HotfixContent.WriteInt16(Resistances1);
            record.HotfixContent.WriteInt16(Resistances2);
            record.HotfixContent.WriteInt16(Resistances3);
            record.HotfixContent.WriteInt16(Resistances4);
            record.HotfixContent.WriteInt16(Resistances5);
            record.HotfixContent.WriteInt16(Resistances6);
            record.HotfixContent.WriteInt16(Resistances7);
            record.HotfixContent.WriteUInt16(MinDamage1);
            record.HotfixContent.WriteUInt16(MinDamage2);
            record.HotfixContent.WriteUInt16(MinDamage3);
            record.HotfixContent.WriteUInt16(MinDamage4);
            record.HotfixContent.WriteUInt16(MinDamage5);
            record.HotfixContent.WriteUInt16(MaxDamage1);
            record.HotfixContent.WriteUInt16(MaxDamage2);
            record.HotfixContent.WriteUInt16(MaxDamage3);
            record.HotfixContent.WriteUInt16(MaxDamage4);
            record.HotfixContent.WriteUInt16(MaxDamage5);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void WriteItemHotfix(ItemTemplate item, ByteBuffer buffer)
    {
        var fileDataId = (int)GetItemIconFileDataIdByDisplayId(item.DisplayID);
        buffer.WriteUInt8((byte)item.Class);
        buffer.WriteUInt8((byte)item.SubClass);
        buffer.WriteUInt8((byte)item.Material);
        buffer.WriteInt8((sbyte)item.InventoryType);
        buffer.WriteInt32((int)item.RequiredLevel);
        buffer.WriteUInt8((byte)item.SheathType);
        buffer.WriteUInt16((ushort)item.RandomProperty);
        buffer.WriteUInt16((ushort)item.RandomSuffix);
        buffer.WriteInt8(-1);
        buffer.WriteUInt16(0);
        buffer.WriteInt32(fileDataId);
        buffer.WriteUInt8(0);
        buffer.WriteInt32(0);
        buffer.WriteUInt32(item.MaxDurability);
        buffer.WriteUInt8((byte)item.AmmoType);
        buffer.WriteUInt8((byte)item.DamageTypes[0]);
        buffer.WriteUInt8((byte)item.DamageTypes[1]);
        buffer.WriteUInt8((byte)item.DamageTypes[2]);
        buffer.WriteUInt8((byte)item.DamageTypes[3]);
        buffer.WriteUInt8((byte)item.DamageTypes[4]);
        buffer.WriteInt16((short)item.Armor);
        buffer.WriteInt16((short)item.HolyResistance);
        buffer.WriteInt16((short)item.FireResistance);
        buffer.WriteInt16((short)item.NatureResistance);
        buffer.WriteInt16((short)item.FrostResistance);
        buffer.WriteInt16((short)item.ShadowResistance);
        buffer.WriteInt16((short)item.ArcaneResistance);
        buffer.WriteUInt16((ushort)item.DamageMins[0]);
        buffer.WriteUInt16((ushort)item.DamageMins[1]);
        buffer.WriteUInt16((ushort)item.DamageMins[2]);
        buffer.WriteUInt16((ushort)item.DamageMins[3]);
        buffer.WriteUInt16((ushort)item.DamageMins[4]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[0]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[1]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[2]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[3]);
        buffer.WriteUInt16((ushort)item.DamageMaxs[4]);
    }

    public static void WriteItemHotfix(ItemRecord row, ByteBuffer buffer)
    {
        buffer.WriteUInt8(row.ClassId);
        buffer.WriteUInt8(row.SubclassId);
        buffer.WriteUInt8(row.Material);
        buffer.WriteInt8(row.InventoryType);
        buffer.WriteInt32(row.RequiredLevel);
        buffer.WriteUInt8(row.SheatheType);
        buffer.WriteUInt16(row.RandomProperty);
        buffer.WriteUInt16(row.ItemRandomSuffixGroupId);
        buffer.WriteInt8(row.SoundOverrideSubclassId);
        buffer.WriteUInt16(row.ScalingStatDistributionId);
        buffer.WriteInt32(row.IconFileDataId);
        buffer.WriteUInt8(row.ItemGroupSoundsId);
        buffer.WriteInt32(row.ContentTuningId);
        buffer.WriteUInt32(row.MaxDurability);
        buffer.WriteUInt8(row.AmmoType);
        buffer.WriteUInt8(row.DamageType[0]);
        buffer.WriteUInt8(row.DamageType[1]);
        buffer.WriteUInt8(row.DamageType[2]);
        buffer.WriteUInt8(row.DamageType[3]);
        buffer.WriteUInt8(row.DamageType[4]);
        buffer.WriteInt16(row.Resistances[0]);
        buffer.WriteInt16(row.Resistances[1]);
        buffer.WriteInt16(row.Resistances[2]);
        buffer.WriteInt16(row.Resistances[3]);
        buffer.WriteInt16(row.Resistances[4]);
        buffer.WriteInt16(row.Resistances[5]);
        buffer.WriteInt16(row.Resistances[6]);
        buffer.WriteUInt16(row.MinDamage[0]);
        buffer.WriteUInt16(row.MinDamage[1]);
        buffer.WriteUInt16(row.MinDamage[2]);
        buffer.WriteUInt16(row.MinDamage[3]);
        buffer.WriteUInt16(row.MinDamage[4]);
        buffer.WriteUInt16(row.MaxDamage[0]);
        buffer.WriteUInt16(row.MaxDamage[1]);
        buffer.WriteUInt16(row.MaxDamage[2]);
        buffer.WriteUInt16(row.MaxDamage[3]);
        buffer.WriteUInt16(row.MaxDamage[4]);
    }

    public static void WriteItemAppearanceHotfix(ItemAppearance appearance, ByteBuffer buffer)
    {
        buffer.WriteUInt8(appearance.DisplayType);
        buffer.WriteInt32(appearance.ItemDisplayInfoID);
        buffer.WriteInt32(appearance.DefaultIconFileDataID);
        buffer.WriteInt32(appearance.UiOrder);
    }

    public static void WriteItemModifiedAppearanceHotfix(ItemModifiedAppearance modAppearance, ByteBuffer buffer)
    {
        buffer.WriteInt32(modAppearance.Id);
        buffer.WriteInt32(modAppearance.ItemID);
        buffer.WriteInt32(modAppearance.ItemAppearanceModifierID);
        buffer.WriteInt32(modAppearance.ItemAppearanceID);
        buffer.WriteInt32(modAppearance.OrderIndex);
        buffer.WriteInt32(modAppearance.TransmogSourceTypeEnum);
    }

    public static void WriteItemEffectHotfix(ItemEffect effect, ByteBuffer buffer)
    {
        buffer.WriteUInt8(effect.LegacySlotIndex);
        buffer.WriteInt8(effect.TriggerType);
        buffer.WriteInt16(effect.Charges);
        buffer.WriteInt32(effect.CoolDownMSec);
        buffer.WriteInt32(effect.CategoryCoolDownMSec);
        buffer.WriteUInt16(effect.SpellCategoryID);
        buffer.WriteInt32(effect.SpellID);
        buffer.WriteUInt16(effect.ChrSpecializationID);
        buffer.WriteInt32(effect.ParentItemID);
    }

    public static List<HotfixRecord> FindHotfixesByRecordIdAndTable(uint id, DB2Hash table, uint startId = 0u)
    {
        return Hotfixes.Values
            .Where(hotfix => hotfix.HotfixId >= startId && hotfix.TableHash == table && hotfix.RecordId == id).ToList();
    }

    public static void UpdateHotfix(object obj, bool remove = false)
    {
        if (obj is ItemRecord)
        {
            var item = (ItemRecord)obj;
            DoStuff((uint)item.Id, DB2Hash.Item,
                delegate(ByteBuffer hotfixContentTargetBuffer) { WriteItemHotfix(item, hotfixContentTargetBuffer); });
        }

        if (obj is ItemSparseRecord)
        {
            var itemSparse = (ItemSparseRecord)obj;
            DoStuff((uint)itemSparse.Id, DB2Hash.ItemSparse,
                delegate(ByteBuffer hotfixContentTargetBuffer)
                {
                    WriteItemSparseHotfix(itemSparse, hotfixContentTargetBuffer);
                });
        }

        if (obj is ItemEffect)
        {
            var effect = (ItemEffect)obj;
            DoStuff((uint)effect.Id, DB2Hash.ItemEffect,
                delegate(ByteBuffer hotfixContentTargetBuffer)
                {
                    WriteItemEffectHotfix(effect, hotfixContentTargetBuffer);
                });
        }

        if (obj is ItemAppearance)
        {
            var appearance = (ItemAppearance)obj;
            DoStuff((uint)appearance.Id, DB2Hash.ItemAppearance,
                delegate(ByteBuffer hotfixContentTargetBuffer)
                {
                    WriteItemAppearanceHotfix(appearance, hotfixContentTargetBuffer);
                });
        }

        if (obj is ItemModifiedAppearance)
        {
            var modAppearance = (ItemModifiedAppearance)obj;
            DoStuff((uint)modAppearance.Id, DB2Hash.ItemModifiedAppearance,
                delegate(ByteBuffer hotfixContentTargetBuffer)
                {
                    WriteItemModifiedAppearanceHotfix(modAppearance, hotfixContentTargetBuffer);
                });
        }

        static void DoStuff(uint recordId, DB2Hash table, Action<ByteBuffer> writer)
        {
            var oldRecords = FindHotfixesByRecordIdAndTable(recordId, table, 210000u);
            if (oldRecords.Count == 0)
            {
                var record = new HotfixRecord
                {
                    Status = HotfixStatus.Valid,
                    TableHash = table,
                    HotfixId = GetFirstFreeId(Hotfixes, 210000u)
                };
                record.UniqueId = record.HotfixId;
                record.RecordId = recordId;
                writer(record.HotfixContent);
                Hotfixes[record.HotfixId] = record;
            }
            else
            {
                var oldRecordsToBeInvalided = oldRecords.SkipLast(1);
                foreach (var record2 in oldRecordsToBeInvalided)
                {
                    record2.Status = HotfixStatus.Invalid;
                    record2.HotfixContent = new ByteBuffer();
                    Log.Print(LogType.Storage,
                        $"Got duplicate record for record {record2.RecordId} in {record2.TableHash}", "GameData.cs");
                }

                var recordToOverwrite = oldRecords.Last();
                recordToOverwrite.HotfixContent = new ByteBuffer();
                writer(recordToOverwrite.HotfixContent);
                Hotfixes[recordToOverwrite.HotfixId] = recordToOverwrite;
            }
        }
    }

    public static HotFixMessage? GenerateItemUpdateIfNeeded(ItemTemplate item)
    {
        ItemRecordsStore.TryGetValue(item.Entry, out var row);
        if (row != null)
        {
            var iconFileDataId = (int)GetItemIconFileDataIdByDisplayId(item.DisplayID);
            if (row.ClassId != (byte)item.Class || row.SubclassId != (byte)item.SubClass ||
                row.Material != (byte)item.Material || row.InventoryType != (sbyte)item.InventoryType ||
                row.RequiredLevel != (int)item.RequiredLevel || row.SheatheType != (byte)item.SheathType ||
                row.RandomProperty != (ushort)item.RandomProperty ||
                row.ItemRandomSuffixGroupId != (ushort)item.RandomSuffix ||
                (row.IconFileDataId != iconFileDataId && iconFileDataId != 0) ||
                row.MaxDurability != item.MaxDurability || row.AmmoType != (byte)item.AmmoType ||
                row.DamageType[0] != (byte)item.DamageTypes[0] || row.DamageType[1] != (byte)item.DamageTypes[1] ||
                row.DamageType[2] != (byte)item.DamageTypes[2] || row.DamageType[3] != (byte)item.DamageTypes[3] ||
                row.DamageType[4] != (byte)item.DamageTypes[4] || row.Resistances[1] != (short)item.HolyResistance ||
                row.Resistances[2] != (short)item.FireResistance ||
                row.Resistances[3] != (short)item.NatureResistance ||
                row.Resistances[4] != (short)item.FrostResistance ||
                row.Resistances[5] != (short)item.ShadowResistance ||
                row.Resistances[6] != (short)item.ArcaneResistance)
            {
                Log.Print(LogType.Storage, $"Item #{item.Entry} needs to be updated.", "GameData.cs");
                if (row.ClassId != (byte)item.Class)
                {
                    Log.Print(LogType.Storage, $"ClassId {row.ClassId} vs {item.Class}", "GameData.cs");
                }

                if (row.SubclassId != (byte)item.SubClass)
                {
                    Log.Print(LogType.Storage, $"SubclassId {row.SubclassId} vs {item.SubClass}", "GameData.cs");
                }

                if (row.Material != (byte)item.Material)
                {
                    Log.Print(LogType.Storage, $"Material {row.Material} vs {item.Material}", "GameData.cs");
                }

                if (row.InventoryType != (sbyte)item.InventoryType)
                {
                    Log.Print(LogType.Storage, $"InventoryType {row.InventoryType} vs {item.InventoryType}",
                        "GameData.cs");
                }

                if (row.RequiredLevel != (int)item.RequiredLevel)
                {
                    Log.Print(LogType.Storage, $"RequiredLevel {row.RequiredLevel} vs {item.RequiredLevel}",
                        "GameData.cs");
                }

                if (row.SheatheType != (byte)item.SheathType)
                {
                    Log.Print(LogType.Storage, $"SheatheType {row.SheatheType} vs {item.SheathType}", "GameData.cs");
                }

                if (row.RandomProperty != (ushort)item.RandomProperty)
                {
                    Log.Print(LogType.Storage, $"RandomProperty {row.RandomProperty} vs {item.RandomProperty}",
                        "GameData.cs");
                }

                if (row.ItemRandomSuffixGroupId != (ushort)item.RandomSuffix)
                {
                    Log.Print(LogType.Storage,
                        $"ItemRandomSuffixGroupId {row.ItemRandomSuffixGroupId} vs {item.RandomSuffix}", "GameData.cs");
                }

                if (row.IconFileDataId != iconFileDataId)
                {
                    Log.Print(LogType.Storage, $"IconFileDataId {row.IconFileDataId} vs {iconFileDataId}",
                        "GameData.cs");
                }

                if (row.MaxDurability != item.MaxDurability)
                {
                    Log.Print(LogType.Storage, $"MaxDurability {row.MaxDurability} vs {item.MaxDurability}",
                        "GameData.cs");
                }

                if (row.AmmoType != (byte)item.AmmoType)
                {
                    Log.Print(LogType.Storage, $"AmmoType {row.AmmoType} vs {item.AmmoType}", "GameData.cs");
                }

                for (var i = 0; i < 5; i++)
                {
                    if (row.DamageType[i] != (byte)item.DamageTypes[i])
                    {
                        Log.Print(LogType.Storage, $"DamageType[{i}] {row.DamageType[i]} vs {item.DamageTypes[i]}",
                            "GameData.cs");
                    }
                }

                if (row.Resistances[1] != (short)item.HolyResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[1] {row.Resistances[1]} vs {item.HolyResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[2] != (short)item.FireResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[2] {row.Resistances[2]} vs {item.FireResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[3] != (short)item.NatureResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[3] {row.Resistances[3]} vs {item.NatureResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[4] != (short)item.FrostResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[4] {row.Resistances[4]} vs {item.FrostResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[5] != (short)item.ShadowResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[5] {row.Resistances[5]} vs {item.ShadowResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[6] != (short)item.ArcaneResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[6] {row.Resistances[6]} vs {item.ArcaneResistance}",
                        "GameData.cs");
                }

                UpdateItemRecord(row, item);
                UpdateHotfix(row);
                return GenerateHotFixMessage(row);
            }

            return null;
        }

        row = AddItemRecord(item);
        if (row == null)
        {
            return null;
        }

        UpdateHotfix(row);
        return GenerateHotFixMessage(row);
    }

    public static HotFixMessage? GenerateItemSparseUpdateIfNeeded(ItemTemplate item)
    {
        ItemSparseRecordsStore.TryGetValue(item.Entry, out var row);
        if (row != null)
        {
            if (!row.Description.Equals(item.Description) || !row.Name4.Equals(item.Name[3]) ||
                !row.Name3.Equals(item.Name[2]) || !row.Name2.Equals(item.Name[1]) || !row.Name1.Equals(item.Name[0]) ||
                row.DurationInInventory != item.Duration || row.BagFamily != item.BagFamily ||
                row.RangeMod != item.RangedMod || row.RequiredAbility != item.RequiredSpell ||
                row.SellPrice != item.SellPrice || row.BuyPrice != item.BuyPrice ||
                row.MaxDurability != item.MaxDurability || row.RequiredHoliday != (ushort)item.HolidayID ||
                row.LimitCategory != (ushort)item.ItemLimitCategory ||
                row.GemProperties != (ushort)item.GemProperties ||
                row.SocketMatchEnchantmentId != (ushort)item.SocketBonus ||
                row.TotemCategoryId != (ushort)item.TotemCategory || row.InstanceBound != (ushort)item.MapID ||
                row.ZoneBound[0] != (ushort)item.AreaID || row.ItemSet != (ushort)item.ItemSet ||
                row.LockId != (ushort)item.LockId || row.StartQuestId != (ushort)item.StartQuestId ||
                row.PageText != (ushort)item.PageText || row.Delay != (ushort)item.Delay ||
                row.RequiredReputationId != (ushort)item.RequiredRepFaction ||
                row.RequiredSkillRank != (ushort)item.RequiredSkillLevel ||
                row.RequiredSkill != (ushort)item.RequiredSkillId || row.ItemLevel != (ushort)item.ItemLevel ||
                row.ItemRandomSuffixGroupId != (ushort)item.RandomSuffix ||
                row.RandomProperty != (ushort)item.RandomProperty || row.Resistances[1] != (short)item.HolyResistance ||
                row.Resistances[2] != (short)item.FireResistance ||
                row.Resistances[3] != (short)item.NatureResistance ||
                row.Resistances[4] != (short)item.FrostResistance ||
                row.Resistances[5] != (short)item.ShadowResistance ||
                row.Resistances[6] != (short)item.ArcaneResistance ||
                row.ScalingStatDistributionId != (ushort)item.ScalingStatDistribution ||
                row.SocketType[0] != ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[0]) ||
                row.SocketType[1] != ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[1]) ||
                row.SocketType[2] != ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[2]) ||
                row.SheatheType != (byte)item.SheathType || row.Material != (byte)item.Material ||
                row.PageMaterial != (byte)item.PageMaterial || row.PageLanguage != (byte)item.Language ||
                row.Bonding != (byte)item.Bonding || row.DamageType != (byte)item.DamageTypes[0] ||
                (row.StatType[0] != (sbyte)item.StatTypes[0] &&
                 (row.StatModifierBonusAmount[0] != 0 || item.StatValues[0] != 0)) ||
                (row.StatType[1] != (sbyte)item.StatTypes[1] &&
                 (row.StatModifierBonusAmount[1] != 0 || item.StatValues[1] != 0)) ||
                (row.StatType[2] != (sbyte)item.StatTypes[2] &&
                 (row.StatModifierBonusAmount[2] != 0 || item.StatValues[2] != 0)) ||
                (row.StatType[3] != (sbyte)item.StatTypes[3] &&
                 (row.StatModifierBonusAmount[3] != 0 || item.StatValues[3] != 0)) ||
                (row.StatType[4] != (sbyte)item.StatTypes[4] &&
                 (row.StatModifierBonusAmount[4] != 0 || item.StatValues[4] != 0)) ||
                (row.StatType[5] != (sbyte)item.StatTypes[5] &&
                 (row.StatModifierBonusAmount[5] != 0 || item.StatValues[5] != 0)) ||
                (row.StatType[6] != (sbyte)item.StatTypes[6] &&
                 (row.StatModifierBonusAmount[6] != 0 || item.StatValues[6] != 0)) ||
                (row.StatType[7] != (sbyte)item.StatTypes[7] &&
                 (row.StatModifierBonusAmount[7] != 0 || item.StatValues[7] != 0)) ||
                (row.StatType[8] != (sbyte)item.StatTypes[8] &&
                 (row.StatModifierBonusAmount[8] != 0 || item.StatValues[8] != 0)) ||
                (row.StatType[9] != (sbyte)item.StatTypes[9] &&
                 (row.StatModifierBonusAmount[9] != 0 || item.StatValues[9] != 0)) ||
                row.ContainerSlots != (byte)item.ContainerSlots ||
                row.RequiredReputationRank != (byte)item.RequiredRepValue ||
                row.RequiredCityRank != (byte)item.RequiredCityRank ||
                row.RequiredHonorRank != (byte)item.RequiredHonorRank ||
                row.InventoryType != (byte)item.InventoryType || row.OverallQualityId != (byte)item.Quality ||
                row.AmmoType != (byte)item.AmmoType || row.StatModifierBonusAmount[0] != (sbyte)item.StatValues[0] ||
                row.StatModifierBonusAmount[1] != (sbyte)item.StatValues[1] ||
                row.StatModifierBonusAmount[2] != (sbyte)item.StatValues[2] ||
                row.StatModifierBonusAmount[3] != (sbyte)item.StatValues[3] ||
                row.StatModifierBonusAmount[4] != (sbyte)item.StatValues[4] ||
                row.StatModifierBonusAmount[5] != (sbyte)item.StatValues[5] ||
                row.StatModifierBonusAmount[6] != (sbyte)item.StatValues[6] ||
                row.StatModifierBonusAmount[7] != (sbyte)item.StatValues[7] ||
                row.StatModifierBonusAmount[8] != (sbyte)item.StatValues[8] ||
                row.StatModifierBonusAmount[9] != (sbyte)item.StatValues[9] ||
                row.RequiredLevel != (sbyte)item.RequiredLevel)
            {
                Log.Print(LogType.Storage, $"ItemSparse #{item.Entry} needs to be updated.", "GameData.cs");
                if (!row.Description.Equals(item.Description))
                {
                    Log.Print(LogType.Storage, $"Description \"{row.Description}\" vs \"{item.Description}\"",
                        "GameData.cs");
                }

                if (!row.Name4.Equals(item.Name[3]))
                {
                    Log.Print(LogType.Storage, $"Name4 \"{row.Name4}\" vs \"{item.Name[3]}\"", "GameData.cs");
                }

                if (!row.Name3.Equals(item.Name[2]))
                {
                    Log.Print(LogType.Storage, $"Name3 \"{row.Name3}\" vs \"{item.Name[2]}\"", "GameData.cs");
                }

                if (!row.Name2.Equals(item.Name[1]))
                {
                    Log.Print(LogType.Storage, $"Name2 \"{row.Name2}\" vs \"{item.Name[1]}\"", "GameData.cs");
                }

                if (!row.Name1.Equals(item.Name[0]))
                {
                    Log.Print(LogType.Storage, $"Name1 \"{row.Name1}\" vs \"{item.Name[0]}\"", "GameData.cs");
                }

                if (row.DurationInInventory != item.Duration)
                {
                    Log.Print(LogType.Storage, $"DurationInInventory {row.DurationInInventory} vs {item.Duration}",
                        "GameData.cs");
                }

                if (row.BagFamily != item.BagFamily)
                {
                    Log.Print(LogType.Storage, $"BagFamily {row.BagFamily} vs {item.BagFamily}", "GameData.cs");
                }

                if (row.RangeMod != item.RangedMod)
                {
                    Log.Print(LogType.Storage, $"RangeMod {row.RangeMod} vs {item.RangedMod}", "GameData.cs");
                }

                if (row.RequiredAbility != item.RequiredSpell)
                {
                    Log.Print(LogType.Storage, $"RequiredAbility {row.RequiredAbility} vs {item.RequiredSpell}",
                        "GameData.cs");
                }

                if (row.SellPrice != item.SellPrice)
                {
                    Log.Print(LogType.Storage, $"SellPrice {row.SellPrice} vs {item.SellPrice}", "GameData.cs");
                }

                if (row.BuyPrice != item.BuyPrice)
                {
                    Log.Print(LogType.Storage, $"BuyPrice {row.BuyPrice} vs {item.BuyPrice}", "GameData.cs");
                }

                if (row.MaxDurability != item.MaxDurability)
                {
                    Log.Print(LogType.Storage, $"MaxDurability {row.MaxDurability} vs {item.MaxDurability}",
                        "GameData.cs");
                }

                if (row.RequiredHoliday != (ushort)item.HolidayID)
                {
                    Log.Print(LogType.Storage, $"RequiredHoliday {row.RequiredHoliday} vs {item.HolidayID}",
                        "GameData.cs");
                }

                if (row.LimitCategory != (ushort)item.ItemLimitCategory)
                {
                    Log.Print(LogType.Storage, $"LimitCategory {row.LimitCategory} vs {item.ItemLimitCategory}",
                        "GameData.cs");
                }

                if (row.GemProperties != (ushort)item.GemProperties)
                {
                    Log.Print(LogType.Storage, $"GemProperties {row.GemProperties} vs {item.GemProperties}",
                        "GameData.cs");
                }

                if (row.SocketMatchEnchantmentId != (ushort)item.SocketBonus)
                {
                    Log.Print(LogType.Storage,
                        $"SocketMatchEnchantmentId {row.SocketMatchEnchantmentId} vs {item.SocketBonus}",
                        "GameData.cs");
                }

                if (row.TotemCategoryId != (ushort)item.TotemCategory)
                {
                    Log.Print(LogType.Storage, $"TotemCategoryId {row.TotemCategoryId} vs {item.TotemCategory}",
                        "GameData.cs");
                }

                if (row.InstanceBound != (ushort)item.MapID)
                {
                    Log.Print(LogType.Storage, $"InstanceBound {row.InstanceBound} vs {item.MapID}", "GameData.cs");
                }

                if (row.ZoneBound[0] != (ushort)item.AreaID)
                {
                    Log.Print(LogType.Storage, $"ZoneBound[0] {row.ZoneBound[0]} vs {item.AreaID}", "GameData.cs");
                }

                if (row.ItemSet != (ushort)item.ItemSet)
                {
                    Log.Print(LogType.Storage, $"ItemSet {row.ItemSet} vs {item.ItemSet}", "GameData.cs");
                }

                if (row.LockId != (ushort)item.LockId)
                {
                    Log.Print(LogType.Storage, $"LockId {row.LockId} vs {item.LockId}", "GameData.cs");
                }

                if (row.StartQuestId != (ushort)item.StartQuestId)
                {
                    Log.Print(LogType.Storage, $"StartQuestId {row.StartQuestId} vs {item.StartQuestId}",
                        "GameData.cs");
                }

                if (row.PageText != (ushort)item.PageText)
                {
                    Log.Print(LogType.Storage, $"PageText {row.PageText} vs {item.PageText}", "GameData.cs");
                }

                if (row.Delay != (ushort)item.Delay)
                {
                    Log.Print(LogType.Storage, $"Delay {row.Delay} vs {item.Delay}", "GameData.cs");
                }

                if (row.RequiredReputationId != (ushort)item.RequiredRepFaction)
                {
                    Log.Print(LogType.Storage,
                        $"RequiredReputationId {row.RequiredReputationId} vs {item.RequiredRepFaction}", "GameData.cs");
                }

                if (row.RequiredSkillRank != (ushort)item.RequiredSkillLevel)
                {
                    Log.Print(LogType.Storage,
                        $"RequiredSkillRank {row.RequiredSkillRank} vs {item.RequiredSkillLevel}", "GameData.cs");
                }

                if (row.RequiredSkill != (ushort)item.RequiredSkillId)
                {
                    Log.Print(LogType.Storage, $"RequiredSkill {row.RequiredSkill} vs {item.RequiredSkillId}",
                        "GameData.cs");
                }

                if (row.ItemLevel != (ushort)item.ItemLevel)
                {
                    Log.Print(LogType.Storage, $"ItemLevel {row.ItemLevel} vs {item.ItemLevel}", "GameData.cs");
                }

                if (row.ItemRandomSuffixGroupId != (ushort)item.RandomSuffix)
                {
                    Log.Print(LogType.Storage,
                        $"ItemRandomSuffixGroupId {row.ItemRandomSuffixGroupId} vs {item.RandomSuffix}", "GameData.cs");
                }

                if (row.RandomProperty != (ushort)item.RandomProperty)
                {
                    Log.Print(LogType.Storage, $"RandomProperty {row.RandomProperty} vs {item.RandomProperty}",
                        "GameData.cs");
                }

                if (row.Resistances[1] != (short)item.HolyResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[1] {row.Resistances[1]} vs {item.HolyResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[2] != (short)item.FireResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[2] {row.Resistances[2]} vs {item.FireResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[3] != (short)item.NatureResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[3]  {row.Resistances[3]} vs {item.NatureResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[4] != (short)item.FrostResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[4] {row.Resistances[4]} vs {item.FrostResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[5] != (short)item.ShadowResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[5] {row.Resistances[5]} vs {item.ShadowResistance}",
                        "GameData.cs");
                }

                if (row.Resistances[6] != (short)item.ArcaneResistance)
                {
                    Log.Print(LogType.Storage, $"Resistances[6] {row.Resistances[6]} vs {item.ArcaneResistance}",
                        "GameData.cs");
                }

                if (row.ScalingStatDistributionId != (ushort)item.ScalingStatDistribution)
                {
                    Log.Print(LogType.Storage,
                        $"ScalingStatDistributionId {row.ScalingStatDistributionId} vs {item.ScalingStatDistribution}",
                        "GameData.cs");
                }

                for (var i = 0; i < 3; i++)
                {
                    if (row.SocketType[i] != ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[i]))
                    {
                        Log.Print(LogType.Storage,
                            $"SocketType[{i}] {row.SocketType[i]} vs {ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[i])}",
                            "GameData.cs");
                    }
                }

                if (row.SheatheType != (byte)item.SheathType)
                {
                    Log.Print(LogType.Storage, $"SheatheType {row.SheatheType} vs {item.SheathType}", "GameData.cs");
                }

                if (row.Material != (byte)item.Material)
                {
                    Log.Print(LogType.Storage, $"Material {row.Material} vs {item.Material}", "GameData.cs");
                }

                if (row.PageMaterial != (byte)item.PageMaterial)
                {
                    Log.Print(LogType.Storage, $"PageMaterial {row.PageMaterial} vs {item.PageMaterial}",
                        "GameData.cs");
                }

                if (row.PageLanguage != (byte)item.Language)
                {
                    Log.Print(LogType.Storage, $"PageLanguage {row.PageLanguage} vs {item.Language}", "GameData.cs");
                }

                if (row.Bonding != (byte)item.Bonding)
                {
                    Log.Print(LogType.Storage, $"Bonding {row.Bonding} vs {item.Bonding}", "GameData.cs");
                }

                if (row.DamageType != (byte)item.DamageTypes[0])
                {
                    Log.Print(LogType.Storage, $"DamageType {row.DamageType} vs {item.DamageTypes[0]}", "GameData.cs");
                }

                for (var j = 0; j < 10; j++)
                {
                    if (row.StatType[j] != (sbyte)item.StatTypes[j] &&
                        (row.StatModifierBonusAmount[j] != 0 || item.StatValues[j] != 0))
                    {
                        Log.Print(LogType.Storage, $"StatType[{j}] {row.StatType[j]} vs {item.StatTypes[j]}",
                            "GameData.cs");
                    }
                }

                if (row.ContainerSlots != (byte)item.ContainerSlots)
                {
                    Log.Print(LogType.Storage, $"ContainerSlots {row.ContainerSlots} vs {item.ContainerSlots}",
                        "GameData.cs");
                }

                if (row.RequiredReputationRank != (byte)item.RequiredRepValue)
                {
                    Log.Print(LogType.Storage,
                        $"RequiredReputationRank {row.RequiredReputationRank} vs {item.RequiredRepValue}",
                        "GameData.cs");
                }

                if (row.RequiredCityRank != (byte)item.RequiredCityRank)
                {
                    Log.Print(LogType.Storage, $"RequiredCityRank {row.RequiredCityRank} vs {item.RequiredCityRank}",
                        "GameData.cs");
                }

                if (row.RequiredHonorRank != (byte)item.RequiredHonorRank)
                {
                    Log.Print(LogType.Storage, $"RequiredHonorRank {row.RequiredHonorRank} vs {item.RequiredHonorRank}",
                        "GameData.cs");
                }

                if (row.InventoryType != (byte)item.InventoryType)
                {
                    Log.Print(LogType.Storage, $"InventoryType {row.InventoryType} vs {item.InventoryType}",
                        "GameData.cs");
                }

                if (row.OverallQualityId != (byte)item.Quality)
                {
                    Log.Print(LogType.Storage, $"OverallQualityId {row.OverallQualityId} vs {item.Quality}",
                        "GameData.cs");
                }

                if (row.AmmoType != (byte)item.AmmoType)
                {
                    Log.Print(LogType.Storage, $"AmmoType {row.AmmoType} vs {item.AmmoType}", "GameData.cs");
                }

                for (var k = 0; k < 10; k++)
                {
                    if (row.StatModifierBonusAmount[0] != (sbyte)item.StatValues[0])
                    {
                        Log.Print(LogType.Storage,
                            $"StatValue[{k}] {row.StatModifierBonusAmount[k]} vs {item.StatValues[k]}", "GameData.cs");
                    }
                }

                if (row.RequiredLevel != (sbyte)item.RequiredLevel)
                {
                    Log.Print(LogType.Storage, $"RequiredLevel {row.RequiredLevel} vs {item.RequiredLevel}",
                        "GameData.cs");
                }

                UpdateItemSparseRecord(row, item);
                UpdateHotfix(row);
                return null;
            }

            return null;
        }

        row = AddItemSparseRecord(item);
        if (row == null)
        {
            return null;
        }

        UpdateHotfix(row);
        return GenerateHotFixMessage(row);
    }

    public static HotFixMessage? GenerateItemEffectUpdateIfNeeded(ItemTemplate item, byte slot)
    {
        var effect = GetItemEffectByItemId(item.Entry, slot);
        if (effect != null)
        {
            var wrongCategory = false;
            var wrongCooldown = false;
            var wrongCatCooldown = false;
            if (item.TriggeredSpellIds[slot] > 0)
            {
                ItemSpellsDataStore.TryGetValue((uint)item.TriggeredSpellIds[slot], out var data);
                if (data != null)
                {
                    if (effect.SpellCategoryID != item.TriggeredSpellCategories[slot])
                    {
                        wrongCategory = data.Category != item.TriggeredSpellCategories[slot];
                    }

                    if (Math.Abs(effect.CoolDownMSec - item.TriggeredSpellCooldowns[slot]) > 1)
                    {
                        wrongCooldown = data.RecoveryTime != item.TriggeredSpellCooldowns[slot];
                    }

                    if (Math.Abs(effect.CategoryCoolDownMSec - item.TriggeredSpellCategoryCooldowns[slot]) > 1)
                    {
                        wrongCatCooldown = data.CategoryRecoveryTime != item.TriggeredSpellCategoryCooldowns[slot];
                    }
                }
            }

            if (effect.TriggerType != item.TriggeredSpellTypes[slot] ||
                effect.Charges != item.TriggeredSpellCharges[slot] || wrongCooldown || wrongCatCooldown ||
                wrongCategory || effect.SpellID != item.TriggeredSpellIds[slot])
            {
                if (item.TriggeredSpellIds[slot] > 0)
                {
                    Log.Print(LogType.Storage, $"ItemEffect for item #{item.Entry} slot #{slot} needs to be updated.",
                        "GameData.cs");
                    if (effect.TriggerType != item.TriggeredSpellTypes[slot])
                    {
                        Log.Print(LogType.Storage,
                            $"TriggerType {effect.TriggerType} vs {item.TriggeredSpellTypes[slot]}", "GameData.cs");
                    }

                    if (effect.Charges != item.TriggeredSpellCharges[slot])
                    {
                        Log.Print(LogType.Storage, $"Charges {effect.Charges} vs {item.TriggeredSpellCharges[slot]}",
                            "GameData.cs");
                    }

                    if (wrongCooldown)
                    {
                        Log.Print(LogType.Storage,
                            $"CoolDownMSec {effect.CoolDownMSec} vs {item.TriggeredSpellCooldowns[slot]}",
                            "GameData.cs");
                    }

                    if (wrongCatCooldown)
                    {
                        Log.Print(LogType.Storage,
                            $"CategoryCoolDownMSec {effect.CategoryCoolDownMSec} vs {item.TriggeredSpellCategoryCooldowns[slot]}",
                            "GameData.cs");
                    }

                    if (wrongCategory)
                    {
                        Log.Print(LogType.Storage,
                            $"SpellCategoryId {effect.SpellCategoryID} vs {item.TriggeredSpellCategories[slot]}",
                            "GameData.cs");
                    }

                    if (effect.SpellID != item.TriggeredSpellIds[slot])
                    {
                        Log.Print(LogType.Storage, $"SpellId {effect.SpellID} vs {item.TriggeredSpellIds[slot]}",
                            "GameData.cs");
                    }

                    effect.TriggerType = (sbyte)item.TriggeredSpellTypes[slot];
                    effect.Charges = (short)item.TriggeredSpellCharges[slot];
                    effect.CoolDownMSec = wrongCooldown ? item.TriggeredSpellCooldowns[slot] : -1;
                    effect.CategoryCoolDownMSec = wrongCatCooldown ? item.TriggeredSpellCategoryCooldowns[slot] : -1;
                    effect.SpellCategoryID = (ushort)(wrongCategory ? (ushort)item.TriggeredSpellCategories[slot] : 0);
                    effect.SpellID = item.TriggeredSpellIds[slot];
                    UpdateItemEffectRecord(effect, item);
                    UpdateHotfix(effect);
                    return GenerateHotFixMessage(effect);
                }

                RemoveItemEffectRecord(effect);
                UpdateHotfix(effect, remove: true);
                return GenerateHotFixMessage(effect, remove: true);
            }
        }
        else if (item.TriggeredSpellIds[slot] > 0)
        {
            effect = AddItemEffectRecord(item, slot);
            if (effect == null)
            {
                return null;
            }

            UpdateHotfix(effect);
            return GenerateHotFixMessage(effect);
        }

        return null;
    }

    public static HotFixMessage? GenerateItemAppearanceUpdateIfNeeded(ItemTemplate item)
    {
        var appearance = GetItemAppearanceByDisplayId(item.DisplayID);
        if (appearance == null)
        {
            appearance = AddItemAppearanceRecord(item);
            if (appearance == null)
            {
                return null;
            }

            UpdateHotfix(appearance);
            return GenerateHotFixMessage(appearance);
        }

        return null;
    }

    public static HotFixMessage? GenerateItemModifiedAppearanceUpdateIfNeeded(ItemTemplate item)
    {
        var modAppearance = GetItemModifiedAppearanceByItemId(item.Entry);
        if (modAppearance != null)
        {
            ItemAppearanceStore.TryGetValue((uint)modAppearance.ItemAppearanceID, out var appearance);
            if (appearance == null || appearance.ItemDisplayInfoID != item.DisplayID)
            {
                Log.Print(LogType.Storage,
                    $"ItemModifiedAppearance #{modAppearance.Id} for item #{item.Entry} needs to be updated.",
                    "GameData.cs");
                if (appearance == null)
                {
                    Log.Print(LogType.Storage, $"ItemAppearance #{modAppearance.ItemAppearanceID} missing.",
                        "GameData.cs");
                }
                else if (appearance.ItemDisplayInfoID != item.DisplayID)
                {
                    Log.Print(LogType.Storage, $"DisplayID {appearance.ItemDisplayInfoID} vs {item.DisplayID}",
                        "GameData.cs");
                }

                UpdateItemModifiedAppearanceRecord(modAppearance, item);
                UpdateHotfix(modAppearance);
                return GenerateHotFixMessage(modAppearance);
            }

            return null;
        }

        modAppearance = AddItemModifiedAppearanceRecord(item);
        if (modAppearance == null)
        {
            return null;
        }

        UpdateHotfix(modAppearance);
        return GenerateHotFixMessage(modAppearance);
    }

    public static HotFixMessage? GenerateHotFixMessage(object obj, bool remove = false)
    {
        var reply = new HotFixMessage();
        if (obj == null)
        {
            Log.Print(LogType.Error, "DBReply for NULL object requested!", "GameData.cs");
            return null;
        }

        var type = obj.GetType();
        if (obj is ItemRecord)
        {
            var records = FindHotfixesByRecordIdAndTable((uint)((ItemRecord)obj).Id, DB2Hash.Item);
            reply.Hotfixes.AddRange(records);
        }
        else if (obj is ItemSparseRecord)
        {
            var records2 = FindHotfixesByRecordIdAndTable((uint)((ItemSparseRecord)obj).Id, DB2Hash.ItemSparse);
            reply.Hotfixes.AddRange(records2);
        }
        else if (obj is ItemEffect)
        {
            var records3 = FindHotfixesByRecordIdAndTable((uint)((ItemEffect)obj).Id, DB2Hash.ItemEffect);
            reply.Hotfixes.AddRange(records3);
        }
        else if (obj is ItemAppearance)
        {
            var records4 = FindHotfixesByRecordIdAndTable((uint)((ItemAppearance)obj).Id, DB2Hash.ItemAppearance);
            reply.Hotfixes.AddRange(records4);
        }
        else
        {
            if (!(obj is ItemModifiedAppearance))
            {
                Log.Print(LogType.Error, $"Unsupported DBReply requested! ({type})", "GameData.cs");
                return null;
            }

            var records5 = FindHotfixesByRecordIdAndTable((uint)((ItemModifiedAppearance)obj).Id,
                DB2Hash.ItemModifiedAppearance);
            reply.Hotfixes.AddRange(records5);
        }

        return reply;
    }

    public static ItemRecord AddItemRecord(ItemTemplate item)
    {
        var record = new ItemRecord
        {
            Id = (int)item.Entry
        };
        UpdateItemRecord(record, item);
        ItemRecordsStore.Add((uint)record.Id, record);
        Log.Print(LogType.Storage, $"Item #{record.Id} created.", "GameData.cs");
        return record;
    }

    public static void UpdateItemRecord(ItemRecord row, ItemTemplate item)
    {
        row.ClassId = (byte)item.Class;
        row.SubclassId = (byte)item.SubClass;
        row.Material = (byte)item.Material;
        row.InventoryType = (sbyte)item.InventoryType;
        row.RequiredLevel = (int)item.RequiredLevel;
        row.SheatheType = (byte)item.SheathType;
        row.RandomProperty = (ushort)item.RandomProperty;
        row.ItemRandomSuffixGroupId = (ushort)item.RandomSuffix;
        row.SoundOverrideSubclassId = -1;
        row.ScalingStatDistributionId = 0;
        row.IconFileDataId = (int)GetItemIconFileDataIdByDisplayId(item.DisplayID);
        row.ItemGroupSoundsId = 0;
        row.ContentTuningId = 0;
        row.MaxDurability = item.MaxDurability;
        row.AmmoType = (byte)item.AmmoType;
        row.DamageType[0] = (byte)item.DamageTypes[0];
        row.DamageType[1] = (byte)item.DamageTypes[1];
        row.DamageType[2] = (byte)item.DamageTypes[2];
        row.DamageType[3] = (byte)item.DamageTypes[3];
        row.DamageType[4] = (byte)item.DamageTypes[4];
        row.Resistances[0] = (short)item.Armor;
        row.Resistances[1] = (short)item.HolyResistance;
        row.Resistances[2] = (short)item.FireResistance;
        row.Resistances[3] = (short)item.NatureResistance;
        row.Resistances[4] = (short)item.FrostResistance;
        row.Resistances[5] = (short)item.ShadowResistance;
        row.Resistances[6] = (short)item.ArcaneResistance;
        row.MinDamage[0] = (ushort)item.DamageMins[0];
        row.MinDamage[1] = (ushort)item.DamageMins[1];
        row.MinDamage[2] = (ushort)item.DamageMins[2];
        row.MinDamage[3] = (ushort)item.DamageMins[3];
        row.MinDamage[4] = (ushort)item.DamageMins[4];
        row.MaxDamage[0] = (ushort)item.DamageMaxs[0];
        row.MaxDamage[1] = (ushort)item.DamageMaxs[1];
        row.MaxDamage[2] = (ushort)item.DamageMaxs[2];
        row.MaxDamage[3] = (ushort)item.DamageMaxs[3];
        row.MaxDamage[4] = (ushort)item.DamageMaxs[4];
        if (ItemRecordsStore.ContainsKey(item.Entry))
        {
            ItemRecordsStore[item.Entry] = row;
        }
    }

    public static ItemSparseRecord AddItemSparseRecord(ItemTemplate item)
    {
        var record = new ItemSparseRecord
        {
            Id = (int)item.Entry
        };
        UpdateItemSparseRecord(record, item);
        ItemSparseRecordsStore.Add((uint)record.Id, record);
        Log.Print(LogType.Storage, $"ItemSparse #{record.Id} created.", "GameData.cs");
        return record;
    }

    public static void UpdateItemSparseRecord(ItemSparseRecord row, ItemTemplate item)
    {
        row.AllowableRace = item.AllowedRaces;
        row.Description = item.Description;
        row.Name4 = item.Name[3];
        row.Name3 = item.Name[2];
        row.Name2 = item.Name[1];
        row.Name1 = item.Name[0];
        row.DurationInInventory = item.Duration;
        row.BagFamily = item.BagFamily;
        row.StartQuestID = (int)item.StartQuestId;
        row.RangeMod = item.RangedMod;
        row.Stackable = item.MaxStackSize;
        row.MaxCount = item.MaxCount;
        row.MinReputation = (int)item.RequiredRepValue;
        row.RequiredAbility = item.RequiredSpell;
        row.SellPrice = item.SellPrice;
        row.BuyPrice = item.BuyPrice;
        row.Flags[0] = item.Flags;
        row.Flags[1] = item.FlagsExtra;
        row.MaxDurability = item.MaxDurability;
        row.RequiredHoliday = (ushort)item.HolidayID;
        row.LimitCategory = (ushort)item.ItemLimitCategory;
        row.GemProperties = (ushort)item.GemProperties;
        row.SocketMatchEnchantmentId = (ushort)item.SocketBonus;
        row.TotemCategoryId = (ushort)item.TotemCategory;
        row.InstanceBound = (ushort)item.MapID;
        row.ZoneBound[0] = (ushort)item.AreaID;
        row.ItemSet = (ushort)item.ItemSet;
        row.LockId = (ushort)item.LockId;
        row.StartQuestId = (ushort)item.StartQuestId;
        row.PageText = (ushort)item.PageText;
        row.Delay = (ushort)item.Delay;
        row.RequiredReputationId = (ushort)item.RequiredRepFaction;
        row.RequiredSkillRank = (ushort)item.RequiredSkillLevel;
        row.RequiredSkill = (ushort)item.RequiredSkillId;
        row.ItemLevel = (ushort)item.ItemLevel;
        row.AllowableClass = (short)item.AllowedClasses;
        row.ItemRandomSuffixGroupId = (ushort)item.RandomSuffix;
        row.RandomProperty = (ushort)item.RandomProperty;
        row.MinDamage[0] = (ushort)item.DamageMins[0];
        row.MinDamage[1] = (ushort)item.DamageMins[1];
        row.MinDamage[2] = (ushort)item.DamageMins[2];
        row.MinDamage[3] = (ushort)item.DamageMins[3];
        row.MinDamage[4] = (ushort)item.DamageMins[4];
        row.MaxDamage[0] = (ushort)item.DamageMaxs[0];
        row.MaxDamage[1] = (ushort)item.DamageMaxs[1];
        row.MaxDamage[2] = (ushort)item.DamageMaxs[2];
        row.MaxDamage[3] = (ushort)item.DamageMaxs[3];
        row.MaxDamage[4] = (ushort)item.DamageMaxs[4];
        row.Resistances[0] = (short)item.Armor;
        row.Resistances[1] = (short)item.HolyResistance;
        row.Resistances[2] = (short)item.FireResistance;
        row.Resistances[3] = (short)item.NatureResistance;
        row.Resistances[4] = (short)item.FrostResistance;
        row.Resistances[5] = (short)item.ShadowResistance;
        row.Resistances[6] = (short)item.ArcaneResistance;
        row.ScalingStatDistributionId = (ushort)item.ScalingStatDistribution;
        row.SocketType[0] = ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[0]);
        row.SocketType[1] = ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[1]);
        row.SocketType[2] = ModernVersion.ConvertSocketColor((byte)item.ItemSocketColors[2]);
        row.SheatheType = (byte)item.SheathType;
        row.Material = (byte)item.Material;
        row.PageMaterial = (byte)item.PageMaterial;
        row.PageLanguage = (byte)item.Language;
        row.Bonding = (byte)item.Bonding;
        row.DamageType = (byte)item.DamageTypes[0];
        row.StatType[0] = (sbyte)item.StatTypes[0];
        row.StatType[1] = (sbyte)item.StatTypes[1];
        row.StatType[2] = (sbyte)item.StatTypes[2];
        row.StatType[3] = (sbyte)item.StatTypes[3];
        row.StatType[4] = (sbyte)item.StatTypes[4];
        row.StatType[5] = (sbyte)item.StatTypes[5];
        row.StatType[6] = (sbyte)item.StatTypes[6];
        row.StatType[7] = (sbyte)item.StatTypes[7];
        row.StatType[8] = (sbyte)item.StatTypes[8];
        row.StatType[9] = (sbyte)item.StatTypes[9];
        row.ContainerSlots = (byte)item.ContainerSlots;
        row.RequiredReputationRank = (byte)item.RequiredRepValue;
        row.RequiredCityRank = (byte)item.RequiredCityRank;
        row.RequiredHonorRank = (byte)item.RequiredHonorRank;
        row.InventoryType = (byte)item.InventoryType;
        row.OverallQualityId = (byte)item.Quality;
        row.AmmoType = (byte)item.AmmoType;
        for (var i = 0; i < item.StatsCount && i < 10; i++)
            row.StatModifierBonusAmount[i] = (short)Math.Clamp(item.StatValues[i], short.MinValue, short.MaxValue);
        row.RequiredLevel = (sbyte)item.RequiredLevel;
        if (ItemSparseRecordsStore.ContainsKey(item.Entry))
        {
            ItemSparseRecordsStore[item.Entry] = row;
        }
    }

    public static ItemEffect AddItemEffectRecord(ItemTemplate item, byte slot)
    {
        var record = new ItemEffect
        {
            Id = (int)GetFirstFreeId(ItemEffectStore),
            LegacySlotIndex = slot
        };
        UpdateItemEffectRecord(record, item);
        ItemEffectStore[(uint)record.Id] = record;
        Log.Print(LogType.Storage, $"ItemEffect #{record.Id} created for item #{item.Entry} slot #{slot}.",
            "GameData.cs");
        return record;
    }

    public static void UpdateItemEffectRecord(ItemEffect effect, ItemTemplate item)
    {
        var i = effect.LegacySlotIndex;
        effect.TriggerType = (sbyte)item.TriggeredSpellTypes[i];
        effect.Charges = (short)item.TriggeredSpellCharges[i];
        effect.CoolDownMSec = item.TriggeredSpellCooldowns[i];
        effect.CategoryCoolDownMSec = item.TriggeredSpellCategoryCooldowns[i];
        effect.SpellCategoryID = (ushort)item.TriggeredSpellCategories[i];
        effect.SpellID = item.TriggeredSpellIds[i];
        effect.ChrSpecializationID = 0;
        effect.ParentItemID = (int)item.Entry;
        if (ItemEffectStore.ContainsKey((uint)effect.Id))
        {
            ItemEffectStore[(uint)effect.Id] = effect;
        }
    }

    public static void RemoveItemEffectRecord(ItemEffect effect)
    {
        ItemEffectStore.Remove((uint)effect.Id);
        Log.Print(LogType.Storage,
            $"ItemEffect #{effect.Id} removed for item #{effect.ParentItemID} slot #{effect.LegacySlotIndex}.",
            "GameData.cs");
    }

    public static ItemAppearance AddItemAppearanceRecord(ItemTemplate item)
    {
        var record = new ItemAppearance
        {
            Id = (int)GetFirstFreeId(ItemAppearanceStore)
        };
        UpdateItemAppearanceRecord(record, item);
        ItemAppearanceStore[(uint)record.Id] = record;
        Log.Print(LogType.Storage, $"ItemAppearance #{record.Id} created for DisplayID #{item.DisplayID}.",
            "GameData.cs");
        return record;
    }

    public static void UpdateItemAppearanceRecord(ItemAppearance appearance, ItemTemplate item)
    {
        var fileDataId = (int)GetItemIconFileDataIdByDisplayId(item.DisplayID);
        appearance.DisplayType = 11;
        appearance.ItemDisplayInfoID = (int)item.DisplayID;
        appearance.DefaultIconFileDataID = fileDataId;
        appearance.UiOrder = 0;
        if (ItemAppearanceStore.ContainsKey((uint)appearance.Id))
        {
            ItemAppearanceStore[(uint)appearance.Id] = appearance;
        }
    }

    public static ItemModifiedAppearance? AddItemModifiedAppearanceRecord(ItemTemplate item)
    {
        var record = new ItemModifiedAppearance
        {
            Id = (int)GetFirstFreeId(ItemModifiedAppearanceStore)
        };
        UpdateItemModifiedAppearanceRecord(record, item);
        if (record.ItemID != item.Entry)
        {
            Log.Print(LogType.Error, $"ItemModifiedAppearance #{record.Id} create failed for item #{record.ItemID}.",
                "GameData.cs");
            return null;
        }

        ItemModifiedAppearanceStore[(uint)record.Id] = record;
        Log.Print(LogType.Storage, $"ItemModifiedAppearance #{record.Id} created for item #{record.ItemID}.",
            "GameData.cs");
        return record;
    }

    public static void UpdateItemModifiedAppearanceRecord(ItemModifiedAppearance modAppearance, ItemTemplate item)
    {
        var appearance = GetItemAppearanceByDisplayId(item.DisplayID);
        if (appearance == null)
        {
            Log.Print(LogType.Error,
                $"ItemModifiedAppearance #{modAppearance.Id} update failed: no ItemAppearance for DisplayID #{item.DisplayID}",
                "GameData.cs");
            return;
        }

        modAppearance.ItemID = (int)item.Entry;
        modAppearance.ItemAppearanceModifierID = 0;
        modAppearance.ItemAppearanceID = appearance.Id;
        modAppearance.OrderIndex = 0;
        modAppearance.TransmogSourceTypeEnum = 0;
        if (ItemModifiedAppearanceStore.ContainsKey((uint)modAppearance.Id))
        {
            ItemModifiedAppearanceStore[(uint)modAppearance.Id] = modAppearance;
        }
    }

    public static bool ItemCanHaveModel(ItemTemplate item)
    {
        switch (item.Class)
        {
            case 2:
            case 4 when item.SubClass != 7 &&
                        item.SubClass != 8 &&
                        item.SubClass != 9 &&
                        item.InventoryType != 0 &&
                        item.InventoryType != 2 &&
                        item.InventoryType != 11 &&
                        item.InventoryType != 12 &&
                        item.InventoryType != 18 &&
                        item.InventoryType != 28:
                return true;
            default:
                return item is { Class: 11, SubClass: 2 };
        }
    }

    private static void LoadCreatureDisplayInfoHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"CreatureDisplayInfo{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new[] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var modelId = ushort.Parse(fields[1]);
            var soundId = ushort.Parse(fields[2]);
            var sizeClass = sbyte.Parse(fields[3]);
            var creatureModelScale = float.Parse(fields[4]);
            var creatureModelAlpha = byte.Parse(fields[5]);
            var bloodId = byte.Parse(fields[6]);
            var extendedDisplayInfoId = int.Parse(fields[7]);
            var nPCSoundId = ushort.Parse(fields[8]);
            var particleColorId = ushort.Parse(fields[9]);
            var portraitCreatureDisplayInfoId = int.Parse(fields[10]);
            var portraitTextureFileDataId = int.Parse(fields[11]);
            var objectEffectPackageId = ushort.Parse(fields[12]);
            var animReplacementSetId = ushort.Parse(fields[13]);
            var flags = byte.Parse(fields[14]);
            var stateSpellVisualKitId = int.Parse(fields[15]);
            var playerOverrideScale = float.Parse(fields[16]);
            var petInstanceScale = float.Parse(fields[17]);
            var unarmedWeaponType = sbyte.Parse(fields[18]);
            var mountPoofSpellVisualKitId = int.Parse(fields[19]);
            var dissolveEffectId = int.Parse(fields[20]);
            var gender = sbyte.Parse(fields[21]);
            var dissolveOutEffectId = int.Parse(fields[22]);
            var creatureModelMinLod = sbyte.Parse(fields[23]);
            var textureVariationFileDataId1 = int.Parse(fields[24]);
            var textureVariationFileDataId2 = int.Parse(fields[25]);
            var textureVariationFileDataId3 = int.Parse(fields[26]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.CreatureDisplayInfo,
                HotfixId = 270000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt32(id);
            record.HotfixContent.WriteUInt16(modelId);
            record.HotfixContent.WriteUInt16(soundId);
            record.HotfixContent.WriteInt8(sizeClass);
            record.HotfixContent.WriteFloat(creatureModelScale);
            record.HotfixContent.WriteUInt8(creatureModelAlpha);
            record.HotfixContent.WriteUInt8(bloodId);
            record.HotfixContent.WriteInt32(extendedDisplayInfoId);
            record.HotfixContent.WriteUInt16(nPCSoundId);
            record.HotfixContent.WriteUInt16(particleColorId);
            record.HotfixContent.WriteInt32(portraitCreatureDisplayInfoId);
            record.HotfixContent.WriteInt32(portraitTextureFileDataId);
            record.HotfixContent.WriteUInt16(objectEffectPackageId);
            record.HotfixContent.WriteUInt16(animReplacementSetId);
            record.HotfixContent.WriteUInt8(flags);
            record.HotfixContent.WriteInt32(stateSpellVisualKitId);
            record.HotfixContent.WriteFloat(playerOverrideScale);
            record.HotfixContent.WriteFloat(petInstanceScale);
            record.HotfixContent.WriteInt8(unarmedWeaponType);
            record.HotfixContent.WriteInt32(mountPoofSpellVisualKitId);
            record.HotfixContent.WriteInt32(dissolveEffectId);
            record.HotfixContent.WriteInt8(gender);
            record.HotfixContent.WriteInt32(dissolveOutEffectId);
            record.HotfixContent.WriteInt8(creatureModelMinLod);
            record.HotfixContent.WriteInt32(textureVariationFileDataId1);
            record.HotfixContent.WriteInt32(textureVariationFileDataId2);
            record.HotfixContent.WriteInt32(textureVariationFileDataId3);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadCreatureDisplayInfoExtraHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"CreatureDisplayInfoExtra{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var displayRaceId = sbyte.Parse(fields[1]);
            var displaySexId = sbyte.Parse(fields[2]);
            var displayClassId = sbyte.Parse(fields[3]);
            var skinId = sbyte.Parse(fields[4]);
            var faceId = sbyte.Parse(fields[5]);
            var hairStyleId = sbyte.Parse(fields[6]);
            var hairColorId = sbyte.Parse(fields[7]);
            var facialHairId = sbyte.Parse(fields[8]);
            var flags = sbyte.Parse(fields[9]);
            var bakeMaterialResourcesId = int.Parse(fields[10]);
            var hDBakeMaterialResourcesId = int.Parse(fields[11]);
            var customDisplayOption1 = byte.Parse(fields[12]);
            var customDisplayOption2 = byte.Parse(fields[13]);
            var customDisplayOption3 = byte.Parse(fields[14]);
            var record = new HotfixRecord
            {
                TableHash = DB2Hash.CreatureDisplayInfoExtra,
                HotfixId = 280000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.Status = HotfixStatus.Valid;
            record.HotfixContent.WriteUInt32(id);
            record.HotfixContent.WriteInt8(displayRaceId);
            record.HotfixContent.WriteInt8(displaySexId);
            record.HotfixContent.WriteInt8(displayClassId);
            record.HotfixContent.WriteInt8(skinId);
            record.HotfixContent.WriteInt8(faceId);
            record.HotfixContent.WriteInt8(hairStyleId);
            record.HotfixContent.WriteInt8(hairColorId);
            record.HotfixContent.WriteInt8(facialHairId);
            record.HotfixContent.WriteInt8(flags);
            record.HotfixContent.WriteInt32(bakeMaterialResourcesId);
            record.HotfixContent.WriteInt32(hDBakeMaterialResourcesId);
            record.HotfixContent.WriteUInt8(customDisplayOption1);
            record.HotfixContent.WriteUInt8(customDisplayOption2);
            record.HotfixContent.WriteUInt8(customDisplayOption3);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadCreatureDisplayInfoOptionHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"CreatureDisplayInfoOption{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var chrCustomizationOptionId = int.Parse(fields[1]);
            var chrCustomizationChoiceId = int.Parse(fields[2]);
            var creatureDisplayInfoExtraId = int.Parse(fields[3]);
            var record = new HotfixRecord
            {
                Status = HotfixStatus.Valid,
                TableHash = DB2Hash.CreatureDisplayInfoOption,
                HotfixId = 290000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.HotfixContent.WriteInt32(chrCustomizationOptionId);
            record.HotfixContent.WriteInt32(chrCustomizationChoiceId);
            record.HotfixContent.WriteInt32(creatureDisplayInfoExtraId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadItemEffectHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"ItemEffect{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var legacySlotIndex = byte.Parse(fields[1]);
            var triggerType = byte.Parse(fields[2]);
            var charges = short.Parse(fields[3]);
            var coolDownMSec = int.Parse(fields[4]);
            var categoryCoolDownMSec = int.Parse(fields[5]);
            var spellCategoryId = short.Parse(fields[6]);
            var spellId = int.Parse(fields[7]);
            var chrSpecializationId = short.Parse(fields[8]);
            var parentItemId = int.Parse(fields[9]);
            var record = new HotfixRecord
            {
                Status = HotfixStatus.Valid,
                TableHash = DB2Hash.ItemEffect,
                HotfixId = 250000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.HotfixContent.WriteUInt8(legacySlotIndex);
            record.HotfixContent.WriteUInt8(triggerType);
            record.HotfixContent.WriteInt16(charges);
            record.HotfixContent.WriteInt32(coolDownMSec);
            record.HotfixContent.WriteInt32(categoryCoolDownMSec);
            record.HotfixContent.WriteInt16(spellCategoryId);
            record.HotfixContent.WriteInt32(spellId);
            record.HotfixContent.WriteInt16(chrSpecializationId);
            record.HotfixContent.WriteInt32(parentItemId);
            Hotfixes[record.HotfixId] = record;
        }
    }

    public static void LoadItemDisplayInfoHotfixes()
    {
        var path = Path.Combine("CSV", "Hotfix", $"ItemDisplayInfo{ModernVersion.ExpansionVersion}.csv");
        using var csvParser = new TextFieldParser(path);
        csvParser.CommentTokens = new string[1] { "#" };
        csvParser.SetDelimiters(",");
        csvParser.HasFieldsEnclosedInQuotes = false;
        csvParser.ReadLine();
        var counter = 0u;
        while (!csvParser.EndOfData)
        {
            counter++;
            var fields = csvParser.ReadFields();
            var id = uint.Parse(fields[0]);
            var itemVisual = int.Parse(fields[1]);
            var particleColorID = int.Parse(fields[2]);
            var itemRangedDisplayInfoID = uint.Parse(fields[3]);
            var overrideSwooshSoundKitID = uint.Parse(fields[4]);
            var sheatheTransformMatrixID = int.Parse(fields[5]);
            var stateSpellVisualKitID = int.Parse(fields[6]);
            var sheathedSpellVisualKitID = int.Parse(fields[7]);
            var unsheathedSpellVisualKitID = uint.Parse(fields[8]);
            var flags = int.Parse(fields[9]);
            var modelResourcesID1 = uint.Parse(fields[10]);
            var modelResourcesID2 = uint.Parse(fields[11]);
            var modelMaterialResourcesID1 = int.Parse(fields[12]);
            var modelMaterialResourcesID2 = int.Parse(fields[13]);
            var modelType1 = int.Parse(fields[14]);
            var modelType2 = int.Parse(fields[15]);
            var geosetGroup1 = int.Parse(fields[16]);
            var geosetGroup2 = int.Parse(fields[17]);
            var geosetGroup3 = int.Parse(fields[18]);
            var geosetGroup4 = int.Parse(fields[19]);
            var geosetGroup5 = int.Parse(fields[20]);
            var geosetGroup6 = int.Parse(fields[21]);
            var attachmentGeosetGroup1 = int.Parse(fields[22]);
            var attachmentGeosetGroup2 = int.Parse(fields[23]);
            var attachmentGeosetGroup3 = int.Parse(fields[24]);
            var attachmentGeosetGroup4 = int.Parse(fields[25]);
            var attachmentGeosetGroup5 = int.Parse(fields[26]);
            var attachmentGeosetGroup6 = int.Parse(fields[27]);
            var helmetGeosetVis1 = int.Parse(fields[28]);
            var helmetGeosetVis2 = int.Parse(fields[29]);
            var record = new HotfixRecord
            {
                Status = HotfixStatus.Valid,
                TableHash = DB2Hash.ItemDisplayInfo,
                HotfixId = 260000 + counter
            };
            record.UniqueId = record.HotfixId;
            record.RecordId = id;
            record.HotfixContent.WriteInt32(itemVisual);
            record.HotfixContent.WriteInt32(particleColorID);
            record.HotfixContent.WriteUInt32(itemRangedDisplayInfoID);
            record.HotfixContent.WriteUInt32(overrideSwooshSoundKitID);
            record.HotfixContent.WriteInt32(sheatheTransformMatrixID);
            record.HotfixContent.WriteInt32(stateSpellVisualKitID);
            record.HotfixContent.WriteInt32(sheathedSpellVisualKitID);
            record.HotfixContent.WriteUInt32(unsheathedSpellVisualKitID);
            record.HotfixContent.WriteInt32(flags);
            record.HotfixContent.WriteUInt32(modelResourcesID1);
            record.HotfixContent.WriteUInt32(modelResourcesID2);
            record.HotfixContent.WriteInt32(modelMaterialResourcesID1);
            record.HotfixContent.WriteInt32(modelMaterialResourcesID2);
            record.HotfixContent.WriteInt32(modelType1);
            record.HotfixContent.WriteInt32(modelType2);
            record.HotfixContent.WriteInt32(geosetGroup1);
            record.HotfixContent.WriteInt32(geosetGroup2);
            record.HotfixContent.WriteInt32(geosetGroup3);
            record.HotfixContent.WriteInt32(geosetGroup4);
            record.HotfixContent.WriteInt32(geosetGroup5);
            record.HotfixContent.WriteInt32(geosetGroup6);
            record.HotfixContent.WriteInt32(attachmentGeosetGroup1);
            record.HotfixContent.WriteInt32(attachmentGeosetGroup2);
            record.HotfixContent.WriteInt32(attachmentGeosetGroup3);
            record.HotfixContent.WriteInt32(attachmentGeosetGroup4);
            record.HotfixContent.WriteInt32(attachmentGeosetGroup5);
            record.HotfixContent.WriteInt32(attachmentGeosetGroup6);
            record.HotfixContent.WriteInt32(helmetGeosetVis1);
            record.HotfixContent.WriteInt32(helmetGeosetVis2);
            Hotfixes[record.HotfixId] = record;
        }
    }
}