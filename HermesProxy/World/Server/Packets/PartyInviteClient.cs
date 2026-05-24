namespace HermesProxy.World.Server.Packets;

internal class PartyInviteClient : ClientPacket
{
	public byte PartyIndex;

	public uint VirtualRealmAddress;

	public WowGuid128 TargetGUID;

	public string TargetName;

	public string TargetRealm;

	public PartyInviteClient(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		PartyIndex = _worldPacket.ReadUInt8();
		var targetNameLen = _worldPacket.ReadBits<uint>(9);
		var targetRealmLen = _worldPacket.ReadBits<uint>(9);
		VirtualRealmAddress = _worldPacket.ReadUInt32();
		TargetGUID = _worldPacket.ReadPackedGuid128();
		TargetName = _worldPacket.ReadString(targetNameLen);
		TargetRealm = _worldPacket.ReadString(targetRealmLen);
	}
}
