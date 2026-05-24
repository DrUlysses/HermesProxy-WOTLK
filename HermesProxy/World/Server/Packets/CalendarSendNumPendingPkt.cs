using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CalendarSendNumPendingPkt : ServerPacket
{
	public uint NumPending;

	public CalendarSendNumPendingPkt()
		: base(Opcode.SMSG_CALENDAR_SEND_NUM_PENDING)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(NumPending);
	}
}