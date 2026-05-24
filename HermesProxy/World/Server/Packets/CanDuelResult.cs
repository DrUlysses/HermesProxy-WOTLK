using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CanDuelResult : ServerPacket
{
	public WowGuid128 TargetGUID;

	public bool Result;

	public CanDuelResult()
		: base(Opcode.SMSG_CAN_DUEL_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WriteBit(this.Result);
		base._worldPacket.FlushBits();
	}
}
