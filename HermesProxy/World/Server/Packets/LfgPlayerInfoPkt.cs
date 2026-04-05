using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public struct LfgBlackListSlot
{
	public uint Slot;
	public uint Reason;
	public int SubReason1;
	public int SubReason2;
	public uint SoftLock;
}

public struct LfgBlackList
{
	public WowGuid128 PlayerGuid; // null = no guid
	public List<LfgBlackListSlot> Slots;
}

public struct LfgPlayerQuestRewardItem
{
	public int ItemID;
	public int Quantity;
}

public struct LfgPlayerQuestRewardCurrency
{
	public int CurrencyID;
	public int Quantity;
}

public struct LfgPlayerQuestReward
{
	public byte Mask;
	public int RewardMoney;
	public int RewardXP;
	public List<LfgPlayerQuestRewardItem> Items;
	public List<LfgPlayerQuestRewardCurrency> Currency;
	public List<LfgPlayerQuestRewardCurrency> BonusCurrency;

	public void Write(WorldPacket data)
	{
		data.WriteUInt8(this.Mask);
		data.WriteInt32(this.RewardMoney);
		data.WriteInt32(this.RewardXP);
		data.WriteUInt32((uint)(this.Items?.Count ?? 0));
		data.WriteUInt32((uint)(this.Currency?.Count ?? 0));
		data.WriteUInt32((uint)(this.BonusCurrency?.Count ?? 0));
		if (this.Items != null)
			foreach (var item in this.Items)
			{
				data.WriteInt32(item.ItemID);
				data.WriteInt32(item.Quantity);
			}
		if (this.Currency != null)
			foreach (var cur in this.Currency)
			{
				data.WriteInt32(cur.CurrencyID);
				data.WriteInt32(cur.Quantity);
			}
		if (this.BonusCurrency != null)
			foreach (var cur in this.BonusCurrency)
			{
				data.WriteInt32(cur.CurrencyID);
				data.WriteInt32(cur.Quantity);
			}
		// Optional fields: RewardSpellID, Unused1, Unused2, Honor — all absent
		data.WriteBit(false);
		data.WriteBit(false);
		data.WriteBit(false);
		data.WriteBit(false);
		data.FlushBits();
	}
}

public struct LfgPlayerDungeonInfo
{
	public uint Slot;
	public int CompletionQuantity;
	public int CompletionLimit;
	public int CompletionCurrencyID;
	public int SpecificQuantity;
	public int SpecificLimit;
	public int OverallQuantity;
	public int OverallLimit;
	public int PurseWeeklyQuantity;
	public int PurseWeeklyLimit;
	public int PurseQuantity;
	public int PurseLimit;
	public int Quantity;
	public uint CompletedMask;
	public uint EncounterMask;
	public bool FirstReward;
	public bool ShortageEligible;
	public LfgPlayerQuestReward Rewards;

	public void Write(WorldPacket data)
	{
		data.WriteUInt32(this.Slot);
		data.WriteInt32(this.CompletionQuantity);
		data.WriteInt32(this.CompletionLimit);
		data.WriteInt32(this.CompletionCurrencyID);
		data.WriteInt32(this.SpecificQuantity);
		data.WriteInt32(this.SpecificLimit);
		data.WriteInt32(this.OverallQuantity);
		data.WriteInt32(this.OverallLimit);
		data.WriteInt32(this.PurseWeeklyQuantity);
		data.WriteInt32(this.PurseWeeklyLimit);
		data.WriteInt32(this.PurseQuantity);
		data.WriteInt32(this.PurseLimit);
		data.WriteInt32(this.Quantity);
		data.WriteUInt32(this.CompletedMask);
		data.WriteUInt32(this.EncounterMask);
		data.WriteUInt32(0); // ShortageReward count
		data.WriteBit(this.FirstReward);
		data.WriteBit(this.ShortageEligible);
		data.FlushBits();
		this.Rewards.Write(data);
	}
}

public class LfgPlayerInfoPkt : ServerPacket
{
	public List<LfgPlayerDungeonInfo> Dungeons = new List<LfgPlayerDungeonInfo>();
	public LfgBlackList BlackList;

	public LfgPlayerInfoPkt()
		: base(Opcode.SMSG_LFG_PLAYER_INFO, ConnectionType.Instance)
	{
	}

	public override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.Dungeons.Count);
		// Write BlackList
		bool hasGuid = this.BlackList.PlayerGuid != null;
		base._worldPacket.WriteBit(hasGuid);
		base._worldPacket.WriteUInt32((uint)(this.BlackList.Slots?.Count ?? 0));
		if (hasGuid)
			base._worldPacket.WritePackedGuid128(this.BlackList.PlayerGuid);
		if (this.BlackList.Slots != null)
			foreach (var slot in this.BlackList.Slots)
			{
				base._worldPacket.WriteUInt32(slot.Slot);
				base._worldPacket.WriteUInt32(slot.Reason);
				base._worldPacket.WriteInt32(slot.SubReason1);
				base._worldPacket.WriteInt32(slot.SubReason2);
				base._worldPacket.WriteUInt32(slot.SoftLock);
			}
		// Write Dungeons
		foreach (var dungeon in this.Dungeons)
		{
			dungeon.Write(base._worldPacket);
		}
	}
}
