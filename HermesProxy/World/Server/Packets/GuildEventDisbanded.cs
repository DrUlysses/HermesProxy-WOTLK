using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventDisbanded : ServerPacket
{
	public GuildEventDisbanded()
		: base(Opcode.SMSG_GUILD_EVENT_DISBANDED)
	{
	}

	protected override void Write()
	{
	}
}
