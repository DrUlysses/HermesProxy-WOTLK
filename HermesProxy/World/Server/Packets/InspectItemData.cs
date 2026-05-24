using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class InspectItemData
{
	public readonly WowGuid128 CreatorGUID = WowGuid128.Empty;

	public readonly ItemInstance Item = new();

	public byte Index;

	public bool Usable;

	public readonly List<InspectEnchantData> Enchants = new();

	public readonly List<ItemGemData> Gems = new();

	public readonly List<int> AzeritePowers = new();

	public readonly List<AzeriteEssenceData> AzeriteEssences = new();

	public void Write(WorldPacket data)
	{
		data.WritePackedGuid128(CreatorGUID);
		data.WriteUInt8(Index);
		data.WriteInt32(AzeritePowers.Count);
		data.WriteInt32(AzeriteEssences.Count);
		foreach (var id in AzeritePowers)
		{
			data.WriteInt32(id);
		}
		Item.Write(data);
		data.WriteBit(Usable);
		data.WriteBits(Enchants.Count, 4);
		data.WriteBits(Gems.Count, 2);
		data.FlushBits();
		foreach (var azeriteEssence in AzeriteEssences)
		{
			azeriteEssence.Write(data);
		}
		foreach (var enchant in Enchants)
		{
			enchant.Write(data);
		}
		foreach (var gem in Gems)
		{
			gem.Write(data);
		}
	}
}
