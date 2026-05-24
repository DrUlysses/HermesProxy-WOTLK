using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class ChatRegisterAddonPrefixes : ClientPacket
{
	public List<string> Prefixes = new List<string>();

	public ChatRegisterAddonPrefixes(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var count = _worldPacket.ReadInt32();
		for (var i = 0; i < count && i < 64; i++)
		{
			Prefixes.Add(_worldPacket.ReadString(_worldPacket.ReadBits<uint>(5)));
		}
	}
}
