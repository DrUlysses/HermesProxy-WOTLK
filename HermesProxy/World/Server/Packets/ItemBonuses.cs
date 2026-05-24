using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ItemBonuses
{
	public ItemContext Context;

	public List<uint> BonusListIDs = new List<uint>();

	public void Write(WorldPacket data)
	{
		data.WriteUInt8((byte)Context);
		data.WriteInt32(BonusListIDs.Count);
		foreach (var bonusID in BonusListIDs)
		{
			data.WriteUInt32(bonusID);
		}
	}

	public void Read(WorldPacket data)
	{
		Context = (ItemContext)data.ReadUInt8();
		var bonusListIdSize = data.ReadUInt32();
		BonusListIDs = new List<uint>();
		for (var i = 0u; i < bonusListIdSize; i++)
		{
			var bonusId = data.ReadUInt32();
			BonusListIDs.Add(bonusId);
		}
	}
}
