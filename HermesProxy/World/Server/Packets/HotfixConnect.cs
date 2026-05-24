using System.Collections.Generic;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

internal class HotfixConnect : ServerPacket
{
	public List<HotfixRecord> Hotfixes = new List<HotfixRecord>();

	public HotfixConnect()
		: base(Opcode.SMSG_HOTFIX_CONNECT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Hotfixes.Count);
		uint totalDataSize = 0u;
		foreach (HotfixRecord hotfix in this.Hotfixes)
		{
			totalDataSize += hotfix.HotfixContent.GetSize();
			hotfix.WriteHotFixMessageContent(base._worldPacket);
		}
		base._worldPacket.WriteUInt32(totalDataSize);
		foreach (HotfixRecord hotfix2 in this.Hotfixes)
		{
			base._worldPacket.WriteBytes(hotfix2.HotfixContent);
		}
	}
}
