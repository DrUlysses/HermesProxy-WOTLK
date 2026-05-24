using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class StartMirrorTimer : ServerPacket
{
	public MirrorTimerType Timer;

	public int Value;

	public int MaxValue;

	public int Scale;

	public int SpellID;

	public bool Paused;

	public StartMirrorTimer()
		: base(Opcode.SMSG_START_MIRROR_TIMER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32((int)this.Timer);
		base._worldPacket.WriteInt32(this.Value);
		base._worldPacket.WriteInt32(this.MaxValue);
		base._worldPacket.WriteInt32(this.Scale);
		base._worldPacket.WriteInt32(this.SpellID);
		base._worldPacket.WriteBit(this.Paused);
		base._worldPacket.FlushBits();
	}
}
