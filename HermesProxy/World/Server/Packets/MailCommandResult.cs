using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MailCommandResult : ServerPacket
{
	public uint MailID;

	public MailActionType Command;

	public MailErrorType ErrorCode;

	public InventoryResult BagResult;

	public uint AttachID;

	public uint QtyInInventory;

	public MailCommandResult()
		: base(Opcode.SMSG_MAIL_COMMAND_RESULT)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt64(MailID);
		_worldPacket.WriteInt32((int)Command);
		_worldPacket.WriteInt32((int)ErrorCode);
		_worldPacket.WriteInt32((int)BagResult);
		_worldPacket.WriteUInt64(AttachID);
		_worldPacket.WriteInt32((int)QtyInInventory);
	}
}
