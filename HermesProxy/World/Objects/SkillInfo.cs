namespace HermesProxy.World.Objects;

public class SkillInfo
{
	public readonly ushort?[] SkillLineID = new ushort?[256];

	public readonly ushort?[] SkillStep = new ushort?[256];

	public readonly ushort?[] SkillRank = new ushort?[256];

	public readonly ushort?[] SkillStartingRank = new ushort?[256];

	public readonly ushort?[] SkillMaxRank = new ushort?[256];

	public readonly short?[] SkillTempBonus = new short?[256];

	public readonly ushort?[] SkillPermBonus = new ushort?[256];
}
