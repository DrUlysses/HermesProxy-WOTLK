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
		_worldPacket.WriteInt32(MythicPlusSeasonID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteInt32(MythicPlusMilestoneSeasonID);
		}
		_worldPacket.WriteInt32(CurrentSeason);
		_worldPacket.WriteInt32(PreviousSeason);
		_worldPacket.WriteInt32(ConquestWeeklyProgressCurrencyID);
		_worldPacket.WriteInt32(PvpSeasonID);
		_worldPacket.WriteBit(WeeklyRewardChestsEnabled);
		_worldPacket.FlushBits();
	}
}
