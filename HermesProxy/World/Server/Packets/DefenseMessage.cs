using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class DefenseMessage : ServerPacket
{
	public uint ZoneID;

	public string MessageText = "";

	public DefenseMessage()
		: base(Opcode.SMSG_DEFENSE_MESSAGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.ZoneID);
		base._worldPacket.WriteBits(this.MessageText.GetByteCount(), 12);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.MessageText);
	}
}
