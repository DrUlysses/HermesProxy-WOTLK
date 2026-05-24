using System;
using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PVPMatchStatisticsMessage : ServerPacket
{
	public class ArenaTeamsInfo
	{
		public WowGuid128[] Guids = new WowGuid128[2];

		public string[] Names = new string[2];

		public void Write(WorldPacket data)
		{
			string[] names = this.Names;
			foreach (string str in names)
			{
				data.WriteBits(str.GetByteCount(), 7);
			}
			data.FlushBits();
			for (int j = 0; j < 2; j++)
			{
				data.WritePackedGuid128(this.Guids[j]);
				data.WriteString(this.Names[j]);
			}
		}
	}

	public class RatingData
	{
		public uint[] Prematch = new uint[2];

		public uint[] Postmatch = new uint[2];

		public uint[] PrematchMMR = new uint[2];

		public void Write(WorldPacket data)
		{
			uint[] prematch = this.Prematch;
			foreach (uint id in prematch)
			{
				data.WriteUInt32(id);
			}
			uint[] postmatch = this.Postmatch;
			foreach (uint id2 in postmatch)
			{
				data.WriteUInt32(id2);
			}
			uint[] prematchMMR = this.PrematchMMR;
			foreach (uint id3 in prematchMMR)
			{
				data.WriteUInt32(id3);
			}
		}
	}

	public class HonorData
	{
		public uint HonorKills;

		public uint Deaths;

		public uint ContributionPoints;

		public void Write(WorldPacket data)
		{
			data.WriteUInt32(this.HonorKills);
			data.WriteUInt32(this.Deaths);
			data.WriteUInt32(this.ContributionPoints);
		}
	}

	public class PVPMatchPlayerStatistics
	{
		public WowGuid128 PlayerGUID;

		public uint Kills;

		public bool Faction;

		public bool IsInWorld = true;

		public HonorData Honor;

		public uint DamageDone;

		public uint HealingDone;

		public uint? PreMatchRating;

		public int? RatingChange;

		public uint? PreMatchMMR;

		public int? MmrChange;

		public List<uint> Stats = new List<uint>();

		public int PrimaryTalentTree;

		public Gender Sex;

		public Race PlayerRace;

		public Class PlayerClass;

		public int CreatureID;

		public int HonorLevel = 1;

		public int Rank;

		public void Write(WorldPacket data)
		{
			data.WritePackedGuid128(this.PlayerGUID);
			data.WriteUInt32(this.Kills);
			data.WriteUInt32(this.DamageDone);
			data.WriteUInt32(this.HealingDone);
			data.WriteInt32(this.Stats.Count);
			data.WriteInt32(this.PrimaryTalentTree);
			data.WriteUInt32((uint)this.Sex);
			data.WriteUInt32((uint)this.PlayerRace);
			data.WriteUInt32((uint)this.PlayerClass);
			data.WriteInt32(this.CreatureID);
			data.WriteInt32(this.HonorLevel);
			data.WriteInt32(this.Rank);
			foreach (uint pvpStat in this.Stats)
			{
				data.WriteUInt32(pvpStat);
			}
			data.WriteBit(this.Faction);
			data.WriteBit(this.IsInWorld);
			data.WriteBit(this.Honor != null);
			data.WriteBit(this.PreMatchRating.HasValue);
			data.WriteBit(this.RatingChange.HasValue);
			data.WriteBit(this.PreMatchMMR.HasValue);
			data.WriteBit(this.MmrChange.HasValue);
			data.FlushBits();
			if (this.Honor != null)
			{
				this.Honor.Write(data);
			}
			if (this.PreMatchRating.HasValue)
			{
				data.WriteUInt32(this.PreMatchRating.Value);
			}
			if (this.RatingChange.HasValue)
			{
				data.WriteInt32(this.RatingChange.Value);
			}
			if (this.PreMatchMMR.HasValue)
			{
				data.WriteUInt32(this.PreMatchMMR.Value);
			}
			if (this.MmrChange.HasValue)
			{
				data.WriteInt32(this.MmrChange.Value);
			}
		}
	}

	public RatingData Ratings;

	public ArenaTeamsInfo ArenaTeams;

	public byte? Winner;

	public List<PVPMatchPlayerStatistics> Statistics = new List<PVPMatchPlayerStatistics>();

	public sbyte[] PlayerCount = new sbyte[2];

	public PVPMatchStatisticsMessage()
		: base(Opcode.SMSG_PVP_MATCH_STATISTICS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Ratings != null);
		base._worldPacket.WriteBit(this.ArenaTeams != null);
		base._worldPacket.WriteBit(this.Winner.HasValue);
		if (this.ArenaTeams != null)
		{
			this.ArenaTeams.Write(base._worldPacket);
		}
		base._worldPacket.WriteInt32(this.Statistics.Count);
		sbyte[] playerCount = this.PlayerCount;
		foreach (sbyte count in playerCount)
		{
			base._worldPacket.WriteInt8(count);
		}
		if (this.Ratings != null)
		{
			this.Ratings.Write(base._worldPacket);
		}
		if (this.Winner.HasValue)
		{
			base._worldPacket.WriteUInt8(this.Winner.Value);
		}
		foreach (PVPMatchPlayerStatistics player in this.Statistics)
		{
			player.Write(base._worldPacket);
		}
	}
}
