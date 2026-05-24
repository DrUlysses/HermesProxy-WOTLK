using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GossipMessagePkt : ServerPacket
{
	public readonly List<ClientGossipOption> GossipOptions = new();

	public int FriendshipFactionID;

	public WowGuid128 GossipGUID;

	public readonly List<ClientGossipQuest> GossipQuests = new();

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
			WriteWotLK();
			return;
		}
		_worldPacket.WritePackedGuid128(GossipGUID);
		_worldPacket.WriteInt32(GossipID);
		_worldPacket.WriteInt32(FriendshipFactionID);
		_worldPacket.WriteInt32(TextID);
		_worldPacket.WriteInt32(GossipOptions.Count);
		_worldPacket.WriteInt32(GossipQuests.Count);
		foreach (var options in GossipOptions)
		{
			_worldPacket.WriteInt32(options.OptionIndex);
			_worldPacket.WriteUInt8(options.OptionIcon);
			_worldPacket.WriteUInt8(options.OptionFlags);
			_worldPacket.WriteInt32(options.OptionCost);
			if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
			{
				_worldPacket.WriteUInt32(options.Language);
			}
			_worldPacket.WriteBits(options.Text.GetByteCount(), 12);
			_worldPacket.WriteBits(options.Confirm.GetByteCount(), 12);
			_worldPacket.WriteBits((byte)options.Status, 2);
			_worldPacket.WriteBit(options.SpellID.HasValue);
			_worldPacket.FlushBits();
			options.Treasure.Write(_worldPacket);
			_worldPacket.WriteString(options.Text);
			_worldPacket.WriteString(options.Confirm);
			if (options.SpellID.HasValue)
			{
				_worldPacket.WriteInt32(options.SpellID.Value);
			}
		}
		foreach (var text in GossipQuests)
		{
			text.Write(_worldPacket);
		}
	}

	private void WriteWotLK()
	{
		_worldPacket.WritePackedGuid128(GossipGUID);
		_worldPacket.WriteInt32(GossipID);
		_worldPacket.WriteInt32(FriendshipFactionID);
		_worldPacket.WriteUInt32((uint)GossipOptions.Count);
		_worldPacket.WriteUInt32((uint)GossipQuests.Count);
		_worldPacket.WriteBit(bit: true);
		_worldPacket.WriteBit(bit: false);
		_worldPacket.FlushBits();
		foreach (var options in GossipOptions)
		{
			_worldPacket.WriteInt32(options.OptionIndex);
			_worldPacket.WriteUInt8(options.OptionIcon);
			_worldPacket.WriteInt8((sbyte)options.OptionFlags);
			_worldPacket.WriteInt32(options.OptionCost);
			_worldPacket.WriteUInt32(options.Language);
			_worldPacket.WriteInt32(0);
			_worldPacket.WriteInt32(options.OptionIndex);
			_worldPacket.WriteBits(options.Text.GetByteCount(), 12);
			_worldPacket.WriteBits(options.Confirm.GetByteCount(), 12);
			_worldPacket.WriteBits((byte)options.Status, 2);
			_worldPacket.WriteBit(options.SpellID.HasValue);
			_worldPacket.WriteBit(bit: false);
			_worldPacket.FlushBits();
			options.Treasure.Write(_worldPacket);
			_worldPacket.WriteString(options.Text);
			_worldPacket.WriteString(options.Confirm);
			if (options.SpellID.HasValue)
			{
				_worldPacket.WriteInt32(options.SpellID.Value);
			}
		}
		_worldPacket.WriteInt32(TextID);
		foreach (var quest in GossipQuests)
		{
			quest.WriteWotLK(_worldPacket);
		}
	}
}
