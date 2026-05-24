using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PowerUpdate : ServerPacket
{
	public WowGuid128 Guid;

	public List<PowerUpdatePower> Powers;

	public PowerUpdate(WowGuid128 guid)
		: base(Opcode.SMSG_POWER_UPDATE)
	{
		this.Guid = guid;
		this.Powers = new List<PowerUpdatePower>();
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteInt32(this.Powers.Count);
		foreach (PowerUpdatePower power in this.Powers)
		{
			base._worldPacket.WriteInt32(power.Power);
			base._worldPacket.WriteUInt8(power.PowerType);
		}
	}
}
