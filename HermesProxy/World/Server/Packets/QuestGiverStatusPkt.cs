using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverStatusPkt : ServerPacket
{
	public QuestGiverInfo QuestGiver;

	public QuestGiverStatusPkt()
		: base(Opcode.SMSG_QUEST_GIVER_STATUS, ConnectionType.Instance)
	{
		this.QuestGiver = new QuestGiverInfo();
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.QuestGiver.Guid);
		base._worldPacket.WriteUInt64((ulong)this.QuestGiver.Status);
	}
}
