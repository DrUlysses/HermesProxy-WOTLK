using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class DBQueryBulk : ClientPacket
{
	public DB2Hash TableHash;

	public List<uint> Queries = new List<uint>();

	public DBQueryBulk(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		TableHash = (DB2Hash)_worldPacket.ReadUInt32();
		var count = _worldPacket.ReadBits<uint>(13);
		for (var i = 0u; i < count; i++)
		{
			Queries.Add(_worldPacket.ReadUInt32());
		}
	}
}
