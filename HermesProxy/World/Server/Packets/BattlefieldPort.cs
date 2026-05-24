namespace HermesProxy.World.Server.Packets;

internal class BattlefieldPort : ClientPacket
{
	public readonly RideTicket Ticket = new();

	public bool AcceptedInvite;

	public BattlefieldPort(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Ticket.Read(_worldPacket);
		AcceptedInvite = _worldPacket.HasBit();
	}
}
