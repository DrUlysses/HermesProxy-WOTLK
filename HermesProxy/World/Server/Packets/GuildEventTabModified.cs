using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventTabModified : ServerPacket
{
	public int Tab;

	public string Name;

	public string Icon;

	public GuildEventTabModified()
		: base(Opcode.SMSG_GUILD_EVENT_TAB_MODIFIED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Tab);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 7);
		base._worldPacket.WriteBits(this.Icon.GetByteCount(), 9);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Name);
		base._worldPacket.WriteString(this.Icon);
	}
}
