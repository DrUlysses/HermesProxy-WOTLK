using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AuctionListMyItemsResult : ServerPacket
{
	public List<AuctionItem> Items = new List<AuctionItem>();

	public List<AuctionItem> SoldItems = new List<AuctionItem>();

	public uint DesiredDelay = 300u;

	public bool HasMoreResults;

	public AuctionListMyItemsResult(Opcode opcode)
		: base(opcode)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Items.Count);
		base._worldPacket.WriteInt32(this.SoldItems.Count);
		base._worldPacket.WriteUInt32(this.DesiredDelay);
		base._worldPacket.WriteBit(this.HasMoreResults);
		base._worldPacket.FlushBits();
		foreach (AuctionItem item in this.Items)
		{
			item.Write(base._worldPacket);
		}
		foreach (AuctionItem item in this.SoldItems)
		{
			item.Write(base._worldPacket);
		}
	}
}
