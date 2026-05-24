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

	public readonly string PortraitGiverText = "";

	public readonly string PortraitGiverName = "";

	public readonly string PortraitTurnInText = "";

	public readonly string PortraitTurnInName = "";

	public readonly QuestGiverOfferReward QuestData = new();

	public int QuestPackageID;

	public QuestGiverOfferRewardMessage()
		: base(Opcode.SMSG_QUEST_GIVER_OFFER_REWARD_MESSAGE)
	{
	}

	protected override void Write()
	{
		QuestData.Write(_worldPacket);
		_worldPacket.WriteInt32(QuestPackageID);
		_worldPacket.WriteInt32((int)PortraitGiver);
		_worldPacket.WriteInt32((int)PortraitGiverMount);
		_worldPacket.WriteInt32((int)PortraitGiverModelSceneID);
		_worldPacket.WriteInt32((int)PortraitTurnIn);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteInt32(QuestGiverCreatureID);
			_worldPacket.WriteUInt32(0u);
		}
		_worldPacket.WriteBits(QuestTitle.GetByteCount(), 9);
		_worldPacket.WriteBits(RewardText.GetByteCount(), 12);
		_worldPacket.WriteBits(PortraitGiverText.GetByteCount(), 10);
		_worldPacket.WriteBits(PortraitGiverName.GetByteCount(), 8);
		_worldPacket.WriteBits(PortraitTurnInText.GetByteCount(), 10);
		_worldPacket.WriteBits(PortraitTurnInName.GetByteCount(), 8);
		_worldPacket.FlushBits();
		_worldPacket.WriteString(QuestTitle);
		_worldPacket.WriteString(RewardText);
		_worldPacket.WriteString(PortraitGiverText);
		_worldPacket.WriteString(PortraitGiverName);
		_worldPacket.WriteString(PortraitTurnInText);
		_worldPacket.WriteString(PortraitTurnInName);
	}
}
