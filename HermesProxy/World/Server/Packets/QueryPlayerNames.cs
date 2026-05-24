using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class QueryPlayerNames : ClientPacket
{
	public readonly List<WowGuid128> Players = new();

	public QueryPlayerNames(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var count = _worldPacket.ReadUInt32();
		for (var i = 0u; i < count; i++)
		{
			Players.Add(_worldPacket.ReadPackedGuid128());
		}
	}
}
