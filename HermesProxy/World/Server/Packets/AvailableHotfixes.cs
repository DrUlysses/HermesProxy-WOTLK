using System.Collections.Generic;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

internal class AvailableHotfixes : ServerPacket
{
	public uint VirtualRealmAddress;

	public AvailableHotfixes()
		: base(Opcode.SMSG_AVAILABLE_HOTFIXES)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(VirtualRealmAddress);
		_worldPacket.WriteInt32(GameData.Hotfixes.Count);
		foreach (KeyValuePair<uint, HotfixRecord> hotfix2 in GameData.Hotfixes)
		{
			hotfix2.Value.WriteAvailable(_worldPacket);
		}
	}
}
