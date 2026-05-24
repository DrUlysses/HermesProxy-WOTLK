using HermesProxy.World.Enums;

namespace HermesProxy.World;

public abstract class WowGuid
{
	public ulong Low { get; protected set; }

	public HighGuid HighGuid { get; protected set; }

	public ulong High { get; protected set; }

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
		switch (GetHighType())
		{
		case HighGuidType.Player:
			return ObjectType.Player;
		case HighGuidType.DynamicObject:
			return ObjectType.DynamicObject;
		case HighGuidType.Corpse:
			return ObjectType.Corpse;
		case HighGuidType.Item:
			return ObjectType.Item;
		case HighGuidType.Transport:
		case HighGuidType.MOTransport:
		case HighGuidType.GameObject:
			return ObjectType.GameObject;
		case HighGuidType.Creature:
		case HighGuidType.Vehicle:
		case HighGuidType.Pet:
			return ObjectType.Unit;
		case HighGuidType.AreaTrigger:
			return ObjectType.AreaTrigger;
		default:
			return ObjectType.Object;
		}
	}

	public bool IsWorldObject()
	{
		switch (GetHighType())
		{
		case HighGuidType.Player:
		case HighGuidType.Transport:
		case HighGuidType.MOTransport:
		case HighGuidType.Creature:
		case HighGuidType.Vehicle:
		case HighGuidType.Pet:
		case HighGuidType.GameObject:
		case HighGuidType.DynamicObject:
		case HighGuidType.Corpse:
			return true;
		default:
			return false;
		}
	}

	public bool IsTransport()
	{
		var highType = GetHighType();
		var highGuidType = highType;
		if ((uint)(highGuidType - 6) <= 1u)
		{
			return true;
		}
		return false;
	}

	public bool IsPlayer()
	{
		var objectType = GetObjectType();
		var objectType2 = objectType;
		if ((uint)(objectType2 - 6) <= 1u)
		{
			return true;
		}
		return false;
	}

	public bool IsCreature()
	{
		return GetObjectType() == ObjectType.Unit;
	}

	public bool IsItem()
	{
		var objectType = GetObjectType();
		var objectType2 = objectType;
		if ((uint)(objectType2 - 1) <= 1u)
		{
			return true;
		}
		return false;
	}

	public static WowGuid64 ConvertUniqGuid(WowGuid128 guid)
	{
		var uniqGuid = (UniqGuid)guid.GetLowValue();
		var uniqGuid2 = uniqGuid;
		if (uniqGuid2 == UniqGuid.SpellTargetTradeItem)
		{
			return new WowGuid64(6uL);
		}
		return WowGuid64.Empty;
	}

	public static bool operator ==(WowGuid? first, WowGuid other)
	{
		if ((object)first == other)
		{
			return true;
		}
		if ((object)first == null || (object)other == null)
		{
			return false;
		}
		return first.Equals(other);
	}

	public static bool operator !=(WowGuid first, WowGuid other)
	{
		return !(first == other);
	}

	public override bool Equals(object obj)
	{
		return obj is WowGuid && Equals((WowGuid)obj);
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
