using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ZoneUnderAttack : ServerPacket
{
	public int AreaID;

	public ZoneUnderAttack()
		: base(Opcode.SMSG_ZONE_UNDER_ATTACK)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.AreaID);
	}
}
