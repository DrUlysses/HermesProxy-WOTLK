using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryGuildInfoResponse : ServerPacket
{
	public class GuildInfo
	{
		public struct RankInfo
		{
			public uint RankID;

			public uint RankOrder;

			public string RankName;

			public RankInfo(uint id, uint order, string name)
			{
				this.RankID = id;
				this.RankOrder = order;
				this.RankName = name;
			}
		}

		public WowGuid128 GuildGuid;

		public uint VirtualRealmAddress;

		public uint EmblemStyle;

		public uint EmblemColor;

		public uint BorderStyle;

		public uint BorderColor;

		public uint BackgroundColor;

		public List<RankInfo> Ranks = new List<RankInfo>();

		public string GuildName = "";
	}

	public WowGuid128 GuildGUID;

	public WowGuid128 PlayerGuid;

	public GuildInfo Info = new GuildInfo();

	public bool HasGuildInfo;

	public QueryGuildInfoResponse()
		: base(Opcode.SMSG_QUERY_GUILD_INFO_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.GuildGUID);
		if (ModernVersion.RemovedInVersion(9, 2, 0, 1, 14, 2, 2, 5, 3))
		{
			base._worldPacket.WritePackedGuid128(this.PlayerGuid);
		}
		base._worldPacket.WriteBit(this.HasGuildInfo);
		base._worldPacket.FlushBits();
		if (!this.HasGuildInfo)
		{
			return;
		}
		base._worldPacket.WritePackedGuid128(this.Info.GuildGuid);
		base._worldPacket.WriteUInt32(this.Info.VirtualRealmAddress);
		base._worldPacket.WriteInt32(this.Info.Ranks.Count);
		base._worldPacket.WriteUInt32(this.Info.EmblemStyle);
		base._worldPacket.WriteUInt32(this.Info.EmblemColor);
		base._worldPacket.WriteUInt32(this.Info.BorderStyle);
		base._worldPacket.WriteUInt32(this.Info.BorderColor);
		base._worldPacket.WriteUInt32(this.Info.BackgroundColor);
		base._worldPacket.WriteBits(this.Info.GuildName.GetByteCount(), 7);
		base._worldPacket.FlushBits();
		foreach (GuildInfo.RankInfo rank in this.Info.Ranks)
		{
			base._worldPacket.WriteUInt32(rank.RankID);
			base._worldPacket.WriteUInt32(rank.RankOrder);
			base._worldPacket.WriteBits(rank.RankName.GetByteCount(), 7);
			base._worldPacket.WriteString(rank.RankName);
		}
		base._worldPacket.WriteString(this.Info.GuildName);
	}
}
