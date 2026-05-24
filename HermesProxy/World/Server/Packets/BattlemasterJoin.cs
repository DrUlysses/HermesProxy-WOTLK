namespace HermesProxy.World.Server.Packets;

internal class BattlemasterJoin : ClientPacket
{
	public uint BattlefieldListId;

	public byte Roles;

	public readonly int[] BlacklistMap = new int[2];

	public WowGuid128 BattlemasterGuid;

	public int Verification;

	public int BattlefieldInstanceID;

	public bool JoinAsGroup;

	public BattlemasterJoin(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var queueId = _worldPacket.ReadInt64();
		BattlefieldListId = (uint)(queueId & -2238289014803136513L);
		Roles = _worldPacket.ReadUInt8();
		BlacklistMap[0] = _worldPacket.ReadInt32();
		BlacklistMap[1] = _worldPacket.ReadInt32();
		BattlemasterGuid = _worldPacket.ReadPackedGuid128();
		Verification = _worldPacket.ReadInt32();
		BattlefieldInstanceID = _worldPacket.ReadInt32();
		JoinAsGroup = _worldPacket.HasBit();
	}
}
