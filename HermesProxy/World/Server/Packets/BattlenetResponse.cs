using Framework.Constants;
using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class BattlenetResponse : ServerPacket
{
	public BattlenetRpcErrorCode Status = BattlenetRpcErrorCode.Ok;

	public MethodCall Method;

	public ByteBuffer Data = new ByteBuffer();

	public BattlenetResponse()
		: base(Opcode.SMSG_BATTLENET_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.Status);
		this.Method.Write(base._worldPacket);
		base._worldPacket.WriteUInt32(this.Data.GetSize());
		base._worldPacket.WriteBytes(this.Data);
	}
}
