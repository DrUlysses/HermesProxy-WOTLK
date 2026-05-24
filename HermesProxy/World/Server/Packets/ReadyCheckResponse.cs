using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ReadyCheckResponse : ServerPacket
{
	public WowGuid128 PartyGUID;

	public WowGuid128 Player;

	public bool IsReady;

	public ReadyCheckResponse()
		: base(Opcode.SMSG_READY_CHECK_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PartyGUID);
		base._worldPacket.WritePackedGuid128(this.Player);
		base._worldPacket.WriteBit(this.IsReady);
		base._worldPacket.FlushBits();
	}
}
