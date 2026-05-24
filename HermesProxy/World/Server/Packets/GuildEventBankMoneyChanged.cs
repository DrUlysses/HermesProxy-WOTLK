using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventBankMoneyChanged : ServerPacket
{
	public ulong Money;

	public GuildEventBankMoneyChanged()
		: base(Opcode.SMSG_GUILD_EVENT_BANK_MONEY_CHANGED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt64(this.Money);
	}
}
