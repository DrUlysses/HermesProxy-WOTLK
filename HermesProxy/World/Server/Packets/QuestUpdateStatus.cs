using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QuestUpdateStatus : ServerPacket
{
	public uint QuestID;

	public QuestUpdateStatus(Opcode opcode)
		: base(opcode)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.QuestID);
	}
}
