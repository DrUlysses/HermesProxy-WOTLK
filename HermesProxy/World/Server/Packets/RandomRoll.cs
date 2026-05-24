using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class RandomRoll : ServerPacket
{
	public WowGuid128 Roller;

	public WowGuid128 RollerWowAccount;

	public int Min;

	public int Max;

	public int Result;

	public RandomRoll()
		: base(Opcode.SMSG_RANDOM_ROLL)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Roller);
		base._worldPacket.WritePackedGuid128(this.RollerWowAccount);
		base._worldPacket.WriteInt32(this.Min);
		base._worldPacket.WriteInt32(this.Max);
		base._worldPacket.WriteInt32(this.Result);
	}
}
