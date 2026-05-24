namespace HermesProxy.World.Server.Packets;

public class DfGetSystemInfoPkt : ClientPacket
{
	public bool Player;

	public DfGetSystemInfoPkt(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Player = _worldPacket.HasBit();
		// PartyIndex optional - skip
	}
}
