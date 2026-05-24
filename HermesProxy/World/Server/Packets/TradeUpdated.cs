using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TradeUpdated : ServerPacket
{
	public class UnwrappedTradeItem
	{
		public int EnchantID;

		public int OnUseEnchantmentID;

		public WowGuid128 Creator;

		public int Charges;

		public bool Lock;

		public uint MaxDurability;

		public uint Durability;

		public List<ItemGemData> Gems = new List<ItemGemData>();

		public void Write(WorldPacket data)
		{
			data.WriteInt32(this.EnchantID);
			data.WriteInt32(this.OnUseEnchantmentID);
			data.WritePackedGuid128(this.Creator);
			data.WriteInt32(this.Charges);
			data.WriteUInt32(this.MaxDurability);
			data.WriteUInt32(this.Durability);
			data.WriteBits(this.Gems.Count, 2);
			data.WriteBit(this.Lock);
			data.FlushBits();
			foreach (ItemGemData gem in this.Gems)
			{
				gem.Write(data);
			}
		}
	}

	public class TradeItem
	{
		public byte Slot;

		public ItemInstance Item = new ItemInstance();

		public int StackCount;

		public WowGuid128 GiftCreator;

		public UnwrappedTradeItem Unwrapped;

		public void Write(WorldPacket data)
		{
			data.WriteUInt8(this.Slot);
			data.WriteInt32(this.StackCount);
			data.WritePackedGuid128(this.GiftCreator);
			this.Item.Write(data);
			data.WriteBit(this.Unwrapped != null);
			data.FlushBits();
			if (this.Unwrapped != null)
			{
				this.Unwrapped.Write(data);
			}
		}
	}

	public ulong Gold;

	public uint CurrentStateIndex;

	public byte WhichPlayer;

	public uint ClientStateIndex;

	public List<TradeItem> Items = new List<TradeItem>();

	public int CurrencyType;

	public uint Id;

	public int ProposedEnchantment;

	public int CurrencyQuantity;

	public TradeUpdated()
		: base(Opcode.SMSG_TRADE_UPDATED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.WhichPlayer);
		base._worldPacket.WriteUInt32(this.Id);
		base._worldPacket.WriteUInt32(this.ClientStateIndex);
		base._worldPacket.WriteUInt32(this.CurrentStateIndex);
		base._worldPacket.WriteUInt64(this.Gold);
		base._worldPacket.WriteInt32(this.CurrencyType);
		base._worldPacket.WriteInt32(this.CurrencyQuantity);
		base._worldPacket.WriteInt32(this.ProposedEnchantment);
		base._worldPacket.WriteInt32(this.Items.Count);
		this.Items.ForEach(delegate(TradeItem item)
		{
			item.Write(base._worldPacket);
		});
	}
}
