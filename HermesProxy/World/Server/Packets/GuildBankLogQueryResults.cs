using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildBankLogQueryResults : ServerPacket
{
	public int Tab;

	public List<GuildBankLogEntry> Entry;

	public ulong? WeeklyBonusMoney;

	public GuildBankLogQueryResults()
		: base(Opcode.SMSG_GUILD_BANK_LOG_QUERY_RESULTS)
	{
		Entry = new List<GuildBankLogEntry>();
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Tab);
		_worldPacket.WriteInt32(Entry.Count);
		_worldPacket.WriteBit(WeeklyBonusMoney.HasValue);
		_worldPacket.FlushBits();
		foreach (GuildBankLogEntry logEntry in Entry)
		{
			_worldPacket.WritePackedGuid128(logEntry.PlayerGUID);
			_worldPacket.WriteUInt32(logEntry.TimeOffset);
			_worldPacket.WriteInt8(logEntry.EntryType);
			_worldPacket.WriteBit(logEntry.Money.HasValue);
			_worldPacket.WriteBit(logEntry.ItemID.HasValue);
			_worldPacket.WriteBit(logEntry.Count.HasValue);
			_worldPacket.WriteBit(logEntry.OtherTab.HasValue);
			_worldPacket.FlushBits();
			if (logEntry.Money.HasValue)
			{
				_worldPacket.WriteUInt64(logEntry.Money.Value);
			}
			if (logEntry.ItemID.HasValue)
			{
				_worldPacket.WriteInt32(logEntry.ItemID.Value);
			}
			if (logEntry.Count.HasValue)
			{
				_worldPacket.WriteInt32(logEntry.Count.Value);
			}
			if (logEntry.OtherTab.HasValue)
			{
				_worldPacket.WriteInt8(logEntry.OtherTab.Value);
			}
		}
		if (WeeklyBonusMoney.HasValue)
		{
			_worldPacket.WriteUInt64(WeeklyBonusMoney.Value);
		}
	}
}
