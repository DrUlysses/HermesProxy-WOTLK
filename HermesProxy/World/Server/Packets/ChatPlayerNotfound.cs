using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ChatPlayerNotfound : ServerPacket
{
	public string Name;

	public ChatPlayerNotfound()
		: base(Opcode.SMSG_CHAT_PLAYER_NOTFOUND)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
		base._worldPacket.WriteString(this.Name);
	}
}
