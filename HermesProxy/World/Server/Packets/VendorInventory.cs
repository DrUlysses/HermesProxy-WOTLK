using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class VendorInventory : ServerPacket
{
	public byte Reason = 0;

	public List<VendorItem> Items = new List<VendorItem>();

	public WowGuid128 VendorGUID;

	public VendorInventory()
		: base(Opcode.SMSG_VENDOR_INVENTORY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.VendorGUID);
		base._worldPacket.WriteUInt8(this.Reason);
		base._worldPacket.WriteInt32(this.Items.Count);
		foreach (VendorItem item in this.Items)
		{
			item.Write(base._worldPacket);
		}
	}
}
