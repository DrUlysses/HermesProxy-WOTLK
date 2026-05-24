using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SellResponse : ServerPacket
{
	public WowGuid128 VendorGUID;

	public WowGuid128 ItemGUID;

	public byte Reason;

	public SellResponse()
		: base(Opcode.SMSG_SELL_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.VendorGUID);
		base._worldPacket.WriteUInt32(1); // ItemGUIDs count
		base._worldPacket.WriteInt32((int)this.Reason);
		base._worldPacket.WritePackedGuid128(this.ItemGUID);
	}
}
