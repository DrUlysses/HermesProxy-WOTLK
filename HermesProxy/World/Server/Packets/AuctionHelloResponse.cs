using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuctionHelloResponse : ServerPacket
{
	public WowGuid128 Guid;

	public uint PurchasedItemDeliveryDelay;

	public uint CancelledItemDeliveryDelay;

	public bool OpenForBusiness = true;

	public AuctionHelloResponse()
		: base(Opcode.SMSG_AUCTION_HELLO_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteUInt32(this.PurchasedItemDeliveryDelay);
		base._worldPacket.WriteUInt32(this.CancelledItemDeliveryDelay);
		base._worldPacket.WriteBit(this.OpenForBusiness);
		base._worldPacket.FlushBits();
	}
}
