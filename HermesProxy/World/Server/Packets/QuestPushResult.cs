using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QuestPushResult : ServerPacket
{
	public WowGuid128 SenderGUID;

	public QuestPushReason Result;

	public QuestPushResult()
		: base(Opcode.SMSG_QUEST_PUSH_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.SenderGUID);
		base._worldPacket.WriteUInt8((byte)this.Result);
	}
}
