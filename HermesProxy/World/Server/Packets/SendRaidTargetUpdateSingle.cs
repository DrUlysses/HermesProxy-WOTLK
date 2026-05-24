using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SendRaidTargetUpdateSingle : ServerPacket
{
	public sbyte PartyIndex;

	public sbyte Symbol;

	public WowGuid128 Target;

	public WowGuid128 ChangedBy;

	public SendRaidTargetUpdateSingle()
		: base(Opcode.SMSG_SEND_RAID_TARGET_UPDATE_SINGLE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8(this.PartyIndex);
		base._worldPacket.WriteInt8(this.Symbol);
		base._worldPacket.WritePackedGuid128(this.Target);
		base._worldPacket.WritePackedGuid128(this.ChangedBy);
	}
}
