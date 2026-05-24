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
		data.WriteUInt8(Mask);
		data.WriteInt32(RewardMoney);
		data.WriteInt32(RewardXP);
		data.WriteUInt32((uint)(Items?.Count ?? 0));
		data.WriteUInt32((uint)(Currency?.Count ?? 0));
		data.WriteUInt32((uint)(BonusCurrency?.Count ?? 0));
		if (Items != null)
			foreach (var item in Items)
			{
				data.WriteInt32(item.ItemID);
				data.WriteInt32(item.Quantity);
			}
		if (Currency != null)
			foreach (var cur in Currency)
			{
				data.WriteInt32(cur.CurrencyID);
				data.WriteInt32(cur.Quantity);
			}
		if (BonusCurrency != null)
			foreach (var cur in BonusCurrency)
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
		data.WriteUInt32(Slot);
		data.WriteInt32(CompletionQuantity);
		data.WriteInt32(CompletionLimit);
		data.WriteInt32(CompletionCurrencyID);
		data.WriteInt32(SpecificQuantity);
		data.WriteInt32(SpecificLimit);
		data.WriteInt32(OverallQuantity);
		data.WriteInt32(OverallLimit);
		data.WriteInt32(PurseWeeklyQuantity);
		data.WriteInt32(PurseWeeklyLimit);
		data.WriteInt32(PurseQuantity);
		data.WriteInt32(PurseLimit);
		data.WriteInt32(Quantity);
		data.WriteUInt32(CompletedMask);
		data.WriteUInt32(EncounterMask);
		data.WriteUInt32(0); // ShortageReward count
		data.WriteBit(FirstReward);
		data.WriteBit(ShortageEligible);
		data.FlushBits();
		Rewards.Write(data);
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

	protected override void Write()
	{
		_worldPacket.WriteUInt32((uint)Dungeons.Count);
		// Write BlackList
		var hasGuid = BlackList.PlayerGuid != null;
		_worldPacket.WriteBit(hasGuid);
		_worldPacket.WriteUInt32((uint)(BlackList.Slots?.Count ?? 0));
		if (hasGuid)
			_worldPacket.WritePackedGuid128(BlackList.PlayerGuid);
		if (BlackList.Slots != null)
			foreach (var slot in BlackList.Slots)
			{
				_worldPacket.WriteUInt32(slot.Slot);
				_worldPacket.WriteUInt32(slot.Reason);
				_worldPacket.WriteInt32(slot.SubReason1);
				_worldPacket.WriteInt32(slot.SubReason2);
				_worldPacket.WriteUInt32(slot.SoftLock);
			}
		// Write Dungeons
		foreach (var dungeon in Dungeons)
		{
			dungeon.Write(_worldPacket);
		}
	}
}
