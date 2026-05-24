using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class Pong : ServerPacket
{
	private uint Serial;

	public Pong(uint serial)
		: base(Opcode.SMSG_PONG)
	{
		this.Serial = serial;
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Serial);
	}
}
