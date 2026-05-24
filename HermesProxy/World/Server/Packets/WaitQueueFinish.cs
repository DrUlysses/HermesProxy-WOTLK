using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class WaitQueueFinish : ServerPacket
{
	public WaitQueueFinish()
		: base(Opcode.SMSG_WAIT_QUEUE_FINISH)
	{
	}

	protected override void Write()
	{
	}
}
