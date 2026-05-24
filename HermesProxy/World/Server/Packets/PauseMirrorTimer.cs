using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PauseMirrorTimer : ServerPacket
{
	public MirrorTimerType Timer;

	public bool Paused;

	public PauseMirrorTimer()
		: base(Opcode.SMSG_PAUSE_MIRROR_TIMER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32((int)this.Timer);
		base._worldPacket.WriteBit(this.Paused);
		base._worldPacket.FlushBits();
	}
}
