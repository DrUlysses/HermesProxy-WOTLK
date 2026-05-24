using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ServerTimeOffset : ServerPacket
{
	public long Time;

	public ServerTimeOffset()
		: base(Opcode.SMSG_SERVER_TIME_OFFSET)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt64(this.Time);
	}
}
