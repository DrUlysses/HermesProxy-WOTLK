using System;
using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class GossipPOI : ServerPacket
{
	public uint Id = 1u;

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

	public override void Write()
	{
		base._worldPacket.WriteInt32((int)this.Id);
		base._worldPacket.WriteInt32((int)this.Flags);
		base._worldPacket.WriteFloat(this.Pos.X);
		base._worldPacket.WriteFloat(this.Pos.Y);
		base._worldPacket.WriteFloat(this.Pos.Z);
		base._worldPacket.WriteInt32((int)this.Icon);
		base._worldPacket.WriteInt32((int)this.Importance);
		base._worldPacket.WriteInt32((int)this.Unknown905);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Name);
	}
}
