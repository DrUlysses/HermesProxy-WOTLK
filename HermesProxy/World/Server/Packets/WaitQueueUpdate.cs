using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class WaitQueueUpdate : ServerPacket
{
	public readonly AuthWaitInfo WaitInfo = new();

	public WaitQueueUpdate()
		: base(Opcode.SMSG_WAIT_QUEUE_UPDATE)
	{
	}

	protected override void Write()
	{
		WaitInfo.Write(_worldPacket);
	}
}
