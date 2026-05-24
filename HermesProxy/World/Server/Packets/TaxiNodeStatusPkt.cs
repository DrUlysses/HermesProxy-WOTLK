using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class TaxiNodeStatusPkt : ServerPacket
{
	public WowGuid128 FlightMaster;

	public TaxiNodeStatus Status;

	public TaxiNodeStatusPkt()
		: base(Opcode.SMSG_TAXI_NODE_STATUS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.FlightMaster);
		base._worldPacket.WriteBits(this.Status, 2);
		base._worldPacket.FlushBits();
	}
}
