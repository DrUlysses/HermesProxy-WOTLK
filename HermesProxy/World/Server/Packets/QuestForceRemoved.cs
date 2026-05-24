using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestForceRemoved : ServerPacket
{
	public uint QuestID;

	public QuestForceRemoved(uint questId)
		: base(Opcode.SMSG_QUEST_FORCE_REMOVED)
	{
		QuestID = questId;
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32((int)QuestID);
	}
}
