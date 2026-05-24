using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class RespecWipeConfirm : ServerPacket
{
	public SpecResetType RespecType = SpecResetType.Talents;

	public uint Cost;

	public WowGuid128 TrainerGUID;

	public RespecWipeConfirm()
		: base(Opcode.SMSG_RESPEC_WIPE_CONFIRM)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8((sbyte)this.RespecType);
		base._worldPacket.WriteUInt32(this.Cost);
		base._worldPacket.WritePackedGuid128(this.TrainerGUID);
	}
}
