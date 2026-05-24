using Framework.Constants;
using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LoginVerifyWorld : ServerPacket
{
	public uint MapID;

	public Position Pos;

	public uint Reason;

	public LoginVerifyWorld()
		: base(Opcode.SMSG_LOGIN_VERIFY_WORLD, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteFloat(this.Pos.X);
		base._worldPacket.WriteFloat(this.Pos.Y);
		base._worldPacket.WriteFloat(this.Pos.Z);
		base._worldPacket.WriteFloat(this.Pos.Orientation);
		base._worldPacket.WriteUInt32(this.Reason);
	}
}
