using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class NewWorld : ServerPacket
{
	public uint MapID;

	public uint Reason;

	public Vector3 Position = default(Vector3);

	public float Orientation;

	public Vector3 MovementOffset;

	public NewWorld()
		: base(Opcode.SMSG_NEW_WORLD)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteVector3(this.Position);
		base._worldPacket.WriteFloat(this.Orientation);
		base._worldPacket.WriteUInt32(this.Reason);
		base._worldPacket.WriteVector3(this.MovementOffset);
	}
}
