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
		_worldPacket.WritePackedGuid128(VendorGUID);
		_worldPacket.WriteUInt32(Muid);
		_worldPacket.WriteInt32(NewQuantity);
		_worldPacket.WriteUInt32(QuantityBought);
	}
}
