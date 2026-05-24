using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ArenaTeamInvite : ServerPacket
{
	public WowGuid128 PlayerGuid;

	public uint PlayerVirtualAddress;

	public WowGuid128 TeamGuid;

	public string PlayerName;

	public string TeamName;

	public ArenaTeamInvite()
		: base(Opcode.SMSG_ARENA_TEAM_INVITE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PlayerGuid);
		base._worldPacket.WriteUInt32(this.PlayerVirtualAddress);
		base._worldPacket.WritePackedGuid128(this.TeamGuid);
		base._worldPacket.WriteBits(this.PlayerName.GetByteCount(), 6);
		base._worldPacket.WriteBits(this.TeamName.GetByteCount(), 7);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.PlayerName);
		base._worldPacket.WriteString(this.TeamName);
	}
}
