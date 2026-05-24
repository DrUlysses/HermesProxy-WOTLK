using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverOfferRewardMessage : ServerPacket
{
	public uint PortraitTurnIn;

	public uint PortraitGiver;

	public uint PortraitGiverMount;

	public uint PortraitGiverModelSceneID;

	public int QuestGiverCreatureID;

	public string QuestTitle = "";

	public string RewardText = "";

	public string PortraitGiverText = "";

	public string PortraitGiverName = "";

	public string PortraitTurnInText = "";

	public string PortraitTurnInName = "";

	public QuestGiverOfferReward QuestData = new QuestGiverOfferReward();

	public int QuestPackageID;

	public QuestGiverOfferRewardMessage()
		: base(Opcode.SMSG_QUEST_GIVER_OFFER_REWARD_MESSAGE)
	{
	}

	protected override void Write()
	{
		this.QuestData.Write(base._worldPacket);
		base._worldPacket.WriteInt32(this.QuestPackageID);
		base._worldPacket.WriteInt32((int)this.PortraitGiver);
		base._worldPacket.WriteInt32((int)this.PortraitGiverMount);
		base._worldPacket.WriteInt32((int)this.PortraitGiverModelSceneID);
		base._worldPacket.WriteInt32((int)this.PortraitTurnIn);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteInt32(this.QuestGiverCreatureID);
			base._worldPacket.WriteUInt32(0u);
		}
		base._worldPacket.WriteBits(this.QuestTitle.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.RewardText.GetByteCount(), 12);
		base._worldPacket.WriteBits(this.PortraitGiverText.GetByteCount(), 10);
		base._worldPacket.WriteBits(this.PortraitGiverName.GetByteCount(), 8);
		base._worldPacket.WriteBits(this.PortraitTurnInText.GetByteCount(), 10);
		base._worldPacket.WriteBits(this.PortraitTurnInName.GetByteCount(), 8);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.QuestTitle);
		base._worldPacket.WriteString(this.RewardText);
		base._worldPacket.WriteString(this.PortraitGiverText);
		base._worldPacket.WriteString(this.PortraitGiverName);
		base._worldPacket.WriteString(this.PortraitTurnInText);
		base._worldPacket.WriteString(this.PortraitTurnInName);
	}
}
