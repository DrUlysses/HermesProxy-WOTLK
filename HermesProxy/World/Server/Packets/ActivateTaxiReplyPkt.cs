using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ActivateTaxiReplyPkt : ServerPacket
{
	public ActivateTaxiReply Reply;

	public ActivateTaxiReplyPkt()
		: base(Opcode.SMSG_ACTIVATE_TAXI_REPLY)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Reply, 4);
		base._worldPacket.FlushBits();
	}
}
