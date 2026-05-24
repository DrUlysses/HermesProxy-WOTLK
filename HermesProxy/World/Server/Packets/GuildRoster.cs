using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildRoster : ServerPacket
{
	public List<GuildRosterMemberData> MemberData = new List<GuildRosterMemberData>();

	public string WelcomeText;

	public string InfoText;

	public uint CreateDate;

	public uint NumAccounts;

	public int GuildFlags = 2;

	public GuildRoster()
		: base(Opcode.SMSG_GUILD_ROSTER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.NumAccounts);
		base._worldPacket.WritePackedTime(this.CreateDate);
		base._worldPacket.WriteInt32(this.GuildFlags);
		base._worldPacket.WriteInt32(this.MemberData.Count);
		base._worldPacket.WriteBits(this.WelcomeText.GetByteCount(), 11);
		base._worldPacket.WriteBits(this.InfoText.GetByteCount(), 11);
		base._worldPacket.FlushBits();
		this.MemberData.ForEach(delegate(GuildRosterMemberData p)
		{
			p.Write(base._worldPacket);
		});
		base._worldPacket.WriteString(this.WelcomeText);
		base._worldPacket.WriteString(this.InfoText);
	}
}
