using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildInviteDeclined : ServerPacket
{
	public bool AutoDecline;

	public uint InviterVirtualRealmAddress;

	public string InviterName;

	public GuildInviteDeclined()
		: base(Opcode.SMSG_GUILD_INVITE_DECLINED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.InviterName.GetByteCount(), 6);
		base._worldPacket.WriteBit(this.AutoDecline);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteUInt32(this.InviterVirtualRealmAddress);
		base._worldPacket.WriteString(this.InviterName);
	}
}
