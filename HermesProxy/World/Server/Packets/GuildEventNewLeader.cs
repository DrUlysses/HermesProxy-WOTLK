using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventNewLeader : ServerPacket
{
	public bool SelfPromoted;

	public WowGuid128 NewLeaderGUID;

	public uint NewLeaderVirtualRealmAddress;

	public string NewLeaderName;

	public WowGuid128 OldLeaderGUID;

	public uint OldLeaderVirtualRealmAddress;

	public string OldLeaderName;

	public GuildEventNewLeader()
		: base(Opcode.SMSG_GUILD_EVENT_NEW_LEADER)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.SelfPromoted);
		base._worldPacket.WriteBits(this.OldLeaderName.GetByteCount(), 6);
		base._worldPacket.WriteBits(this.NewLeaderName.GetByteCount(), 6);
		base._worldPacket.WritePackedGuid128(this.OldLeaderGUID);
		base._worldPacket.WriteUInt32(this.OldLeaderVirtualRealmAddress);
		base._worldPacket.WritePackedGuid128(this.NewLeaderGUID);
		base._worldPacket.WriteUInt32(this.NewLeaderVirtualRealmAddress);
		base._worldPacket.WriteString(this.OldLeaderName);
		base._worldPacket.WriteString(this.NewLeaderName);
	}
}
