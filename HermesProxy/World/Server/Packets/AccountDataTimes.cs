using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AccountDataTimes : ServerPacket
{
	public WowGuid128 PlayerGuid;

	public long ServerTime;

	public long[] AccountTimes;

	public AccountDataTimes()
		: base(Opcode.SMSG_ACCOUNT_DATA_TIMES)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PlayerGuid);
		base._worldPacket.WriteInt64(this.ServerTime);
		long[] accountTimes = this.AccountTimes;
		foreach (long accounttime in accountTimes)
		{
			base._worldPacket.WriteInt64(accounttime);
		}
	}
}
