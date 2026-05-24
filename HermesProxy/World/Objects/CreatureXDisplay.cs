namespace HermesProxy.World.Objects;

public class CreatureXDisplay
{
	public readonly uint CreatureDisplayID;

	public readonly float Scale = 1f;

	public readonly float Probability = 1f;

	public CreatureXDisplay(uint creatureDisplayID, float displayScale, float probability)
	{
		CreatureDisplayID = creatureDisplayID;
		Scale = displayScale;
		Probability = probability;
	}
}
