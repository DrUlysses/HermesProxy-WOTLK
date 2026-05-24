using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LFGListUpdateBlacklist : ServerPacket
{
	public List<LFGListBlacklistEntry> Blacklist = new List<LFGListBlacklistEntry>();

	public LFGListUpdateBlacklist()
		: base(Opcode.SMSG_LFG_LIST_UPDATE_BLACKLIST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Blacklist.Count);
		foreach (LFGListBlacklistEntry item in this.Blacklist)
		{
			item.Write(base._worldPacket);
		}
	}

	public void AddBlacklist(int activity, int reason)
	{
		LFGListBlacklistEntry entry = new LFGListBlacklistEntry
		{
			ActivityID = activity,
			Reason = reason
		};
		this.Blacklist.Add(entry);
	}
}
