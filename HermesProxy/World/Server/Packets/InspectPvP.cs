using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InspectPvP : ServerPacket
{
	public WowGuid128 PlayerGUID;

	public List<PvPBracketInspectData> Brackets = new List<PvPBracketInspectData>();

	public List<ArenaTeamInspectData> ArenaTeams = new List<ArenaTeamInspectData>();

	public InspectPvP()
		: base(Opcode.SMSG_INSPECT_PVP)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PlayerGUID);
		base._worldPacket.WriteBits(this.Brackets.Count, 3);
		base._worldPacket.WriteBits(this.ArenaTeams.Count, 2);
		base._worldPacket.FlushBits();
		foreach (PvPBracketInspectData bracket in this.Brackets)
		{
			bracket.Write(base._worldPacket);
		}
		foreach (ArenaTeamInspectData team in this.ArenaTeams)
		{
			team.Write(base._worldPacket);
		}
	}
}
