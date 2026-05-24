using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InspectHonorStatsResultClassic : ServerPacket
{
	public WowGuid128 PlayerGUID;

	public byte LifetimeHighestRank;

	public ushort TodayHonorableKills;

	public ushort TodayDishonorableKills;

	public ushort YesterdayHonorableKills;

	public ushort YesterdayDishonorableKills;

	public ushort LastWeekHonorableKills;

	public ushort LastWeekDishonorableKills;

	public ushort ThisWeekHonorableKills;

	public ushort ThisWeekDishonorableKills;

	public uint LifetimeHonorableKills;

	public uint LifetimeDishonorableKills;

	public uint YesterdayHonor;

	public uint LastWeekHonor;

	public uint ThisWeekHonor;

	public uint Standing;

	public byte RankProgress;

	public InspectHonorStatsResultClassic()
		: base(Opcode.SMSG_INSPECT_HONOR_STATS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PlayerGUID);
		base._worldPacket.WriteUInt8(this.LifetimeHighestRank);
		base._worldPacket.WriteUInt16(this.TodayHonorableKills);
		base._worldPacket.WriteUInt16(this.TodayDishonorableKills);
		base._worldPacket.WriteUInt16(this.YesterdayHonorableKills);
		base._worldPacket.WriteUInt16(this.YesterdayDishonorableKills);
		base._worldPacket.WriteUInt16(this.LastWeekHonorableKills);
		base._worldPacket.WriteUInt16(this.LastWeekDishonorableKills);
		base._worldPacket.WriteUInt16(this.ThisWeekHonorableKills);
		base._worldPacket.WriteUInt16(this.ThisWeekDishonorableKills);
		base._worldPacket.WriteUInt32(this.LifetimeHonorableKills);
		base._worldPacket.WriteUInt32(this.LifetimeDishonorableKills);
		base._worldPacket.WriteUInt32(this.YesterdayHonor);
		base._worldPacket.WriteUInt32(this.LastWeekHonor);
		base._worldPacket.WriteUInt32(this.ThisWeekHonor);
		base._worldPacket.WriteUInt32(this.Standing);
		base._worldPacket.WriteUInt8(this.RankProgress);
	}
}
