using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ReadyCheckStarted : ServerPacket
{
	public sbyte PartyIndex;

	public WowGuid128 PartyGUID;

	public WowGuid128 InitiatorGUID;

	public ulong Duration = 35000uL;

	public ReadyCheckStarted()
		: base(Opcode.SMSG_READY_CHECK_STARTED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8(this.PartyIndex);
		base._worldPacket.WritePackedGuid128(this.PartyGUID);
		base._worldPacket.WritePackedGuid128(this.InitiatorGUID);
		base._worldPacket.WriteUInt64(this.Duration);
	}
}
