using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TradeStatusPkt : ServerPacket
{
	public bool PartnerIsSameBnetAccount;

	public TradeStatus Status = TradeStatus.Initiated;

	public bool FailureForYou;

	public InventoryResult BagResult;

	public uint ItemID;

	public uint Id;

	public WowGuid128 Partner;

	public WowGuid128 PartnerAccount;

	public byte TradeSlot;

	public int CurrencyType;

	public int CurrencyQuantity;

	public TradeStatusPkt()
		: base(Opcode.SMSG_TRADE_STATUS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.PartnerIsSameBnetAccount);
		base._worldPacket.WriteBits(this.Status, 5);
		switch (this.Status)
		{
		case TradeStatus.Failed:
			base._worldPacket.WriteBit(this.FailureForYou);
			base._worldPacket.WriteInt32((int)this.BagResult);
			base._worldPacket.WriteUInt32(this.ItemID);
			break;
		case TradeStatus.Initiated:
			base._worldPacket.WriteUInt32(this.Id);
			break;
		case TradeStatus.Proposed:
			base._worldPacket.WritePackedGuid128(this.Partner);
			base._worldPacket.WritePackedGuid128(this.PartnerAccount);
			break;
		case TradeStatus.WrongRealm:
		case TradeStatus.NotOnTaplist:
			base._worldPacket.WriteUInt8(this.TradeSlot);
			break;
		case TradeStatus.CurrencyNotTradable:
		case TradeStatus.NotEnoughCurrency:
			base._worldPacket.WriteInt32(this.CurrencyType);
			base._worldPacket.WriteInt32(this.CurrencyQuantity);
			break;
		default:
			base._worldPacket.FlushBits();
			break;
		}
	}
}
