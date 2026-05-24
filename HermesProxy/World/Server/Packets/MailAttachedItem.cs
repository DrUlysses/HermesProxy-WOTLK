using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class MailAttachedItem
{
	public byte Position;

	public int AttachID;

	public readonly ItemInstance Item = new();

	public uint Count;

	public int Charges;

	public uint MaxDurability;

	public uint Durability;

	public bool Unlocked;

	public readonly List<ItemEnchantData> Enchants = new();

	public readonly List<ItemGemData> Gems = new();

	public void Write(WorldPacket data)
	{
		data.WriteUInt8(Position);
		data.WriteUInt64((ulong)AttachID);
		data.WriteInt32((int)Count);
		data.WriteInt32(Charges);
		data.WriteUInt32(MaxDurability);
		data.WriteInt32((int)Durability);
		Item.Write(data);
		data.WriteBits(Enchants.Count, 4);
		data.WriteBits(Gems.Count, 2);
		data.WriteBit(Unlocked);
		data.FlushBits();
		foreach (var gem in Gems)
		{
			gem.Write(data);
		}
		foreach (var en in Enchants)
		{
			en.Write(data);
		}
	}
}
