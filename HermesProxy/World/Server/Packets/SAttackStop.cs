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
		_worldPacket.WritePackedGuid128(Attacker ?? WowGuid128.Empty);
		_worldPacket.WritePackedGuid128(Victim ?? WowGuid128.Empty);
		_worldPacket.WriteBit(NowDead);
		_worldPacket.FlushBits();
	}
}
