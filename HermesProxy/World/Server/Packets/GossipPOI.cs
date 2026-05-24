using System;
using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class GossipPOI : ServerPacket
{
	public readonly uint Id = 1u;

	public uint Flags;

	public Vector3 Pos;

	public uint Icon;

	public uint Importance;

	public uint Unknown905;

	public string Name;

	public GossipPOI()
		: base(Opcode.SMSG_GOSSIP_POI)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32((int)Id);
		_worldPacket.WriteInt32((int)Flags);
		_worldPacket.WriteFloat(Pos.X);
		_worldPacket.WriteFloat(Pos.Y);
		_worldPacket.WriteFloat(Pos.Z);
		_worldPacket.WriteInt32((int)Icon);
		_worldPacket.WriteInt32((int)Importance);
		_worldPacket.WriteInt32((int)Unknown905);
		_worldPacket.WriteBits(Name.GetByteCount(), 6);
		_worldPacket.FlushBits();
		_worldPacket.WriteString(Name);
	}
}
