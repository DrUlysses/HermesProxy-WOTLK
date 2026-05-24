using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SendRaidTargetUpdateAll : ServerPacket
{
	public sbyte PartyIndex;

	public readonly List<Tuple<sbyte, WowGuid128>> TargetIcons = new();

	public SendRaidTargetUpdateAll()
		: base(Opcode.SMSG_SEND_RAID_TARGET_UPDATE_ALL)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt8(PartyIndex);
		_worldPacket.WriteInt32(TargetIcons.Count);
		foreach (var pair in TargetIcons)
		{
			_worldPacket.WritePackedGuid128(pair.Item2);
			_worldPacket.WriteInt8(pair.Item1);
		}
	}
}
