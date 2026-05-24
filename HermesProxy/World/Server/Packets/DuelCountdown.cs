using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DuelCountdown : ServerPacket
{
	public uint Countdown;

	public DuelCountdown()
		: base(Opcode.SMSG_DUEL_COUNTDOWN)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Countdown);
	}
}
