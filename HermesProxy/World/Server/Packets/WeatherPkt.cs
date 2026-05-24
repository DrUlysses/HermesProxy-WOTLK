using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class WeatherPkt : ServerPacket
{
	public bool Abrupt;

	public float Intensity;

	public WeatherState WeatherID;

	public WeatherPkt()
		: base(Opcode.SMSG_WEATHER, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.WeatherID);
		base._worldPacket.WriteFloat(this.Intensity);
		base._worldPacket.WriteBit(this.Abrupt);
		base._worldPacket.FlushBits();
	}
}
