using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TurnInPetitionResult : ServerPacket
{
	public PetitionTurnResult Result = PetitionTurnResult.Ok;

	public TurnInPetitionResult()
		: base(Opcode.SMSG_TURN_IN_PETITION_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Result, 4);
		base._worldPacket.FlushBits();
	}
}
