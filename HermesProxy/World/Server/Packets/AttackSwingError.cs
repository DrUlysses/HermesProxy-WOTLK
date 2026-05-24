using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AttackSwingError : ServerPacket
{
	public AttackSwingErr Reason;

	public AttackSwingError()
		: base(Opcode.SMSG_ATTACK_SWING_ERROR)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits((uint)this.Reason, 3);
		base._worldPacket.FlushBits();
	}
}
