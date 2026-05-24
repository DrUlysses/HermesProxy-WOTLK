using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QuestConfirmAccept : ServerPacket
{
	public WowGuid128 InitiatedBy;

	public uint QuestID;

	public string QuestTitle;

	public QuestConfirmAccept()
		: base(Opcode.SMSG_QUEST_CONFIRM_ACCEPT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WritePackedGuid128(this.InitiatedBy);
		base._worldPacket.WriteBits(this.QuestTitle.GetByteCount(), 10);
		base._worldPacket.WriteString(this.QuestTitle);
	}
}
