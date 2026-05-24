using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class HealthUpdate : ServerPacket
{
	public WowGuid128 Guid;
	public long Health;

	public HealthUpdate()
		: base(Opcode.SMSG_HEALTH_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(Guid);
		_worldPacket.WriteInt64(Health);
	}
}
