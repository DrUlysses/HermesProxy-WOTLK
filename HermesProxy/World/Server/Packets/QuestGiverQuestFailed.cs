using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QuestGiverQuestFailed : ServerPacket
{
	public uint QuestID;

	public InventoryResult Reason;

	public QuestGiverQuestFailed()
		: base(Opcode.SMSG_QUEST_GIVER_QUEST_FAILED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WriteUInt32((uint)this.Reason);
	}
}
