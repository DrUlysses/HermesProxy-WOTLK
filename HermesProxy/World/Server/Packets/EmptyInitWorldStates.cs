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
		base._worldPacket.WriteUInt32(this.MapId);
		base._worldPacket.WriteInt32(this.ZoneId);
		base._worldPacket.WriteInt32(this.AreaId);
		base._worldPacket.WriteInt32(0);
	}
}
