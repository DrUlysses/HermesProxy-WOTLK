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
		base._worldPacket.WriteBits(this.ServerTimeTZ.GetByteCount(), 7);
		base._worldPacket.WriteBits(this.GameTimeTZ.GetByteCount(), 7);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteBits((this.ServerRegionalTZ ?? "US/Eastern").GetByteCount(), 7);
		}
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.ServerTimeTZ);
		base._worldPacket.WriteString(this.GameTimeTZ);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteString(this.ServerRegionalTZ ?? "US/Eastern");
		}
	}
}
