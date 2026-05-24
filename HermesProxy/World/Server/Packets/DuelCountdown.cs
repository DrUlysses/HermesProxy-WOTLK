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
		_worldPacket.WriteUInt32(Countdown);
	}
}
