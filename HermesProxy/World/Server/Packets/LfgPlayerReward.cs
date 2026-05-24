using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LfgPlayerRewardItem
{
	public uint ItemID;
	public uint Quantity;
	public int BonusCurrency;
	public bool IsCurrency;
}

public class LfgPlayerReward : ServerPacket
{
	public uint QueuedSlot;
	public uint ActualSlot;
	public int RewardMoney;
	public int AddedXP;
	public readonly List<LfgPlayerRewardItem> Rewards = new();

	public LfgPlayerReward()
		: base(Opcode.SMSG_LFG_PLAYER_REWARD)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(QueuedSlot);
		_worldPacket.WriteUInt32(ActualSlot);
		_worldPacket.WriteInt32(RewardMoney);
		_worldPacket.WriteInt32(AddedXP);
		_worldPacket.WriteUInt32((uint)Rewards.Count);
		foreach (var reward in Rewards)
		{
			_worldPacket.WriteUInt32(reward.ItemID);
			_worldPacket.WriteUInt32(reward.Quantity);
			_worldPacket.WriteInt32(reward.BonusCurrency);
			_worldPacket.WriteBit(reward.IsCurrency);
			_worldPacket.FlushBits();
		}
	}
}
