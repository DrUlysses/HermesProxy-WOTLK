using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public struct SpellMissStatus
{
	public readonly SpellMissInfo Reason;

	public readonly SpellMissInfo ReflectStatus;

	public SpellMissStatus(SpellMissInfo reason, SpellMissInfo reflectStatus)
	{
		Reason = reason;
		ReflectStatus = reflectStatus;
	}

	public void Write(WorldPacket data)
	{
		data.WriteBits((byte)Reason, 4);
		if (Reason == SpellMissInfo.Reflect)
		{
			data.WriteBits(ReflectStatus, 4);
		}
		data.FlushBits();
	}
}
