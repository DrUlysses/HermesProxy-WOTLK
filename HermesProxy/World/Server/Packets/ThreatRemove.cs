using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ThreatRemove : ServerPacket
{
	public WowGuid128 AboutGUID;
	public WowGuid128 UnitGUID;

	public ThreatRemove()
		: base(Opcode.SMSG_THREAT_REMOVE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.AboutGUID);
		base._worldPacket.WritePackedGuid128(this.UnitGUID);
	}
}
