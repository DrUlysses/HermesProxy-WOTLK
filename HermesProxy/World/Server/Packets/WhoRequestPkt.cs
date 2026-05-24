using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class WhoRequestPkt : ClientPacket
{
	public readonly WhoRequest Request = new();

	public uint RequestID;

	public readonly List<int> Areas = new();

	public WhoRequestPkt(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var areasCount = _worldPacket.ReadBits<uint>(4);
		Request.Read(_worldPacket);
		RequestID = _worldPacket.ReadUInt32();
		for (var i = 0; i < areasCount; i++)
		{
			Areas.Add(_worldPacket.ReadInt32());
		}
	}
}
