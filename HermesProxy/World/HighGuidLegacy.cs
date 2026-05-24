using System.Collections.Generic;
using Framework.Logging;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public class HighGuidLegacy : HighGuid
{
	private HighGuidTypeLegacy high;

	private static readonly Dictionary<HighGuidTypeLegacy, HighGuidType> HighLegacyToHighType = new()
	{
		{
			HighGuidTypeLegacy.None,
			HighGuidType.Null
		},
		{
			HighGuidTypeLegacy.Player,
			HighGuidType.Player
		},
		{
			HighGuidTypeLegacy.Group,
			HighGuidType.RaidGroup
		},
		{
			HighGuidTypeLegacy.Group2,
			HighGuidType.RaidGroup
		},
		{
			HighGuidTypeLegacy.MOTransport,
			HighGuidType.MOTransport
		},
		{
			HighGuidTypeLegacy.Item,
			HighGuidType.Item
		},
		{
			HighGuidTypeLegacy.DynamicObject,
			HighGuidType.DynamicObject
		},
		{
			HighGuidTypeLegacy.GameObject,
			HighGuidType.GameObject
		},
		{
			HighGuidTypeLegacy.Transport,
			HighGuidType.Transport
		},
		{
			HighGuidTypeLegacy.Creature,
			HighGuidType.Creature
		},
		{
			HighGuidTypeLegacy.Pet,
			HighGuidType.Pet
		},
		{
			HighGuidTypeLegacy.Vehicle,
			HighGuidType.Vehicle
		},
		{
			HighGuidTypeLegacy.Corpse,
			HighGuidType.Corpse
		}
	};

	public HighGuidLegacy(HighGuidTypeLegacy high)
	{
		this.high = high;
		if (!HighLegacyToHighType.ContainsKey(high))
		{
			Log.Print(LogType.Warn, $"Unknown HighGuidTypeLegacy: 0x{high:X}, defaulting to Null", "HighGuidLegacy", "HighGuidLegacy.cs");
			highGuidType = HighGuidType.Null;
			return;
		}
		highGuidType = HighLegacyToHighType[high];
	}
}
