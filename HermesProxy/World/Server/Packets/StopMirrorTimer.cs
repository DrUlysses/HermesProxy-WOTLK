using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class StopMirrorTimer : ServerPacket
{
	public MirrorTimerType Timer;

	public StopMirrorTimer()
		: base(Opcode.SMSG_STOP_MIRROR_TIMER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32((int)this.Timer);
	}
}
