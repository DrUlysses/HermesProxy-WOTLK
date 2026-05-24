using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverQuestComplete : ServerPacket
{
	public uint QuestID;

	public uint XPReward;

	public long MoneyReward;

	public uint SkillLineIDReward;

	public uint NumSkillUpsReward;

	public bool UseQuestReward;

	public bool LaunchGossip;

	public bool LaunchQuest = true;

	public bool HideChatMessage;

	public ItemInstance ItemReward = new ItemInstance();

	public QuestGiverQuestComplete()
		: base(Opcode.SMSG_QUEST_GIVER_QUEST_COMPLETE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WriteUInt32(this.XPReward);
		base._worldPacket.WriteInt64(this.MoneyReward);
		base._worldPacket.WriteUInt32(this.SkillLineIDReward);
		base._worldPacket.WriteUInt32(this.NumSkillUpsReward);
		base._worldPacket.WriteBit(this.UseQuestReward);
		base._worldPacket.WriteBit(this.LaunchGossip);
		base._worldPacket.WriteBit(this.LaunchQuest);
		base._worldPacket.WriteBit(this.HideChatMessage);
		this.ItemReward.Write(base._worldPacket);
	}
}
