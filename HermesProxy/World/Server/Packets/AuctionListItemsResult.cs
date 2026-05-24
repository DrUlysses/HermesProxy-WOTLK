using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AuctionListItemsResult : ServerPacket
{
	public List<AuctionItem> Items = new List<AuctionItem>();

	public int TotalItemsCount;

	public uint DesiredDelay = 300u;

	public bool HasMoreResults;

	public AuctionListItemsResult()
		: base(Opcode.SMSG_AUCTION_LIST_ITEMS_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Items.Count);
		base._worldPacket.WriteUInt32(0u); // Unknown830
		base._worldPacket.WriteInt32(this.TotalItemsCount);
		base._worldPacket.WriteUInt32(this.DesiredDelay);
		base._worldPacket.WriteBits(0, 2); // ListType
		base._worldPacket.WriteBit(this.HasMoreResults);
		base._worldPacket.FlushBits();
		// Empty AuctionBucketKey: ItemID=0, no optional fields
		base._worldPacket.WriteBits(0, 20); // ItemID
		base._worldPacket.WriteBit(false); // no BattlePetSpeciesID
		base._worldPacket.WriteBits(0, 11); // ItemLevel
		base._worldPacket.WriteBit(false); // no SuffixItemNameDescriptionID
		base._worldPacket.FlushBits();
		foreach (AuctionItem item in this.Items)
		{
			item.Write(base._worldPacket);
		}
	}
}
