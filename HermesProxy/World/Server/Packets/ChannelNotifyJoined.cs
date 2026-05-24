using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ChannelNotifyJoined : ServerPacket
{
	public string ChannelWelcomeMsg = "";

	public int ChatChannelID;

	public ulong InstanceID;

	public ChannelFlags ChannelFlags;

	public string Channel = "";

	public WowGuid128 ChannelGUID;

	public ChannelNotifyJoined()
		: base(Opcode.SMSG_CHANNEL_NOTIFY_JOINED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Channel.GetByteCount(), 7);
		base._worldPacket.WriteBits(this.ChannelWelcomeMsg.GetByteCount(), 11);
		base._worldPacket.WriteUInt32((uint)this.ChannelFlags);
		base._worldPacket.WriteInt32(this.ChatChannelID);
		base._worldPacket.WriteUInt64(this.InstanceID);
		base._worldPacket.WritePackedGuid128(this.ChannelGUID);
		base._worldPacket.WriteString(this.Channel);
		base._worldPacket.WriteString(this.ChannelWelcomeMsg);
	}
}
