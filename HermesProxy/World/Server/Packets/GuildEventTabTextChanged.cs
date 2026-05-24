using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventTabTextChanged : ServerPacket
{
	public int Tab;

	public GuildEventTabTextChanged()
		: base(Opcode.SMSG_GUILD_EVENT_TAB_TEXT_CHANGED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Tab);
	}
}
