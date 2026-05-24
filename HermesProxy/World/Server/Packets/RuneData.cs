using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class RuneData
{
	public byte Start;

	public byte Count;

	public readonly List<byte> Cooldowns = new();

	public void Write(WorldPacket data)
	{
		data.WriteUInt8(Start);
		data.WriteUInt8(Count);
		data.WriteInt32(Cooldowns.Count);
		foreach (var cd in Cooldowns)
		{
			data.WriteUInt8(cd);
		}
	}
}
