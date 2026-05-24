using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QuestUpdateAddCreditSimple : ServerPacket
{
	public uint QuestID;

	public int ObjectID;

	public QuestObjectiveType ObjectiveType;

	public QuestUpdateAddCreditSimple()
		: base(Opcode.SMSG_QUEST_UPDATE_ADD_CREDIT_SIMPLE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WriteInt32(this.ObjectID);
		base._worldPacket.WriteUInt8((byte)this.ObjectiveType);
	}
}
