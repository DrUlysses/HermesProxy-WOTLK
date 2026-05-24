using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TransferAborted : ServerPacket
{
	public uint MapID;

	public byte Arg;

	public int MapDifficultyXConditionID = -6;

	public TransferAbortReasonModern Reason;

	public TransferAborted()
		: base(Opcode.SMSG_TRANSFER_ABORTED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteUInt8(this.Arg);
		base._worldPacket.WriteInt32(this.MapDifficultyXConditionID);
		base._worldPacket.WriteBits(this.Reason, 6);
		base._worldPacket.FlushBits();
	}
}
