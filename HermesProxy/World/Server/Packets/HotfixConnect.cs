using System.Collections.Generic;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

internal class HotfixConnect : ServerPacket
{
	public readonly List<HotfixRecord> Hotfixes = new();

	public HotfixConnect()
		: base(Opcode.SMSG_HOTFIX_CONNECT)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Hotfixes.Count);
		var totalDataSize = 0u;
		foreach (var hotfix in Hotfixes)
		{
			totalDataSize += hotfix.HotfixContent.GetSize();
			hotfix.WriteHotFixMessageContent(_worldPacket);
		}
		_worldPacket.WriteUInt32(totalDataSize);
		foreach (var hotfix2 in Hotfixes)
		{
			_worldPacket.WriteBytes(hotfix2.HotfixContent);
		}
	}
}
