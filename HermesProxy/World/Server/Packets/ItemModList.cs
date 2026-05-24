using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class ItemModList
{
	public readonly Array<ItemMod> Values = new(38);

	public void Read(WorldPacket data)
	{
		var itemModListCount = data.ReadBits<uint>(6);
		data.ResetBitPos();
		for (var i = 0; i < itemModListCount; i++)
		{
			var itemMod = new ItemMod();
			itemMod.Read(data);
			Values[i] = itemMod;
		}
	}

	public void Write(WorldPacket data)
	{
		data.WriteBits(Values.Count, 6);
		data.FlushBits();
		foreach (var itemMod in Values)
		{
			itemMod.Write(data);
		}
	}
}
