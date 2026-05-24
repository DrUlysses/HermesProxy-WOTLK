using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuctionOwnerBidNotification : ServerPacket
{
	public AuctionOwnerNotification Info;

	public ulong MinIncrement;

	public WowGuid128 Bidder;

	public AuctionOwnerBidNotification()
		: base(Opcode.SMSG_AUCTION_OWNER_BID_NOTIFICATION)
	{
	}

	protected override void Write()
	{
		this.Info.Write(base._worldPacket);
		base._worldPacket.WriteUInt64(this.MinIncrement);
		base._worldPacket.WritePackedGuid128(this.Bidder);
	}
}
