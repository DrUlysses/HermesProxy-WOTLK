using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildBankQueryResults : ServerPacket
{
	public List<GuildBankItemInfo> ItemInfo;

	public List<GuildBankTabInfo> TabInfo;

	public int WithdrawalsRemaining;

	public int Tab;

	public ulong Money;

	public bool FullUpdate;

	public GuildBankQueryResults()
		: base(Opcode.SMSG_GUILD_BANK_QUERY_RESULTS)
	{
		this.ItemInfo = new List<GuildBankItemInfo>();
		this.TabInfo = new List<GuildBankTabInfo>();
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt64(this.Money);
		base._worldPacket.WriteInt32(this.Tab);
		base._worldPacket.WriteInt32(this.WithdrawalsRemaining);
		base._worldPacket.WriteInt32(this.TabInfo.Count);
		base._worldPacket.WriteInt32(this.ItemInfo.Count);
		base._worldPacket.WriteBit(this.FullUpdate);
		base._worldPacket.FlushBits();
		foreach (GuildBankTabInfo tab in this.TabInfo)
		{
			base._worldPacket.WriteInt32(tab.TabIndex);
			base._worldPacket.WriteBits(tab.Name.GetByteCount(), 7);
			base._worldPacket.WriteBits(tab.Icon.GetByteCount(), 9);
			base._worldPacket.WriteString(tab.Name);
			base._worldPacket.WriteString(tab.Icon);
		}
		foreach (GuildBankItemInfo item in this.ItemInfo)
		{
			base._worldPacket.WriteInt32(item.Slot);
			base._worldPacket.WriteInt32(item.Count);
			base._worldPacket.WriteInt32(item.EnchantmentID);
			base._worldPacket.WriteInt32(item.Charges);
			base._worldPacket.WriteInt32(item.OnUseEnchantmentID);
			base._worldPacket.WriteUInt32(item.Flags);
			item.Item.Write(base._worldPacket);
			base._worldPacket.WriteBits(item.SocketEnchant.Count, 2);
			base._worldPacket.WriteBit(item.Locked);
			base._worldPacket.FlushBits();
			foreach (ItemGemData socketEnchant in item.SocketEnchant)
			{
				socketEnchant.Write(base._worldPacket);
			}
		}
	}
}
