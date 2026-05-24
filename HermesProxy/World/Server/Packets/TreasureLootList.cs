using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class TreasureLootList
{
	public readonly List<TreasureItem> Items = new();

	public void Write(WorldPacket data)
	{
		data.WriteInt32(Items.Count);
		foreach (var item in Items)
		{
			item.Write(data);
		}
	}
}
