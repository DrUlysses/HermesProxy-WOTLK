using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildRanks : ServerPacket
{
	public readonly List<GuildRankData> Ranks = new();

	public GuildRanks()
		: base(Opcode.SMSG_GUILD_RANKS)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Ranks.Count);
		Ranks.ForEach(delegate(GuildRankData p)
		{
			p.Write(_worldPacket);
		});
	}
}
