using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LoginSetTimeSpeed : ServerPacket
{
	public uint ServerTime;

	public uint GameTime;

	public float NewSpeed;

	public int ServerTimeHolidayOffset;

	public int GameTimeHolidayOffset;

	public LoginSetTimeSpeed()
		: base(Opcode.SMSG_LOGIN_SET_TIME_SPEED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.ServerTime);
		base._worldPacket.WriteUInt32(this.GameTime);
		base._worldPacket.WriteFloat(this.NewSpeed);
		base._worldPacket.WriteInt32(this.ServerTimeHolidayOffset);
		base._worldPacket.WriteInt32(this.GameTimeHolidayOffset);
	}
}
