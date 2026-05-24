using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LFGListUpdateBlacklist : ServerPacket
{
	public readonly List<LFGListBlacklistEntry> Blacklist = new();

	public LFGListUpdateBlacklist()
		: base(Opcode.SMSG_LFG_LIST_UPDATE_BLACKLIST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Blacklist.Count);
		foreach (var item in Blacklist)
		{
			item.Write(_worldPacket);
		}
	}

	public void AddBlacklist(int activity, int reason)
	{
		var entry = new LFGListBlacklistEntry
		{
			ActivityID = activity,
			Reason = reason
		};
		Blacklist.Add(entry);
	}
}
