using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GossipMessagePkt : ServerPacket
{
	public List<ClientGossipOption> GossipOptions = new List<ClientGossipOption>();

	public int FriendshipFactionID;

	public WowGuid128 GossipGUID;

	public List<ClientGossipQuest> GossipQuests = new List<ClientGossipQuest>();

	public int TextID;

	public int GossipID;

	public GossipMessagePkt()
		: base(Opcode.SMSG_GOSSIP_MESSAGE)
	{
	}

	protected override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			this.WriteWotLK();
			return;
		}
		base._worldPacket.WritePackedGuid128(this.GossipGUID);
		base._worldPacket.WriteInt32(this.GossipID);
		base._worldPacket.WriteInt32(this.FriendshipFactionID);
		base._worldPacket.WriteInt32(this.TextID);
		base._worldPacket.WriteInt32(this.GossipOptions.Count);
		base._worldPacket.WriteInt32(this.GossipQuests.Count);
		foreach (ClientGossipOption options in this.GossipOptions)
		{
			base._worldPacket.WriteInt32(options.OptionIndex);
			base._worldPacket.WriteUInt8(options.OptionIcon);
			base._worldPacket.WriteUInt8(options.OptionFlags);
			base._worldPacket.WriteInt32(options.OptionCost);
			if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
			{
				base._worldPacket.WriteUInt32(options.Language);
			}
			base._worldPacket.WriteBits(options.Text.GetByteCount(), 12);
			base._worldPacket.WriteBits(options.Confirm.GetByteCount(), 12);
			base._worldPacket.WriteBits((byte)options.Status, 2);
			base._worldPacket.WriteBit(options.SpellID.HasValue);
			base._worldPacket.FlushBits();
			options.Treasure.Write(base._worldPacket);
			base._worldPacket.WriteString(options.Text);
			base._worldPacket.WriteString(options.Confirm);
			if (options.SpellID.HasValue)
			{
				base._worldPacket.WriteInt32(options.SpellID.Value);
			}
		}
		foreach (ClientGossipQuest text in this.GossipQuests)
		{
			text.Write(base._worldPacket);
		}
	}

	private void WriteWotLK()
	{
		base._worldPacket.WritePackedGuid128(this.GossipGUID);
		base._worldPacket.WriteInt32(this.GossipID);
		base._worldPacket.WriteInt32(this.FriendshipFactionID);
		base._worldPacket.WriteUInt32((uint)this.GossipOptions.Count);
		base._worldPacket.WriteUInt32((uint)this.GossipQuests.Count);
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.FlushBits();
		foreach (ClientGossipOption options in this.GossipOptions)
		{
			base._worldPacket.WriteInt32(options.OptionIndex);
			base._worldPacket.WriteUInt8(options.OptionIcon);
			base._worldPacket.WriteInt8((sbyte)options.OptionFlags);
			base._worldPacket.WriteInt32(options.OptionCost);
			base._worldPacket.WriteUInt32(options.Language);
			base._worldPacket.WriteInt32(0);
			base._worldPacket.WriteInt32(options.OptionIndex);
			base._worldPacket.WriteBits(options.Text.GetByteCount(), 12);
			base._worldPacket.WriteBits(options.Confirm.GetByteCount(), 12);
			base._worldPacket.WriteBits((byte)options.Status, 2);
			base._worldPacket.WriteBit(options.SpellID.HasValue);
			base._worldPacket.WriteBit(bit: false);
			base._worldPacket.FlushBits();
			options.Treasure.Write(base._worldPacket);
			base._worldPacket.WriteString(options.Text);
			base._worldPacket.WriteString(options.Confirm);
			if (options.SpellID.HasValue)
			{
				base._worldPacket.WriteInt32(options.SpellID.Value);
			}
		}
		base._worldPacket.WriteInt32(this.TextID);
		foreach (ClientGossipQuest quest in this.GossipQuests)
		{
			quest.WriteWotLK(base._worldPacket);
		}
	}
}
