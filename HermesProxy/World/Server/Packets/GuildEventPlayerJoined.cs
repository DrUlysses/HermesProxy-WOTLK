using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventPlayerJoined : ServerPacket
{
	public WowGuid128 Guid;

	public uint VirtualRealmAddress;

	public string Name;

	public GuildEventPlayerJoined()
		: base(Opcode.SMSG_GUILD_EVENT_PLAYER_JOINED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
		base._worldPacket.WriteString(this.Name);
	}
}
