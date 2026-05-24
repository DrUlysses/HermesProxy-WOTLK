using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class NotifyReceivedMail : ServerPacket
{
	public float Delay;

	public NotifyReceivedMail()
		: base(Opcode.SMSG_NOTIFY_RECEIVED_MAIL)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteFloat(this.Delay);
	}
}
