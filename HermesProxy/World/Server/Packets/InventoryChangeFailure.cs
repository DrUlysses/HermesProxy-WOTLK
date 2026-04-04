using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InventoryChangeFailure : ServerPacket
{
	public InventoryResult BagResult;

	public byte ContainerBSlot;

	public WowGuid128 SrcContainer;

	public WowGuid128 DstContainer;

	public int SrcSlot;

	public int LimitCategory;

	public int Level;

	public WowGuid128[] Item = new WowGuid128[2];

	public InventoryChangeFailure()
		: base(Opcode.SMSG_INVENTORY_CHANGE_FAILURE)
	{
	}

	public override void Write()
	{
		base._worldPacket.WriteInt32((int)this.BagResult);
		base._worldPacket.WritePackedGuid128(this.Item[0]);
		base._worldPacket.WritePackedGuid128(this.Item[1]);
		base._worldPacket.WriteUInt8(this.ContainerBSlot);
		switch (this.BagResult)
		{
		case InventoryResult.CantEquipLevel:
		case InventoryResult.PurchaseLevelTooLow:
			base._worldPacket.WriteInt32(this.Level);
			break;
		case InventoryResult.EventAutoEquipBindConfirm:
			base._worldPacket.WritePackedGuid128(this.SrcContainer);
			base._worldPacket.WriteInt32(this.SrcSlot);
			base._worldPacket.WritePackedGuid128(this.DstContainer);
			break;
		case InventoryResult.ItemMaxLimitCategoryCountExceeded:
		case InventoryResult.ItemMaxLimitCategorySocketedExceeded:
		case InventoryResult.ItemMaxLimitCategoryEquippedExceeded:
			base._worldPacket.WriteInt32(this.LimitCategory);
			break;
		}
	}
}
