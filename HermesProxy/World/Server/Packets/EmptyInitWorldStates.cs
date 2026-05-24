using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyInitWorldStates : ServerPacket
{
	public uint MapId;

	public int ZoneId;

	public int AreaId;

	public EmptyInitWorldStates()
		: base(Opcode.SMSG_INIT_WORLD_STATES, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(MapId);
		_worldPacket.WriteInt32(ZoneId);
		_worldPacket.WriteInt32(AreaId);
		_worldPacket.WriteInt32(0);
	}
}
