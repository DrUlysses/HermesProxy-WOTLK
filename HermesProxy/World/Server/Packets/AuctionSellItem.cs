using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class AuctionSellItem : ClientPacket
{
	public ulong BuyoutPrice;

	public WowGuid128 Auctioneer;

	public ulong MinBid;

	public uint ExpireTime;

	public AddOnInfo TaintedBy;

	public List<AuctionItemForSale> Items = new List<AuctionItemForSale>();

	public AuctionSellItem(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Auctioneer = _worldPacket.ReadPackedGuid128();
		MinBid = _worldPacket.ReadUInt64();
		BuyoutPrice = _worldPacket.ReadUInt64();
		ExpireTime = _worldPacket.ReadUInt32();
		if (_worldPacket.HasBit())
		{
			TaintedBy = new AddOnInfo();
		}
		var itemCountBits = (ModernVersion.AddedInClassicVersion(1, 14, 3, 2, 5, 4) ? 6 : 5);
		var itemCount = _worldPacket.ReadBits<uint>(itemCountBits);
		if (TaintedBy != null)
		{
			TaintedBy.Read(_worldPacket);
		}
		for (var i = 0; i < itemCount; i++)
		{
			Items.Add(new AuctionItemForSale(_worldPacket));
		}
	}
}
