using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InspectPvP : ServerPacket
{
	public WowGuid128 PlayerGUID;

	public readonly List<PvPBracketInspectData> Brackets = new();

	public readonly List<ArenaTeamInspectData> ArenaTeams = new();

	public InspectPvP()
		: base(Opcode.SMSG_INSPECT_PVP)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(PlayerGUID);
		_worldPacket.WriteBits(Brackets.Count, 3);
		_worldPacket.WriteBits(ArenaTeams.Count, 2);
		_worldPacket.FlushBits();
		foreach (var bracket in Brackets)
		{
			bracket.Write(_worldPacket);
		}
		foreach (var team in ArenaTeams)
		{
			team.Write(_worldPacket);
		}
	}
}
