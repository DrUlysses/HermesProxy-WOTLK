using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class FishNotHooked : ServerPacket
{
	public FishNotHooked()
		: base(Opcode.SMSG_FISH_NOT_HOOKED)
	{
	}

	protected override void Write()
	{
	}
}
