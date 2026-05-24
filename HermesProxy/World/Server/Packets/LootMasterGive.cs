using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class LootMasterGive : ClientPacket
{
	public WowGuid128 TargetGUID;

	public readonly List<LootRequest> Loot = new();

	public LootMasterGive(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var Count = _worldPacket.ReadUInt32();
		TargetGUID = _worldPacket.ReadPackedGuid128();
		for (var i = 0; i < Count; i++)
		{
			var lootRequest = new LootRequest
			{
				LootObj = _worldPacket.ReadPackedGuid128(),
				LootListID = _worldPacket.ReadUInt8()
			};
			Loot.Add(lootRequest);
		}
	}
}
