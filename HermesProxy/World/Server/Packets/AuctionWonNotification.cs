using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuctionWonNotification : ServerPacket
{
	public AuctionBidderNotification Info;

	public AuctionWonNotification()
		: base(Opcode.SMSG_AUCTION_WON_NOTIFICATION)
	{
	}

	protected override void Write()
	{
		Info.Write(_worldPacket);
	}
}
