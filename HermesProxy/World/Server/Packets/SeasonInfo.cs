using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SeasonInfo : ServerPacket
{
	public int MythicPlusSeasonID;

	public int MythicPlusMilestoneSeasonID;

	public int PreviousSeason;

	public int CurrentSeason;

	public int PvpSeasonID;

	public int ConquestWeeklyProgressCurrencyID;

	public bool WeeklyRewardChestsEnabled;

	public SeasonInfo()
		: base(Opcode.SMSG_SEASON_INFO)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.MythicPlusSeasonID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteInt32(this.MythicPlusMilestoneSeasonID);
		}
		base._worldPacket.WriteInt32(this.CurrentSeason);
		base._worldPacket.WriteInt32(this.PreviousSeason);
		base._worldPacket.WriteInt32(this.ConquestWeeklyProgressCurrencyID);
		base._worldPacket.WriteInt32(this.PvpSeasonID);
		base._worldPacket.WriteBit(this.WeeklyRewardChestsEnabled);
		base._worldPacket.FlushBits();
	}
}
