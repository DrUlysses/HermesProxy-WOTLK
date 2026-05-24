using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LogoutResponse : ServerPacket
{
	public int LogoutResult;

	public bool Instant = false;

	public LogoutResponse()
		: base(Opcode.SMSG_LOGOUT_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.LogoutResult);
		base._worldPacket.WriteBit(this.Instant);
		base._worldPacket.FlushBits();
	}
}
