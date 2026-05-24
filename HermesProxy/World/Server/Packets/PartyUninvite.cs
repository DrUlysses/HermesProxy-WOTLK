namespace HermesProxy.World.Server.Packets;

internal class PartyUninvite : ClientPacket
{
	public byte PartyIndex;

	public WowGuid128 TargetGUID;

	public string Reason;

	public PartyUninvite(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		if (ModernVersion.ExpansionVersion == 3)
		{
            var hasPartyIndex = _worldPacket.HasBit();
            var reasonLen = _worldPacket.ReadBits<uint>(8);
            TargetGUID = _worldPacket.ReadPackedGuid128();
            if (hasPartyIndex)
                PartyIndex = _worldPacket.ReadUInt8();
            Reason = _worldPacket.ReadString(reasonLen);
        }
		else
		{
            PartyIndex = _worldPacket.ReadUInt8();
            TargetGUID = _worldPacket.ReadPackedGuid128();
            var reasonLen = _worldPacket.ReadBits<byte>(8);
            Reason = _worldPacket.ReadString(reasonLen);
        }
	}
}
