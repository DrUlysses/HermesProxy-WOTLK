using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ArenaTeamCommandResult : ServerPacket
{
	public ArenaTeamCommandType Action;

	public ArenaTeamCommandErrorModern Error;

	public string TeamName;

	public string PlayerName;

	public ArenaTeamCommandResult()
		: base(Opcode.SMSG_ARENA_TEAM_COMMAND_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8((byte)this.Action);
		base._worldPacket.WriteUInt8((byte)this.Error);
		base._worldPacket.WriteBits(this.TeamName.GetByteCount(), 7);
		base._worldPacket.WriteBits(this.PlayerName.GetByteCount(), 6);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.TeamName);
		base._worldPacket.WriteString(this.PlayerName);
	}
}
