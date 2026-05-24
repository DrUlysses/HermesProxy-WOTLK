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
		this.Entry = new List<GuildBankLogEntry>();
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Tab);
		base._worldPacket.WriteInt32(this.Entry.Count);
		base._worldPacket.WriteBit(this.WeeklyBonusMoney.HasValue);
		base._worldPacket.FlushBits();
		foreach (GuildBankLogEntry logEntry in this.Entry)
		{
			base._worldPacket.WritePackedGuid128(logEntry.PlayerGUID);
			base._worldPacket.WriteUInt32(logEntry.TimeOffset);
			base._worldPacket.WriteInt8(logEntry.EntryType);
			base._worldPacket.WriteBit(logEntry.Money.HasValue);
			base._worldPacket.WriteBit(logEntry.ItemID.HasValue);
			base._worldPacket.WriteBit(logEntry.Count.HasValue);
			base._worldPacket.WriteBit(logEntry.OtherTab.HasValue);
			base._worldPacket.FlushBits();
			if (logEntry.Money.HasValue)
			{
				base._worldPacket.WriteUInt64(logEntry.Money.Value);
			}
			if (logEntry.ItemID.HasValue)
			{
				base._worldPacket.WriteInt32(logEntry.ItemID.Value);
			}
			if (logEntry.Count.HasValue)
			{
				base._worldPacket.WriteInt32(logEntry.Count.Value);
			}
			if (logEntry.OtherTab.HasValue)
			{
				base._worldPacket.WriteInt8(logEntry.OtherTab.Value);
			}
		}
		if (this.WeeklyBonusMoney.HasValue)
		{
			base._worldPacket.WriteUInt64(this.WeeklyBonusMoney.Value);
		}
	}
}
