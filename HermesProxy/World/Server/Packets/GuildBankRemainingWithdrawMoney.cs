using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildBankRemainingWithdrawMoney : ServerPacket
{
	public long RemainingWithdrawMoney;

	public GuildBankRemainingWithdrawMoney()
		: base(Opcode.SMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt64(this.RemainingWithdrawMoney);
	}
}
