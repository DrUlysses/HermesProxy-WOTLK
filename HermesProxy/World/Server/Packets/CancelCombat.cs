using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CancelCombat : ServerPacket
{
	public CancelCombat()
		: base(Opcode.SMSG_CANCEL_COMBAT)
	{
	}

	protected override void Write()
	{
	}
}
