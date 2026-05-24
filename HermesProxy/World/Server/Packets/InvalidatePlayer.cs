using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InvalidatePlayer : ServerPacket
{
	public WowGuid128 Guid;

	public InvalidatePlayer()
		: base(Opcode.SMSG_INVALIDATE_PLAYER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
	}
}
