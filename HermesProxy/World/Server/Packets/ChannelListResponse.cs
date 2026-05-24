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
		Members = new List<ChannelPlayer>();
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(Display);
		_worldPacket.WriteBits(ChannelName.GetByteCount(), 7);
		_worldPacket.WriteUInt32((uint)ChannelFlags);
		_worldPacket.WriteInt32(Members.Count);
		_worldPacket.WriteString(ChannelName);
		foreach (ChannelPlayer player in Members)
		{
			_worldPacket.WritePackedGuid128(player.Guid);
			_worldPacket.WriteUInt32(player.VirtualRealmAddress);
			_worldPacket.WriteUInt8(player.Flags);
		}
	}
}
