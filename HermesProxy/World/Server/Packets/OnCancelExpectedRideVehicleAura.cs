using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class OnCancelExpectedRideVehicleAura : ServerPacket
{
	public OnCancelExpectedRideVehicleAura()
		: base(Opcode.SMSG_ON_CANCEL_EXPECTED_RIDE_VEHICLE_AURA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
	}
}
