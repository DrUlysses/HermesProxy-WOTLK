using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ArenaTeamQueryResponse : ServerPacket
{
	public uint TeamId;

	public ArenaTeamEmblem Emblem;

	public ArenaTeamQueryResponse()
		: base(Opcode.SMSG_QUERY_ARENA_TEAM_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.TeamId);
		base._worldPacket.WriteBit(this.Emblem != null);
		base._worldPacket.FlushBits();
		if (this.Emblem != null)
		{
			this.Emblem.Write(base._worldPacket);
		}
	}
}
