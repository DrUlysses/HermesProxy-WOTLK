using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SAttackStop : ServerPacket
{
	public WowGuid128 Attacker;

	public WowGuid128 Victim;

	public bool NowDead;

	public SAttackStop()
		: base(Opcode.SMSG_ATTACK_STOP, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Attacker ?? WowGuid128.Empty);
		base._worldPacket.WritePackedGuid128(this.Victim ?? WowGuid128.Empty);
		base._worldPacket.WriteBit(this.NowDead);
		base._worldPacket.FlushBits();
	}
}
