using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class RaidInstanceInfo : ServerPacket
{
	public List<InstanceLock> LockList = new List<InstanceLock>();

	public RaidInstanceInfo()
		: base(Opcode.SMSG_RAID_INSTANCE_INFO)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(LockList.Count);
		foreach (InstanceLock lockInfos in LockList)
		{
			lockInfos.Write(_worldPacket);
		}
	}
}
