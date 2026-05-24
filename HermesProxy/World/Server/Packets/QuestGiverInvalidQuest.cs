using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QuestGiverInvalidQuest : ServerPacket
{
	public QuestFailedReasons Reason;

	public int ContributionRewardID;

	public bool SendErrorMessage = true;

	public string ReasonText = "";

	public QuestGiverInvalidQuest()
		: base(Opcode.SMSG_QUEST_GIVER_INVALID_QUEST)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.Reason);
		base._worldPacket.WriteInt32(this.ContributionRewardID);
		base._worldPacket.WriteBit(this.SendErrorMessage);
		base._worldPacket.WriteBits(this.ReasonText.GetByteCount(), 9);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.ReasonText);
	}
}
