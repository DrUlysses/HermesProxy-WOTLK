namespace HermesProxy.World.Server.Packets;

public struct AuctionItemForSale
{
	public readonly WowGuid128 Guid;

	public readonly uint UseCount;

	public AuctionItemForSale(WorldPacket data)
	{
		Guid = data.ReadPackedGuid128();
		UseCount = data.ReadUInt32();
	}
}
