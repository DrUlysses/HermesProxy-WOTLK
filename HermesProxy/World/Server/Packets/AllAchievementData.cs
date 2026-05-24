using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public struct EarnedAchievement
{
	public uint Id;
	public long Date;
	public WowGuid128 Owner;
	public uint VirtualRealmAddress;
	public uint NativeRealmAddress;

	public void Write(WorldPacket data)
	{
		data.WriteUInt32(Id);
		data.WritePackedTime(Date);
		data.WritePackedGuid128(Owner);
		data.WriteUInt32(VirtualRealmAddress);
		data.WriteUInt32(NativeRealmAddress);
	}
}

public class AllAchievementData : ServerPacket
{
	public List<EarnedAchievement> Earned = new List<EarnedAchievement>();
	public List<CriteriaProgressPkt> Progress = new List<CriteriaProgressPkt>();

	public AllAchievementData()
		: base(Opcode.SMSG_ALL_ACHIEVEMENT_DATA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Earned.Count);
		_worldPacket.WriteInt32(Progress.Count);
		foreach (EarnedAchievement earned in Earned)
		{
			earned.Write(_worldPacket);
		}
		foreach (CriteriaProgressPkt progress in Progress)
		{
			progress.Write(_worldPacket);
		}
	}
}
