using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ChangeRealmTicketResponse : ServerPacket
{
	public uint Token;

	public bool Allow = true;

	public ByteBuffer Ticket = new ByteBuffer();

	public ChangeRealmTicketResponse()
		: base(Opcode.SMSG_CHANGE_REALM_TICKET_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Token);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.WriteUInt32(this.Ticket.GetSize());
		base._worldPacket.WriteBytes(this.Ticket);
	}
}
