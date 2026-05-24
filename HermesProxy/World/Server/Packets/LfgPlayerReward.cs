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
	public List<LfgPlayerRewardItem> Rewards = new List<LfgPlayerRewardItem>();

	public LfgPlayerReward()
		: base(Opcode.SMSG_LFG_PLAYER_REWARD)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.QueuedSlot);
		base._worldPacket.WriteUInt32(this.ActualSlot);
		base._worldPacket.WriteInt32(this.RewardMoney);
		base._worldPacket.WriteInt32(this.AddedXP);
		base._worldPacket.WriteUInt32((uint)this.Rewards.Count);
		foreach (LfgPlayerRewardItem reward in this.Rewards)
		{
			base._worldPacket.WriteUInt32(reward.ItemID);
			base._worldPacket.WriteUInt32(reward.Quantity);
			base._worldPacket.WriteInt32(reward.BonusCurrency);
			base._worldPacket.WriteBit(reward.IsCurrency);
			base._worldPacket.FlushBits();
		}
	}
}
