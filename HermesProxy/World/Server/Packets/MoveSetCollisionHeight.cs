using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class MoveSetCollisionHeight : ServerPacket
{
	public enum UpdateCollisionHeightReason : byte
	{
		Scale = 0,
		Mount = 1,
		Force = 2
	}

	public WowGuid128 MoverGUID;

	public uint SequenceIndex = 1u;

	public float Height = 1f;

	public float Scale = 1f;

	public UpdateCollisionHeightReason Reason;

	public uint MountDisplayID;

	public int ScaleDuration = 2000;

	public MoveSetCollisionHeight()
		: base(Opcode.SMSG_MOVE_SET_COLLISION_HEIGHT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.MoverGUID);
		base._worldPacket.WriteUInt32(this.SequenceIndex);
		base._worldPacket.WriteFloat(this.Height);
		base._worldPacket.WriteFloat(this.Scale);
		base._worldPacket.WriteByteEnum(this.Reason);
		base._worldPacket.WriteUInt32(this.MountDisplayID);
		base._worldPacket.WriteInt32(this.ScaleDuration);
	}
}
