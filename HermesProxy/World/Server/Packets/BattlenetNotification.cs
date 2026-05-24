using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class BattlenetNotification : ServerPacket
{
	public MethodCall Method;

	public ByteBuffer Data = new ByteBuffer();

	public BattlenetNotification()
		: base(Opcode.SMSG_BATTLENET_NOTIFICATION)
	{
	}

	protected override void Write()
	{
		this.Method.Write(base._worldPacket);
		base._worldPacket.WriteUInt32(this.Data.GetSize());
		base._worldPacket.WriteBytes(this.Data);
	}
}
