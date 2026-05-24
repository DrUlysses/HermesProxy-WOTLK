using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PlayedTime : ServerPacket
{
	public uint TotalTime;

	public uint LevelTime;

	public bool TriggerEvent;

	public PlayedTime()
		: base(Opcode.SMSG_PLAYED_TIME, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.TotalTime);
		base._worldPacket.WriteUInt32(this.LevelTime);
		base._worldPacket.WriteBit(this.TriggerEvent);
		base._worldPacket.FlushBits();
	}
}
