using Framework.Constants;
using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MoveTeleport : ServerPacket
{
	public Vector3 Position;

	public VehicleTeleport Vehicle;

	public uint MoveCounter;

	public WowGuid128 MoverGUID;

	public WowGuid128 TransportGUID;

	public float Orientation;

	public byte PreloadWorld;

	public MoveTeleport()
		: base(Opcode.SMSG_MOVE_TELEPORT, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.MoverGUID);
		base._worldPacket.WriteUInt32(this.MoveCounter);
		base._worldPacket.WriteVector3(this.Position);
		base._worldPacket.WriteFloat(this.Orientation);
		base._worldPacket.WriteUInt8(this.PreloadWorld);
		base._worldPacket.WriteBit(this.TransportGUID != null);
		base._worldPacket.WriteBit(this.Vehicle != null);
		base._worldPacket.FlushBits();
		if (this.Vehicle != null)
		{
			base._worldPacket.WriteInt8(this.Vehicle.VehicleSeatIndex);
			base._worldPacket.WriteBit(this.Vehicle.VehicleExitVoluntary);
			base._worldPacket.WriteBit(this.Vehicle.VehicleExitTeleport);
			base._worldPacket.FlushBits();
		}
		if (this.TransportGUID != null)
		{
			base._worldPacket.WritePackedGuid128(this.TransportGUID);
		}
	}
}
