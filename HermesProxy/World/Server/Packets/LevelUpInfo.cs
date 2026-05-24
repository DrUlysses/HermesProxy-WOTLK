using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LevelUpInfo : ServerPacket
{
	public int Level = 0;

	public int HealthDelta = 0;

	public readonly int[] PowerDelta = new int[10];

	public readonly int[] StatDelta = new int[5];

	public int NumNewTalents;

	public int NumNewPvpTalentSlots;

	public LevelUpInfo()
		: base(Opcode.SMSG_LEVEL_UP_INFO)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Level);
		_worldPacket.WriteInt32(HealthDelta);
		var powerCount = ModernVersion.ExpansionVersion >= 3 ? 10 : ModernVersion.GetPowerCountForClientVersion();
		for (var i = 0; i < powerCount; i++)
		{
			_worldPacket.WriteInt32(i < PowerDelta.Length ? PowerDelta[i] : 0);
		}
		var statDelta = StatDelta;
		foreach (var stat in statDelta)
		{
			_worldPacket.WriteInt32(stat);
		}
		_worldPacket.WriteInt32(NumNewTalents);
		if (ModernVersion.ExpansionVersion < 3)
		{
			_worldPacket.WriteInt32(NumNewPvpTalentSlots);
		}
	}
}
