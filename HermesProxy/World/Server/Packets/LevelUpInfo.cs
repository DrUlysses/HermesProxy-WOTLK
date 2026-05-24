using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LevelUpInfo : ServerPacket
{
	public int Level = 0;

	public int HealthDelta = 0;

	public int[] PowerDelta = new int[10];

	public int[] StatDelta = new int[5];

	public int NumNewTalents;

	public int NumNewPvpTalentSlots;

	public LevelUpInfo()
		: base(Opcode.SMSG_LEVEL_UP_INFO)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Level);
		base._worldPacket.WriteInt32(this.HealthDelta);
		int powerCount = ((ModernVersion.ExpansionVersion >= 3) ? 10 : ModernVersion.GetPowerCountForClientVersion());
		for (int i = 0; i < powerCount; i++)
		{
			base._worldPacket.WriteInt32((i < this.PowerDelta.Length) ? this.PowerDelta[i] : 0);
		}
		int[] statDelta = this.StatDelta;
		foreach (int stat in statDelta)
		{
			base._worldPacket.WriteInt32(stat);
		}
		base._worldPacket.WriteInt32(this.NumNewTalents);
		if (ModernVersion.ExpansionVersion < 3)
		{
			base._worldPacket.WriteInt32(this.NumNewPvpTalentSlots);
		}
	}
}
