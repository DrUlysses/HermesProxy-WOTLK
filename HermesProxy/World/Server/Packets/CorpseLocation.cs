using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CorpseLocation : ServerPacket
{
	public WowGuid128 Player;

	public WowGuid128 Transport;

	public Vector3 Position;

	public int ActualMapID;

	public int MapID;

	public bool Valid;

	public CorpseLocation()
		: base(Opcode.SMSG_CORPSE_LOCATION)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Valid);
		base._worldPacket.FlushBits();
		base._worldPacket.WritePackedGuid128(this.Player);
		base._worldPacket.WriteInt32(this.ActualMapID);
		base._worldPacket.WriteVector3(this.Position);
		base._worldPacket.WriteInt32(this.MapID);
		base._worldPacket.WritePackedGuid128(this.Transport);
	}
}
