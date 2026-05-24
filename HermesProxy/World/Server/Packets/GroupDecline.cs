using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class GroupDecline : ServerPacket
{
	public string Name;

	public GroupDecline()
		: base(Opcode.SMSG_GROUP_DECLINE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Name);
	}
}
