using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ArenaTeamRosterResponse : ServerPacket
{
	public uint TeamId;

	public uint TeamSize;

	public uint TeamPlayed;

	public uint TeamWins;

	public uint SeasonPlayed;

	public uint SeasonWins;

	public uint TeamRating;

	public uint PlayerRating;

	public bool UnkBit;

	public List<ArenaTeamMember> Members = new List<ArenaTeamMember>();

	public ArenaTeamRosterResponse()
		: base(Opcode.SMSG_ARENA_TEAM_ROSTER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.TeamId);
		base._worldPacket.WriteUInt32(this.TeamSize);
		base._worldPacket.WriteUInt32(this.TeamPlayed);
		base._worldPacket.WriteUInt32(this.TeamWins);
		base._worldPacket.WriteUInt32(this.SeasonPlayed);
		base._worldPacket.WriteUInt32(this.SeasonWins);
		base._worldPacket.WriteUInt32(this.TeamRating);
		base._worldPacket.WriteUInt32(this.PlayerRating);
		base._worldPacket.WriteInt32(this.Members.Count);
		if (ModernVersion.AddedInClassicVersion(1, 14, 2, 2, 5, 3))
		{
			base._worldPacket.WriteBit(this.UnkBit);
			base._worldPacket.FlushBits();
		}
		foreach (ArenaTeamMember member2 in this.Members)
		{
			member2.Write(base._worldPacket);
		}
	}
}
