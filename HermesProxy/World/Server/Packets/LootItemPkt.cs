using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class LootItemPkt : ClientPacket
{
	public List<LootRequest> Loot = new List<LootRequest>();

	public LootItemPkt(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint Count = _worldPacket.ReadUInt32();
		for (uint i = 0u; i < Count; i++)
		{
			LootRequest loot = new LootRequest
			{
				LootObj = _worldPacket.ReadPackedGuid128(),
				LootListID = _worldPacket.ReadUInt8()
			};
			Loot.Add(loot);
		}
	}
}
