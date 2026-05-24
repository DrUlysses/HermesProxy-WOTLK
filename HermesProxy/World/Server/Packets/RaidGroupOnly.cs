using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class RaidGroupOnly : ServerPacket
{
	public int Delay;

	public RaidGroupReason Reason;

	public RaidGroupOnly()
		: base(Opcode.SMSG_RAID_GROUP_ONLY)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Delay);
		base._worldPacket.WriteUInt32((uint)this.Reason);
	}
}
