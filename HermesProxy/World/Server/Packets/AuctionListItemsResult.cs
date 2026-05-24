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
		_worldPacket.WriteInt32(Items.Count);
		_worldPacket.WriteUInt32(0u); // Unknown830
		_worldPacket.WriteInt32(TotalItemsCount);
		_worldPacket.WriteUInt32(DesiredDelay);
		_worldPacket.WriteBits(0, 2); // ListType
		_worldPacket.WriteBit(HasMoreResults);
		_worldPacket.FlushBits();
		// Empty AuctionBucketKey: ItemID=0, no optional fields
		_worldPacket.WriteBits(0, 20); // ItemID
		_worldPacket.WriteBit(false); // no BattlePetSpeciesID
		_worldPacket.WriteBits(0, 11); // ItemLevel
		_worldPacket.WriteBit(false); // no SuffixItemNameDescriptionID
		_worldPacket.FlushBits();
		foreach (AuctionItem item in Items)
		{
			item.Write(_worldPacket);
		}
	}
}
