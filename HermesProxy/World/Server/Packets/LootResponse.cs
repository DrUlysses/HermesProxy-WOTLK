using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LootResponse : ServerPacket
{
	public WowGuid128 Owner;

	public WowGuid128 LootObj;

	public LootError FailureReason = LootError.NoLoot;

	public LootType AcquireReason;

	public LootMethod LootMethod;

	public byte Threshold = 2;

	public uint Coins;

	public List<LootItemData> Items = new List<LootItemData>();

	public List<LootCurrency> Currencies = new List<LootCurrency>();

	public bool Acquired = true;

	public bool AELooting;

	public LootResponse()
		: base(Opcode.SMSG_LOOT_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Owner);
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WriteUInt8((byte)this.FailureReason);
		base._worldPacket.WriteUInt8((byte)this.AcquireReason);
		base._worldPacket.WriteUInt8((byte)this.LootMethod);
		base._worldPacket.WriteUInt8(this.Threshold);
		base._worldPacket.WriteUInt32(this.Coins);
		base._worldPacket.WriteInt32(this.Items.Count);
		base._worldPacket.WriteInt32(this.Currencies.Count);
		base._worldPacket.WriteBit(this.Acquired);
		base._worldPacket.WriteBit(this.AELooting);
		base._worldPacket.FlushBits();
		foreach (LootItemData item in this.Items)
		{
			item.Write(base._worldPacket);
		}
		foreach (LootCurrency currency in this.Currencies)
		{
			base._worldPacket.WriteUInt32(currency.CurrencyID);
			base._worldPacket.WriteUInt32(currency.Quantity);
			base._worldPacket.WriteUInt8(currency.LootListID);
			base._worldPacket.WriteBits(currency.UIType, 3);
			base._worldPacket.FlushBits();
		}
	}
}
