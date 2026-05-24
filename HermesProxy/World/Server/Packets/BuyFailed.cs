using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BuyFailed : ServerPacket
{
	public WowGuid128 VendorGUID;

	public uint Muid;

	public BuyResult Reason = BuyResult.CantFindItem;

	public BuyFailed()
		: base(Opcode.SMSG_BUY_FAILED)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(VendorGUID);
		_worldPacket.WriteUInt32(Muid);
		_worldPacket.WriteUInt8((byte)Reason);
	}
}
