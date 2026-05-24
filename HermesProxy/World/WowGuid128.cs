using Framework.Logging;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public class WowGuid128 : WowGuid
{
    private const ulong UNKNOWN_TMP_GUID_START = 10000000000uL;

    private static ulong _nextUnknownTmpGuid = 10000000000uL;

    public static readonly WowGuid128 Empty = new();

    public WowGuid128()
    {
        Low = 0uL;
        High = 0uL;
        HighGuid = new HighGuid703((byte)((High >> 58) & 0x3F));
    }

    public WowGuid128(ulong high, ulong low)
    {
        Low = low;
        High = high;
        HighGuid = new HighGuid703((byte)((High >> 58) & 0x3F));
    }

    public static WowGuid128 Create(WowGuid64 guid, GameSessionData gamestate)
    {
        return guid.GetHighType() switch
        {
            HighGuidType.Player => Create(HighGuidType703.Player, guid.GetCounter()),
            HighGuidType.Item => Create(HighGuidType703.Item, guid.GetCounter()),
            HighGuidType.Transport or HighGuidType.MOTransport => TransportCreate(guid.GetCounter(), guid.GetEntry()),
            HighGuidType.RaidGroup => Create(HighGuidType703.RaidGroup, guid.GetCounter()),
            HighGuidType.GameObject => Create(HighGuidType703.GameObject, gamestate.GetObjectSpawnCounter(guid),
                guid.GetEntry(), guid.GetCounter()),
            HighGuidType.Creature => Create(HighGuidType703.Creature, gamestate.GetObjectSpawnCounter(guid),
                guid.GetEntry(), guid.GetCounter()),
            HighGuidType.Pet => Create(HighGuidType703.Pet, 0u, guid.GetEntry(), guid.GetCounter()),
            HighGuidType.Vehicle => Create(HighGuidType703.Vehicle, 0u, guid.GetEntry(), guid.GetCounter()),
            HighGuidType.DynamicObject => Create(HighGuidType703.DynamicObject, 0u, guid.GetEntry(), guid.GetCounter()),
            HighGuidType.Corpse => Create(HighGuidType703.Corpse, 0u, guid.GetEntry(), guid.GetCounter()),
            _ => Empty
        };
    }

    public static WowGuid128 Create(HighGuidType703 type, ulong counter)
    {
        switch (type)
        {
            case HighGuidType703.Uniq:
            case HighGuidType703.Party:
            case HighGuidType703.WowAccount:
            case HighGuidType703.BNetAccount:
            case HighGuidType703.GMTask:
            case HighGuidType703.RaidGroup:
            case HighGuidType703.Spell:
            case HighGuidType703.Mail:
            case HighGuidType703.UserRouter:
            case HighGuidType703.PVPQueueGroup:
            case HighGuidType703.UserClient:
            case HighGuidType703.UniqUserClient:
            case HighGuidType703.BattlePet:
            case HighGuidType703.CommerceObj:
            case HighGuidType703.ClientSession:
            case HighGuidType703.ArenaTeam:
                return GlobalCreate(type, counter);
            case HighGuidType703.Player:
            case HighGuidType703.Item:
            case HighGuidType703.Transport:
            case HighGuidType703.Guild:
                return RealmSpecificCreate(type, counter);
            case HighGuidType703.Null:
            case HighGuidType703.WorldTransaction:
            case HighGuidType703.StaticDoor:
            case HighGuidType703.Conversation:
            case HighGuidType703.Creature:
            case HighGuidType703.Vehicle:
            case HighGuidType703.Pet:
            case HighGuidType703.GameObject:
            case HighGuidType703.DynamicObject:
            case HighGuidType703.AreaTrigger:
            case HighGuidType703.Corpse:
            case HighGuidType703.LootObject:
            case HighGuidType703.SceneObject:
            case HighGuidType703.Scenario:
            case HighGuidType703.AIGroup:
            case HighGuidType703.DynamicDoor:
            case HighGuidType703.ClientActor:
            case HighGuidType703.Vignette:
            case HighGuidType703.CallForHelp:
            case HighGuidType703.AIResource:
            case HighGuidType703.AILock:
            case HighGuidType703.AILockTicket:
            case HighGuidType703.ChatChannel:
            case HighGuidType703.MobileSession:
            case HighGuidType703.WebObj:
            case HighGuidType703.LFGObject:
            case HighGuidType703.LFGList:
            case HighGuidType703.PetBattle:
            case HighGuidType703.Cast:
            case HighGuidType703.ClientConnection:
            case HighGuidType703.ClubFinder:
            case HighGuidType703.ToolsClient:
            case HighGuidType703.WorldLayer:
            case HighGuidType703.Invalid:
            default:
                Log.Print(LogType.Error,
                    $"This guid type cannot be constructed using Create(HighGuid: {type} ulong counter).",
                    "WowGuid.cs");
                return Empty;
        }
    }

    public static WowGuid128 Create(HighGuidType703 type, uint mapId, uint entry, ulong counter)
    {
        return MapSpecificCreate(type, 0, (ushort)mapId, 0u, entry, counter);
    }

    public static WowGuid128 Create(HighGuidType703 type, SpellCastSource subType, uint mapId, uint entry,
        ulong counter)
    {
        return MapSpecificCreate(type, (byte)subType, (ushort)mapId, 0u, entry, counter);
    }

    public static WowGuid128 CreateLootGuid(HighGuidTypeLegacy type, uint entry, ulong counter)
    {
        return MapSpecificCreate(HighGuidType703.LootObject, 0, 0, (uint)type, entry, counter);
    }

    public static WowGuid128 CreateUnknownPlayerGuid()
    {
        return Create(HighGuidType703.Player, _nextUnknownTmpGuid++);
    }

    public static bool IsUnknownPlayerGuid(WowGuid128 playerGuid)
    {
        return playerGuid.IsPlayer() && playerGuid.GetCounter() >= 10000000000L;
    }

    private static WowGuid128 GlobalCreate(HighGuidType703 type, ulong counter)
    {
        return new WowGuid128((ulong)((long)type << 58), counter);
    }

    private static WowGuid128 TransportCreate(ulong counter, uint entry)
    {
        return new WowGuid128(0x1800000000000000L | (counter << 38) | entry, 0uL);
    }

    private static WowGuid128 RealmSpecificCreate(HighGuidType703 type, ulong counter)
    {
        return type == HighGuidType703.Transport
            ? new WowGuid128((ulong)((long)type << 58) | (counter << 38), 0uL)
            : new WowGuid128((ulong)(((long)type << 58) | 0x40000000000L), counter);
    }

    private static WowGuid128 MapSpecificCreate(HighGuidType703 type, byte subType, ushort mapId, uint serverId,
        uint entry, ulong counter)
    {
        return new WowGuid128(
            (ulong)(((long)type << 58) | 0x40000000000L | ((long)(mapId & 0x1FFF) << 29)) |
            ((ulong)(entry & 0x7FFFFF) << 6) | (subType & 0x3FuL),
            ((ulong)(serverId & 0xFFFFFF) << 40) | (counter & 0xFFFFFFFFFFL));
    }

    public override bool HasEntry()
    {
        var highType = GetHighType();
        return (uint)(highType - 9) <= 3u || highType == HighGuidType.AreaTrigger;
    }

    private byte GetSubType()
    {
        return (byte)(High & 0x3F);
    }

    private ushort GetRealmId()
    {
        return (ushort)((High >> 42) & 0x1FFF);
    }

    public uint GetServerId()
    {
        return (uint)((Low >> 40) & 0xFFFFFF);
    }

    public ushort GetMapId()
    {
        return (ushort)((High >> 29) & 0x1FFF);
    }

    public override uint GetEntry()
    {
        if (GetHighType() == HighGuidType.Transport)
        {
            return (uint)(High & 0xFFFFFFFFu);
        }

        return (uint)((High >> 6) & 0x7FFFFF);
    }

    public override ulong GetCounter()
    {
        if (GetHighType() == HighGuidType.Transport)
        {
            return (High >> 38) & 0xFFFFF;
        }

        return Low & 0xFFFFFFFFFFL;
    }

    public override string ToString()
    {
        if (Low == 0L && High == 0)
        {
            return "Full: 0x0";
        }

        return !HasEntry()
            ? $"Full: 0x{High:X16}{Low:X16} {GetHighType()}/{GetSubType()} R{GetRealmId()}/S{GetServerId()} Map: {GetMapId()} Low: {GetCounter()}"
            : $"Full: 0x{High:X16}{Low:X16} {GetHighType()}/{GetSubType()} R{GetRealmId()}/S{GetServerId()} Map: {GetMapId()} Entry: {GetEntry()} Low: {GetCounter()}";
    }

    public override WowGuid64 To64()
    {
        return WowGuid64.Create(this);
    }

    public override WowGuid128 To128(GameSessionData gameState)
    {
        return this;
    }
}