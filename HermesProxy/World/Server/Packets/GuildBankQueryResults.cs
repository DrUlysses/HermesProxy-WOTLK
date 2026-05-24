using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildBankQueryResults : ServerPacket
{
	public readonly List<GuildBankItemInfo> ItemInfo;

	public readonly List<GuildBankTabInfo> TabInfo;

	public int WithdrawalsRemaining;

	public int Tab;

	public ulong Money;

	public bool FullUpdate;

	public GuildBankQueryResults()
		: base(Opcode.SMSG_GUILD_BANK_QUERY_RESULTS)
	{
		ItemInfo = new List<GuildBankItemInfo>();
		TabInfo = new List<GuildBankTabInfo>();
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt64(Money);
		_worldPacket.WriteInt32(Tab);
		_worldPacket.WriteInt32(WithdrawalsRemaining);
		_worldPacket.WriteInt32(TabInfo.Count);
		_worldPacket.WriteInt32(ItemInfo.Count);
		_worldPacket.WriteBit(FullUpdate);
		_worldPacket.FlushBits();
		foreach (var tab in TabInfo)
		{
			_worldPacket.WriteInt32(tab.TabIndex);
			_worldPacket.WriteBits(tab.Name.GetByteCount(), 7);
			_worldPacket.WriteBits(tab.Icon.GetByteCount(), 9);
			_worldPacket.WriteString(tab.Name);
			_worldPacket.WriteString(tab.Icon);
		}
		foreach (var item in ItemInfo)
		{
			_worldPacket.WriteInt32(item.Slot);
			_worldPacket.WriteInt32(item.Count);
			_worldPacket.WriteInt32(item.EnchantmentID);
			_worldPacket.WriteInt32(item.Charges);
			_worldPacket.WriteInt32(item.OnUseEnchantmentID);
			_worldPacket.WriteUInt32(item.Flags);
			item.Item.Write(_worldPacket);
			_worldPacket.WriteBits(item.SocketEnchant.Count, 2);
			_worldPacket.WriteBit(item.Locked);
			_worldPacket.FlushBits();
			foreach (var socketEnchant in item.SocketEnchant)
			{
				socketEnchant.Write(_worldPacket);
			}
		}
	}
}
