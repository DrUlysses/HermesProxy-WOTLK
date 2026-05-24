using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildRanks : ServerPacket
{
	public List<GuildRankData> Ranks = new List<GuildRankData>();

	public GuildRanks()
		: base(Opcode.SMSG_GUILD_RANKS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Ranks.Count);
		this.Ranks.ForEach(delegate(GuildRankData p)
		{
			p.Write(base._worldPacket);
		});
	}
}
