using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GMTicketSystemStatus : ServerPacket
{
	public int Status;

	public GMTicketSystemStatus()
		: base(Opcode.SMSG_GM_TICKET_SYSTEM_STATUS)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Status);
	}
}
