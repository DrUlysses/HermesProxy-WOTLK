using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DuelComplete : ServerPacket
{
	public bool Started;

	public DuelComplete()
		: base(Opcode.SMSG_DUEL_COMPLETE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Started);
		base._worldPacket.FlushBits();
	}
}
