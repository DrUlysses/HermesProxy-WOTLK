using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MoveSetFlag : ServerPacket
{
	public WowGuid128 MoverGUID;

	public uint MoveCounter = 0u;

	public MoveSetFlag(Opcode opcode)
		: base(opcode, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(MoverGUID);
		_worldPacket.WriteUInt32(MoveCounter);
	}
}
