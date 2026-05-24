namespace HermesProxy.World.Server.Packets;

internal class PartyInviteResponse : ClientPacket
{
	public byte? PartyIndex;

	public bool Accept;

	public byte? RolesDesired;

	public PartyInviteResponse(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		bool hasPartyIndex = _worldPacket.HasBit();
		Accept = _worldPacket.HasBit();
		bool hasRolesDesired = _worldPacket.HasBit();

		if (hasPartyIndex)
			PartyIndex = _worldPacket.ReadUInt8();

		if (hasRolesDesired)
			RolesDesired = _worldPacket.ReadUInt8();
	}
}
