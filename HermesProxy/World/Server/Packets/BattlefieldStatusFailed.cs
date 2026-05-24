using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BattlefieldStatusFailed : ServerPacket
{
	public RideTicket Ticket = new RideTicket();

	public byte Unk;

	public ulong BattlefieldListId;

	public WowGuid128 ClientID = WowGuid128.Empty;

	public int Reason;

	public BattlefieldStatusFailed()
		: base(Opcode.SMSG_BATTLEFIELD_STATUS_FAILED)
	{
	}

	protected override void Write()
	{
		this.Ticket.Write(base._worldPacket);
		ulong queueID = this.BattlefieldListId | 0x1F10000000000000L;
		base._worldPacket.WriteUInt64(queueID);
		base._worldPacket.WriteInt32(this.Reason);
		base._worldPacket.WritePackedGuid128(this.ClientID);
	}
}
