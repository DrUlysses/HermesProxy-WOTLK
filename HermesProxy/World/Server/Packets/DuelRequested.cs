using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DuelRequested : ServerPacket
{
	public WowGuid128 ArbiterGUID;

	public WowGuid128 RequestedByGUID;

	public WowGuid128 RequestedByWowAccount;

	public DuelRequested()
		: base(Opcode.SMSG_DUEL_REQUESTED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ArbiterGUID);
		base._worldPacket.WritePackedGuid128(this.RequestedByGUID);
		base._worldPacket.WritePackedGuid128(this.RequestedByWowAccount);
	}
}
