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
		foreach (uint bonusID in BonusListIDs)
		{
			data.WriteUInt32(bonusID);
		}
	}

	public void Read(WorldPacket data)
	{
		Context = (ItemContext)data.ReadUInt8();
		uint bonusListIdSize = data.ReadUInt32();
		BonusListIDs = new List<uint>();
		for (uint i = 0u; i < bonusListIdSize; i++)
		{
			uint bonusId = data.ReadUInt32();
			BonusListIDs.Add(bonusId);
		}
	}
}
