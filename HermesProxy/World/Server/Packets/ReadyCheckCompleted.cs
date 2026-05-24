using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ReadyCheckCompleted : ServerPacket
{
	public sbyte PartyIndex;

	public WowGuid128 PartyGUID;

	public ReadyCheckCompleted()
		: base(Opcode.SMSG_READY_CHECK_COMPLETED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8(this.PartyIndex);
		base._worldPacket.WritePackedGuid128(this.PartyGUID);
	}
}
