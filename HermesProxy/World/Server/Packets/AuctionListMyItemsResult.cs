using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AuctionListMyItemsResult : ServerPacket
{
	public readonly List<AuctionItem> Items = new();

	public readonly List<AuctionItem> SoldItems = new();

	public uint DesiredDelay = 300u;

	public bool HasMoreResults;

	public AuctionListMyItemsResult(Opcode opcode)
		: base(opcode)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Items.Count);
		_worldPacket.WriteInt32(SoldItems.Count);
		_worldPacket.WriteUInt32(DesiredDelay);
		_worldPacket.WriteBit(HasMoreResults);
		_worldPacket.FlushBits();
		foreach (var item in Items)
		{
			item.Write(_worldPacket);
		}
		foreach (var item in SoldItems)
		{
			item.Write(_worldPacket);
		}
	}
}
