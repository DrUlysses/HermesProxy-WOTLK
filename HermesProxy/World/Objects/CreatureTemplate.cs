using System.Collections.Generic;
using Framework.Collections;

namespace HermesProxy.World.Objects;

public class CreatureTemplate
{
	public string Title;

	public string TitleAlt;

	public string CursorName;

	public int Type;

	public int Family;

	public int Classification;

	public uint PetSpellDataId;

	public readonly CreatureDisplayStats Display = new();

	public float HpMulti;

	public float EnergyMulti;

	public bool Civilian;

	public bool Leader;

	public readonly List<uint> QuestItems = new();

	public uint MovementInfoID;

	public int HealthScalingExpansion;

	public uint RequiredExpansion;

	public uint VignetteID;

	public int Class;

	public int DifficultyID;

	public int WidgetSetID;

	public int WidgetSetUnitConditionID;

	public readonly uint[] Flags = new uint[2];

	public readonly uint[] ProxyCreatureID = new uint[2];

	public readonly StringArray Name = new(4);

	public readonly StringArray NameAlt = new(4);
}
