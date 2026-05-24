using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildSendRankChange : ServerPacket
{
	public WowGuid128 Other;

	public WowGuid128 Officer;

	public bool Promote;

	public uint RankID;

	public GuildSendRankChange()
		: base(Opcode.SMSG_GUILD_SEND_RANK_CHANGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Officer);
		base._worldPacket.WritePackedGuid128(this.Other);
		base._worldPacket.WriteUInt32(this.RankID);
		base._worldPacket.WriteBit(this.Promote);
		base._worldPacket.FlushBits();
	}
}
