using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ChannelNotifyLeft : ServerPacket
{
	public string Channel;

	public int ChatChannelID;

	public bool Suspended;

	public ChannelNotifyLeft()
		: base(Opcode.SMSG_CHANNEL_NOTIFY_LEFT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Channel.GetByteCount(), 7);
		base._worldPacket.WriteBit(this.Suspended);
		base._worldPacket.WriteInt32(this.ChatChannelID);
		base._worldPacket.WriteString(this.Channel);
	}
}
