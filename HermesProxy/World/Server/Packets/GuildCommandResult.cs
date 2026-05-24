using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildCommandResult : ServerPacket
{
	public string Name;

	public GuildCommandError Result;

	public GuildCommandType Command;

	public GuildCommandResult()
		: base(Opcode.SMSG_GUILD_COMMAND_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.Result);
		base._worldPacket.WriteUInt32((uint)this.Command);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 8);
		base._worldPacket.WriteString(this.Name);
	}
}
