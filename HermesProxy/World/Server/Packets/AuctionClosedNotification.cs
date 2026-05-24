using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuctionClosedNotification : ServerPacket
{
	public AuctionOwnerNotification Info;

	public float ProceedsMailDelay = 3600f;

	public bool Sold = true;

	public AuctionClosedNotification()
		: base(Opcode.SMSG_AUCTION_CLOSED_NOTIFICATION)
	{
	}

	protected override void Write()
	{
		this.Info.Write(base._worldPacket);
		base._worldPacket.WriteFloat(this.ProceedsMailDelay);
		base._worldPacket.WriteBit(this.Sold);
		base._worldPacket.FlushBits();
	}
}
