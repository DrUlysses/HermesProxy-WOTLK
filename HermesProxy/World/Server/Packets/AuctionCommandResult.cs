using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuctionCommandResult : ServerPacket
{
	public uint AuctionID;

	public AuctionHouseAction Command;

	public AuctionHouseError ErrorCode;

	public InventoryResult BagResult = InventoryResult.InternalBagError;

	public WowGuid128 Guid = WowGuid128.Empty;

	public ulong MinIncrement;

	public ulong Money;

	public uint DesiredDelay;

	public AuctionCommandResult()
		: base(Opcode.SMSG_AUCTION_COMMAND_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.AuctionID);
		base._worldPacket.WriteInt32((int)this.Command);
		base._worldPacket.WriteInt32((int)this.ErrorCode);
		base._worldPacket.WriteInt32((int)this.BagResult);
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteUInt64(this.MinIncrement);
		base._worldPacket.WriteUInt64(this.Money);
		base._worldPacket.WriteUInt32(this.DesiredDelay);
	}
}
