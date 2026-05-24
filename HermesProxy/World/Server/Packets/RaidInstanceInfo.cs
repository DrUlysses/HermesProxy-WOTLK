using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class RaidInstanceInfo : ServerPacket
{
	public readonly List<InstanceLock> LockList = new();

	public RaidInstanceInfo()
		: base(Opcode.SMSG_RAID_INSTANCE_INFO)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(LockList.Count);
		foreach (var lockInfos in LockList)
		{
			lockInfos.Write(_worldPacket);
		}
	}
}
