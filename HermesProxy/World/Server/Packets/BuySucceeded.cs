using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BuySucceeded : ServerPacket
{
	public WowGuid128 VendorGUID;

	public uint Muid;

	public int NewQuantity;

	public uint QuantityBought;

	public BuySucceeded()
		: base(Opcode.SMSG_BUY_SUCCEEDED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.VendorGUID);
		base._worldPacket.WriteUInt32(this.Muid);
		base._worldPacket.WriteInt32(this.NewQuantity);
		base._worldPacket.WriteUInt32(this.QuantityBought);
	}
}
