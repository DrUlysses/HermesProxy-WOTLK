using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LfgLockInfoData
{
	public uint Slot;
	public uint LockStatus;
	public uint SubReason1;
	public uint SubReason2;
}

public class LfgBlackListEntry
{
	public WowGuid128 PlayerGuid;
	public List<LfgLockInfoData> Locks = new List<LfgLockInfoData>();
}

public class LfgPartyInfo : ServerPacket
{
	public List<LfgBlackListEntry> Players = new List<LfgBlackListEntry>();

	public LfgPartyInfo()
		: base(Opcode.SMSG_LFG_PARTY_INFO)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32((uint)Players.Count);
		foreach (LfgBlackListEntry player in Players)
		{
			_worldPacket.WritePackedGuid128(player.PlayerGuid);
			_worldPacket.WriteUInt32((uint)player.Locks.Count);
			foreach (LfgLockInfoData lockInfo in player.Locks)
			{
				_worldPacket.WriteUInt32(lockInfo.Slot);
				_worldPacket.WriteUInt32(lockInfo.LockStatus);
				_worldPacket.WriteUInt32(lockInfo.SubReason1);
				_worldPacket.WriteUInt32(lockInfo.SubReason2);
			}
		}
	}
}
