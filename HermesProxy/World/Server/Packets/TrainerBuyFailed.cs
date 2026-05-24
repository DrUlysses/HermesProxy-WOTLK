using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class TrainerBuyFailed : ServerPacket
{
	public WowGuid128 TrainerGUID;

	public uint SpellID;

	public uint TrainerFailedReason;

	public TrainerBuyFailed()
		: base(Opcode.SMSG_TRAINER_BUY_FAILED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.TrainerGUID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.TrainerFailedReason);
	}
}
