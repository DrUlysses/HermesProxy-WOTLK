using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MoveSetSpeed : ServerPacket
{
	public WowGuid128 MoverGUID;

	public uint MoveCounter = 0u;

	public float Speed = 1f;

	public MoveSetSpeed(Opcode opcode)
		: base(opcode, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.MoverGUID);
		base._worldPacket.WriteUInt32(this.MoveCounter);
		base._worldPacket.WriteFloat(this.Speed);
	}
}
