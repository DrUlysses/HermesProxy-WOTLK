using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SendRaidTargetUpdateAll : ServerPacket
{
	public sbyte PartyIndex;

	public List<Tuple<sbyte, WowGuid128>> TargetIcons = new List<Tuple<sbyte, WowGuid128>>();

	public SendRaidTargetUpdateAll()
		: base(Opcode.SMSG_SEND_RAID_TARGET_UPDATE_ALL)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8(this.PartyIndex);
		base._worldPacket.WriteInt32(this.TargetIcons.Count);
		foreach (Tuple<sbyte, WowGuid128> pair in this.TargetIcons)
		{
			base._worldPacket.WritePackedGuid128(pair.Item2);
			base._worldPacket.WriteInt8(pair.Item1);
		}
	}
}
