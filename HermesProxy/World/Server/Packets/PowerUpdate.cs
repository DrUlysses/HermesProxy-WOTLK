using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PowerUpdate : ServerPacket
{
	public readonly WowGuid128 Guid;

	public readonly List<PowerUpdatePower> Powers;

	public PowerUpdate(WowGuid128 guid)
		: base(Opcode.SMSG_POWER_UPDATE)
	{
		Guid = guid;
		Powers = new List<PowerUpdatePower>();
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(Guid);
		_worldPacket.WriteInt32(Powers.Count);
		foreach (var power in Powers)
		{
			_worldPacket.WriteInt32(power.Power);
			_worldPacket.WriteUInt8(power.PowerType);
		}
	}
}
