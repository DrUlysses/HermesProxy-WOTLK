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
		_worldPacket.WritePackedGuid128(PlayerGuid);
		_worldPacket.WriteInt64(ServerTime);
		var accountTimes = AccountTimes;
		foreach (var accounttime in accountTimes)
		{
			_worldPacket.WriteInt64(accounttime);
		}
	}
}
