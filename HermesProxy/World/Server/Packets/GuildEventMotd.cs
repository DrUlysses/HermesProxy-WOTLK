using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventMotd : ServerPacket
{
	public string MotdText;

	public GuildEventMotd()
		: base(Opcode.SMSG_GUILD_EVENT_MOTD)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.MotdText.GetByteCount(), 11);
		base._worldPacket.WriteString(this.MotdText);
	}
}
