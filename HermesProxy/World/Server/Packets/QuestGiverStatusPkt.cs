using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverStatusPkt : ServerPacket
{
	public readonly QuestGiverInfo QuestGiver;

	public QuestGiverStatusPkt()
		: base(Opcode.SMSG_QUEST_GIVER_STATUS, ConnectionType.Instance)
	{
		QuestGiver = new QuestGiverInfo();
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(QuestGiver.Guid);
		_worldPacket.WriteUInt64((ulong)QuestGiver.Status);
	}
}
