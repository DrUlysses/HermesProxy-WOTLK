using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class HotfixRequest : ClientPacket
{
	public uint ClientBuild;

	public uint DataBuild;

	public List<uint> Hotfixes = new List<uint>();

	public HotfixRequest(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		ClientBuild = _worldPacket.ReadUInt32();
		DataBuild = _worldPacket.ReadUInt32();
		var hotfixCount = _worldPacket.ReadUInt32();
		for (var i = 0; i < hotfixCount; i++)
		{
			Hotfixes.Add(_worldPacket.ReadUInt32());
		}
	}
}
