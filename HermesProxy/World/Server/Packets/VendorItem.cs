namespace HermesProxy.World.Server.Packets;

public class VendorItem
{
	public int Slot;

	public int Type = 1;

	public ItemInstance Item = new ItemInstance();

	public int Quantity = -1;

	public ulong Price;

	public int Durability;

	public uint StackCount;

	public int ExtendedCostID;

	public int PlayerConditionFailed;

	public bool DoNotFilterOnVendor;

	public bool Refundable;

	public uint MuID;

	public void Write(WorldPacket data)
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			WriteWotLK(data);
			return;
		}
		data.WriteInt32(Slot);
		data.WriteInt32(Type);
		data.WriteInt32(Quantity);
		data.WriteUInt64(Price);
		data.WriteInt32(Durability);
		data.WriteUInt32(StackCount);
		data.WriteInt32(ExtendedCostID);
		data.WriteInt32(PlayerConditionFailed);
		Item.Write(data);
		data.WriteBit(DoNotFilterOnVendor);
		data.WriteBit(Refundable);
		data.FlushBits();
	}

	private void WriteWotLK(WorldPacket data)
	{
		data.WriteUInt64(Price);
		data.WriteUInt32(MuID);
		data.WriteInt32(Type);
		data.WriteInt32(Durability);
		data.WriteInt32((int)StackCount);
		data.WriteInt32(Quantity);
		data.WriteInt32(ExtendedCostID);
		data.WriteInt32(PlayerConditionFailed);
		data.WriteBit(bit: false);
		data.WriteBit(DoNotFilterOnVendor);
		data.WriteBit(Refundable);
		data.FlushBits();
		Item.Write(data);
	}
}
