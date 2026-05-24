using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PetCastFailed : ServerPacket
{
	public WowGuid128 CastID;

	public uint SpellID;

	public uint Reason;

	public int FailedArg1 = -1;

	public int FailedArg2 = -1;

	public PetCastFailed()
		: base(Opcode.SMSG_PET_CAST_FAILED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.CastID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.Reason);
		base._worldPacket.WriteInt32(this.FailedArg1);
		base._worldPacket.WriteInt32(this.FailedArg2);
	}
}
