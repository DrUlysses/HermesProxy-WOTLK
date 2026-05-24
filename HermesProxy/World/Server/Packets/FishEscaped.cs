using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class FishEscaped : ServerPacket
{
	public FishEscaped()
		: base(Opcode.SMSG_FISH_ESCAPED)
	{
	}

	protected override void Write()
	{
	}
}
