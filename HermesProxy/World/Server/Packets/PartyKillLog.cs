using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PartyKillLog : ServerPacket
{
	public WowGuid128 Player;

	public WowGuid128 Victim;

	public PartyKillLog()
		: base(Opcode.SMSG_PARTY_KILL_LOG)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Player);
		base._worldPacket.WritePackedGuid128(this.Victim);
	}
}
