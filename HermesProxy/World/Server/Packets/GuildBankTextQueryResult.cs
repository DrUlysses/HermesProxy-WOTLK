using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildBankTextQueryResult : ServerPacket
{
	public int Tab;

	public string Text;

	public GuildBankTextQueryResult()
		: base(Opcode.SMSG_GUILD_BANK_TEXT_QUERY_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Tab);
		base._worldPacket.WriteBits(this.Text.GetByteCount(), 14);
		base._worldPacket.WriteString(this.Text);
	}
}
