using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class GroupNewLeader : ServerPacket
{
	public sbyte PartyIndex;

	public string Name;

	public GroupNewLeader()
		: base(Opcode.SMSG_GROUP_NEW_LEADER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8(this.PartyIndex);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
		base._worldPacket.WriteString(this.Name);
	}
}
