using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ArenaTeamEvent : ServerPacket
{
	public ArenaTeamEventModern Event;

	public string Param1 = "";

	public string Param2 = "";

	public string Param3 = "";

	public ArenaTeamEvent()
		: base(Opcode.SMSG_ARENA_TEAM_EVENT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8((byte)this.Event);
		base._worldPacket.WriteBits(this.Param1.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.Param2.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.Param3.GetByteCount(), 9);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Param1);
		base._worldPacket.WriteString(this.Param2);
		base._worldPacket.WriteString(this.Param3);
	}
}
