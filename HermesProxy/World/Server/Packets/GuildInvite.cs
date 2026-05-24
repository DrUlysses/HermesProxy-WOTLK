using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildInvite : ServerPacket
{
	public WowGuid128 GuildGUID;

	public WowGuid128 OldGuildGUID = WowGuid128.Empty;

	public uint EmblemColor;

	public uint EmblemStyle;

	public uint BorderStyle;

	public uint BorderColor;

	public uint BackgroundColor;

	public int AchievementPoints = -1;

	public uint GuildVirtualRealmAddress;

	public uint OldGuildVirtualRealmAddress;

	public uint InviterVirtualRealmAddress;

	public string InviterName;

	public string GuildName;

	public string OldGuildName = "";

	public GuildInvite()
		: base(Opcode.SMSG_GUILD_INVITE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.InviterName.GetByteCount(), 6);
		base._worldPacket.WriteBits(this.GuildName.GetByteCount(), 7);
		base._worldPacket.WriteBits(this.OldGuildName.GetByteCount(), 7);
		base._worldPacket.WriteUInt32(this.InviterVirtualRealmAddress);
		base._worldPacket.WriteUInt32(this.GuildVirtualRealmAddress);
		base._worldPacket.WritePackedGuid128(this.GuildGUID);
		base._worldPacket.WriteUInt32(this.OldGuildVirtualRealmAddress);
		base._worldPacket.WritePackedGuid128(this.OldGuildGUID);
		base._worldPacket.WriteUInt32(this.EmblemStyle);
		base._worldPacket.WriteUInt32(this.EmblemColor);
		base._worldPacket.WriteUInt32(this.BorderStyle);
		base._worldPacket.WriteUInt32(this.BorderColor);
		base._worldPacket.WriteUInt32(this.BackgroundColor);
		base._worldPacket.WriteInt32(this.AchievementPoints);
		base._worldPacket.WriteString(this.InviterName);
		base._worldPacket.WriteString(this.GuildName);
		base._worldPacket.WriteString(this.OldGuildName);
	}
}
