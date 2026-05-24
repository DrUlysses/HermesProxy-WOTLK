using Framework.Constants;
using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BindPointUpdate : ServerPacket
{
	public uint BindMapID = uint.MaxValue;

	public Vector3 BindPosition;

	public uint BindAreaID;

	public BindPointUpdate()
		: base(Opcode.SMSG_BIND_POINT_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteVector3(this.BindPosition);
		base._worldPacket.WriteUInt32(this.BindMapID);
		base._worldPacket.WriteUInt32(this.BindAreaID);
	}
}
