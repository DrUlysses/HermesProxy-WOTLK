using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ChatServerMessage : ServerPacket
{
	public int MessageID;

	public string StringParam = "";

	public ChatServerMessage()
		: base(Opcode.SMSG_CHAT_SERVER_MESSAGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.MessageID);
		base._worldPacket.WriteBits(this.StringParam.GetByteCount(), 11);
		base._worldPacket.WriteString(this.StringParam);
	}
}
