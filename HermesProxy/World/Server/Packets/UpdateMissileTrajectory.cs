namespace HermesProxy.World.Server.Packets;

public class UpdateMissileTrajectory : ClientPacket
{
	public WowGuid128 Guid;
	public WowGuid128 CastID;
	public ushort MoveMsgID;
	public int SpellID;
	public float Pitch;
	public float Speed;
	public float FirePosX;
	public float FirePosY;
	public float FirePosZ;
	public float ImpactPosX;
	public float ImpactPosY;
	public float ImpactPosZ;

	public UpdateMissileTrajectory(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Guid = _worldPacket.ReadPackedGuid128();
		CastID = _worldPacket.ReadPackedGuid128();
		MoveMsgID = _worldPacket.ReadUInt16();
		SpellID = _worldPacket.ReadInt32();
		Pitch = _worldPacket.ReadFloat();
		Speed = _worldPacket.ReadFloat();
		FirePosX = _worldPacket.ReadFloat();
		FirePosY = _worldPacket.ReadFloat();
		FirePosZ = _worldPacket.ReadFloat();
		ImpactPosX = _worldPacket.ReadFloat();
		ImpactPosY = _worldPacket.ReadFloat();
		ImpactPosZ = _worldPacket.ReadFloat();
		// Optional MovementInfo follows but we skip it
	}
}
