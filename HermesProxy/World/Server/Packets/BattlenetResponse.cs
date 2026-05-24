using Framework.Constants;
using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class BattlenetResponse : ServerPacket
{
	public BattlenetRpcErrorCode Status = BattlenetRpcErrorCode.Ok;

	public MethodCall Method;

	public ByteBuffer Data = new();

	public BattlenetResponse()
		: base(Opcode.SMSG_BATTLENET_RESPONSE)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32((uint)Status);
		Method.Write(_worldPacket);
		_worldPacket.WriteUInt32(Data.GetSize());
		_worldPacket.WriteBytes(Data);
	}
}
