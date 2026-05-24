using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CharacterCreateInfo
{
	public Race RaceId = Race.None;

	public Class ClassId = Class.None;

	public Gender Sex = Gender.None;

	public readonly Array<ChrCustomizationChoice> Customizations = new(50);

	public uint? TemplateSet;

	public bool IsTrialBoost;

	public bool UseNPE;

	public string Name;

	public byte CharCount = 0;
}
