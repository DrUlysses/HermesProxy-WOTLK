using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SetTimeZoneInformation : ServerPacket
{
	public string ServerTimeTZ;

	public string GameTimeTZ;

	public string ServerRegionalTZ;

	public SetTimeZoneInformation()
		: base(Opcode.SMSG_SET_TIME_ZONE_INFORMATION)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBits(ServerTimeTZ.GetByteCount(), 7);
		_worldPacket.WriteBits(GameTimeTZ.GetByteCount(), 7);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteBits((ServerRegionalTZ ?? "US/Eastern").GetByteCount(), 7);
		}
		_worldPacket.FlushBits();
		_worldPacket.WriteString(ServerTimeTZ);
		_worldPacket.WriteString(GameTimeTZ);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteString(ServerRegionalTZ ?? "US/Eastern");
		}
	}
}
