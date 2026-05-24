using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GMTicketCaseStatus : ServerPacket
{
	public GMTicketCaseStatus()
		: base(Opcode.SMSG_GM_TICKET_CASE_STATUS)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(0); // Cases.size() = 0, no open tickets
	}
}
