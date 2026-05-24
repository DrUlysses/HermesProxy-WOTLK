using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ChannelListResponse : ServerPacket
{
	public struct ChannelPlayer
	{
		public WowGuid128 Guid;

		public uint VirtualRealmAddress;

		public byte Flags;
	}

	public List<ChannelPlayer> Members;

	public string ChannelName;

	public ChannelFlags ChannelFlags;

	public bool Display;

	public ChannelListResponse()
		: base(Opcode.SMSG_CHANNEL_LIST)
	{
		this.Members = new List<ChannelPlayer>();
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Display);
		base._worldPacket.WriteBits(this.ChannelName.GetByteCount(), 7);
		base._worldPacket.WriteUInt32((uint)this.ChannelFlags);
		base._worldPacket.WriteInt32(this.Members.Count);
		base._worldPacket.WriteString(this.ChannelName);
		foreach (ChannelPlayer player in this.Members)
		{
			base._worldPacket.WritePackedGuid128(player.Guid);
			base._worldPacket.WriteUInt32(player.VirtualRealmAddress);
			base._worldPacket.WriteUInt8(player.Flags);
		}
	}
}
