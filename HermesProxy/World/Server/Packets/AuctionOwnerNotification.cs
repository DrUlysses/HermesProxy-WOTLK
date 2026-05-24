namespace HermesProxy.World.Server.Packets;

public class AuctionOwnerNotification
{
	public uint AuctionID;

	public ulong BidAmount;

	public readonly ItemInstance Item = new();

	public void Write(WorldPacket data)
	{
		data.WriteUInt32(AuctionID);
		data.WriteUInt64(BidAmount);
		Item.Write(data);
	}
}
