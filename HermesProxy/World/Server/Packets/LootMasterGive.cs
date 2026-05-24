using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class LootMasterGive : ClientPacket
{
	public WowGuid128 TargetGUID;

	public List<LootRequest> Loot = new List<LootRequest>();

	public LootMasterGive(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint Count = _worldPacket.ReadUInt32();
		TargetGUID = _worldPacket.ReadPackedGuid128();
		for (int i = 0; i < Count; i++)
		{
			LootRequest lootRequest = new LootRequest
			{
				LootObj = _worldPacket.ReadPackedGuid128(),
				LootListID = _worldPacket.ReadUInt8()
			};
			Loot.Add(lootRequest);
		}
	}
}
