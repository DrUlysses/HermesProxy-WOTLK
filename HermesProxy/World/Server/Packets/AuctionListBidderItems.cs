using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class AuctionListBidderItems : ClientPacket
{
	public WowGuid128 Auctioneer;

	public uint Offset;

	public readonly List<uint> AuctionItemIDs = new();

	public AuctionListBidderItems(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Auctioneer = _worldPacket.ReadPackedGuid128();
		Offset = _worldPacket.ReadUInt32();
		var auctionIDCount = _worldPacket.ReadBits<uint>(7);
		_worldPacket.ResetBitPos();
		for (var i = 0; i < auctionIDCount; i++)
		{
			AuctionItemIDs[i] = _worldPacket.ReadUInt32();
		}
	}
}
