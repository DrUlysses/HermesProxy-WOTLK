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
		data.WriteUInt32(this.Id);
		data.WritePackedTime(this.Date);
		data.WritePackedGuid128(this.Owner);
		data.WriteUInt32(this.VirtualRealmAddress);
		data.WriteUInt32(this.NativeRealmAddress);
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

	public override void Write()
	{
		base._worldPacket.WriteInt32(this.Earned.Count);
		base._worldPacket.WriteInt32(this.Progress.Count);
		foreach (EarnedAchievement earned in this.Earned)
		{
			earned.Write(base._worldPacket);
		}
		foreach (CriteriaProgressPkt progress in this.Progress)
		{
			progress.Write(base._worldPacket);
		}
	}
}
