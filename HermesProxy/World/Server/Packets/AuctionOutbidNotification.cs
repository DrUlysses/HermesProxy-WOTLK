using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuctionOutbidNotification : ServerPacket
{
	public AuctionBidderNotification Info;

	public ulong BidAmount;

	public ulong MinIncrement;

	public AuctionOutbidNotification()
		: base(Opcode.SMSG_AUCTION_OUTBID_NOTIFICATION)
	{
	}

	protected override void Write()
	{
		this.Info.Write(base._worldPacket);
		base._worldPacket.WriteUInt64(this.BidAmount);
		base._worldPacket.WriteUInt64(this.MinIncrement);
	}
}
