using HermesProxy.World.Enums;

namespace HermesProxy.World;

public abstract class WowGuid
{
    public ulong Low { get; protected set; } = 0uL;

    protected HighGuid HighGuid { get; init; } = new HighGuidLegacy(HighGuidTypeLegacy.None);

    protected ulong High { get; set; } = 0uL;

    public abstract bool HasEntry();

    public abstract ulong GetCounter();

    public ulong GetLowValue()
    {
        return Low;
    }

    public abstract uint GetEntry();

    public HighGuidType GetHighType()
    {
        return HighGuid.GetHighGuidType();
    }

    public ulong GetHighValue()
    {
        return High;
    }

    public ObjectType GetObjectType()
    {
        return GetHighType() switch
        {
            HighGuidType.Player => ObjectType.Player,
            HighGuidType.DynamicObject => ObjectType.DynamicObject,
            HighGuidType.Corpse => ObjectType.Corpse,
            HighGuidType.Item => ObjectType.Item,
            HighGuidType.Transport or
                HighGuidType.MOTransport or
                HighGuidType.GameObject => ObjectType.GameObject,
            HighGuidType.Creature or
                HighGuidType.Vehicle or
                HighGuidType.Pet => ObjectType.Unit,
            HighGuidType.AreaTrigger => ObjectType.AreaTrigger,
            _ => ObjectType.Object
        };
    }

    public bool IsWorldObject()
    {
        return GetHighType() switch
        {
            HighGuidType.Player or
                HighGuidType.Transport or
                HighGuidType.MOTransport or
                HighGuidType.Creature or
                HighGuidType.Vehicle or
                HighGuidType.Pet or
                HighGuidType.GameObject or
                HighGuidType.DynamicObject or
                HighGuidType.Corpse => true,
            _ => false
        };
    }

    public bool IsTransport()
    {
        var highType = GetHighType();
        var highGuidType = highType;
        return (uint)(highGuidType - 6) <= 1u;
    }

    public bool IsPlayer()
    {
        var objectType = GetObjectType();
        var objectType2 = objectType;
        return (uint)(objectType2 - 6) <= 1u;
    }

    public bool IsCreature()
    {
        return GetObjectType() == ObjectType.Unit;
    }

    public bool IsItem()
    {
        var objectType = GetObjectType();
        var objectType2 = objectType;
        return (uint)(objectType2 - 1) <= 1u;
    }

    public static WowGuid64 ConvertUniqGuid(WowGuid128 guid)
    {
        var uniqGuid = (UniqGuid)guid.GetLowValue();
        var uniqGuid2 = uniqGuid;
        return uniqGuid2 == UniqGuid.SpellTargetTradeItem ? new WowGuid64(6uL) : WowGuid64.Empty;
    }

    public static bool operator ==(WowGuid? first, WowGuid? other)
    {
        if ((object?)first == other)
        {
            return true;
        }

        if ((object?)first == null || (object?)other == null)
        {
            return false;
        }

        return first.Equals(other);
    }

    public static bool operator !=(WowGuid? first, WowGuid? other)
    {
        return !(first == other);
    }

    public override bool Equals(object? obj)
    {
        return obj is WowGuid guid && Equals(guid);
    }

    public bool Equals(WowGuid other)
    {
        return other.Low == Low && other.High == High;
    }

    public override int GetHashCode()
    {
        return new { Low, High }.GetHashCode();
    }

    public bool IsEmpty()
    {
        return High == 0L && Low == 0;
    }

    public abstract WowGuid64 To64();

    public abstract WowGuid128 To128(GameSessionData gameState);
}