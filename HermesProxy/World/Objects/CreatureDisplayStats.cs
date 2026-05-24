using System.Collections.Generic;

namespace HermesProxy.World.Objects;

public class CreatureDisplayStats
{
	public float TotalProbability;

	public readonly List<CreatureXDisplay> CreatureDisplay = new();
}
