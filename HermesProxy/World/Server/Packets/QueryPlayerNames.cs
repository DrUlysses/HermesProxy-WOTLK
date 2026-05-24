using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class QueryPlayerNames : ClientPacket
{
	public List<WowGuid128> Players = new List<WowGuid128>();

	public QueryPlayerNames(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint count = _worldPacket.ReadUInt32();
		for (uint i = 0u; i < count; i++)
		{
			Players.Add(_worldPacket.ReadPackedGuid128());
		}
	}
}
