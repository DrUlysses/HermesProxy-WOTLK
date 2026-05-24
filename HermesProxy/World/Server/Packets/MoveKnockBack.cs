using Framework.Constants;
using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class MoveKnockBack : ServerPacket
{
	public WowGuid128 MoverGUID;

	public uint MoveCounter;

	public Vector2 Direction;

	public float HorizontalSpeed;

	public float VerticalSpeed;

	public MoveKnockBack()
		: base(Opcode.SMSG_MOVE_KNOCK_BACK, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.MoverGUID);
		base._worldPacket.WriteUInt32(this.MoveCounter);
		base._worldPacket.WriteVector2(this.Direction);
		base._worldPacket.WriteFloat(this.HorizontalSpeed);
		base._worldPacket.WriteFloat(this.VerticalSpeed);
	}
}
