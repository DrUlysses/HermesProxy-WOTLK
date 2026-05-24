namespace HermesProxy.World.Server.Packets;

public class RequestVehicleSwitchSeat : ClientPacket
{
	public WowGuid128 Vehicle;
	public byte SeatIndex;

	public RequestVehicleSwitchSeat(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Vehicle = _worldPacket.ReadPackedGuid128();
		SeatIndex = _worldPacket.ReadUInt8();
	}
}
