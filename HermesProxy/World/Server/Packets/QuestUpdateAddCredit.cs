using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestUpdateAddCredit : ServerPacket
{
	public WowGuid128 VictimGUID;

	public int ObjectID;

	public uint QuestID;

	public ushort Count;

	public ushort Required;

	public QuestObjectiveType ObjectiveType;

	public QuestUpdateAddCredit()
		: base(Opcode.SMSG_QUEST_UPDATE_ADD_CREDIT, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.VictimGUID);
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WriteInt32(this.ObjectID);
		base._worldPacket.WriteUInt16(this.Count);
		base._worldPacket.WriteUInt16(this.Required);
		base._worldPacket.WriteUInt8((byte)this.ObjectiveType);
	}
}
