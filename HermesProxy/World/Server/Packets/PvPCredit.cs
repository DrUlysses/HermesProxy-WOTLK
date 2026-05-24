using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PvPCredit : ServerPacket
{
	public int OriginalHonor;

	public int Honor;

	public WowGuid128 Target;

	public uint Rank;

	public PvPCredit()
		: base(Opcode.SMSG_PVP_CREDIT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.OriginalHonor);
		base._worldPacket.WriteInt32(this.Honor);
		base._worldPacket.WritePackedGuid128(this.Target);
		base._worldPacket.WriteUInt32(this.Rank);
	}
}
