using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Framework;
using Framework.Constants;
using Framework.Cryptography;
using Framework.GameMath;
using Framework.IO;
using Framework.Logging;
using Framework.Networking;
using HermesProxy.Enums;
using HermesProxy.World.Enums;
using HermesProxy.World.Enums.Classic;
using HermesProxy.World.Objects;
using HermesProxy.World.Server;
using HermesProxy.World.Server.Packets;
using HashAlgorithm = Framework.Cryptography.HashAlgorithm;
using Quaternion = Framework.GameMath.Quaternion;
using Vector3 = Framework.GameMath.Vector3;

namespace HermesProxy.World.Client;

public class WorldClient
{
	private uint _requestBgPlayerPosCounter;

	private Socket _clientSocket;

	private bool? _isSuccessful;

	private uint _queuePosition;

	private string _username;

	private Realm _realm;

	private LegacyWorldCrypt _worldCrypt;

	private Dictionary<Opcode, Action<WorldPacket>> _packetHandlers;

	private GlobalSessionData _globalSession;

	private Mutex _sendMutex = new Mutex();

	private Dictionary<Opcode, List<WorldPacket>> _delayedPacketsToServer;

	private Dictionary<Opcode, List<ServerPacket>> _delayedPacketsToClient;

	public GlobalSessionData Session => _globalSession;

	[PacketHandler(Opcode.SMSG_ARENA_TEAM_QUERY_RESPONSE)]
	private void HandleArenaTeamQueryResponse(WorldPacket packet)
	{
		var teamId = packet.ReadUInt32();
		if (!GetSession().GameState.ArenaTeams.TryGetValue(teamId, out var team))
		{
			team = new ArenaTeamData();
			GetSession().GameState.ArenaTeams.Add(teamId, team);
		}
		team.Name = packet.ReadCString();
		team.TeamSize = packet.ReadUInt32();
		team.BackgroundColor = packet.ReadUInt32();
		team.EmblemStyle = packet.ReadUInt32();
		team.EmblemColor = packet.ReadUInt32();
		team.BorderStyle = packet.ReadUInt32();
		team.BorderColor = packet.ReadUInt32();
	}

	[PacketHandler(Opcode.SMSG_ARENA_TEAM_STATS)]
	private void HandleArenaTeamStats(WorldPacket packet)
	{
		var teamId = packet.ReadUInt32();
		if (!GetSession().GameState.ArenaTeams.TryGetValue(teamId, out var team))
		{
			team = new ArenaTeamData();
			GetSession().GameState.ArenaTeams.Add(teamId, team);
		}
		team.Rating = packet.ReadUInt32();
		team.WeekPlayed = packet.ReadUInt32();
		team.WeekWins = packet.ReadUInt32();
		team.SeasonPlayed = packet.ReadUInt32();
		team.SeasonWins = packet.ReadUInt32();
		team.Rank = packet.ReadUInt32();
	}

	[PacketHandler(Opcode.SMSG_ARENA_TEAM_ROSTER)]
	private void HandleArenaTeamRoster(WorldPacket packet)
	{
		var arena = new ArenaTeamRosterResponse
		{
			TeamId = packet.ReadUInt32()
		};
		var hiddenRating = false;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
		{
			packet.ReadBool();
		}
		var count = packet.ReadUInt32();
		arena.TeamSize = packet.ReadUInt32();
		for (var i = 0; i < count; i++)
		{
			var member = default(ArenaTeamMember);
			var cache = new PlayerCache();
			member.MemberGUID = packet.ReadGuid().To128(GetSession().GameState);
			member.Online = packet.ReadBool();
			member.Name = (cache.Name = packet.ReadCString());
			member.Captain = packet.ReadInt32();
			member.Level = (cache.Level = packet.ReadUInt8());
			member.ClassId = (cache.ClassId = (Class)packet.ReadUInt8());
			GetSession().GameState.UpdatePlayerCache(member.MemberGUID, cache);
			member.WeekGamesPlayed = packet.ReadUInt32();
			member.WeekGamesWon = packet.ReadUInt32();
			member.SeasonGamesPlayed = packet.ReadUInt32();
			member.SeasonGamesWon = packet.ReadUInt32();
			member.PersonalRating = packet.ReadUInt32();
			if (hiddenRating)
			{
				member.dword60 = packet.ReadFloat();
				member.dword68 = packet.ReadFloat();
			}
			arena.Members.Add(member);
		}
		if (GetSession().GameState.ArenaTeams.TryGetValue(arena.TeamId, out var team))
		{
			arena.TeamPlayed = team.WeekPlayed;
			arena.TeamWins = team.WeekWins;
			arena.SeasonPlayed = team.SeasonPlayed;
			arena.SeasonWins = team.SeasonWins;
			arena.TeamRating = team.Rating;
			arena.PlayerRating = team.Rank;
		}
		SendPacketToClient(arena);
	}

	[PacketHandler(Opcode.SMSG_ARENA_TEAM_EVENT)]
	private void HandleArenaTeamEvent(WorldPacket packet)
	{
		var arena = new ArenaTeamEvent();
		var eventType = (ArenaTeamEventLegacy)packet.ReadUInt8();
		arena.Event = (ArenaTeamEventModern)Enum.Parse(typeof(ArenaTeamEventModern), eventType.ToString());
		var count = packet.ReadUInt8();
		for (byte i = 0; i < count; i++)
		{
			var str = packet.ReadCString();
			switch (i)
			{
			case 0:
				arena.Param1 = str;
				break;
			case 1:
				arena.Param2 = str;
				break;
			case 2:
				arena.Param3 = str;
				break;
			}
		}
		if (packet.CanRead())
		{
			packet.ReadGuid();
		}
		SendPacketToClient(arena);
	}

	[PacketHandler(Opcode.SMSG_ARENA_TEAM_COMMAND_RESULT)]
	private void HandleArenaTeamCommandResult(WorldPacket packet)
	{
		var arena = new ArenaTeamCommandResult
		{
			Action = (ArenaTeamCommandType)packet.ReadUInt32(),
			TeamName = packet.ReadCString(),
			PlayerName = packet.ReadCString()
		};
		var errorType = (ArenaTeamCommandErrorLegacy)packet.ReadUInt32();
		arena.Error = (ArenaTeamCommandErrorModern)Enum.Parse(typeof(ArenaTeamCommandErrorModern), errorType.ToString());
		SendPacketToClient(arena);
	}

	[PacketHandler(Opcode.SMSG_ARENA_TEAM_INVITE)]
	private void HandleArenaTeamInvite(WorldPacket packet)
	{
		var arena = new ArenaTeamInvite
		{
			PlayerName = packet.ReadCString(),
			TeamName = packet.ReadCString()
		};
		arena.PlayerGuid = GetSession().GameState.GetPlayerGuidByName(arena.PlayerName);
		if (arena.PlayerGuid == null)
		{
			arena.PlayerGuid = WowGuid128.Empty;
		}
		arena.PlayerVirtualAddress = GetSession().RealmId.GetAddress();
		arena.TeamGuid = WowGuid128.Create(HighGuidType703.ArenaTeam, 1uL);
		SendPacketToClient(arena);
	}

	[PacketHandler(Opcode.MSG_AUCTION_HELLO)]
	private void HandleAuctionHello(WorldPacket packet)
	{
		var auction = new AuctionHelloResponse
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = auction.Guid;
		packet.ReadUInt32(); // AuctionHouseID - not used by modern client
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			auction.OpenForBusiness = packet.ReadBool();
		}
		// Modern client requires NPC interaction open result before the AH frame will work
		var npcInteraction = new ShowBank
		{
			Guid = auction.Guid,
			InteractionType = 21 // PlayerInteractionType::Auctioneer
		};
		SendPacketToClient(npcInteraction);
		SendPacketToClient(auction);
		var packet2 = new WorldPacket(Opcode.CMSG_AUCTION_LIST_OWNED_ITEMS);
		packet2.WriteGuid(auction.Guid.To64());
		packet2.WriteUInt32(0u);
		SendPacketToServer(packet2);
	}

	private AuctionItem ReadAuctionItem(WorldPacket packet)
	{
		var item = new AuctionItem
		{
			AuctionID = packet.ReadUInt32(),
			Item = new ItemInstance
			{
				ItemID = packet.ReadUInt32()
			}
		};
		var enchantmentCount = (byte)(LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? 7 : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? 1 : 6));
		for (byte j = 0; j < enchantmentCount; j++)
		{
			var enchant = new ItemEnchantData
			{
				Slot = j,
				ID = packet.ReadUInt32()
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				enchant.Expiration = packet.ReadUInt32();
				enchant.Charges = packet.ReadInt32();
			}
			if (enchant.ID != 0)
			{
				item.Enchantments.Add(enchant);
			}
		}
		item.Item.RandomPropertiesID = packet.ReadUInt32();
		item.Item.RandomPropertiesSeed = packet.ReadUInt32();
		item.Count = packet.ReadInt32();
		item.Charges = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			item.Flags = packet.ReadUInt32();
		}
		item.Owner = packet.ReadGuid().To128(GetSession().GameState);
		item.OwnerAccountID = GetSession().GetGameAccountGuidForPlayer(item.Owner);
		item.MinBid = packet.ReadUInt32();
		item.MinIncrement = packet.ReadUInt32();
		item.BuyoutPrice = packet.ReadUInt32();
		item.DurationLeft = packet.ReadInt32();
		item.Bidder = packet.ReadGuid().To128(GetSession().GameState);
		item.BidAmount = packet.ReadUInt32();
		if (item.Item.ItemID == 0)
		{
			item.Item = null;
		}
		return item;
	}

	[PacketHandler(Opcode.SMSG_AUCTION_LIST_BIDDED_ITEMS_RESULT)]
	[PacketHandler(Opcode.SMSG_AUCTION_LIST_OWNED_ITEMS_RESULT)]
	private void HandleAuctionListMyItemsResult(WorldPacket packet)
	{
		var auction = new AuctionListMyItemsResult(packet.GetUniversalOpcode(isModern: false));
		var count = packet.ReadUInt32();
		for (var i = 0u; i < count; i++)
		{
			var item = ReadAuctionItem(packet);
			auction.Items.Add(item);
		}
		var totalCount = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
		{
			auction.DesiredDelay = packet.ReadUInt32();
		}
		auction.HasMoreResults = totalCount > (int)count;
		SendPacketToClient(auction);
	}

	[PacketHandler(Opcode.SMSG_AUCTION_LIST_ITEMS_RESULT)]
	private void HandleAuctionListItemsResult(WorldPacket packet)
	{
		var auction = new AuctionListItemsResult();
		var count = packet.ReadUInt32();
		for (var i = 0u; i < count; i++)
		{
			var item = ReadAuctionItem(packet);
			item.CensorServerSideInfo = true;
			auction.Items.Add(item);
		}
		var totalCount = packet.ReadInt32();
		auction.TotalItemsCount = totalCount;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
		{
			auction.DesiredDelay = packet.ReadUInt32();
		}
		auction.HasMoreResults = totalCount > (int)count;
		SendPacketToClient(auction);
	}

	[PacketHandler(Opcode.SMSG_AUCTION_COMMAND_RESULT)]
	private void HandleAuctionCommandResult(WorldPacket packet)
	{
		var auction = new AuctionCommandResult
		{
			AuctionID = packet.ReadUInt32(),
			Command = (AuctionHouseAction)packet.ReadUInt32(),
			ErrorCode = (AuctionHouseError)packet.ReadUInt32()
		};
		switch (auction.ErrorCode)
		{
		case AuctionHouseError.Ok:
			if (auction.Command == AuctionHouseAction.Bid)
			{
				auction.MinIncrement = packet.ReadUInt32();
			}
			break;
		case AuctionHouseError.Inventory:
			auction.BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32());
			break;
		case AuctionHouseError.HigherBid:
			auction.Guid = packet.ReadGuid().To128(GetSession().GameState);
			auction.Money = packet.ReadUInt32();
			auction.MinIncrement = packet.ReadUInt32();
			break;
		}
		SendPacketToClient(auction);
	}

	[PacketHandler(Opcode.SMSG_AUCTION_OWNER_NOTIFICATION)]
	private void HandleAuctionOwnerNotification(WorldPacket packet)
	{
		var info = new AuctionOwnerNotification
		{
			AuctionID = packet.ReadUInt32(),
			BidAmount = packet.ReadUInt32()
		};
		var minIncrement = packet.ReadUInt32();
		WowGuid buyer = packet.ReadGuid();
		info.Item.ItemID = packet.ReadUInt32();
		info.Item.RandomPropertiesID = packet.ReadUInt32();
		var mailDelay = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056)) ? 3600f : packet.ReadFloat());
		if (buyer.IsEmpty())
		{
			var auction = new AuctionClosedNotification
			{
				Info = info,
				Sold = info.BidAmount != 0,
				ProceedsMailDelay = mailDelay
			};
			SendPacketToClient(auction);
		}
		else
		{
			var auction2 = new AuctionOwnerBidNotification
			{
				Info = info,
				MinIncrement = minIncrement,
				Bidder = buyer.To128(GetSession().GameState)
			};
			SendPacketToClient(auction2);
		}
	}

	[PacketHandler(Opcode.SMSG_AUCTION_BIDDER_NOTIFICATION)]
	private void HandleAuctionBidderNotification(WorldPacket packet)
	{
		var info = new AuctionBidderNotification();
		var auctionHouseId = packet.ReadUInt32();
		info.AuctionID = packet.ReadUInt32();
		info.Bidder = packet.ReadGuid().To128(GetSession().GameState);
		var bidAmount = packet.ReadUInt32();
		var minIncrement = packet.ReadUInt32();
		info.Item.ItemID = packet.ReadUInt32();
		info.Item.RandomPropertiesID = packet.ReadUInt32();
		if (bidAmount == 0)
		{
			var auction = new AuctionWonNotification
			{
				Info = info
			};
			SendPacketToClient(auction);
		}
		else
		{
			var auction2 = new AuctionOutbidNotification
			{
				Info = info,
				BidAmount = bidAmount,
				MinIncrement = minIncrement
			};
			SendPacketToClient(auction2);
		}
	}

	[PacketHandler(Opcode.SMSG_BATTLEFIELD_LIST, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleBattlefieldListVanilla(WorldPacket packet)
	{
		var bglist = new BattlefieldList
		{
			BattlemasterGuid = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = bglist.BattlemasterGuid;
		bglist.BattlemasterListID = GameData.GetBattlegroundIdFromMapId(packet.ReadUInt32());
		packet.ReadUInt8();
		var instancesCount = packet.ReadUInt32();
		for (var i = 0; i < instancesCount; i++)
		{
			var instanceId = packet.ReadInt32();
			bglist.BattlefieldInstances.Add(instanceId);
		}
		SendPacketToClient(bglist);
	}

	[PacketHandler(Opcode.SMSG_BATTLEFIELD_LIST, ClientVersionBuild.V2_0_1_6180, ClientVersionBuild.V3_0_2_9056)]
	private void HandleBattlefieldListTBC(WorldPacket packet)
	{
		var bglist = new BattlefieldList
		{
			BattlemasterGuid = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = bglist.BattlemasterGuid;
		bglist.BattlemasterListID = packet.ReadUInt32();
		packet.ReadUInt8();
		var instancesCount = packet.ReadUInt32();
		for (var i = 0; i < instancesCount; i++)
		{
			var instanceId = packet.ReadInt32();
			bglist.BattlefieldInstances.Add(instanceId);
		}
		SendPacketToClient(bglist);
	}

	[PacketHandler(Opcode.SMSG_BATTLEFIELD_LIST, ClientVersionBuild.V3_0_2_9056)]
	private void HandleBattlefieldListWotLK(WorldPacket packet)
	{
		var bglist = new BattlefieldList
		{
			BattlemasterGuid = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = bglist.BattlemasterGuid;
		bglist.PvpAnywhere = packet.ReadBool();
		bglist.BattlemasterListID = packet.ReadUInt32();
		bglist.MinLevel = packet.ReadUInt8();
		bglist.MaxLevel = packet.ReadUInt8();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
		{
			packet.ReadBool();
			packet.ReadInt32();
			packet.ReadInt32();
			packet.ReadInt32();
			if (packet.ReadBool())
			{
				bglist.HasRandomWinToday = packet.ReadBool();
				packet.ReadInt32();
				packet.ReadInt32();
				packet.ReadInt32();
			}
		}
		var instancesCount = packet.ReadUInt32();
		for (var i = 0; i < instancesCount; i++)
		{
			var instanceId = packet.ReadInt32();
			bglist.BattlefieldInstances.Add(instanceId);
		}
		SendPacketToClient(bglist);
	}

	[PacketHandler(Opcode.SMSG_BATTLEFIELD_STATUS, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleBattlefieldStatusVanilla(WorldPacket packet)
	{
		var hdr = new BattlefieldStatusHeader
		{
			Ticket =
			{
				Id = 1 + packet.ReadUInt32(),
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid
			}
		};
		hdr.Ticket.Time = GetSession().GameState.GetBattleFieldQueueTime(hdr.Ticket.Id);
		hdr.Ticket.Type = RideType.Battlegrounds;
		var mapId = packet.ReadUInt32();
		if (mapId != 0)
		{
			var battlefieldListId = GameData.GetBattlegroundIdFromMapId(mapId);
			hdr.BattlefieldListIDs.Add(battlefieldListId);
			packet.ReadUInt8();
			hdr.InstanceID = packet.ReadUInt32();
			var status = (BattleGroundStatus)packet.ReadUInt32();
			switch (status)
			{
			case BattleGroundStatus.WaitQueue:
			{
				var queue = new BattlefieldStatusQueued
				{
					Hdr = hdr,
					AverageWaitTime = packet.ReadUInt32(),
					WaitTime = packet.ReadUInt32()
				};
				SendPacketToClient(queue);
				break;
			}
			case BattleGroundStatus.WaitJoin:
			{
				var confirm = new BattlefieldStatusNeedConfirmation
				{
					Hdr = hdr,
					Mapid = mapId,
					Timeout = packet.ReadUInt32()
				};
				SendPacketToClient(confirm);
				break;
			}
			case BattleGroundStatus.InProgress:
			{
				var active = new BattlefieldStatusActive
				{
					Hdr = hdr,
					Mapid = mapId,
					ShutdownTimer = packet.ReadUInt32(),
					StartTimer = packet.ReadUInt32()
				};
				if (active.ShutdownTimer == 0)
				{
					var init = new BattlegroundInit
					{
						Milliseconds = 1154756799u
					};
					SendPacketToClient(init);
				}
				SendPacketToClient(active);
				break;
			}
			default:
				Log.Print(LogType.Error, $"Unexpected BG status {status}.", "BattleGroundHandler.cs");
				break;
			}
		}
		else
		{
			var queuedMapId = GetSession().GameState.GetBattleFieldQueueType(hdr.Ticket.Id);
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180) && queuedMapId == GetSession().GameState.CurrentMapId)
			{
				var bgGroup = GetSession().GameState.CurrentGroups[1];
				if (bgGroup != null)
				{
					var party = new PartyUpdate
					{
						SequenceNum = GetSession().GameState.GroupUpdateCounter++,
						PartyFlags = GroupFlags.FakeRaid | GroupFlags.Destroyed,
						PartyIndex = 1,
						PartyGUID = bgGroup.PartyGUID,
						LeaderGUID = WowGuid128.Empty,
						MyIndex = -1
					};
					GetSession().GameState.CurrentGroups[1] = null;
					SendPacketToClient(party);
				}
			}
			var failed = new BattlefieldStatusFailed
			{
				Ticket = hdr.Ticket,
				Reason = 30,
				BattlefieldListId = GameData.GetBattlegroundIdFromMapId(queuedMapId)
			};
			SendPacketToClient(failed);
			GetSession().GameState.BattleFieldQueueTimes.Remove(hdr.Ticket.Id);
		}
		GetSession().GameState.StoreBattleFieldQueueType(hdr.Ticket.Id, mapId);
	}

	[PacketHandler(Opcode.SMSG_BATTLEFIELD_STATUS, ClientVersionBuild.V2_0_1_6180)]
	private void HandleBattlefieldStatusTBC(WorldPacket packet)
	{
		var hdr = new BattlefieldStatusHeader
		{
			Ticket =
			{
				Id = 1 + packet.ReadUInt32(),
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid
			}
		};
		hdr.Ticket.Time = GetSession().GameState.GetBattleFieldQueueTime(hdr.Ticket.Id);
		hdr.Ticket.Type = RideType.Battlegrounds;
		hdr.ArenaTeamSize = packet.ReadUInt8();
		packet.ReadUInt8();
		var battlefieldListId = packet.ReadUInt32();
		packet.ReadUInt16();
		if (battlefieldListId != 0)
		{
			hdr.BattlefieldListIDs.Add(battlefieldListId);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
			{
				hdr.RangeMin = packet.ReadUInt8();
				hdr.RangeMax = packet.ReadUInt8();
			}
			hdr.InstanceID = packet.ReadUInt32();
			hdr.IsArena = packet.ReadBool();
			var status = (BattleGroundStatus)packet.ReadUInt32();
			switch (status)
			{
			case BattleGroundStatus.WaitQueue:
			{
				var queue = new BattlefieldStatusQueued
				{
					Hdr = hdr,
					AverageWaitTime = packet.ReadUInt32(),
					WaitTime = packet.ReadUInt32()
				};
				SendPacketToClient(queue);
				break;
			}
			case BattleGroundStatus.WaitJoin:
			{
				var confirm = new BattlefieldStatusNeedConfirmation
				{
					Hdr = hdr,
					Mapid = packet.ReadUInt32()
				};
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_5_12213))
				{
					packet.ReadUInt64();
				}
				confirm.Timeout = packet.ReadUInt32();
				SendPacketToClient(confirm);
				break;
			}
			case BattleGroundStatus.InProgress:
			{
				var active = new BattlefieldStatusActive
				{
					Hdr = hdr,
					Mapid = packet.ReadUInt32()
				};
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_5_12213))
				{
					packet.ReadUInt64();
				}
				active.ShutdownTimer = packet.ReadUInt32();
				active.StartTimer = packet.ReadUInt32();
				active.ArenaFaction = packet.ReadUInt8();
				if (active.ShutdownTimer == 0)
				{
					var init = new BattlegroundInit
					{
						Milliseconds = 1154756799u
					};
					SendPacketToClient(init);
				}
				SendPacketToClient(active);
				break;
			}
			default:
				Log.Print(LogType.Error, $"Unexpected BG status {status}.", "BattleGroundHandler.cs");
				break;
			}
		}
		else
		{
			var failed = new BattlefieldStatusFailed
			{
				Ticket = hdr.Ticket,
				Reason = 30,
				BattlefieldListId = GetSession().GameState.GetBattleFieldQueueType(hdr.Ticket.Id)
			};
			SendPacketToClient(failed);
			GetSession().GameState.BattleFieldQueueTimes.Remove(hdr.Ticket.Id);
		}
		GetSession().GameState.StoreBattleFieldQueueType(hdr.Ticket.Id, battlefieldListId);
	}

	[PacketHandler(Opcode.MSG_PVP_LOG_DATA, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePvPLogDataVanilla(WorldPacket packet)
	{
		var pvp = new PVPMatchStatisticsMessage();
		if (packet.ReadBool())
		{
			pvp.Winner = packet.ReadUInt8();
		}
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var player = new PVPMatchStatisticsMessage.PVPMatchPlayerStatistics
			{
				PlayerGUID = packet.ReadGuid().To128(GetSession().GameState),
				Rank = packet.ReadInt32(),
				Kills = packet.ReadUInt32(),
				Honor = new PVPMatchStatisticsMessage.HonorData
				{
					HonorKills = packet.ReadUInt32(),
					Deaths = packet.ReadUInt32(),
					ContributionPoints = packet.ReadUInt32()
				}
			};
			var statsCount = packet.ReadInt32();
			for (var j = 0; j < statsCount; j++)
			{
				player.Stats.Add(packet.ReadUInt32());
			}
			if (GetSession().GameState.CachedPlayers.TryGetValue(player.PlayerGUID, out var cache))
			{
				player.Sex = cache.SexId;
				player.PlayerRace = cache.RaceId;
				player.PlayerClass = cache.ClassId;
				player.Faction = GameData.IsAllianceRace(cache.RaceId);
			}
			else
			{
				player.Sex = Gender.Male;
				player.PlayerRace = Race.Human;
				player.PlayerClass = Class.Warrior;
			}
			pvp.Statistics.Add(player);
		}
		SendPacketToClient(pvp);
	}

	[PacketHandler(Opcode.MSG_PVP_LOG_DATA, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePvPLogDataTBC(WorldPacket packet)
	{
		var pvp = new PVPMatchStatisticsMessage();
		if (packet.ReadBool())
		{
			pvp.ArenaTeams = new PVPMatchStatisticsMessage.ArenaTeamsInfo
			{
				Guids =
				{
					[0] = WowGuid128.Empty,
					[1] = WowGuid128.Empty
				}
			};
			for (var i = 0; i < 2; i++)
			{
				packet.ReadUInt32();
				packet.ReadUInt32();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					packet.ReadUInt32();
				}
			}
			for (var j = 0; j < 2; j++)
			{
				pvp.ArenaTeams.Names[j] = packet.ReadCString();
			}
		}
		if (packet.ReadBool())
		{
			pvp.Winner = packet.ReadUInt8();
		}
		var count = packet.ReadInt32();
		for (var k = 0; k < count; k++)
		{
			var player = new PVPMatchStatisticsMessage.PVPMatchPlayerStatistics
			{
				PlayerGUID = packet.ReadGuid().To128(GetSession().GameState),
				Kills = packet.ReadUInt32()
			};
			if (pvp.ArenaTeams == null)
			{
				player.Honor = new PVPMatchStatisticsMessage.HonorData
				{
					HonorKills = packet.ReadUInt32(),
					Deaths = packet.ReadUInt32(),
					ContributionPoints = packet.ReadUInt32()
				};
			}
			else
			{
				player.Faction = packet.ReadBool();
				pvp.PlayerCount[player.Faction ? 1 : 0]++;
			}
			player.DamageDone = packet.ReadUInt32();
			player.HealingDone = packet.ReadUInt32();
			var statsCount = packet.ReadInt32();
			for (var l = 0; l < statsCount; l++)
			{
				player.Stats.Add(packet.ReadUInt32());
			}
			if (GetSession().GameState.CachedPlayers.TryGetValue(player.PlayerGUID, out var cache))
			{
				player.Sex = cache.SexId;
				player.PlayerRace = cache.RaceId;
				player.PlayerClass = cache.ClassId;
				if (pvp.ArenaTeams == null)
				{
					player.Faction = GameData.IsAllianceRace(cache.RaceId);
				}
			}
			else
			{
				player.Sex = Gender.Male;
				player.PlayerRace = Race.Human;
				player.PlayerClass = Class.Warrior;
			}
			pvp.Statistics.Add(player);
		}
		SendPacketToClient(pvp);
	}

	private BattlegroundPlayerPosition ReadBattlegroundPlayerPosition(WorldPacket packet)
	{
		return new BattlegroundPlayerPosition
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState),
			Pos = packet.ReadVector2()
		};
	}

	[PacketHandler(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleBattlegroundPlayerPositionsVanilla(WorldPacket packet)
	{
		GetSession().GameState.FlagCarrierGuids.Clear();
		var bglist = new BattlegroundPlayerPositions();
		var teamMembersCount = packet.ReadUInt32();
		for (var i = 0u; i < teamMembersCount; i++)
		{
			ReadBattlegroundPlayerPosition(packet);
		}
		if (packet.ReadBool())
		{
			var position = ReadBattlegroundPlayerPosition(packet);
			if (GetSession().GameState.IsAlliancePlayer(position.Guid))
			{
				position.IconID = 1;
				position.ArenaSlot = 3;
			}
			else
			{
				position.IconID = 2;
				position.ArenaSlot = 2;
			}
			bglist.FlagCarriers.Add(position);
			GetSession().GameState.FlagCarrierGuids.Add(position.Guid);
		}
		SendPacketToClient(bglist);
	}

	[PacketHandler(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS, ClientVersionBuild.V2_0_1_6180)]
	private void HandleBattlegroundPlayerPositionsTBC(WorldPacket packet)
	{
		var bglist = new BattlegroundPlayerPositions();
		var teamMembersCount = packet.ReadUInt32();
		var flagCarriersCount = packet.ReadUInt32();
		for (var i = 0u; i < teamMembersCount; i++)
		{
			ReadBattlegroundPlayerPosition(packet);
		}
		GetSession().GameState.FlagCarrierGuids.Clear();
		for (var i2 = 0u; i2 < flagCarriersCount; i2++)
		{
			var position = ReadBattlegroundPlayerPosition(packet);
			if (GetSession().GameState.IsAlliancePlayer(position.Guid))
			{
				position.IconID = 1;
				position.ArenaSlot = 3;
			}
			else
			{
				position.IconID = 2;
				position.ArenaSlot = 2;
			}
			bglist.FlagCarriers.Add(position);
			GetSession().GameState.FlagCarrierGuids.Add(position.Guid);
		}
		SendPacketToClient(bglist);
	}

	[PacketHandler(Opcode.SMSG_BATTLEGROUND_PLAYER_JOINED)]
	[PacketHandler(Opcode.SMSG_BATTLEGROUND_PLAYER_LEFT)]
	private void HandleBattlegroundPlayerLeftOrJoined(WorldPacket packet)
	{
		var player = new BattlegroundPlayerLeftOrJoined(packet.GetUniversalOpcode(isModern: false))
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(player);
	}

	[PacketHandler(Opcode.SMSG_AREA_SPIRIT_HEALER_TIME)]
	private void HandleAreaSpiritHealerTime(WorldPacket packet)
	{
		var healer = new AreaSpiritHealerTime
		{
			HealerGuid = packet.ReadGuid().To128(GetSession().GameState),
			TimeLeft = packet.ReadUInt32()
		};
		SendPacketToClient(healer);
	}

	[PacketHandler(Opcode.SMSG_PVP_CREDIT)]
	private void HandlePvPCredit(WorldPacket packet)
	{
		var credit = new PvPCredit
		{
			OriginalHonor = packet.ReadInt32(),
			Target = packet.ReadGuid().To128(GetSession().GameState),
			Rank = packet.ReadUInt32()
		};
		SendPacketToClient(credit);
	}

	[PacketHandler(Opcode.SMSG_PLAYER_SKINNED)]
	private void HandlePlayerSkinned(WorldPacket packet)
	{
		var skinned = new PlayerSkinned();
		if (packet.CanRead())
		{
			skinned.FreeRepop = packet.ReadBool();
		}
		SendPacketToClient(skinned);
	}

	[PacketHandler(Opcode.SMSG_ENUM_CHARACTERS_RESULT)]
	private void HandleEnumCharactersResult(WorldPacket packet)
	{
		var charEnum = new EnumCharactersResult
		{
			Success = true,
			IsDeletedCharacters = false,
			IsNewPlayerRestrictionSkipped = false,
			IsNewPlayerRestricted = false,
			IsNewPlayer = false,
			IsAlliedRacesCreationAllowed = false,
			DisabledClassesMask = null
		};
		GetSession().GameState.OwnCharacters.Clear();
		var count = packet.ReadUInt8();
		for (byte i = 0; i < count; i++)
		{
			var char1 = new EnumCharactersResult.CharacterInfo
			{
				ListPosition = i
			};
			var cache = new PlayerCache();
			char1.Guid = packet.ReadGuid().To128(GetSession().GameState);
			char1.Name = (cache.Name = packet.ReadCString());
			char1.RaceId = (cache.RaceId = (Race)packet.ReadUInt8());
			char1.ClassId = (cache.ClassId = (Class)packet.ReadUInt8());
			char1.SexId = (cache.SexId = (Gender)packet.ReadUInt8());
			var skin = packet.ReadUInt8();
			var face = packet.ReadUInt8();
			var hairStyle = packet.ReadUInt8();
			var hairColor = packet.ReadUInt8();
			var facialHair = packet.ReadUInt8();
			char1.Customizations = CharacterCustomizations.ConvertLegacyCustomizationsToModern(char1.RaceId, char1.SexId, skin, face, hairStyle, hairColor, facialHair);
			char1.ExperienceLevel = (cache.Level = packet.ReadUInt8());
			if (char1.ExperienceLevel > charEnum.MaxCharacterLevel)
			{
				charEnum.MaxCharacterLevel = char1.ExperienceLevel;
			}
			GetSession().GameState.UpdatePlayerCache(char1.Guid, cache);
			char1.ZoneId = packet.ReadUInt32();
			char1.MapId = packet.ReadUInt32();
			char1.PreloadPos = packet.ReadVector3();
			var guildId = packet.ReadUInt32();
			GetSession().GameState.StorePlayerGuildId(char1.Guid, guildId);
			char1.GuildGuid = ((guildId != 0) ? WowGuid128.Create(HighGuidType703.Guild, guildId) : WowGuid128.Empty);
			char1.Flags = (CharacterFlags)packet.ReadUInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				char1.Flags2 = packet.ReadUInt32();
			}
			char1.FirstLogin = packet.ReadUInt8() != 0;
			char1.PetCreatureDisplayId = packet.ReadUInt32();
			char1.PetExperienceLevel = packet.ReadUInt32();
			char1.PetCreatureFamilyId = packet.ReadUInt32();
			for (var j = 0; j < 19; j++)
			{
				char1.VisualItems[j].DisplayId = packet.ReadUInt32();
				char1.VisualItems[j].InvType = packet.ReadUInt8();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					char1.VisualItems[j].DisplayEnchantId = packet.ReadUInt32();
				}
			}
			var bagCount = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685)) ? 1 : 4);
			for (var k = 0; k < bagCount; k++)
			{
				char1.VisualItems[19 + k].DisplayId = packet.ReadUInt32();
				char1.VisualItems[19 + k].InvType = packet.ReadUInt8();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					char1.VisualItems[19 + k].DisplayEnchantId = packet.ReadUInt32();
				}
			}
			char1.Flags2 = 0u;
			char1.Flags3 = 0u;
			char1.Flags4 = 0u;
			char1.ProfessionIds[0] = 0u;
			char1.ProfessionIds[1] = 0u;
			char1.LastPlayedTime = (ulong)Time.UnixTime;
			char1.SpecID = 0;
			char1.Unknown703 = 0u;
			char1.LastLoginVersion = (uint)Settings.ClientBuild;
			char1.OverrideSelectScreenFileDataID = 0u;
			char1.BoostInProgress = false;
			char1.unkWod61x = 0;
			char1.ExpansionChosen = true;
			charEnum.Characters.Add(char1);
			GetSession().GameState.OwnCharacters.Add(new OwnCharacterInfo
			{
				AccountId = GetSession().GameAccountInfo.WoWAccountGuid,
				CharacterGuid = char1.Guid,
				Realm = GetSession().Realm,
				LastLoginUnixSec = char1.LastPlayedTime,
				Name = char1.Name,
				RaceId = char1.RaceId,
				ClassId = char1.ClassId,
				SexId = char1.SexId,
				Level = char1.ExperienceLevel
			});
		}
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(1, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(2, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(3, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(4, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(5, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(6, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(7, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(8, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		if (ModernVersion.ExpansionVersion >= 2 && LegacyVersion.ExpansionVersion >= 2)
		{
			charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(10, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
			charEnum.RaceUnlockData.Add(new EnumCharactersResult.RaceUnlock(11, hasExpansion: true, hasAchievement: false, hasHeritageArmor: false));
		}
		SendPacketToClient(charEnum);
	}

	[PacketHandler(Opcode.SMSG_CREATE_CHAR)]
	private void HandleCreateChar(WorldPacket packet)
	{
		var result = packet.ReadUInt8();
		var createChar = new CreateChar
		{
			Guid = new WowGuid128(),
			Code = ModernVersion.ConvertResponseCodesValue(result)
		};
		SendPacketToClient(createChar);
	}

	[PacketHandler(Opcode.SMSG_DELETE_CHAR)]
	private void HandleDeleteChar(WorldPacket packet)
	{
		var result = packet.ReadUInt8();
		var deleteChar = new DeleteChar
		{
			Code = ModernVersion.ConvertResponseCodesValue(result)
		};
		SendPacketToClient(deleteChar);
	}

	[PacketHandler(Opcode.SMSG_QUERY_PLAYER_NAME_RESPONSE)]
	private void HandleQueryPlayerNameResponse(WorldPacket packet)
	{
		WowGuid128 playerGuid;
		byte result = 0;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			playerGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
			if (packet.ReadBool())
				result = 1;
		}
		else
		{
			playerGuid = packet.ReadGuid().To128(GetSession().GameState);
		}

		var data = new PlayerGuidLookupData
		{
			GuidActual = playerGuid
		};

		if (result != 0)
		{
			// Player not found - send error response
			if (ModernVersion.GetCurrentOpcode(Opcode.SMSG_QUERY_PLAYER_NAME_RESPONSE) != 0)
			{
				var response = new QueryPlayerNameResponse
				{
					Player = playerGuid,
					Result = 1
				};
				SendPacketToClient(response);
			}
			else
			{
				var response = new QueryPlayerNamesResponse();
				response.Players.Add(new QueryPlayerNamesResponse.NameCacheLookupResult
				{
					Player = playerGuid,
					Result = 1,
					Data = null
				});
				SendPacketToClient(response);
			}
			return;
		}

		var cache = new PlayerCache();
		data.Name = (cache.Name = packet.ReadCString());
		packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			data.RaceID = (cache.RaceId = (Race)packet.ReadUInt8());
			data.Sex = (cache.SexId = (Gender)packet.ReadUInt8());
			data.ClassID = (cache.ClassId = (Class)packet.ReadUInt8());
		}
		else
		{
			data.RaceID = (cache.RaceId = (Race)packet.ReadUInt32());
			data.Sex = (cache.SexId = (Gender)packet.ReadUInt32());
			data.ClassID = (cache.ClassId = (Class)packet.ReadInt32());
		}
		if (GetSession().GameState.CachedPlayers.ContainsKey(playerGuid))
		{
			data.Level = GetSession().GameState.CachedPlayers[playerGuid].Level;
		}
		if (data.Level == 0)
		{
			data.Level = 1;
		}
		GetSession().GameState.UpdatePlayerCache(playerGuid, cache);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool())
		{
			for (var i = 0; i < 5; i++)
			{
				data.DeclinedNames.name[i] = packet.ReadCString();
			}
		}
		data.IsDeleted = false;
		data.AccountID = GetSession().GetGameAccountGuidForPlayer(playerGuid);
		data.BnetAccountID = GetSession().GetBnetAccountGuidForPlayer(playerGuid);
		data.VirtualRealmAddress = GetSession().RealmId.GetAddress();

		// Use plural format for 3.4.3 (singular opcode doesn't exist)
		if (ModernVersion.GetCurrentOpcode(Opcode.SMSG_QUERY_PLAYER_NAME_RESPONSE) != 0)
		{
			var response = new QueryPlayerNameResponse
			{
				Player = playerGuid,
				Result = 0,
				Data = data
			};
			SendPacketToClient(response);
		}
		else
		{
			var response = new QueryPlayerNamesResponse();
			response.Players.Add(new QueryPlayerNamesResponse.NameCacheLookupResult
			{
				Player = playerGuid,
				Result = 0,
				Data = data
			});
			SendPacketToClient(response);
		}
	}

	[PacketHandler(Opcode.SMSG_LOGIN_VERIFY_WORLD)]
	private void HandleLoginVerifyWorld(WorldPacket packet)
	{
		// Only reset buffer on first login, not on teleports
		// Teleports don't send a new player CreateObject so _playerObjectSent would never become true
		if (!GetSession().GameState.IsInWorld)
		{
			UpdateObject.ResetLoginBuffer(GetSession().GameState);
		}
		var verify = new LoginVerifyWorld
		{
			MapID = packet.ReadUInt32()
		};
		GetSession().GameState.CurrentMapId = verify.MapID;
		verify.Pos.X = packet.ReadFloat();
		verify.Pos.Y = packet.ReadFloat();
		verify.Pos.Z = packet.ReadFloat();
		verify.Pos.Orientation = packet.ReadFloat();
		Log.Print(LogType.Server, $"[LoginVerifyWorld] Map={verify.MapID} Pos=({verify.Pos.X}, {verify.Pos.Y}, {verify.Pos.Z}) Orient={verify.Pos.Orientation}", "CharacterHandler.cs");
		SendPacketToClient(verify);
		GetSession().GameState.IsInWorld = true;
		if (ModernVersion.ExpansionVersion >= 3)
		{
			var worldStates = new EmptyInitWorldStates
			{
				MapId = verify.MapID,
				ZoneId = 0,
				AreaId = 0
			};
			SendPacketToClient(worldStates);
		}
		var info = new WorldServerInfo();
		if (verify.MapID > 1)
		{
			info.DifficultyID = 1u;
			info.InstanceGroupSize = 5u;
		}
		SendPacketToClient(info);
		if (ModernVersion.ExpansionVersion < 3)
		{
			var tasks = new SetAllTaskProgress();
			SendPacketToClient(tasks);
		}
		var setup = new InitialSetup
		{
			ServerExpansionLevel = (byte)(LegacyVersion.ExpansionVersion - 1)
		};
		SendPacketToClient(setup);
		var cuf = new LoadCUFProfiles
		{
			Data = GetSession().AccountDataMgr.LoadCUFProfiles()
		};
		SendPacketToClient(cuf);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			SendPacketToClient(new EmptyAllAchievementData());
			SendPacketToClient(new EmptyAllAccountCriteria());
			SendPacketToClient(new EmptySetupCurrency());
			SendPacketToClient(new EmptySpellHistory());
			SendPacketToClient(new EmptySpellCharges());
			SendPacketToClient(new EmptyTalentData());
			SendPacketToClient(new EmptyActiveGlyphs());
			SendPacketToClient(new EmptyEquipmentSetList());
			SendPacketToClient(new EmptyAccountMountUpdate());
			SendPacketToClient(new EmptyAccountToyUpdate());
			SendPacketToClient(new EmptyAccountHeirloomUpdate());
			SendPacketToClient(new BattlePetJournalLockAcquired());
			var phaseShift = new PhaseShiftChange
			{
				Client = GetSession().GameState.CurrentPlayerGuid
			};
			SendPacketToClient(phaseShift);
		}
	}

	[PacketHandler(Opcode.SMSG_CHARACTER_LOGIN_FAILED)]
	private void HandleCharacterLoginFailed(WorldPacket packet)
	{
		var failed = new CharacterLoginFailed
		{
			Code = (LoginFailureReason)packet.ReadUInt8()
		};
		SendPacketToClient(failed);
		GetSession().GameState.IsInWorld = false;
	}

	[PacketHandler(Opcode.SMSG_UPDATE_ACTION_BUTTONS)]
	private void HandleUpdateActionButtons(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			var type = packet.ReadUInt8();
			if (type == 2)
			{
				return;
			}
		}
		var buttons = new List<int>();
		var buttonCount = 120;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			buttonCount = 144;
		}
		else if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			buttonCount = 132;
		}
		for (var i = 0; i < buttonCount; i++)
		{
			var packed = packet.ReadInt32();
			buttons.Add(packed);
		}
		while (buttons.Count < 180)
		{
			buttons.Add(0);
		}
		GetSession().GameState.ActionButtons = buttons;
		var updateButtons = new UpdateActionButtons
		{
			ActionButtons = buttons,
			Reason = 0
		};
		SendPacketToClient(updateButtons);
	}

	[PacketHandler(Opcode.SMSG_LOGOUT_RESPONSE)]
	private void HandleLogoutResponse(WorldPacket packet)
	{
		var logout = new LogoutResponse
		{
			LogoutResult = packet.ReadInt32(),
			Instant = packet.ReadBool()
		};
		SendPacketToClient(logout);
	}

	[PacketHandler(Opcode.SMSG_LOGOUT_COMPLETE)]
	private void HandleLogoutComplete(WorldPacket packet)
	{
		var logout = new LogoutComplete();
		SendPacketToClient(logout);
		GetSession().GameState = GameSessionData.CreateNewGameSessionData(GetSession());
		GetSession().InstanceSocket.CloseSocket();
		GetSession().InstanceSocket = null;
	}

	[PacketHandler(Opcode.SMSG_LOGOUT_CANCEL_ACK)]
	private void HandleLogoutCancelAck(WorldPacket packet)
	{
		var logout = new LogoutCancelAck();
		SendPacketToClient(logout);
	}

	[PacketHandler(Opcode.SMSG_LOG_XP_GAIN)]
	private void HandleLogXPGain(WorldPacket packet)
	{
		var log = new LogXPGain
		{
			Victim = packet.ReadGuid().To128(GetSession().GameState),
			Original = packet.ReadInt32(),
			Reason = (PlayerLogXPReason)packet.ReadUInt8()
		};
		if (log.Reason == PlayerLogXPReason.Kill)
		{
			log.Amount = packet.ReadInt32();
			log.GroupBonus = packet.ReadFloat();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089) && packet.CanRead())
		{
			log.RAFBonus = packet.ReadUInt8();
		}
		SendPacketToClient(log);
	}

	[PacketHandler(Opcode.SMSG_PLAYED_TIME)]
	private void HandlePlayedTime(WorldPacket packet)
	{
		var played = new PlayedTime
		{
			TotalTime = packet.ReadUInt32(),
			LevelTime = packet.ReadUInt32()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			played.TriggerEvent = packet.ReadBool();
		}
		else
		{
			played.TriggerEvent = GetSession().GameState.ShowPlayedTime;
		}
		SendPacketToClient(played);
	}

	[PacketHandler(Opcode.SMSG_LEVEL_UP_INFO)]
	private void HandleLevelUpInfo(WorldPacket packet)
	{
		var info = new LevelUpInfo
		{
			Level = packet.ReadInt32(),
			HealthDelta = packet.ReadInt32()
		};
		for (var i = 0; i < LegacyVersion.GetPowersCount(); i++)
		{
			info.PowerDelta[i] = packet.ReadInt32();
		}
		for (var j = 0; j < 5; j++)
		{
			info.StatDelta[j] = packet.ReadInt32();
		}
		SendPacketToClient(info);
	}

	[PacketHandler(Opcode.SMSG_UPDATE_COMBO_POINTS)]
	private void HandleUpdateComboPoints(WorldPacket packet)
	{
		var updateData = new ObjectUpdate(GetSession().GameState.CurrentPlayerGuid, UpdateTypeModern.Values, GetSession())
			{
				ActivePlayerData =
				{
					ComboTarget = packet.ReadPackedGuid().To128(GetSession().GameState)
				}
			};
		var comboPoints = packet.ReadUInt8();
		var powerSlot = ClassPowerTypes.GetPowerSlotForClass(GetSession().GameState.GetUnitClass(GetSession().GameState.CurrentPlayerGuid), PowerType.ComboPoints);
		if (powerSlot >= 0)
		{
			updateData.UnitData.Power[powerSlot] = comboPoints;
		}
		var updatePacket = new UpdateObject(GetSession().GameState);
		updatePacket.ObjectUpdates.Add(updateData);
		SendPacketToClient(updatePacket);
	}

	[PacketHandler(Opcode.SMSG_INSPECT_RESULT)]
	[PacketHandler(Opcode.SMSG_INSPECT_TALENT)]
	private void HandleInspectResult(WorldPacket packet)
	{
		var inspect = new InspectResult();
		if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_INSPECT_RESULT)
		{
			inspect.DisplayInfo.GUID = packet.ReadGuid().To128(GetSession().GameState);
		}
		else
		{
			inspect.DisplayInfo.GUID = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		if (!GetSession().GameState.CachedPlayers.TryGetValue(inspect.DisplayInfo.GUID, out var cache))
		{
			return;
		}
		inspect.DisplayInfo.Name = cache.Name;
		inspect.DisplayInfo.ClassId = cache.ClassId;
		inspect.DisplayInfo.RaceId = cache.RaceId;
		inspect.DisplayInfo.SexId = cache.SexId;
		var updates = GetSession().GameState.GetCachedObjectFieldsLegacy(inspect.DisplayInfo.GUID);
		if (updates != null)
		{
			var PLAYER_VISIBLE_ITEM_1_0 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_0);
			if (PLAYER_VISIBLE_ITEM_1_0 >= 0)
			{
				var offset = (byte)(LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 16u : 12u);
				for (byte i = 0; i < 19; i++)
				{
					if (updates.ContainsKey(PLAYER_VISIBLE_ITEM_1_0 + i * offset))
					{
						var itemId = updates[PLAYER_VISIBLE_ITEM_1_0 + i * offset].UInt32Value;
						if (itemId != 0)
						{
							var itemData = new InspectItemData
							{
								Index = i,
								Item =
								{
									ItemID = itemId
								}
							};
							inspect.DisplayInfo.Items.Add(itemData);
						}
					}
				}
			}
			var PLAYER_VISIBLE_ITEM_1_ENTRYID = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_ENTRYID);
			if (PLAYER_VISIBLE_ITEM_1_ENTRYID >= 0)
			{
				var offset2 = 2;
				for (byte i2 = 0; i2 < 19; i2++)
				{
					if (updates.ContainsKey(PLAYER_VISIBLE_ITEM_1_ENTRYID + i2 * offset2))
					{
						var itemId2 = updates[PLAYER_VISIBLE_ITEM_1_ENTRYID + i2 * offset2].UInt32Value;
						if (itemId2 != 0)
						{
							var itemData2 = new InspectItemData
							{
								Index = i2,
								Item =
								{
									ItemID = itemId2
								}
							};
							inspect.DisplayInfo.Items.Add(itemData2);
						}
					}
				}
			}
			var PLAYER_GUILDID = LegacyVersion.GetUpdateField(PlayerField.PLAYER_GUILDID);
			if (PLAYER_GUILDID >= 0 && updates.ContainsKey(PLAYER_GUILDID))
			{
				inspect.GuildData = new InspectGuildData
				{
					GuildGUID = WowGuid128.Create(HighGuidType703.Guild, updates[PLAYER_GUILDID].UInt32Value)
				};
			}
			var PLAYER_FIELD_BYTES = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BYTES);
			if (PLAYER_FIELD_BYTES >= 0 && updates.ContainsKey(PLAYER_FIELD_BYTES))
			{
				inspect.LifetimeMaxRank = (byte)((updates[PLAYER_FIELD_BYTES].UInt32Value >> 24) & 0xFF);
			}
		}
		if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_INSPECT_TALENT)
		{
			var talentsCount = packet.ReadUInt32();
			for (var i3 = 0u; i3 < talentsCount; i3++)
			{
				var talent = packet.ReadUInt8();
				if (i3 < 25)
				{
					inspect.Talents.Add(talent);
				}
			}
		}
		SendPacketToClient(inspect);
	}

	[PacketHandler(Opcode.MSG_INSPECT_HONOR_STATS, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleInspectHonorStatsVanilla(WorldPacket packet)
	{
		var playerGuid = packet.ReadGuid().To128(GetSession().GameState);
		var lifetimeHighestRank = packet.ReadUInt8();
		var todayHonorableKills = packet.ReadUInt16();
		var todayDishonorableKills = packet.ReadUInt16();
		var yesterdayHonorableKills = packet.ReadUInt16();
		var yesterdayDishonorableKills = packet.ReadUInt16();
		var lastWeekHonorableKills = packet.ReadUInt16();
		var lastWeekDishonorableKills = packet.ReadUInt16();
		var thisWeekHonorableKills = packet.ReadUInt16();
		var thisWeekDishonorableKills = packet.ReadUInt16();
		var lifetimeHonorableKills = packet.ReadUInt32();
		var lifetimeDishonorableKills = packet.ReadUInt32();
		var yesterdayHonor = packet.ReadUInt32();
		var lastWeekHonor = packet.ReadUInt32();
		var thisWeekHonor = packet.ReadUInt32();
		var standing = packet.ReadUInt32();
		var rankProgress = packet.ReadUInt8();
		if (ModernVersion.ExpansionVersion == 1)
		{
			var inspect = new InspectHonorStatsResultClassic
			{
				PlayerGUID = playerGuid,
				LifetimeHighestRank = lifetimeHighestRank,
				TodayHonorableKills = todayHonorableKills,
				TodayDishonorableKills = todayDishonorableKills,
				YesterdayHonorableKills = yesterdayHonorableKills,
				YesterdayDishonorableKills = yesterdayDishonorableKills,
				LastWeekHonorableKills = lastWeekHonorableKills,
				LastWeekDishonorableKills = lastWeekDishonorableKills,
				ThisWeekHonorableKills = thisWeekHonorableKills,
				ThisWeekDishonorableKills = thisWeekDishonorableKills,
				LifetimeHonorableKills = lifetimeHonorableKills,
				LifetimeDishonorableKills = lifetimeDishonorableKills,
				YesterdayHonor = yesterdayHonor,
				LastWeekHonor = lastWeekHonor,
				ThisWeekHonor = thisWeekHonor,
				Standing = standing,
				RankProgress = rankProgress
			};
			SendPacketToClient(inspect);
		}
		else
		{
			var inspect2 = new InspectHonorStatsResultTBC
			{
				PlayerGUID = playerGuid,
				LifetimeHighestRank = lifetimeHighestRank,
				YesterdayHonorableKills = yesterdayHonorableKills,
				LifetimeHonorableKills = (ushort)lifetimeHonorableKills
			};
			SendPacketToClient(inspect2);
		}
	}

	[PacketHandler(Opcode.MSG_INSPECT_HONOR_STATS, ClientVersionBuild.V2_0_1_6180)]
	private void HandleInspectHonorStatsTBC(WorldPacket packet)
	{
		var playerGuid = packet.ReadGuid().To128(GetSession().GameState);
		var lifetimeHighestRank = packet.ReadUInt8();
		var todayHonorableKills = packet.ReadUInt16();
		var yesterdayHonorableKills = packet.ReadUInt16();
		var todayHonor = packet.ReadUInt32();
		var yesterdayHonor = packet.ReadUInt32();
		var lifetimeHonorableKills = packet.ReadUInt32();
		if (ModernVersion.ExpansionVersion == 1)
		{
			var inspect = new InspectHonorStatsResultClassic
			{
				PlayerGUID = playerGuid,
				LifetimeHighestRank = lifetimeHighestRank,
				TodayHonorableKills = todayHonorableKills,
				YesterdayHonorableKills = yesterdayHonorableKills,
				LifetimeHonorableKills = lifetimeHonorableKills,
				YesterdayHonor = yesterdayHonor,
				LastWeekHonor = todayHonor
			};
			SendPacketToClient(inspect);
		}
		else
		{
			var inspect2 = new InspectHonorStatsResultTBC
			{
				PlayerGUID = playerGuid,
				LifetimeHighestRank = lifetimeHighestRank,
				YesterdayHonorableKills = yesterdayHonorableKills,
				LifetimeHonorableKills = (ushort)lifetimeHonorableKills
			};
			SendPacketToClient(inspect2);
		}
	}

	[PacketHandler(Opcode.MSG_INSPECT_ARENA_TEAMS)]
	private void HandleInspectArenaTeams(WorldPacket packet)
	{
		var inspect = new InspectPvP
		{
			PlayerGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		var team = new ArenaTeamInspectData();
		var slot = packet.ReadUInt8();
		var teamId = packet.ReadUInt32();
		team.TeamGuid = WowGuid128.Create(HighGuidType703.ArenaTeam, teamId);
		team.TeamRating = packet.ReadInt32();
		team.TeamGamesPlayed = packet.ReadInt32();
		team.TeamGamesWon = packet.ReadInt32();
		team.PersonalGamesPlayed = packet.ReadInt32();
		team.PersonalRating = packet.ReadInt32();
		GetSession().GameState.StoreArenaTeamDataForPlayer(inspect.PlayerGUID, slot, team);
		for (byte i = 0; i < 3; i++)
		{
			inspect.ArenaTeams.Add(GetSession().GameState.GetArenaTeamDataForPlayer(inspect.PlayerGUID, slot));
		}
		SendPacketToClient(inspect);
	}

	[PacketHandler(Opcode.SMSG_CHARACTER_RENAME_RESULT)]
	private void HandleCharacterRenameResult(WorldPacket packet)
	{
		var result = packet.ReadUInt8();
		var rename = new CharacterRenameResult
		{
			Result = ModernVersion.ConvertResponseCodesValue(result)
		};
		if (rename.Result == 0)
		{
			rename.Guid = packet.ReadGuid().To128(GetSession().GameState);
			rename.Name = packet.ReadCString();
		}
		SendPacketToClient(rename);
	}

	[PacketHandler(Opcode.SMSG_CHANNEL_NOTIFY)]
	private void HandleChannelNotify(WorldPacket packet)
	{
		var type = (ChatNotify)packet.ReadUInt8();
		if (type == ChatNotify.InvalidName)
		{
			packet.ReadBytes(3u);
		}
		var channelName = packet.ReadCString();
		switch (type)
		{
		case ChatNotify.Joined:
		case ChatNotify.Left:
		case ChatNotify.PasswordChanged:
		case ChatNotify.OwnerChanged:
		case ChatNotify.AnnouncementsOn:
		case ChatNotify.AnnouncementsOff:
		case ChatNotify.ModerationOn:
		case ChatNotify.ModerationOff:
		case ChatNotify.PlayerAlreadyMember:
		case ChatNotify.Invite:
		case ChatNotify.VoiceOn:
		case ChatNotify.VoiceOff:
			packet.ReadGuid();
			break;
		case ChatNotify.YouJoined:
		{
			var flags = (ChannelFlags)((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? packet.ReadUInt32() : packet.ReadUInt8());
			var channelId = packet.ReadInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				packet.ReadInt32();
			}
			if (channelId == 0)
			{
				channelId = (int)GameData.GetChatChannelIdFromName(channelName);
			}
			GetSession().GameState.SetChannelId(channelName, channelId);
			var joined = new ChannelNotifyJoined
			{
				Channel = channelName,
				ChannelFlags = flags,
				ChatChannelID = channelId,
				ChannelGUID = WowGuid128.Create(HighGuidType703.ChatChannel, GetSession().GameState.CurrentMapId.Value, GetSession().GameState.CurrentZoneId, (ulong)channelId)
			};
			SendPacketToClient(joined);
			break;
		}
		case ChatNotify.YouLeft:
		{
			var left = new ChannelNotifyLeft
			{
				Channel = channelName
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				left.ChatChannelID = packet.ReadInt32();
				left.Suspended = packet.ReadBool();
			}
			else
			{
				left.ChatChannelID = GetSession().GameState.ChannelIds[channelName];
				left.Suspended = false;
			}
			if (string.Equals(GetSession().GameState.LeftChannelName, channelName) || GameData.GetChatChannelIdFromName(channelName) == 0)
			{
				SendPacketToClient(left);
			}
			break;
		}
		case ChatNotify.PlayerNotFound:
		case ChatNotify.ChannelOwner:
		case ChatNotify.PlayerNotBanned:
		case ChatNotify.PlayerInvited:
		case ChatNotify.PlayerInviteBanned:
			packet.ReadCString();
			break;
		case ChatNotify.ModeChange:
			packet.ReadGuid();
			packet.ReadUInt8();
			packet.ReadUInt8();
			break;
		case ChatNotify.PlayerKicked:
		case ChatNotify.PlayerBanned:
		case ChatNotify.PlayerUnbanned:
			packet.ReadGuid();
			packet.ReadGuid();
			break;
		case ChatNotify.TrialRestricted:
			packet.ReadGuid();
			break;
		case ChatNotify.WrongPassword:
		case ChatNotify.NotMember:
		case ChatNotify.NotModerator:
		case ChatNotify.NotOwner:
		case ChatNotify.Muted:
		case ChatNotify.Banned:
		case ChatNotify.InviteWrongFaction:
		case ChatNotify.WrongFaction:
		case ChatNotify.InvalidName:
		case ChatNotify.NotModerated:
		case ChatNotify.Throttled:
		case ChatNotify.NotInArea:
		case ChatNotify.NotInLfg:
			break;
		}
	}

	[PacketHandler(Opcode.SMSG_CHANNEL_LIST)]
	private void HandleChannelList(WorldPacket packet)
	{
		var list = new ChannelListResponse();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			list.Display = packet.ReadBool();
		}
		else
		{
			list.Display = GetSession().GameState.ChannelDisplayList;
		}
		list.ChannelName = packet.ReadCString();
		list.ChannelFlags = (ChannelFlags)packet.ReadUInt8();
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var member = new ChannelListResponse.ChannelPlayer
			{
				Guid = packet.ReadGuid().To128(GetSession().GameState),
				VirtualRealmAddress = GetSession().RealmId.GetAddress(),
				Flags = packet.ReadUInt8()
			};
			list.Members.Add(member);
		}
		SendPacketToClient(list);
	}

	[PacketHandler(Opcode.SMSG_CHAT, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleServerChatMessageVanilla(WorldPacket packet)
	{
		var chatType = (ChatMessageTypeVanilla)packet.ReadUInt8();
		var language = packet.ReadUInt32();
		var senderName = "";
		WowGuid128 sender = null;
		WowGuid128 receiver = null;
		var channelName = "";
		switch (chatType)
		{
		case ChatMessageTypeVanilla.MonsterEmote:
		case ChatMessageTypeVanilla.MonsterWhisper:
		case ChatMessageTypeVanilla.RaidBossEmote:
			packet.ReadUInt32();
			senderName = packet.ReadCString();
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			break;
		case ChatMessageTypeVanilla.Say:
		case ChatMessageTypeVanilla.Party:
		case ChatMessageTypeVanilla.Yell:
			sender = packet.ReadGuid().To128(GetSession().GameState);
			packet.ReadGuid();
			break;
		case ChatMessageTypeVanilla.MonsterSay:
		case ChatMessageTypeVanilla.MonsterYell:
			sender = packet.ReadGuid().To128(GetSession().GameState);
			packet.ReadUInt32();
			senderName = packet.ReadCString();
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			break;
		case ChatMessageTypeVanilla.Channel:
			channelName = packet.ReadCString();
			packet.ReadUInt32();
			sender = packet.ReadGuid().To128(GetSession().GameState);
			break;
		default:
			sender = packet.ReadGuid().To128(GetSession().GameState);
			break;
		}
		var chatMessageTypeVanilla = chatType;
		var chatMessageTypeVanilla2 = chatMessageTypeVanilla;
		if (chatMessageTypeVanilla2 - 83 <= ChatMessageTypeVanilla.Party)
		{
			Utility.Swap(ref sender, ref receiver);
		}
		var textLength = packet.ReadUInt32();
		var text = packet.ReadString(textLength);
		var chatTag = (ChatTag)packet.ReadUInt8();
		var chatFlags = (ChatFlags)Enum.Parse(typeof(ChatFlags), chatTag.ToString());
		if (Session.GameState.IgnoredPlayers.Contains(sender) && !chatFlags.HasFlag(ChatFlags.GM) && chatType != ChatMessageTypeVanilla.Ignored)
		{
			if (chatType == ChatMessageTypeVanilla.Whisper)
			{
				var ignoreResponsePacket = new WorldPacket(Opcode.CMSG_CHAT_REPORT_IGNORED);
				ignoreResponsePacket.WriteGuid(sender.To64());
				SendPacketToServer(ignoreResponsePacket);
			}
			return;
		}
		var addonPrefix = "";
		if (ChatPkt.CheckAddonPrefix(GetSession().GameState.AddonPrefixes, ref language, ref text, ref addonPrefix))
		{
			var chatTypeModern = (ChatMessageTypeModern)Enum.Parse(typeof(ChatMessageTypeModern), chatType.ToString());
			var chat = new ChatPkt(GetSession(), chatTypeModern, text, language, sender, senderName, receiver, "", channelName, chatFlags, addonPrefix);
			SendPacketToClient(chat);
		}
	}

	[PacketHandler(Opcode.SMSG_CHAT, ClientVersionBuild.V2_0_1_6180)]
	[PacketHandler(Opcode.SMSG_GM_MESSAGECHAT, ClientVersionBuild.V2_0_1_6180)]
	private void HandleServerChatMessageWotLK(WorldPacket packet)
	{
		var chatType = (ChatMessageTypeWotLK)packet.ReadUInt8();
		var language = packet.ReadUInt32();
		var sender = packet.ReadGuid().To128(GetSession().GameState);
		var senderName = "";
		var receiverName = "";
		var channelName = "";
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_1_0_6692))
		{
			packet.ReadInt32();
		}
		WowGuid128 receiver;
		switch (chatType)
		{
		case ChatMessageTypeWotLK.Achievement:
		case ChatMessageTypeWotLK.GuildAchievement:
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			break;
		case ChatMessageTypeWotLK.WhisperForeign:
		{
			var senderNameLength3 = packet.ReadUInt32();
			senderName = packet.ReadString(senderNameLength3);
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			break;
		}
		case ChatMessageTypeWotLK.BattlegroundNeutral:
		case ChatMessageTypeWotLK.BattlegroundAlliance:
		case ChatMessageTypeWotLK.BattlegroundHorde:
		{
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			var highType = receiver.GetHighType();
			var highGuidType = highType;
			if (highGuidType == HighGuidType.Transport || (uint)(highGuidType - 9) <= 3u)
			{
				var senderNameLength2 = packet.ReadUInt32();
				senderName = packet.ReadString(senderNameLength2);
			}
			break;
		}
		case ChatMessageTypeWotLK.MonsterSay:
		case ChatMessageTypeWotLK.MonsterParty:
		case ChatMessageTypeWotLK.MonsterYell:
		case ChatMessageTypeWotLK.MonsterWhisper:
		case ChatMessageTypeWotLK.MonsterEmote:
		case ChatMessageTypeWotLK.RaidBossEmote:
		case ChatMessageTypeWotLK.RaidBossWhisper:
		case ChatMessageTypeWotLK.BattleNet:
		{
			var senderNameLength = packet.ReadUInt32();
			senderName = packet.ReadString(senderNameLength);
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			switch (receiver.GetHighType())
			{
			case HighGuidType.Transport:
			case HighGuidType.Creature:
			case HighGuidType.Vehicle:
			case HighGuidType.GameObject:
			{
				var receiverNameLength = packet.ReadUInt32();
				receiverName = packet.ReadString(receiverNameLength);
				break;
			}
			}
			break;
		}
		default:
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_GM_MESSAGECHAT)
			{
				var gmNameLength = packet.ReadUInt32();
				packet.ReadString(gmNameLength);
			}
			if (chatType == ChatMessageTypeWotLK.Channel)
			{
				channelName = packet.ReadCString();
			}
			receiver = packet.ReadGuid().To128(GetSession().GameState);
			break;
		}
		var chatMessageTypeWotLK = chatType;
		var chatMessageTypeWotLK2 = chatMessageTypeWotLK;
		if (chatMessageTypeWotLK2 - 37 <= ChatMessageTypeWotLK.Say)
		{
			Utility.Swap(ref sender, ref receiver);
		}
		var textLength = packet.ReadUInt32();
		var text = packet.ReadString(textLength);
		var chatFlags = (ChatFlags)packet.ReadUInt8();
		if (LegacyVersion.InVersion(ClientVersionBuild.V2_0_1_6180, ClientVersionBuild.V3_0_2_9056) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_GM_MESSAGECHAT)
		{
			var gmNameLength2 = packet.ReadUInt32();
			packet.ReadString(gmNameLength2);
		}
		var achievementId = 0u;
		if (chatType == ChatMessageTypeWotLK.Achievement || chatType == ChatMessageTypeWotLK.GuildAchievement)
		{
			achievementId = packet.ReadUInt32();
		}
		if (Session.GameState.IgnoredPlayers.Contains(sender) && !chatFlags.HasFlag(ChatFlags.GM) && chatType != ChatMessageTypeWotLK.Ignored)
		{
			if (chatType == ChatMessageTypeWotLK.Whisper)
			{
				var ignoreResponsePacket = new WorldPacket(Opcode.CMSG_CHAT_REPORT_IGNORED);
				ignoreResponsePacket.WriteGuid(sender.To64());
				ignoreResponsePacket.WriteUInt8(0);
				SendPacketToServer(ignoreResponsePacket);
			}
		}
		else
		{
			var addonPrefix = "";
			if (ChatPkt.CheckAddonPrefix(GetSession().GameState.AddonPrefixes, ref language, ref text, ref addonPrefix))
			{
				var chatTypeModern = (ChatMessageTypeModern)Enum.Parse(typeof(ChatMessageTypeModern), chatType.ToString());
				var chat = new ChatPkt(GetSession(), chatTypeModern, text, language, sender, senderName, receiver, receiverName, channelName, chatFlags, addonPrefix, achievementId);
				SendPacketToClient(chat);
			}
		}
	}

	public void SendMessageChatVanilla(ChatMessageTypeVanilla type, uint lang, string msg, string channel, string to)
	{
		if (!HandleHermesInternalChatCommand(msg))
		{
			var packet = new WorldPacket(Opcode.CMSG_MESSAGECHAT);
			packet.WriteUInt32((uint)type);
			packet.WriteUInt32(lang);
			switch (type)
			{
			case ChatMessageTypeVanilla.Channel:
				packet.WriteCString(channel);
				packet.WriteCString(msg);
				break;
			case ChatMessageTypeVanilla.Whisper:
				packet.WriteCString(to);
				packet.WriteCString(msg);
				break;
			case ChatMessageTypeVanilla.Say:
			case ChatMessageTypeVanilla.Party:
			case ChatMessageTypeVanilla.Raid:
			case ChatMessageTypeVanilla.Guild:
			case ChatMessageTypeVanilla.Officer:
			case ChatMessageTypeVanilla.Yell:
			case ChatMessageTypeVanilla.Emote:
			case ChatMessageTypeVanilla.Afk:
			case ChatMessageTypeVanilla.Dnd:
			case ChatMessageTypeVanilla.RaidLeader:
			case ChatMessageTypeVanilla.RaidWarning:
			case ChatMessageTypeVanilla.Battleground:
			case ChatMessageTypeVanilla.BattlegroundLeader:
				packet.WriteCString(msg);
				break;
			}
			SendPacket(packet);
		}
	}

	private bool HandleHermesInternalChatCommand(string msg)
	{
		if (msg.StartsWith("!qcomplete"))
		{
			var questIdStr = msg.Remove(0, "!qcomplete".Length);
			if (!uint.TryParse(questIdStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var questId))
			{
				GetSession().SendHermesTextMessage("Chat command invalid questId format '" + questIdStr + "'");
				return true;
			}
			GetSession().GameState.CurrentPlayerStorage.CompletedQuests.MarkQuestAsCompleted(questId);
			return true;
		}
		if (msg.StartsWith("!quncomplete"))
		{
			var questIdStr2 = msg.Remove(0, "!quncomplete".Length);
			if (!uint.TryParse(questIdStr2, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var questId2))
			{
				GetSession().SendHermesTextMessage("Chat command invalid questId format '" + questIdStr2 + "'");
				return true;
			}
			GetSession().GameState.CurrentPlayerStorage.CompletedQuests.MarkQuestAsNotCompleted(questId2);
			return true;
		}
		return false;
	}

	public void SendMessageChatWotLK(ChatMessageTypeWotLK type, uint lang, string msg, string channel, string to)
	{
		if (!HandleHermesInternalChatCommand(msg))
		{
			var packet = new WorldPacket(Opcode.CMSG_MESSAGECHAT);
			packet.WriteUInt32((uint)type);
			packet.WriteUInt32(lang);
			switch (type)
			{
			case ChatMessageTypeWotLK.Channel:
				packet.WriteCString(channel);
				packet.WriteCString(msg);
				break;
			case ChatMessageTypeWotLK.Whisper:
				packet.WriteCString(to);
				packet.WriteCString(msg);
				break;
			case ChatMessageTypeWotLK.Say:
			case ChatMessageTypeWotLK.Party:
			case ChatMessageTypeWotLK.Raid:
			case ChatMessageTypeWotLK.Guild:
			case ChatMessageTypeWotLK.Officer:
			case ChatMessageTypeWotLK.Yell:
			case ChatMessageTypeWotLK.Emote:
			case ChatMessageTypeWotLK.Afk:
			case ChatMessageTypeWotLK.Dnd:
			case ChatMessageTypeWotLK.RaidLeader:
			case ChatMessageTypeWotLK.RaidWarning:
			case ChatMessageTypeWotLK.Battleground:
			case ChatMessageTypeWotLK.BattlegroundLeader:
			case ChatMessageTypeWotLK.PartyLeader:
				packet.WriteCString(msg);
				break;
			}
			SendPacket(packet);
		}
	}

	[PacketHandler(Opcode.SMSG_EMOTE)]
	private void HandleEmote(WorldPacket packet)
	{
		var emote = new EmoteMessage
		{
			EmoteID = packet.ReadUInt32(),
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(emote);
	}

	[PacketHandler(Opcode.SMSG_TEXT_EMOTE)]
	private void HandleTextEmote(WorldPacket packet)
	{
		var emote = new STextEmote
		{
			SourceGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		emote.SourceAccountGUID = GetSession().GetGameAccountGuidForPlayer(emote.SourceGUID);
		emote.EmoteID = packet.ReadInt32();
		emote.SoundIndex = packet.ReadInt32();
		var nameLength = packet.ReadUInt32();
		var targetName = packet.ReadString(nameLength);
		var targetGuid = GetSession().GameState.GetPlayerGuidByName(targetName);
		emote.TargetGUID = ((targetGuid != null) ? targetGuid : WowGuid128.Empty);
		SendPacketToClient(emote);
	}

	[PacketHandler(Opcode.SMSG_PRINT_NOTIFICATION)]
	private void HandlePrintNotification(WorldPacket packet)
	{
		var notify = new PrintNotification
		{
			NotifyText = packet.ReadCString()
		};
		SendPacketToClient(notify);
	}

	[PacketHandler(Opcode.SMSG_CHAT_PLAYER_NOTFOUND)]
	private void HandleChatPlayerNotFound(WorldPacket packet)
	{
		var error = new ChatPlayerNotfound
		{
			Name = packet.ReadCString()
		};
		SendPacketToClient(error);
	}

	[PacketHandler(Opcode.SMSG_DEFENSE_MESSAGE)]
	private void HandleDefenseMessage(WorldPacket packet)
	{
		var message = new DefenseMessage
		{
			ZoneID = packet.ReadUInt32()
		};
		packet.ReadUInt32();
		message.MessageText = packet.ReadCString();
		SendPacketToClient(message);
	}

	[PacketHandler(Opcode.SMSG_CHAT_SERVER_MESSAGE)]
	private void HandleChatServerMessage(WorldPacket packet)
	{
		var message = new ChatServerMessage
		{
			MessageID = packet.ReadInt32(),
			StringParam = packet.ReadCString()
		};
		SendPacketToClient(message);
	}

	public void SendChatJoinChannel(int channelId, string channelName, string password)
	{
		var packet = new WorldPacket(Opcode.CMSG_CHAT_JOIN_CHANNEL);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteInt32(channelId);
			packet.WriteUInt8(0);
			packet.WriteUInt8(0);
		}
		packet.WriteCString(channelName);
		packet.WriteCString(password);
		SendPacketToServer(packet);
	}

	public void SendChatLeaveChannel(int channelId, string channelName)
	{
		var packet = new WorldPacket(Opcode.CMSG_CHAT_LEAVE_CHANNEL);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteInt32(channelId);
		}
		packet.WriteCString(channelName);
		SendPacketToServer(packet);
	}

	private bool IsLocalPlayerOrPet(WowGuid128 guid)
	{
		if (guid == null) return false;
		return guid == GetSession().GameState.CurrentPlayerGuid ||
		       guid == GetSession().GameState.CurrentPetGuid;
	}

	private bool IsLocalPlayerInvolved(WowGuid128 a, WowGuid128 b)
	{
		return IsLocalPlayerOrPet(a) || IsLocalPlayerOrPet(b);
	}

	[PacketHandler(Opcode.SMSG_ATTACK_START)]
	private void HandleAttackStart(WorldPacket packet)
	{
		var attack = new SAttackStart
		{
			Attacker = packet.ReadGuid().To128(GetSession().GameState),
			Victim = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_ATTACK_STOP)]
	private void HandleAttackStop(WorldPacket packet)
	{
		var attack = new SAttackStop();
		if (packet.CanRead())
		{
			attack.Attacker = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		if (packet.CanRead())
		{
			attack.Victim = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		if (packet.CanRead())
		{
			attack.NowDead = packet.ReadUInt32() != 0;
		}
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_HIGHEST_THREAT_UPDATE)]
	private void HandleHighestThreatUpdate(WorldPacket packet)
	{
		// Consume packet to prevent "No handler" warning — client doesn't need this
		var unitGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
		Log.Print(LogType.Debug, $"[Combat] HIGHEST_THREAT_UPDATE unit={unitGuid} (consumed, not forwarded)", "WorldClient.cs");
	}

	[PacketHandler(Opcode.SMSG_THREAT_CLEAR)]
	private void HandleThreatClear(WorldPacket packet)
	{
		// Consume packet to prevent "No handler" warning — client doesn't need this
		var unitGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
		Log.Print(LogType.Debug, $"[Combat] THREAT_CLEAR unit={unitGuid} (consumed, not forwarded)", "WorldClient.cs");
	}

	[PacketHandler(Opcode.SMSG_THREAT_UPDATE)]
	private void HandleThreatUpdate(WorldPacket packet)
	{
		// Consume packet to prevent "No handler" warning — client doesn't need this
		var unitGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
		Log.Print(LogType.Debug, $"[Combat] THREAT_UPDATE unit={unitGuid} (consumed, not forwarded)", "WorldClient.cs");
	}

	[PacketHandler(Opcode.SMSG_THREAT_REMOVE)]
	private void HandleThreatRemove(WorldPacket packet)
	{
		var threat = new ThreatRemove
		{
			UnitGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			AboutGUID = packet.ReadPackedGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(threat);
	}

	[PacketHandler(Opcode.SMSG_HEALTH_UPDATE)]
	private void HandleHealthUpdate(WorldPacket packet)
	{
		var health = new HealthUpdate
		{
			Guid = packet.ReadPackedGuid().To128(GetSession().GameState),
			Health = packet.ReadUInt32()
		};
		SendPacketToClient(health);
	}

	[PacketHandler(Opcode.SMSG_PET_ACTION_FEEDBACK)]
	private void HandlePetActionFeedback(WorldPacket packet)
	{
		var feedback = new PetActionFeedback
		{
			Response = packet.ReadUInt8(),
			SpellID = 0
		};
		SendPacketToClient(feedback);
	}

	[PacketHandler(Opcode.SMSG_PET_TAME_FAILURE)]
	private void HandlePetTameFailure(WorldPacket packet)
	{
		var tame = new PetTameFailure
		{
			Result = packet.ReadUInt8()
		};
		SendPacketToClient(tame);
	}

	[PacketHandler(Opcode.SMSG_PET_GUIDS)]
	private void HandlePetGuids(WorldPacket packet)
	{
		var guids = new PetGuids();
		var count = packet.ReadUInt32();
		for (uint i = 0; i < count; i++)
		{
			guids.Guids.Add(packet.ReadGuid().To128(GetSession().GameState));
		}
		SendPacketToClient(guids);
	}

	[PacketHandler(Opcode.SMSG_TITLE_EARNED)]
	private void HandleTitleEarned(WorldPacket packet)
	{
		var index = packet.ReadUInt32();
		var earned = packet.ReadUInt32();
		var title = new TitleEarned(earned != 0 ? Opcode.SMSG_TITLE_EARNED : Opcode.SMSG_TITLE_LOST)
		{
			Index = index
		};
		SendPacketToClient(title);
	}

	[PacketHandler(Opcode.SMSG_MOUNT_RESULT)]
	private void HandleMountResult(WorldPacket packet)
	{
		var mount = new MountResult
		{
			Result = packet.ReadInt32()
		};
		SendPacketToClient(mount);
	}

	[PacketHandler(Opcode.SMSG_ACHIEVEMENT_DELETED)]
	private void HandleAchievementDeleted(WorldPacket packet)
	{
		var deleted = new AchievementDeleted
		{
			AchievementID = packet.ReadUInt32(),
			Immunities = 0
		};
		SendPacketToClient(deleted);
	}

	[PacketHandler(Opcode.SMSG_CRITERIA_DELETED)]
	private void HandleCriteriaDeleted(WorldPacket packet)
	{
		var deleted = new CriteriaDeleted
		{
			CriteriaID = packet.ReadUInt32()
		};
		SendPacketToClient(deleted);
	}

	[PacketHandler(Opcode.SMSG_GROUP_DESTROYED)]
	private void HandleGroupDestroyed(WorldPacket packet)
	{
		var destroyed = new GroupDestroyed();
		SendPacketToClient(destroyed);
	}

	[PacketHandler(Opcode.SMSG_ON_CANCEL_EXPECTED_RIDE_VEHICLE_AURA)]
	private void HandleOnCancelExpectedRideVehicleAura(WorldPacket packet)
	{
		var cancel = new OnCancelExpectedRideVehicleAura();
		SendPacketToClient(cancel);
	}

	[PacketHandler(Opcode.SMSG_OVERRIDE_LIGHT)]
	private void HandleOverrideLight(WorldPacket packet)
	{
		var light = new OverrideLight
		{
			AreaLightID = packet.ReadInt32(),
			OverrideLightID = packet.ReadInt32(),
			TransitionMilliseconds = packet.ReadInt32()
		};
		SendPacketToClient(light);
	}

	[PacketHandler(Opcode.SMSG_UPDATE_ACCOUNT_DATA)]
	private void HandleUpdateAccountData(WorldPacket packet)
	{
		var guid = packet.ReadGuid();
		var type = packet.ReadUInt32();
		var time = packet.ReadUInt32();
		var size = packet.ReadUInt32();
		byte[] compressedData = null;
		if (packet.CanRead())
		{
			compressedData = packet.ReadToEnd();
		}
		var data = new AccountData
		{
			Guid = guid.To128(GetSession().GameState),
			Type = type,
			Timestamp = time,
			UncompressedSize = size,
			CompressedData = compressedData
		};
		var update = new UpdateAccountData(data);
		SendPacketToClient(update);
	}

	[PacketHandler(Opcode.SMSG_UPDATE_LAST_INSTANCE)]
	private void HandleUpdateLastInstance(WorldPacket packet)
	{
		var update = new UpdateLastInstance
		{
			MapID = packet.ReadUInt32()
		};
		SendPacketToClient(update);
	}

	[PacketHandler(Opcode.SMSG_QUEST_POI_QUERY_RESPONSE)]
	private void HandleQuestPOIQueryResponse(WorldPacket packet)
	{
		var response = new QuestPOIQueryResponse();
		var questCount = packet.ReadUInt32();
		for (uint q = 0; q < questCount; q++)
		{
			var questData = new QuestPOIData
			{
				QuestID = (int)packet.ReadUInt32()
			};
			var poiCount = packet.ReadUInt32();
			for (uint p = 0; p < poiCount; p++)
			{
				var blob = new QuestPOIBlobData
				{
					BlobIndex = (int)packet.ReadUInt32(),
					ObjectiveIndex = packet.ReadInt32(),
					MapID = (int)packet.ReadUInt32(),
					UiMapID = (int)packet.ReadUInt32(), // areaId in legacy
					Priority = 0,
					Flags = (int)packet.ReadUInt32(), // floorId in legacy
					WorldEffectID = 0,
					PlayerConditionID = 0,
					NavigationPlayerConditionID = 0,
					SpawnTrackingID = 0
				};
				// Look up the QuestObjectiveID from our cached objectives
				// The modern client needs this to link POI blobs to specific objectives
				var poiQuest = GameData.GetQuestTemplate((uint)questData.QuestID);
				if (poiQuest != null)
				{
					var matchedObj = poiQuest.Objectives.Find(o => o.StorageIndex == blob.ObjectiveIndex);
					if (matchedObj != null)
					{
						blob.QuestObjectiveID = (int)matchedObj.Id;
						blob.QuestObjectID = matchedObj.ObjectID;
					}
				}
				else
				{
					blob.QuestObjectiveID = 0;
					blob.QuestObjectID = 0;
				}
				packet.ReadUInt32(); // Unk3
				packet.ReadUInt32(); // Unk4
				var pointCount = packet.ReadUInt32();
				for (uint pt = 0; pt < pointCount; pt++)
				{
					var point = new QuestPOIBlobPoint
					{
						X = (short)packet.ReadInt32(),
						Y = (short)packet.ReadInt32(),
						Z = 0
					};
					blob.Points.Add(point);
				}
				questData.Blobs.Add(blob);
			}
			response.QuestPOIDataStats.Add(questData);
		}
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_GM_TICKET_GET_SYSTEM_STATUS)]
	private void HandleGMTicketSystemStatus(WorldPacket packet)
	{
		var status = new GMTicketSystemStatus
		{
			Status = (int)packet.ReadUInt32()
		};
		SendPacketToClient(status);
	}

	[PacketHandler(Opcode.SMSG_LFG_DISABLED)]
	private void HandleLfgDisabled(WorldPacket packet)
	{
		var disabled = new LfgDisabled();
		SendPacketToClient(disabled);
	}

	[PacketHandler(Opcode.SMSG_LFG_OFFER_CONTINUE)]
	private void HandleLfgOfferContinue(WorldPacket packet)
	{
		var offer = new LfgOfferContinue
		{
			Slot = packet.ReadUInt32()
		};
		SendPacketToClient(offer);
	}

	[PacketHandler(Opcode.SMSG_LFG_PLAYER_REWARD)]
	private void HandleLfgPlayerReward(WorldPacket packet)
	{
		var reward = new LfgPlayerReward
		{
			QueuedSlot = packet.ReadUInt32(), // rdungeonEntry
			ActualSlot = packet.ReadUInt32() // sdungeonEntry
		};
		var done = packet.ReadUInt8();
		packet.ReadUInt32(); // always 1
		reward.RewardMoney = (int)packet.ReadUInt32();
		reward.AddedXP = (int)packet.ReadUInt32();
		packet.ReadUInt32(); // unknown
		packet.ReadUInt32(); // unknown
		var itemNum = packet.ReadUInt8();
		for (byte i = 0; i < itemNum; i++)
		{
			var item = new LfgPlayerRewardItem
			{
				ItemID = packet.ReadUInt32()
			};
			packet.ReadUInt32(); // displayId
			item.Quantity = packet.ReadUInt32();
			item.IsCurrency = false;
			item.BonusCurrency = 0;
			reward.Rewards.Add(item);
		}
		SendPacketToClient(reward);
	}

	[PacketHandler(Opcode.SMSG_LFG_ROLE_CHECK_UPDATE)]
	private void HandleLfgRoleCheckUpdate(WorldPacket packet)
	{
		var roleCheck = new LfgRoleCheckUpdate
		{
			PartyIndex = 0,
			RoleCheckStatus = (byte)packet.ReadUInt32(), // state
			IsBeginning = packet.ReadBool(),
			IsRequeue = false,
			GroupFinderActivityID = 0
		};
		var dungeonCount = packet.ReadUInt8();
		for (byte i = 0; i < dungeonCount; i++)
		{
			roleCheck.JoinSlots.Add(packet.ReadUInt32());
		}
		var memberCount = packet.ReadUInt8();
		for (byte i = 0; i < memberCount; i++)
		{
			var member = new LfgRoleCheckMember
			{
				Guid = packet.ReadGuid().To128(GetSession().GameState)
			};
			var ready = packet.ReadBool();
			member.RolesDesired = packet.ReadUInt32();
			member.Level = packet.ReadUInt8();
			member.RoleCheckComplete = ready;
			roleCheck.Members.Add(member);
		}
		SendPacketToClient(roleCheck);
	}

	[PacketHandler(Opcode.SMSG_LFG_PARTY_INFO)]
	private void HandleLfgPartyInfo(WorldPacket packet)
	{
		var partyInfo = new LfgPartyInfo();
		var playerCount = packet.ReadUInt8();
		for (byte i = 0; i < playerCount; i++)
		{
			var entry = new LfgBlackListEntry
			{
				PlayerGuid = packet.ReadGuid().To128(GetSession().GameState)
			};
			var lockCount = packet.ReadUInt32();
			for (uint j = 0; j < lockCount; j++)
			{
				var lockInfo = new LfgLockInfoData
				{
					Slot = packet.ReadUInt32(), // dungeonId
					LockStatus = packet.ReadUInt32(), // lockStatus
					SubReason1 = 0,
					SubReason2 = 0
				};
				entry.Locks.Add(lockInfo);
			}
			partyInfo.Players.Add(entry);
		}
		SendPacketToClient(partyInfo);
	}

	[PacketHandler(Opcode.SMSG_BATTLEFIELD_STATUS_QUEUED)]
	private void HandleBattlefieldStatusQueued(WorldPacket packet)
	{
		// Legacy sends unified SMSG_BATTLEFIELD_STATUS with StatusID field
		var queueSlot = packet.ReadUInt32();
		var arenaType = packet.ReadUInt8();
		packet.ReadUInt8(); // isRatedArena flag
		var bgTypeId = packet.ReadUInt32();
		packet.ReadUInt16(); // unk
		var minLevel = packet.ReadUInt8();
		var maxLevel = packet.ReadUInt8();
		var clientInstanceId = packet.ReadUInt32();
		packet.ReadUInt8(); // isRated
		var statusId = packet.ReadUInt32();

		if (statusId == 1) // STATUS_WAIT_QUEUE
		{
			var avgWaitTime = packet.ReadUInt32();
			var waitTime = packet.ReadUInt32();

			var queued = new BattlefieldStatusQueued();
			queued.Hdr.Ticket.RequesterGuid = GetSession().GameState.CurrentPlayerGuid;
			queued.Hdr.Ticket.Id = queueSlot;
			queued.Hdr.Ticket.Type = RideType.Battlegrounds;
			queued.Hdr.Ticket.Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			queued.Hdr.BattlefieldListIDs.Add(bgTypeId);
			queued.Hdr.RangeMin = minLevel;
			queued.Hdr.RangeMax = maxLevel;
			queued.Hdr.ArenaTeamSize = arenaType;
			queued.Hdr.InstanceID = clientInstanceId;
			queued.AverageWaitTime = avgWaitTime;
			queued.WaitTime = waitTime;
			queued.AsGroup = false;
			queued.EligibleForMatchmaking = true;
			queued.SuspendedQueue = false;
			SendPacketToClient(queued);
		}
	}

	[PacketHandler(Opcode.SMSG_ATTACKER_STATE_UPDATE)]
	private void HandleAttackerStateUpdate(WorldPacket packet)
	{
		var attack = new AttackerStateUpdate();
		var hitInfo = packet.ReadUInt32();
		attack.HitInfo = LegacyVersion.ConvertHitInfoFlags(hitInfo);
		attack.AttackerGUID = packet.ReadPackedGuid().To128(GetSession().GameState);
		attack.VictimGUID = packet.ReadPackedGuid().To128(GetSession().GameState);
		attack.Damage = packet.ReadInt32();
		attack.OriginalDamage = attack.Damage;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
		{
			attack.OverDamage = packet.ReadInt32();
		}
		else
		{
			attack.OverDamage = -1;
		}
		var subDamageCount = packet.ReadUInt8();
		for (var i = 0; i < subDamageCount; i++)
		{
			var subDmg = new SubDamage();
			var school = packet.ReadUInt32();
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				school = (uint)(1 << (byte)school);
			}
			subDmg.SchoolMask = school;
			subDmg.FloatDamage = packet.ReadFloat();
			subDmg.IntDamage = packet.ReadInt32();
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_3_9183) || hitInfo.HasAnyFlag(HitInfo.FullAbsorb | HitInfo.PartialAbsorb))
			{
				subDmg.Absorbed = packet.ReadInt32();
			}
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_3_9183) || hitInfo.HasAnyFlag(HitInfo.FullResist | HitInfo.PartialResist))
			{
				subDmg.Resisted = packet.ReadInt32();
			}
			attack.SubDmg.Add(subDmg);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
		{
			attack.VictimState = packet.ReadUInt8();
		}
		else
		{
			attack.VictimState = (byte)packet.ReadUInt32();
		}
		attack.AttackerState = packet.ReadInt32();
		attack.MeleeSpellID = packet.ReadUInt32();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_3_9183) || hitInfo.HasAnyFlag(HitInfo.Block))
		{
			attack.BlockAmount = packet.ReadInt32();
		}
		if (hitInfo.HasAnyFlag(HitInfo.RageGain))
		{
			attack.RageGained = packet.ReadInt32();
		}
		if (hitInfo.HasAnyFlag(HitInfo.Unk0))
		{
			attack.UnkState = default(UnkAttackerState);
			attack.UnkState.State1 = packet.ReadUInt32();
			attack.UnkState.State2 = packet.ReadFloat();
			attack.UnkState.State3 = packet.ReadFloat();
			attack.UnkState.State4 = packet.ReadFloat();
			attack.UnkState.State5 = packet.ReadFloat();
			attack.UnkState.State6 = packet.ReadFloat();
			attack.UnkState.State7 = packet.ReadFloat();
			attack.UnkState.State8 = packet.ReadFloat();
			attack.UnkState.State9 = packet.ReadFloat();
			attack.UnkState.State10 = packet.ReadFloat();
			attack.UnkState.State11 = packet.ReadFloat();
			attack.UnkState.State12 = packet.ReadUInt32();
			packet.ReadUInt32();
			packet.ReadUInt32();
		}
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_ATTACKSWING_NOTINRANGE)]
	private void HandleAttackSwingNotInRange(WorldPacket packet)
	{
		// Don't forward "not in range" if player has a ranged weapon equipped
		// The modern client needs to stay in attack state to initiate Auto Shot
		if (GetSession().GameState.CurrentPlayerGuid != null)
		{
			var visibleItems = GetSession().GameState.GetCachedObjectFieldsLegacy(GetSession().GameState.CurrentPlayerGuid);
			var PLAYER_VISIBLE_ITEM_1_ENTRYID = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_ENTRYID);
			if (PLAYER_VISIBLE_ITEM_1_ENTRYID >= 0)
			{
				var offset = LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? 2 : (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 16 : 12);
				var rangedIdx = PLAYER_VISIBLE_ITEM_1_ENTRYID + 17 * offset;
				if (visibleItems != null && visibleItems.ContainsKey(rangedIdx) && visibleItems[rangedIdx].UInt32Value != 0)
				{
					Log.Print(LogType.Debug, "[Combat] Suppressing ATTACKSWING_NOTINRANGE - player has ranged weapon equipped", "WorldClient.cs");
					return;
				}
			}
		}
		var attack = new AttackSwingError
		{
			Reason = AttackSwingErr.NotInRange
		};
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_ATTACKSWING_BADFACING)]
	private void HandleAttackSwingBadFacing(WorldPacket packet)
	{
		var attack = new AttackSwingError
		{
			Reason = AttackSwingErr.BadFacing
		};
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_ATTACKSWING_DEADTARGET)]
	private void HandleAttackSwingDeadTarget(WorldPacket packet)
	{
		var attack = new AttackSwingError
		{
			Reason = AttackSwingErr.DeadTarget
		};
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_ATTACKSWING_CANT_ATTACK)]
	private void HandleAttackSwingCantAttack(WorldPacket packet)
	{
		var attack = new AttackSwingError
		{
			Reason = AttackSwingErr.CantAttack
		};
		SendPacketToClient(attack);
	}

	[PacketHandler(Opcode.SMSG_CANCEL_COMBAT)]
	private void HandleCancelCombat(WorldPacket packet)
	{
		var combat = new CancelCombat();
		SendPacketToClient(combat);
	}

	[PacketHandler(Opcode.SMSG_AI_REACTION)]
	private void HandleAIReaction(WorldPacket packet)
	{
		var reaction = new AIReaction
		{
			UnitGUID = packet.ReadGuid().To128(GetSession().GameState),
			Reaction = packet.ReadUInt32()
		};
		SendPacketToClient(reaction);
	}

	[PacketHandler(Opcode.SMSG_PARTY_KILL_LOG)]
	private void HandlePartyKillLog(WorldPacket packet)
	{
		var log = new PartyKillLog
		{
			Player = packet.ReadGuid().To128(GetSession().GameState),
			Victim = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(log);
	}

	[PacketHandler(Opcode.SMSG_DUEL_REQUESTED)]
	private void HandleDuelRequested(WorldPacket packet)
	{
		var duel = new DuelRequested
		{
			ArbiterGUID = packet.ReadGuid().To128(GetSession().GameState),
			RequestedByGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		duel.RequestedByWowAccount = GetSession().GetGameAccountGuidForPlayer(duel.RequestedByGUID);
		SendPacketToClient(duel);
	}

	[PacketHandler(Opcode.SMSG_DUEL_COUNTDOWN)]
	private void HandleDuelCountdown(WorldPacket packet)
	{
		var duel = new DuelCountdown
		{
			Countdown = packet.ReadUInt32()
		};
		SendPacketToClient(duel);
	}

	[PacketHandler(Opcode.SMSG_DUEL_COMPLETE)]
	private void HandleDuelComplete(WorldPacket packet)
	{
		var duel = new DuelComplete
		{
			Started = packet.ReadBool()
		};
		SendPacketToClient(duel);
	}

	[PacketHandler(Opcode.SMSG_DUEL_WINNER)]
	private void HandleDuelWinner(WorldPacket packet)
	{
		var duel = new DuelWinner
		{
			Fled = packet.ReadBool(),
			BeatenName = packet.ReadCString(),
			WinnerName = packet.ReadCString(),
			BeatenVirtualRealmAddress = GetSession().RealmId.GetAddress(),
			WinnerVirtualRealmAddress = GetSession().RealmId.GetAddress()
		};
		SendPacketToClient(duel);
	}

	[PacketHandler(Opcode.SMSG_DUEL_IN_BOUNDS)]
	private void HandleDuelInBounds(WorldPacket packet)
	{
		var duel = new DuelInBounds();
		SendPacketToClient(duel);
	}

	[PacketHandler(Opcode.SMSG_DUEL_OUT_OF_BOUNDS)]
	private void HandleDuelOutOfBounds(WorldPacket packet)
	{
		var duel = new DuelOutOfBounds();
		SendPacketToClient(duel);
	}

	[PacketHandler(Opcode.SMSG_GAME_OBJECT_DESPAWN)]
	private void HandleGameObjectDespawn(WorldPacket packet)
	{
		var guid = packet.ReadGuid();
		var despawn = new GameObjectDespawn
		{
			ObjectGUID = guid.To128(GetSession().GameState)
		};
		SendPacketToClient(despawn);
		GetSession().GameState.DespawnedGameObjects.Add(guid);
	}

	[PacketHandler(Opcode.SMSG_GAME_OBJECT_RESET_STATE)]
	private void HandleGameObjectResetState(WorldPacket packet)
	{
		var reset = new GameObjectResetState
		{
			ObjectGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(reset);
	}

	[PacketHandler(Opcode.SMSG_GAME_OBJECT_CUSTOM_ANIM)]
	private void HandleGameObjectCustomAnim(WorldPacket packet)
	{
		var anim = new GameObjectCustomAnim
		{
			ObjectGUID = packet.ReadGuid().To128(GetSession().GameState),
			CustomAnim = packet.ReadUInt32()
		};
		SendPacketToClient(anim);
	}

	[PacketHandler(Opcode.SMSG_FISH_NOT_HOOKED)]
	private void HandleFishNotHooked(WorldPacket packet)
	{
		var fish = new FishNotHooked();
		SendPacketToClient(fish);
	}

	[PacketHandler(Opcode.SMSG_FISH_ESCAPED)]
	private void HandleFishEscaped(WorldPacket packet)
	{
		var fish = new FishEscaped();
		SendPacketToClient(fish);
	}

	[PacketHandler(Opcode.SMSG_PARTY_COMMAND_RESULT)]
	private void HandlePartyCommandResult(WorldPacket packet)
	{
		var party = new PartyCommandResult
		{
			Command = (byte)packet.ReadUInt32(),
			Name = packet.ReadCString()
		};
		var partyResult = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			party.Result = (byte)partyResult;
		}
		else
		{
			var typeFromHandle = typeof(PartyResultModern);
			var partyResultVanilla = (PartyResultVanilla)partyResult;
			party.Result = (byte)Enum.Parse(typeFromHandle, partyResultVanilla.ToString());
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			party.ResultData = packet.ReadUInt32();
		}
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.SMSG_GROUP_DECLINE)]
	private void HandleGroupDecline(WorldPacket packet)
	{
		var party = new GroupDecline
		{
			Name = packet.ReadCString()
		};
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.SMSG_PARTY_INVITE)]
	private void HandleGroupInvite(WorldPacket packet)
	{
		var party = new PartyInvite();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			party.CanAccept = packet.ReadBool();
		}
		var realm = GetSession().RealmManager.GetRealm(GetSession().RealmId);
		party.InviterRealm = new VirtualRealmInfo(realm.Id.GetAddress(), isHomeRealm: true, isInternalRealm: false, realm.Name, realm.NormalizedName);
		party.InviterName = packet.ReadCString();
		party.InviterGUID = GetSession().GameState.GetPlayerGuidByName(party.InviterName);
		if (party.InviterGUID == null)
		{
			party.InviterGUID = WowGuid128.Empty;
			party.InviterBNetAccountId = WowGuid128.Empty;
		}
		else
		{
			party.InviterBNetAccountId = GetSession().GetBnetAccountGuidForPlayer(party.InviterGUID);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			party.ProposedRoles = packet.ReadUInt32();
			var lfgSlotsCount = packet.ReadUInt8();
			for (var i = 0; i < lfgSlotsCount; i++)
			{
				party.LfgSlots.Add(packet.ReadInt32());
			}
			party.LfgCompletedMask = packet.ReadInt32();
		}
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.SMSG_GROUP_LIST, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleGroupListVanilla(WorldPacket packet)
	{
		var party = new PartyUpdate
		{
			SequenceNum = GetSession().GameState.GroupUpdateCounter++
		};
		var isRaid = packet.ReadBool();
		var ownSubGroupAndFlags = packet.ReadUInt8();
		party.PartyIndex = (byte)((isRaid && GetSession().GameState.IsInBattleground()) ? 1u : 0u);
		party.PartyGUID = WowGuid128.Create(HighGuidType703.Party, (ulong)(1000 + party.PartyIndex));
		if (party.PartyIndex != 0)
		{
			party.PartyFlags |= GroupFlags.FakeRaid;
		}
		var uniqueMembers = new HashSet<WowGuid128>();
		var membersCount = packet.ReadUInt32();
		if (membersCount != 0)
		{
			if (isRaid)
			{
				party.PartyFlags |= GroupFlags.Raid;
			}
			party.DifficultySettings = new PartyDifficultySettings
			{
				DungeonDifficultyID = DifficultyModern.Normal
			};
			if (ModernVersion.ExpansionVersion > 1)
			{
				party.DifficultySettings.RaidDifficultyID = DifficultyModern.Raid25N;
			}
			else
			{
				party.DifficultySettings.RaidDifficultyID = DifficultyModern.Raid40;
			}
			if (party.PartyIndex != 0)
			{
				party.PartyType = GroupType.PvP;
			}
			else
			{
				party.PartyType = GroupType.Normal;
			}
			var player = default(PartyPlayerInfo);
			player.GUID = GetSession().GameState.CurrentPlayerGuid;
			player.Name = GetSession().GameState.GetPlayerName(player.GUID);
			player.Subgroup = (byte)(ownSubGroupAndFlags & 0xF);
			player.Flags = (((ownSubGroupAndFlags & 0x80) != 0) ? GroupMemberFlags.Assistant : GroupMemberFlags.None);
			player.Status = GroupMemberOnlineStatus.Online;
			party.PlayerList.Add(player);
			var allAssist = true;
			for (var i = 0u; i < membersCount; i++)
			{
				var member = default(PartyPlayerInfo);
				member.Name = packet.ReadCString();
				member.GUID = packet.ReadGuid().To128(GetSession().GameState);
				member.Status = (GroupMemberOnlineStatus)packet.ReadUInt8();
				var subGroupAndFlags = packet.ReadUInt8();
				member.Subgroup = (byte)(subGroupAndFlags & 0xF);
				member.Flags = (((subGroupAndFlags & 0x80) != 0) ? GroupMemberFlags.Assistant : GroupMemberFlags.None);
				member.ClassId = GetSession().GameState.GetUnitClass(member.GUID);
				if (!member.Flags.HasAnyFlag(GroupMemberFlags.Assistant))
				{
					allAssist = false;
				}
				if (!uniqueMembers.Contains(member.GUID))
				{
					party.PlayerList.Add(member);
					uniqueMembers.Add(member.GUID);
				}
				Session.GameState.UpdatePlayerCache(member.GUID, new PlayerCache
				{
					Name = member.Name,
					ClassId = member.ClassId
				});
			}
			if (allAssist)
			{
				party.PartyFlags |= GroupFlags.EveryoneAssistant;
			}
			party.LeaderGUID = packet.ReadGuid().To128(GetSession().GameState);
			party.LootSettings = new PartyLootSettings
			{
				Method = (LootMethod)packet.ReadUInt8(),
				LootMaster = packet.ReadGuid().To128(GetSession().GameState),
				Threshold = packet.ReadUInt8()
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958) && packet.CanRead())
			{
				packet.ReadUInt8(); // Dungeon Difficulty
				packet.ReadUInt8(); // Raid Difficulty
				if (packet.CanRead())
					packet.ReadUInt8(); // Dynamic Raid Difficulty (heroic flag)
			}
			GetSession().GameState.WeWantToLeaveGroup = false;
			GetSession().GameState.CurrentGroups[party.PartyIndex] = party;
		}
		else
		{
			party.PartyFlags |= GroupFlags.Destroyed;
			if (party.PartyIndex == 0)
			{
				party.PartyGUID = WowGuid128.Empty;
			}
			party.LeaderGUID = WowGuid128.Empty;
			party.MyIndex = -1;
			GetSession().GameState.CurrentGroups[party.PartyIndex] = null;
			if (!GetSession().GameState.WeWantToLeaveGroup)
			{
				SendPacketToClient(new GroupUninvite());
			}
		}
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.SMSG_GROUP_LIST, ClientVersionBuild.V2_0_1_6180)]
	private void HandleGroupListTBC(WorldPacket packet)
	{
		var party = new PartyUpdate
		{
			SequenceNum = GetSession().GameState.GroupUpdateCounter++
		};
		var groupType = packet.ReadUInt8(); // group type flags
		var isRaid = (groupType & 0x01) != 0;
		var isBattleground = (groupType & 0x04) != 0;
		var isLfg = (groupType & 0x08) != 0;
		var ownSubGroup = packet.ReadUInt8();
		var ownGroupFlags = packet.ReadUInt8();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			packet.ReadUInt8(); // LFG roles
		}
		if (isLfg)
		{
			packet.ReadUInt8(); // LFG dungeon status
			packet.ReadUInt32(); // LFG dungeon ID
		}
		party.PartyIndex = (byte)(isBattleground ? 1u : 0u);
		party.PartyGUID = packet.ReadGuid().To128(GetSession().GameState);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			packet.ReadUInt32(); // group counter
		}
		if (party.PartyIndex != 0)
		{
			party.PartyFlags |= GroupFlags.FakeRaid;
		}
		var uniqueMembers = new HashSet<WowGuid128>();
		var membersCount = packet.ReadUInt32();
		if (membersCount != 0)
		{
			if (isRaid)
			{
				party.PartyFlags |= GroupFlags.Raid;
			}
			if (party.PartyIndex != 0)
			{
				party.PartyType = GroupType.PvP;
			}
			else
			{
				party.PartyType = GroupType.Normal;
			}
			var player = default(PartyPlayerInfo);
			player.GUID = GetSession().GameState.CurrentPlayerGuid;
			player.Name = GetSession().GameState.GetPlayerName(player.GUID);
			player.Subgroup = ownSubGroup;
			player.Flags = (GroupMemberFlags)ownGroupFlags;
			player.Status = GroupMemberOnlineStatus.Online;
			party.PlayerList.Add(player);
			var allAssist = true;
			for (var i = 0u; i < membersCount; i++)
			{
				var member = default(PartyPlayerInfo);
				member.Name = packet.ReadCString();
				member.GUID = packet.ReadGuid().To128(GetSession().GameState);
				member.Status = (GroupMemberOnlineStatus)packet.ReadUInt8();
				member.Subgroup = packet.ReadUInt8();
				member.Flags = (GroupMemberFlags)packet.ReadUInt8();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
				{
					packet.ReadUInt8(); // LFG roles
				}
				member.ClassId = GetSession().GameState.GetUnitClass(member.GUID);
				if (!member.Flags.HasAnyFlag(GroupMemberFlags.Assistant))
				{
					allAssist = false;
				}
				if (!uniqueMembers.Contains(member.GUID))
				{
					party.PlayerList.Add(member);
					uniqueMembers.Add(member.GUID);
				}
				Session.GameState.UpdatePlayerCache(member.GUID, new PlayerCache
				{
					Name = member.Name,
					ClassId = member.ClassId
				});
			}
			if (allAssist)
			{
				party.PartyFlags |= GroupFlags.EveryoneAssistant;
			}
			party.LeaderGUID = packet.ReadGuid().To128(GetSession().GameState);
			party.LootSettings = new PartyLootSettings
			{
				Method = (LootMethod)packet.ReadUInt8(),
				LootMaster = packet.ReadGuid().To128(GetSession().GameState),
				Threshold = packet.ReadUInt8()
			};
			party.DifficultySettings = new PartyDifficultySettings();
			int difficultyId = packet.ReadUInt8();
			party.DifficultySettings.DungeonDifficultyID = (DifficultyModern)Enum.Parse(typeof(DifficultyModern), ((DifficultyLegacy)difficultyId/*cast due to .constrained prefix*/).ToString());
			if (ModernVersion.ExpansionVersion > 1)
			{
				party.DifficultySettings.RaidDifficultyID = DifficultyModern.Raid25N;
			}
			else
			{
				party.DifficultySettings.RaidDifficultyID = DifficultyModern.Raid40;
			}
			GetSession().GameState.WeWantToLeaveGroup = false;
			GetSession().GameState.CurrentGroups[party.PartyIndex] = party;
		}
		else
		{
			party.PartyFlags |= GroupFlags.Destroyed;
			if (party.PartyIndex == 0)
			{
				party.PartyGUID = WowGuid128.Empty;
			}
			party.LeaderGUID = WowGuid128.Empty;
			party.MyIndex = -1;
			GetSession().GameState.CurrentGroups[party.PartyIndex] = null;
			if (!GetSession().GameState.WeWantToLeaveGroup)
			{
				SendPacketToClient(new GroupUninvite());
			}
		}
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.SMSG_GROUP_UNINVITE)]
	private void HandleGroupUninvite(WorldPacket packet)
	{
		var party = new GroupUninvite();
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.SMSG_GROUP_NEW_LEADER)]
	private void HandleGroupNewLeader(WorldPacket packet)
	{
		var party = new GroupNewLeader
		{
			Name = packet.ReadCString(),
			PartyIndex = GetSession().GameState.GetCurrentPartyIndex()
		};
		SendPacketToClient(party);
	}

	[PacketHandler(Opcode.MSG_RAID_READY_CHECK, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleRaidReadyCheckVanilla(WorldPacket packet)
	{
		if (!packet.CanRead())
		{
			var ready = new ReadyCheckStarted
			{
				InitiatorGUID = GetSession().GameState.GetCurrentGroupLeader(),
				PartyIndex = GetSession().GameState.GetCurrentPartyIndex(),
				PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
			};
			SendPacketToClient(ready);
			return;
		}
		var ready2 = new ReadyCheckResponse
		{
			Player = packet.ReadGuid().To128(GetSession().GameState),
			IsReady = packet.ReadBool(),
			PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
		};
		SendPacketToClient(ready2);
		GetSession().GameState.GroupReadyCheckResponses++;
		if (GetSession().GameState.GroupReadyCheckResponses >= GetSession().GameState.GetCurrentGroupSize())
		{
			GetSession().GameState.GroupReadyCheckResponses = 0u;
			var completed = new ReadyCheckCompleted
			{
				PartyIndex = GetSession().GameState.GetCurrentPartyIndex(),
				PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
			};
			SendPacketToClient(completed);
		}
	}

	[PacketHandler(Opcode.MSG_RAID_READY_CHECK, ClientVersionBuild.V2_0_1_6180)]
	private void HandleRaidReadyCheck(WorldPacket packet)
	{
		var ready = new ReadyCheckStarted
		{
			InitiatorGUID = packet.ReadGuid().To128(GetSession().GameState),
			PartyIndex = GetSession().GameState.GetCurrentPartyIndex(),
			PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
		};
		SendPacketToClient(ready);
	}

	[PacketHandler(Opcode.MSG_RAID_READY_CHECK_CONFIRM, ClientVersionBuild.V2_0_1_6180)]
	private void HandleRaidReadyCheckConfirm(WorldPacket packet)
	{
		var ready = new ReadyCheckResponse
		{
			Player = packet.ReadGuid().To128(GetSession().GameState),
			IsReady = packet.ReadBool(),
			PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
		};
		SendPacketToClient(ready);
		GetSession().GameState.GroupReadyCheckResponses++;
		if (GetSession().GameState.GroupReadyCheckResponses >= GetSession().GameState.GetCurrentGroupSize())
		{
			GetSession().GameState.GroupReadyCheckResponses = 0u;
			var completed = new ReadyCheckCompleted
			{
				PartyIndex = GetSession().GameState.GetCurrentPartyIndex(),
				PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
			};
			SendPacketToClient(completed);
		}
	}

	[PacketHandler(Opcode.MSG_RAID_READY_CHECK_FINISHED, ClientVersionBuild.V2_0_1_6180)]
	private void HandleRaidReadyCheckFinished(WorldPacket packet)
	{
		var ready = new ReadyCheckCompleted
		{
			PartyIndex = GetSession().GameState.GetCurrentPartyIndex(),
			PartyGUID = GetSession().GameState.GetCurrentGroupGuid()
		};
		SendPacketToClient(ready);
	}

	[PacketHandler(Opcode.MSG_RAID_TARGET_UPDATE)]
	private void HandleRaidTargetUpdate(WorldPacket packet)
	{
		if (packet.ReadBool())
		{
			var update = new SendRaidTargetUpdateAll
			{
				PartyIndex = GetSession().GameState.GetCurrentPartyIndex()
			};
			while (packet.CanRead())
			{
				var symbol = packet.ReadInt8();
				var guid = packet.ReadGuid().To128(GetSession().GameState);
				update.TargetIcons.Add(new Tuple<sbyte, WowGuid128>(symbol, guid));
			}
			SendPacketToClient(update);
			return;
		}
		var update2 = new SendRaidTargetUpdateSingle
		{
			PartyIndex = GetSession().GameState.GetCurrentPartyIndex()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			update2.ChangedBy = packet.ReadGuid().To128(GetSession().GameState);
		}
		else
		{
			update2.ChangedBy = GetSession().GameState.CurrentPlayerGuid;
		}
		update2.Symbol = packet.ReadInt8();
		update2.Target = packet.ReadGuid().To128(GetSession().GameState);
		SendPacketToClient(update2);
	}

	[PacketHandler(Opcode.SMSG_SUMMON_REQUEST)]
	private void HandleSummonRequest(WorldPacket packet)
	{
		var summon = new SummonRequest
		{
			SummonerGUID = packet.ReadGuid().To128(GetSession().GameState),
			SummonerVirtualRealmAddress = GetSession().RealmId.GetAddress(),
			AreaID = packet.ReadInt32()
		};
		packet.ReadUInt32();
		SendPacketToClient(summon);
	}

	[PacketHandler(Opcode.SMSG_PARTY_MEMBER_PARTIAL_STATE, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePartyMemberStats(WorldPacket packet)
	{
		if (GetSession().GameState.CurrentMapId == 489 && (GetSession().GameState.HasWsgAllyFlagCarrier || GetSession().GameState.HasWsgHordeFlagCarrier) && _requestBgPlayerPosCounter++ > 10)
		{
			var packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet2);
			_requestBgPlayerPosCounter = 0u;
		}
		var state = new PartyMemberPartialState
		{
			AffectedGUID = packet.ReadPackedGuid().To128(GetSession().GameState)
		};
		var updateFlags = (GroupUpdateFlagVanilla)packet.ReadUInt32();
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Status))
		{
			state.StatusFlags = packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentHealth))
		{
			state.CurrentHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.MaxHealth))
		{
			state.MaxHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PowerType))
		{
			state.PowerType = packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentPower))
		{
			state.CurrentPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.MaxPower))
		{
			state.MaxPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Level))
		{
			state.Level = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Zone))
		{
			state.ZoneID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Position))
		{
			state.Position = new PartyMemberPartialState.Vector3_UInt16
			{
				X = packet.ReadInt16(),
				Y = packet.ReadInt16()
			};
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Auras))
		{
			if (state.Auras == null)
			{
				state.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask = packet.ReadUInt32();
			byte maxAura = 32;
			for (byte i = 0; i < maxAura; i++)
			{
				if ((auraMask & (1L << i)) != 0)
				{
					var aura = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura.SpellId != 0)
					{
						aura.ActiveFlags = 1u;
						aura.AuraFlags = 256;
					}
					state.Auras.Add(aura);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.AurasNegative))
		{
			if (state.Auras == null)
			{
				state.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask2 = packet.ReadUInt16();
			byte maxAura2 = 48;
			for (byte i2 = 0; i2 < maxAura2; i2++)
			{
				if ((auraMask2 & (1L << i2)) != 0)
				{
					var aura2 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura2.SpellId != 0)
					{
						aura2.ActiveFlags = 1u;
						aura2.AuraFlags = 16;
					}
					state.Auras.Add(aura2);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetGuid))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetGuid = packet.ReadGuid().To128(GetSession().GameState);
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetName))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetName = packet.ReadCString();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetModelId))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.DisplayID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetCurrentHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.Health = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetMaxHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.MaxHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetPowerType))
		{
			packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetCurrentPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetMaxPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetAuras))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			if (state.Pet.Auras == null)
			{
				state.Pet.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask3 = packet.ReadUInt32();
			byte maxAura3 = 32;
			for (byte i3 = 0; i3 < maxAura3; i3++)
			{
				if ((auraMask3 & (1L << i3)) != 0)
				{
					var aura3 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura3.SpellId != 0)
					{
						aura3.ActiveFlags = 1u;
						aura3.AuraFlags = 256;
					}
					state.Pet.Auras.Add(aura3);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetAurasNegative))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			if (state.Pet.Auras == null)
			{
				state.Pet.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask4 = packet.ReadUInt16();
			byte maxAura4 = 48;
			for (byte i4 = 0; i4 < maxAura4; i4++)
			{
				if ((auraMask4 & (1L << i4)) != 0)
				{
					var aura4 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura4.SpellId != 0)
					{
						aura4.ActiveFlags = 1u;
						aura4.AuraFlags = 16;
					}
					state.Pet.Auras.Add(aura4);
				}
			}
		}
		SendPacketToClient(state);
	}

	[PacketHandler(Opcode.SMSG_PARTY_MEMBER_PARTIAL_STATE, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePartyMemberStatsTbc(WorldPacket packet)
	{
		if (GetSession().GameState.CurrentMapId == 489 && (GetSession().GameState.HasWsgAllyFlagCarrier || GetSession().GameState.HasWsgHordeFlagCarrier) && _requestBgPlayerPosCounter++ > 10)
		{
			var packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet2);
			_requestBgPlayerPosCounter = 0u;
		}
		var state = new PartyMemberPartialState
		{
			AffectedGUID = packet.ReadPackedGuid().To128(GetSession().GameState)
		};
		var updateFlags = (GroupUpdateFlagTBC)packet.ReadUInt32();
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Status))
		{
			state.StatusFlags = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.CurrentHealth))
		{
			state.CurrentHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.MaxHealth))
		{
			state.MaxHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PowerType))
		{
			state.PowerType = packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.CurrentPower))
		{
			state.CurrentPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.MaxPower))
		{
			state.MaxPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Level))
		{
			state.Level = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Zone))
		{
			state.ZoneID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Position))
		{
			state.Position = new PartyMemberPartialState.Vector3_UInt16
			{
				X = packet.ReadInt16(),
				Y = packet.ReadInt16()
			};
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Auras))
		{
			if (state.Auras == null)
			{
				state.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask = packet.ReadUInt64();
			for (byte i = 0; i < LegacyVersion.GetAuraSlotsCount(); i++)
			{
				if ((auraMask & (ulong)(1L << i)) != 0)
				{
					var aura = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					packet.ReadUInt8();
					if (aura.SpellId != 0)
					{
						aura.ActiveFlags = 1u;
						aura.AuraFlags = 256;
					}
					state.Auras.Add(aura);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetGuid))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetGuid = packet.ReadGuid().To128(GetSession().GameState);
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetName))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetName = packet.ReadCString();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetModelId))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.DisplayID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetCurrentHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.Health = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetMaxHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.MaxHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetPowerType))
		{
			packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetCurrentPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetMaxPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetAuras))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			if (state.Pet.Auras == null)
			{
				state.Pet.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask2 = packet.ReadUInt64();
			for (byte i2 = 0; i2 < LegacyVersion.GetAuraSlotsCount(); i2++)
			{
				if ((auraMask2 & (ulong)(1L << i2)) != 0)
				{
					var aura2 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					packet.ReadUInt8();
					if (aura2.SpellId != 0)
					{
						aura2.ActiveFlags = 1u;
						aura2.AuraFlags = 256;
					}
					state.Pet.Auras.Add(aura2);
				}
			}
		}
		SendPacketToClient(state);
	}

	[PacketHandler(Opcode.SMSG_PARTY_MEMBER_FULL_STATE, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePartyMemberStatsFull(WorldPacket packet)
	{
		if (GetSession().GameState.CurrentMapId == 489 && (GetSession().GameState.HasWsgAllyFlagCarrier || GetSession().GameState.HasWsgHordeFlagCarrier) && _requestBgPlayerPosCounter++ > 10)
		{
			var packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet2);
			_requestBgPlayerPosCounter = 0u;
		}
		var state = new PartyMemberFullState();
		if (GetSession().GameState.IsInBattleground())
		{
			state.PartyType[0] = 0;
			state.PartyType[1] = 2;
		}
		else
		{
			state.PartyType[0] = 1;
			state.PartyType[1] = 0;
		}
		state.MemberGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
		var updateFlags = (GroupUpdateFlagVanilla)packet.ReadUInt32();
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Status))
		{
			state.StatusFlags = (GroupMemberOnlineStatus)packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentHealth))
		{
			state.CurrentHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.MaxHealth))
		{
			state.MaxHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PowerType))
		{
			state.PowerType = packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentPower))
		{
			state.CurrentPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.MaxPower))
		{
			state.MaxPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Level))
		{
			state.Level = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Zone))
		{
			state.ZoneID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Position))
		{
			state.PositionX = packet.ReadInt16();
			state.PositionY = packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Auras))
		{
			if (state.Auras == null)
			{
				state.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask = packet.ReadUInt32();
			byte maxAura = 32;
			for (byte i = 0; i < maxAura; i++)
			{
				if ((auraMask & (1L << i)) != 0)
				{
					var aura = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura.SpellId != 0)
					{
						aura.ActiveFlags = 1u;
						aura.AuraFlags = 256;
					}
					state.Auras.Add(aura);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.AurasNegative))
		{
			if (state.Auras == null)
			{
				state.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask2 = packet.ReadUInt16();
			byte maxAura2 = 48;
			for (byte i2 = 0; i2 < maxAura2; i2++)
			{
				if ((auraMask2 & (1L << i2)) != 0)
				{
					var aura2 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura2.SpellId != 0)
					{
						aura2.ActiveFlags = 1u;
						aura2.AuraFlags = 16;
					}
					state.Auras.Add(aura2);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetGuid))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetGuid = packet.ReadGuid().To128(GetSession().GameState);
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetName))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetName = packet.ReadCString();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetModelId))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.DisplayID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetCurrentHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.Health = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetMaxHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.MaxHealth = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetPowerType))
		{
			packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetCurrentPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetMaxPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetAuras))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			if (state.Pet.Auras == null)
			{
				state.Pet.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask3 = packet.ReadUInt32();
			byte maxAura3 = 32;
			for (byte i3 = 0; i3 < maxAura3; i3++)
			{
				if ((auraMask3 & (1L << i3)) != 0)
				{
					var aura3 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura3.SpellId != 0)
					{
						aura3.ActiveFlags = 1u;
						aura3.AuraFlags = 256;
					}
					state.Pet.Auras.Add(aura3);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PetAurasNegative))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			if (state.Pet.Auras == null)
			{
				state.Pet.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask4 = packet.ReadUInt16();
			byte maxAura4 = 48;
			for (byte i4 = 0; i4 < maxAura4; i4++)
			{
				if ((auraMask4 & (1L << i4)) != 0)
				{
					var aura4 = new PartyMemberAuraStates
					{
						SpellId = packet.ReadUInt16()
					};
					if (aura4.SpellId != 0)
					{
						aura4.ActiveFlags = 1u;
						aura4.AuraFlags = 16;
					}
					state.Pet.Auras.Add(aura4);
				}
			}
		}
		SendPacketToClient(state);
	}

	[PacketHandler(Opcode.SMSG_PARTY_MEMBER_FULL_STATE, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePartyMemberStatsFullTBC(WorldPacket packet)
	{
		if (GetSession().GameState.CurrentMapId == 489 && (GetSession().GameState.HasWsgAllyFlagCarrier || GetSession().GameState.HasWsgHordeFlagCarrier) && _requestBgPlayerPosCounter++ > 10)
		{
			var packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet2);
			_requestBgPlayerPosCounter = 0u;
		}
		var state = new PartyMemberFullState();
		if (GetSession().GameState.IsInBattleground())
		{
			state.PartyType[0] = 0;
			state.PartyType[1] = 2;
		}
		else
		{
			state.PartyType[0] = 1;
			state.PartyType[1] = 0;
		}

        state.ForEnemy = packet.ReadUInt8() != 0;

        state.MemberGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
		var updateFlags = (GroupUpdateFlagTBC)packet.ReadUInt32();
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Status))
		{
			state.StatusFlags = (GroupMemberOnlineStatus)packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.CurrentHealth))
		{
			
			if (ModernVersion.ExpansionVersion == 3) // Health is int32 in 3.3.5, source: TC 3.3.5 - GroupHandler.cpp
                state.CurrentHealth = (int)packet.ReadUInt32();
			else
                state.CurrentHealth = packet.ReadUInt16();
        }
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.MaxHealth))
		{
            if (ModernVersion.ExpansionVersion == 3)
                state.MaxHealth = (int)packet.ReadUInt32();
			else
                state.MaxHealth = packet.ReadUInt16();

        }
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PowerType))
		{
			state.PowerType = packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.CurrentPower))
		{
			state.CurrentPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.MaxPower))
		{
			state.MaxPower = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Level))
		{
			state.Level = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Zone))
		{
			state.ZoneID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Position))
		{
			state.PositionX = packet.ReadInt16();
			state.PositionY = packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.Auras))
		{
			if (state.Auras == null)
			{
				state.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask = packet.ReadUInt64();
			for (byte i = 0; i < LegacyVersion.GetAuraSlotsCount(); i++)
			{
				if ((auraMask & (ulong)(1L << i)) != 0)
				{
					var aura = new PartyMemberAuraStates();
                    if (ModernVersion.ExpansionVersion == 3)
                        aura.SpellId = packet.ReadUInt32();
					else
                        aura.SpellId = packet.ReadUInt16();
                    packet.ReadUInt8();
					if (aura.SpellId != 0)
					{
						aura.ActiveFlags = 1u;
						aura.AuraFlags = 256;
					}
					state.Auras.Add(aura);
				}
			}
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetGuid))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetGuid = packet.ReadGuid().To128(GetSession().GameState);
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetName))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.NewPetName = packet.ReadCString();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetModelId))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			state.Pet.DisplayID = packet.ReadUInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetCurrentHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
            if (ModernVersion.ExpansionVersion == 3)
				state.Pet.Health = packet.ReadUInt32();
			else
                state.Pet.Health = packet.ReadUInt16();
        }
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetMaxHealth))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
            if (ModernVersion.ExpansionVersion == 3)
                state.Pet.MaxHealth = packet.ReadUInt32();
			else
                state.Pet.MaxHealth = packet.ReadUInt16();

        }
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetPowerType))
		{
			packet.ReadUInt8();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetCurrentPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetMaxPower))
		{
			packet.ReadInt16();
		}
		if (updateFlags.HasFlag(GroupUpdateFlagTBC.PetAuras))
		{
			if (state.Pet == null)
			{
				state.Pet = new PartyMemberPetStats();
			}
			if (state.Pet.Auras == null)
			{
				state.Pet.Auras = new List<PartyMemberAuraStates>();
			}
			var auraMask2 = packet.ReadUInt64();
			for (byte i2 = 0; i2 < LegacyVersion.GetAuraSlotsCount(); i2++)
			{
				if ((auraMask2 & (ulong)(1L << i2)) != 0)
				{
					var aura2 = new PartyMemberAuraStates();
                    if (ModernVersion.ExpansionVersion == 3)
                        aura2.SpellId = packet.ReadUInt32();
                    else
                        aura2.SpellId = packet.ReadUInt16();
                    packet.ReadUInt8();
					if (aura2.SpellId != 0)
					{
						aura2.ActiveFlags = 1u;
						aura2.AuraFlags = 256;
					}
					state.Pet.Auras.Add(aura2);
				}
			}
		}
		SendPacketToClient(state);
	}

	[PacketHandler(Opcode.MSG_MINIMAP_PING)]
	private void HandleMinimapPing(WorldPacket packet)
	{
		var ping = new MinimapPing
		{
			SenderGUID = packet.ReadGuid().To128(GetSession().GameState),
			Position = packet.ReadVector2()
		};
		SendPacketToClient(ping);
	}

	[PacketHandler(Opcode.MSG_RANDOM_ROLL)]
	private void HandleRandomRoll(WorldPacket packet)
	{
		var roll = new RandomRoll
		{
			Min = packet.ReadInt32(),
			Max = packet.ReadInt32(),
			Result = packet.ReadInt32(),
			Roller = packet.ReadGuid().To128(GetSession().GameState)
		};
		roll.RollerWowAccount = GetSession().GetGameAccountGuidForPlayer(roll.Roller);
		SendPacketToClient(roll);
	}

	[PacketHandler(Opcode.SMSG_GUILD_COMMAND_RESULT)]
	private void HandleGuildCommandResult(WorldPacket packet)
	{
		var result = new GuildCommandResult
		{
			Command = (GuildCommandType)packet.ReadUInt32(),
			Name = packet.ReadCString(),
			Result = (GuildCommandError)packet.ReadUInt32()
		};
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.SMSG_GUILD_EVENT)]
	private void HandleGuildEvent(WorldPacket packet)
	{
		var eventType = (GuildEventType)packet.ReadUInt8();
		var size = packet.ReadUInt8();
		var strings = new string[size];
		for (var i = 0; i < size; i++)
		{
			strings[i] = packet.ReadCString();
		}
		var guid = WowGuid128.Empty;
		if (packet.CanRead())
		{
			guid = packet.ReadGuid().To128(GetSession().GameState);
		}
		switch (eventType)
		{
		case GuildEventType.Promotion:
		case GuildEventType.Demotion:
		{
			var officer = GetSession().GameState.GetPlayerGuidByName(strings[0]);
			var player = GetSession().GameState.GetPlayerGuidByName(strings[1]);
			var rankId = GetSession().GetGuildRankIdByName(GetSession().GameState.GetPlayerGuildId(GetSession().GameState.CurrentPlayerGuid), strings[2]);
			if (officer != null && player != null)
			{
				var promote = new GuildSendRankChange
				{
					Officer = officer,
					Other = player,
					Promote = eventType == GuildEventType.Promotion,
					RankID = rankId
				};
				SendPacketToClient(promote);
			}
			break;
		}
		case GuildEventType.MOTD:
		{
			var motd = new GuildEventMotd
			{
				MotdText = strings[0]
			};
			SendPacketToClient(motd);
			break;
		}
		case GuildEventType.PlayerJoined:
		{
			var joined = new GuildEventPlayerJoined
			{
				Guid = guid,
				VirtualRealmAddress = GetSession().RealmId.GetAddress(),
				Name = strings[0]
			};
			SendPacketToClient(joined);
			break;
		}
		case GuildEventType.PlayerLeft:
		{
			var left = new GuildEventPlayerLeft
			{
				Removed = false,
				LeaverGUID = guid,
				LeaverVirtualRealmAddress = GetSession().RealmId.GetAddress(),
				LeaverName = strings[0]
			};
			SendPacketToClient(left);
			break;
		}
		case GuildEventType.PlayerRemoved:
		{
			var removed = new GuildEventPlayerLeft
			{
				Removed = true,
				LeaverGUID = guid,
				LeaverVirtualRealmAddress = GetSession().RealmId.GetAddress(),
				LeaverName = strings[0],
				RemoverGUID = GetSession().GameState.GetPlayerGuidByName(strings[1]),
				RemoverVirtualRealmAddress = GetSession().RealmId.GetAddress(),
				RemoverName = strings[1]
			};
			SendPacketToClient(removed);
			break;
		}
		case GuildEventType.LeaderIs:
			break;
		case GuildEventType.LeaderChanged:
		{
			var oldLeader = GetSession().GameState.GetPlayerGuidByName(strings[0]);
			var newLeader = GetSession().GameState.GetPlayerGuidByName(strings[1]);
			if (oldLeader != null && newLeader != null)
			{
				var leader = new GuildEventNewLeader
				{
					OldLeaderGUID = oldLeader,
					OldLeaderVirtualRealmAddress = GetSession().RealmId.GetAddress(),
					OldLeaderName = strings[0],
					NewLeaderGUID = newLeader,
					NewLeaderVirtualRealmAddress = GetSession().RealmId.GetAddress(),
					NewLeaderName = strings[1]
				};
				SendPacketToClient(leader);
			}
			break;
		}
		case GuildEventType.Disbanded:
		{
			var disband = new GuildEventDisbanded();
			SendPacketToClient(disband);
			break;
		}
		case GuildEventType.TabardChange:
			break;
		case GuildEventType.RankUpdated:
		{
			var ranks = new GuildEventRanksUpdated();
			SendPacketToClient(ranks);
			break;
		}
		case GuildEventType.Unk11:
			break;
		case GuildEventType.PlayerSignedOn:
		case GuildEventType.PlayerSignedOff:
		{
			var presence = new GuildEventPresenceChange
			{
				Guid = guid,
				VirtualRealmAddress = GetSession().RealmId.GetAddress(),
				LoggedOn = eventType == GuildEventType.PlayerSignedOn,
				Name = strings[0]
			};
			SendPacketToClient(presence);
			break;
		}
		case GuildEventType.BankBagSlotsChanged:
			break;
		case GuildEventType.BankTabPurchased:
		{
			var tab3 = new GuildEventTabAdded();
			SendPacketToClient(tab3);
			break;
		}
		case GuildEventType.BankTabUpdated:
		{
			var tab2 = new GuildEventTabModified
			{
				Name = strings[0],
				Icon = strings[1]
			};
			SendPacketToClient(tab2);
			break;
		}
		case GuildEventType.BankMoneyUpdate:
		{
			var money = new GuildEventBankMoneyChanged
			{
				Money = (ulong)int.Parse(strings[0], NumberStyles.HexNumber)
			};
			SendPacketToClient(money);
			break;
		}
		case GuildEventType.BankMoneyWithdraw:
			break;
		case GuildEventType.BankTextChanged:
		{
			var tab = new GuildEventTabTextChanged();
			SendPacketToClient(tab);
			break;
		}
		}
	}

	[PacketHandler(Opcode.SMSG_QUERY_GUILD_INFO_RESPONSE)]
	private void HandleQueryGuildInfoResponse(WorldPacket packet)
	{
		var guild = new QueryGuildInfoResponse();
		var guildId = packet.ReadUInt32();
		guild.GuildGUID = WowGuid128.Create(HighGuidType703.Guild, guildId);
		guild.PlayerGuid = GetSession().GameState.CurrentPlayerGuid;
		guild.HasGuildInfo = true;
		guild.Info = new QueryGuildInfoResponse.GuildInfo
		{
			GuildGuid = guild.GuildGUID,
			VirtualRealmAddress = GetSession().RealmId.GetAddress(),
			GuildName = packet.ReadCString()
		};
		GetSession().StoreGuildGuidAndName(guild.GuildGUID, guild.Info.GuildName);
		var ranks = new List<string>();
		for (var i = 0u; i < 10; i++)
		{
			var rankName = packet.ReadCString();
			if (!string.IsNullOrEmpty(rankName))
			{
				var rank = new QueryGuildInfoResponse.GuildInfo.RankInfo
				{
					RankID = i,
					RankOrder = i,
					RankName = rankName
				};
				ranks.Add(rankName);
				guild.Info.Ranks.Add(rank);
			}
		}
		GetSession().StoreGuildRankNames(guildId, ranks);
		guild.Info.EmblemStyle = packet.ReadUInt32();
		guild.Info.EmblemColor = packet.ReadUInt32();
		guild.Info.BorderStyle = packet.ReadUInt32();
		guild.Info.BorderColor = packet.ReadUInt32();
		guild.Info.BackgroundColor = packet.ReadUInt32();
		SendPacketToClient(guild);
	}

	[PacketHandler(Opcode.SMSG_GUILD_INFO)]
	private void HandleGuildInfo(WorldPacket packet)
	{
		packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			GetSession().GameState.CurrentGuildCreateTime = packet.ReadPackedTime();
		}
		else
		{
			var day = packet.ReadInt32();
			var month = packet.ReadInt32();
			var year = packet.ReadInt32();
			try
			{
				var date = new DateTime(year, month, day);
				GetSession().GameState.CurrentGuildCreateTime = (uint)Time.DateTimeToUnixTime(date);
			}
			catch
			{
				Log.Print(LogType.Error, $"Invalid guild create date: {day}-{month}-{year}", "GuildHandler.cs");
			}
		}
		packet.ReadUInt32();
		GetSession().GameState.CurrentGuildNumAccounts = packet.ReadUInt32();
	}

	[PacketHandler(Opcode.SMSG_GUILD_ROSTER)]
	private void HandleGuildRoster(WorldPacket packet)
	{
		var guild = new GuildRoster();
		var membersCount = packet.ReadUInt32();
		if (GetSession().GameState.CurrentGuildNumAccounts != 0)
		{
			guild.NumAccounts = GetSession().GameState.CurrentGuildNumAccounts;
		}
		else
		{
			guild.NumAccounts = membersCount;
		}
		guild.WelcomeText = packet.ReadCString();
		guild.InfoText = packet.ReadCString();
		if (GetSession().GameState.CurrentGuildCreateTime != 0)
		{
			guild.CreateDate = GetSession().GameState.CurrentGuildCreateTime;
		}
		else
		{
			guild.CreateDate = (uint)Time.UnixTime;
		}
		var ranksCount = packet.ReadInt32();
		if (ranksCount > 0)
		{
			var ranks = new GuildRanks();
			for (byte i = 0; i < ranksCount; i++)
			{
				var rank = new GuildRankData
				{
					RankID = i,
					RankOrder = i,
					RankName = GetSession().GetGuildRankNameById(GetSession().GameState.GetPlayerGuildId(GetSession().GameState.CurrentPlayerGuid), i),
					Flags = packet.ReadUInt32()
				};
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					rank.WithdrawGoldLimit = packet.ReadInt32();
					for (var j = 0; j < 6; j++)
					{
						rank.TabFlags[j] = packet.ReadUInt32();
						rank.TabWithdrawItemLimit[j] = packet.ReadUInt32();
					}
				}
				ranks.Ranks.Add(rank);
			}
			SendPacketToClient(ranks);
		}
		for (var k = 0; k < membersCount; k++)
		{
			var member = new GuildRosterMemberData();
			var cache = new PlayerCache();
			member.Guid = packet.ReadGuid().To128(GetSession().GameState);
			member.VirtualRealmAddress = GetSession().RealmId.GetAddress();
			member.Status = packet.ReadUInt8();
			member.Name = (cache.Name = packet.ReadCString());
			member.RankID = packet.ReadInt32();
			member.Level = (cache.Level = packet.ReadUInt8());
			member.ClassID = (cache.ClassId = (Class)packet.ReadUInt8());
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
			{
				member.SexID = (cache.SexId = (Gender)packet.ReadUInt8());
			}
			GetSession().GameState.UpdatePlayerCache(member.Guid, cache);
			member.AreaID = packet.ReadInt32();
			if (member.Status == 0)
			{
				member.LastSave = packet.ReadFloat();
			}
			else
			{
				member.Authenticated = true;
			}
			member.Note = packet.ReadCString();
			member.OfficerNote = packet.ReadCString();
			guild.MemberData.Add(member);
		}
		SendPacketToClient(guild);
	}

	[PacketHandler(Opcode.SMSG_GUILD_INVITE)]
	private void HandleGuildInvite(WorldPacket packet)
	{
		var invite = new GuildInvite
		{
			InviterName = packet.ReadCString(),
			InviterVirtualRealmAddress = GetSession().RealmId.GetAddress(),
			GuildName = packet.ReadCString(),
			GuildVirtualRealmAddress = GetSession().RealmId.GetAddress()
		};
		invite.GuildGUID = GetSession().GetGuildGuid(invite.GuildName);
		SendPacketToClient(invite);
	}

	[PacketHandler(Opcode.MSG_GUILD_PERMISSIONS)]
	private void HandleGuildPermissions(WorldPacket packet)
	{
		var results = new GuildPermissionsQueryResults
		{
			GuildID = packet.ReadUInt32(),
			RankID = packet.ReadUInt32(),
			Flags = packet.ReadUInt32(),
			WithdrawGoldLimit = packet.ReadUInt32(),
			RemainingWithdrawGoldLimit = packet.ReadUInt32()
		};
		for (var i = 0; i < 6; i++)
		{
			results.TabPermissions[i] = packet.ReadUInt32();
		}
		SendPacketToClient(results);
	}

	[PacketHandler(Opcode.MSG_TABARDVENDOR_ACTIVATE)]
	private void HandleTabardVendorActivate(WorldPacket packet)
	{
		var activate = new PlayerTabardVendorActivate
		{
			DesignerGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(activate);
	}

	[PacketHandler(Opcode.MSG_SAVE_GUILD_EMBLEM)]
	private void HandleSaveGuildEmblem(WorldPacket packet)
	{
		var emblem = new PlayerSaveGuildEmblem
		{
			Error = (GuildEmblemError)packet.ReadUInt32()
		};
		SendPacketToClient(emblem);
	}

	[PacketHandler(Opcode.SMSG_GUILD_INVITE_DECLINED)]
	private void HandleGuildInviteDeclined(WorldPacket packet)
	{
		var invite = new GuildInviteDeclined
		{
			InviterName = packet.ReadCString(),
			InviterVirtualRealmAddress = GetSession().RealmId.GetAddress()
		};
		SendPacketToClient(invite);
	}

	[PacketHandler(Opcode.SMSG_GUILD_BANK_QUERY_RESULTS)]
	private void HandleGuildBankQueryResults(WorldPacket packet)
	{
		var result = new GuildBankQueryResults
		{
			Money = packet.ReadUInt64(),
			Tab = packet.ReadUInt8(),
			WithdrawalsRemaining = packet.ReadInt32()
		};
		var hasTabs = false;
		if (packet.ReadBool() && result.Tab == 0)
		{
			hasTabs = true;
			var size = packet.ReadUInt8();
			for (var i = 0; i < size; i++)
			{
				var tabInfo = new GuildBankTabInfo
				{
					TabIndex = i,
					Name = packet.ReadCString(),
					Icon = packet.ReadCString()
				};
				result.TabInfo.Add(tabInfo);
			}
		}
		var slots = packet.ReadUInt8();
		for (var j = 0; j < slots; j++)
		{
			var itemInfo = new GuildBankItemInfo
			{
				Slot = packet.ReadUInt8()
			};
			var entry = packet.ReadInt32();
			if (entry > 0)
			{
				itemInfo.Item.ItemID = (uint)entry;
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
				{
					itemInfo.Flags = packet.ReadUInt32();
				}
				itemInfo.Item.RandomPropertiesID = packet.ReadUInt32();
				if (itemInfo.Item.RandomPropertiesID != 0)
				{
					itemInfo.Item.RandomPropertiesSeed = packet.ReadUInt32();
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					itemInfo.Count = packet.ReadInt32();
				}
				else
				{
					itemInfo.Count = packet.ReadUInt8();
				}
				itemInfo.EnchantmentID = packet.ReadInt32();
				itemInfo.Charges = packet.ReadUInt8();
				var enchantments = packet.ReadUInt8();
				for (var k = 0; k < enchantments; k++)
				{
					var slot = packet.ReadUInt8();
					var enchantId = packet.ReadUInt32();
					if (enchantId != 0)
					{
						var itemId = GameData.GetGemFromEnchantId(enchantId);
						if (itemId != 0)
						{
							var gem = new ItemGemData
							{
								Slot = slot,
								Item =
								{
									ItemID = itemId
								}
							};
							itemInfo.SocketEnchant.Add(gem);
						}
					}
				}
			}
			result.ItemInfo.Add(itemInfo);
		}
		result.FullUpdate = hasTabs && slots > 0;
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.MSG_QUERY_GUILD_BANK_TEXT)]
	private void HandleQueryGuildBankText(WorldPacket packet)
	{
		var result = new GuildBankTextQueryResult
		{
			Tab = packet.ReadUInt8(),
			Text = packet.ReadCString()
		};
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.MSG_GUILD_BANK_LOG_QUERY)]
	private void HandleGuildBankLongQuery(WorldPacket packet)
	{
		var result = new GuildBankLogQueryResults
		{
			Tab = packet.ReadUInt8()
		};
		var logSize = packet.ReadUInt8();
		for (byte i = 0; i < logSize; i++)
		{
			var logEntry = new GuildBankLogEntry
			{
				EntryType = packet.ReadInt8(),
				PlayerGUID = packet.ReadGuid().To128(GetSession().GameState)
			};
			if (result.Tab != 6)
			{
				logEntry.ItemID = packet.ReadInt32();
				logEntry.Count = packet.ReadUInt8();
				if (logEntry.EntryType == 3 || logEntry.EntryType == 7)
				{
					logEntry.OtherTab = packet.ReadInt8();
				}
			}
			else
			{
				logEntry.Money = packet.ReadUInt32();
			}
			logEntry.TimeOffset = packet.ReadUInt32();
			result.Entry.Add(logEntry);
		}
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.MSG_GUILD_BANK_MONEY_WITHDRAWN)]
	private void HandleGuildBankMoneyWithdrawn(WorldPacket packet)
	{
		var result = new GuildBankRemainingWithdrawMoney
		{
			RemainingWithdrawMoney = packet.ReadUInt32()
		};
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.SMSG_UPDATE_INSTANCE_OWNERSHIP)]
	private void HandleUpdateInstanceOwnership(WorldPacket packet)
	{
		var instance = new UpdateInstanceOwnership
		{
			IOwnInstance = packet.ReadUInt32()
		};
		SendPacketToClient(instance);
	}

	[PacketHandler(Opcode.SMSG_INSTANCE_RESET)]
	private void HandleInstanceReset(WorldPacket packet)
	{
		var reset = new InstanceReset
		{
			MapID = packet.ReadUInt32()
		};
		SendPacketToClient(reset);
	}

	[PacketHandler(Opcode.SMSG_INSTANCE_RESET_FAILED)]
	private void HandleInstanceResetFailed(WorldPacket packet)
	{
		var reset = new InstanceResetFailed
		{
			ResetFailedReason = (ResetFailedReason)packet.ReadUInt32(),
			MapID = packet.ReadUInt32()
		};
		SendPacketToClient(reset);
	}

	[PacketHandler(Opcode.SMSG_RESET_FAILED_NOTIFY)]
	private void HandleResetFailedNotify(WorldPacket packet)
	{
		var reset = new ResetFailedNotify();
		packet.ReadUInt32();
		SendPacketToClient(reset);
	}

	[PacketHandler(Opcode.SMSG_RAID_INSTANCE_INFO)]
	private void HandleRaidInstanceInfo(WorldPacket packet)
	{
		var infos = new RaidInstanceInfo();
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var instance = new InstanceLock
			{
				MapID = packet.ReadUInt32()
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				instance.DifficultyID = (DifficultyModern)packet.ReadUInt32();
			}
			else if (ModernVersion.ExpansionVersion == 1)
			{
				instance.DifficultyID = DifficultyModern.Raid40;
			}
			else
			{
				instance.DifficultyID = DifficultyModern.Raid25N;
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				instance.InstanceID = packet.ReadUInt64();
				instance.Locked = packet.ReadBool();
				instance.Extended = packet.ReadBool();
				instance.TimeRemaining = packet.ReadInt32();
			}
			else
			{
				instance.TimeRemaining = packet.ReadInt32();
				instance.InstanceID = packet.ReadUInt32();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					packet.ReadUInt32();
				}
			}
			infos.LockList.Add(instance);
		}
		SendPacketToClient(infos);
	}

	[PacketHandler(Opcode.SMSG_INSTANCE_SAVE_CREATED)]
	private void HandleInstanceSaveCreated(WorldPacket packet)
	{
		var save = new InstanceSaveCreated
		{
			Gm = packet.ReadUInt32() != 0
		};
		SendPacketToClient(save);
	}

	[PacketHandler(Opcode.SMSG_RAID_GROUP_ONLY)]
	private void HandleRaidGroupOnly(WorldPacket packet)
	{
		var save = new RaidGroupOnly
		{
			Delay = packet.ReadInt32(),
			Reason = (RaidGroupReason)packet.ReadUInt32()
		};
		SendPacketToClient(save);
	}

	[PacketHandler(Opcode.SMSG_RAID_INSTANCE_MESSAGE)]
	private void HandleRaidInstanceMessage(WorldPacket packet)
	{
		var instance = new RaidInstanceMessage
		{
			Type = (InstanceResetWarningType)packet.ReadUInt32(),
			MapID = packet.ReadUInt32()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			instance.DifficultyID = (DifficultyModern)packet.ReadUInt32();
		}
		else if (ModernVersion.ExpansionVersion == 1)
		{
			instance.DifficultyID = DifficultyModern.Raid40;
		}
		else
		{
			instance.DifficultyID = DifficultyModern.Raid25N;
		}
		packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) && instance.Type == InstanceResetWarningType.Welcome)
		{
			instance.Locked = packet.ReadBool();
			instance.Extended = packet.ReadBool();
		}
		SendPacketToClient(instance);
	}

	[PacketHandler(Opcode.SMSG_SET_PROFICIENCY)]
	private void HandleSetProficiency(WorldPacket packet)
	{
		var proficiency = new SetProficiency
		{
			ProficiencyClass = packet.ReadUInt8(),
			ProficiencyMask = packet.ReadUInt32()
		};
		SendPacketToClient(proficiency);
	}

	[PacketHandler(Opcode.SMSG_BUY_SUCCEEDED)]
	private void HandleBuySucceeded(WorldPacket packet)
	{
		var buy = new BuySucceeded
		{
			VendorGUID = packet.ReadGuid().To128(GetSession().GameState),
			Muid = packet.ReadUInt32(),
			NewQuantity = packet.ReadInt32(),
			QuantityBought = packet.ReadUInt32()
		};
		SendPacketToClient(buy);
	}

	[PacketHandler(Opcode.SMSG_ITEM_PUSH_RESULT)]
	private void HandleItemPushResult(WorldPacket packet)
	{
		var item = new ItemPushResult
		{
			PlayerGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		var fromNPC = packet.ReadUInt32() == 1;
		item.Created = packet.ReadUInt32() == 1;
		var showInChat = packet.ReadUInt32() == 1;
		if (fromNPC && !item.Created)
		{
			item.DisplayText = ItemPushResult.DisplayType.Received;
			item.Pushed = true;
		}
		else if (!showInChat)
		{
			item.DisplayText = ItemPushResult.DisplayType.Hidden;
		}
		else
		{
			item.DisplayText = ItemPushResult.DisplayType.Loot;
		}
		item.Slot = packet.ReadUInt8();
		item.SlotInBag = packet.ReadInt32();
		item.Item.ItemID = packet.ReadUInt32();
		// Pre-query this item's template if not cached, so hotfix is ready before client requests it
		if (!GameData.ItemTemplates.ContainsKey(item.Item.ItemID))
		{
			var queryPacket = new WorldPacket(Opcode.CMSG_ITEM_QUERY_SINGLE);
			queryPacket.WriteUInt32(item.Item.ItemID);
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				queryPacket.WriteGuid(WowGuid64.Empty);
			}
			SendPacketToServer(queryPacket);
		}
		item.Item.RandomPropertiesSeed = packet.ReadUInt32();
		item.Item.RandomPropertiesID = packet.ReadUInt32();
		item.Quantity = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			item.QuantityInInventory = packet.ReadUInt32();
		}
		else
		{
			var currentCount = 0u;
			var objective = GameData.GetQuestObjectiveForItem(item.Item.ItemID);
			if (objective != null)
			{
				var updateFields = GetSession().GameState.GetCachedObjectFieldsLegacy(GetSession().GameState.CurrentPlayerGuid);
				var questsCount = LegacyVersion.GetQuestLogSize();
				for (var i = 0; i < questsCount; i++)
				{
					var logEntry = ReadQuestLogEntry(i, null, updateFields);
					if (logEntry != null && logEntry.QuestID.HasValue && logEntry.QuestID == objective.QuestID && logEntry.ObjectiveProgress[objective.StorageIndex].HasValue)
					{
						currentCount = (uint)logEntry.ObjectiveProgress[objective.StorageIndex].Value;
						break;
					}
				}
			}
			item.QuantityInInventory = item.Quantity + currentCount;
		}
		if (item.Slot == byte.MaxValue && item.SlotInBag >= 0 && item.PlayerGUID == GetSession().GameState.CurrentPlayerGuid)
		{
			item.ItemGUID = GetSession().GameState.GetInventorySlotItem(item.SlotInBag).To128(GetSession().GameState);
		}
		else
		{
			item.ItemGUID = WowGuid128.Empty;
		}
		SendPacketToClient(item);
	}

	[PacketHandler(Opcode.SMSG_READ_ITEM_RESULT_OK)]
	private void HandleReadItemResultOk(WorldPacket packet)
	{
		var read = new ReadItemResultOK
		{
			ItemGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(read);
	}

	[PacketHandler(Opcode.SMSG_READ_ITEM_RESULT_FAILED)]
	private void HandleReadItemResultFailed(WorldPacket packet)
	{
		var read = new ReadItemResultFailed
		{
			ItemGUID = packet.ReadGuid().To128(GetSession().GameState),
			Subcode = 2
		};
		SendPacketToClient(read);
	}

	[PacketHandler(Opcode.SMSG_BUY_FAILED)]
	private void HandleBuyFailed(WorldPacket packet)
	{
		var fail = new BuyFailed
		{
			VendorGUID = packet.ReadGuid().To128(GetSession().GameState),
			Muid = packet.ReadUInt32(),
			Reason = (BuyResult)packet.ReadUInt8()
		};
		SendPacketToClient(fail);
	}

	[PacketHandler(Opcode.SMSG_INVENTORY_CHANGE_FAILURE, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandleInventoryChangeFailureVanilla(WorldPacket packet)
	{
		var failure = new InventoryChangeFailure
		{
			BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt8())
		};
		if (failure.BagResult != InventoryResult.Ok)
		{
			var bagResult = failure.BagResult;
			var inventoryResult = bagResult;
			if (inventoryResult == InventoryResult.CantEquipLevel)
			{
				failure.Level = packet.ReadInt32();
			}
			failure.Item[0] = packet.ReadGuid().To128(GetSession().GameState);
			failure.Item[1] = packet.ReadGuid().To128(GetSession().GameState);
			failure.ContainerBSlot = packet.ReadUInt8();
			SendPacketToClient(failure);
			if (GetSession().GameState.CurrentClientNormalCast != null && !GetSession().GameState.CurrentClientNormalCast.HasStarted && GetSession().GameState.CurrentClientNormalCast.ItemGUID == failure.Item[0])
			{
				GetSession().InstanceSocket.SendCastRequestFailed(GetSession().GameState.CurrentClientNormalCast, isPet: false);
				GetSession().GameState.CurrentClientNormalCast = null;
			}
		}
	}

	[PacketHandler(Opcode.SMSG_INVENTORY_CHANGE_FAILURE, ClientVersionBuild.V2_0_1_6180)]
	private void HandleInventoryChangeFailure(WorldPacket packet)
	{
		var failure = new InventoryChangeFailure
		{
			BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt8())
		};
		if (failure.BagResult != InventoryResult.Ok)
		{
			failure.Item[0] = packet.ReadGuid().To128(GetSession().GameState);
			failure.Item[1] = packet.ReadGuid().To128(GetSession().GameState);
			failure.ContainerBSlot = packet.ReadUInt8();
			switch (failure.BagResult)
			{
			case InventoryResult.CantEquipLevel:
			case InventoryResult.PurchaseLevelTooLow:
				failure.Level = packet.ReadInt32();
				break;
			case InventoryResult.EventAutoEquipBindConfirm:
				failure.SrcContainer = packet.ReadGuid().To128(GetSession().GameState);
				failure.SrcSlot = packet.ReadInt32();
				failure.DstContainer = packet.ReadGuid().To128(GetSession().GameState);
				break;
			case InventoryResult.ItemMaxLimitCategoryCountExceeded:
			case InventoryResult.ItemMaxLimitCategorySocketedExceeded:
			case InventoryResult.ItemMaxLimitCategoryEquippedExceeded:
				failure.LimitCategory = packet.ReadInt32();
				break;
			}
			SendPacketToClient(failure);
			if (GetSession().GameState.CurrentClientNormalCast != null && !GetSession().GameState.CurrentClientNormalCast.HasStarted && GetSession().GameState.CurrentClientNormalCast.ItemGUID == failure.Item[0])
			{
				GetSession().InstanceSocket.SendCastRequestFailed(GetSession().GameState.CurrentClientNormalCast, isPet: false);
				GetSession().GameState.CurrentClientNormalCast = null;
			}
		}
	}

	[PacketHandler(Opcode.SMSG_DURABILITY_DAMAGE_DEATH)]
	private void HandleDurabilityDamageDeath(WorldPacket packet)
	{
		var death = new DurabilityDamageDeath
		{
			Percent = 10u
		};
		SendPacketToClient(death);
	}

	[PacketHandler(Opcode.SMSG_ITEM_COOLDOWN)]
	private void HandleItemCooldown(WorldPacket packet)
	{
		var item = new ItemCooldown
		{
			ItemGuid = packet.ReadGuid().To128(GetSession().GameState),
			SpellID = packet.ReadUInt32(),
			Cooldown = 30000u
		};
		SendPacketToClient(item);
	}

	[PacketHandler(Opcode.SMSG_SELL_RESPONSE)]
	private void HandleSellResponse(WorldPacket packet)
	{
		var sell = new SellResponse
		{
			VendorGUID = packet.ReadGuid().To128(GetSession().GameState),
			ItemGUID = packet.ReadGuid().To128(GetSession().GameState),
			Reason = packet.ReadUInt8()
		};
		Log.Print(LogType.Debug, $"[SellResponse] Item={sell.ItemGUID} Vendor={sell.VendorGUID} Reason={sell.Reason}", "WorldClient.cs");
		SendPacketToClient(sell);
	}

	[PacketHandler(Opcode.SMSG_ITEM_ENCHANT_TIME_UPDATE)]
	private void HandleItemEnchantTimeUpdate(WorldPacket packet)
	{
		var enchant = new ItemEnchantTimeUpdate
		{
			ItemGuid = packet.ReadGuid().To128(GetSession().GameState),
			Slot = packet.ReadUInt32(),
			DurationLeft = packet.ReadUInt32(),
			OwnerGuid = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(enchant);
	}

	[PacketHandler(Opcode.SMSG_ENCHANTMENT_LOG)]
	private void HandleEnchantmentLog(WorldPacket packet)
	{
		var enchantment = new EnchantmentLog();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			enchantment.Owner = packet.ReadPackedGuid().To128(GetSession().GameState);
			enchantment.Caster = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		else
		{
			enchantment.Owner = packet.ReadGuid().To128(GetSession().GameState);
			enchantment.Caster = packet.ReadGuid().To128(GetSession().GameState);
		}
		enchantment.ItemID = packet.ReadInt32();
		var session = GetSession().GameState;
		for (var i = 0; i < 23; i++)
		{
			if (session.GetItemId(session.GetInventorySlotItem(i).To128(session)).Equals((uint)enchantment.ItemID))
			{
				enchantment.ItemGUID = session.GetInventorySlotItem(i).To128(session);
				break;
			}
		}
		if (!(enchantment.ItemGUID == null))
		{
			enchantment.Enchantment = packet.ReadInt32();
			SendPacketToClient(enchantment);
		}
	}

	[PacketHandler(Opcode.SMSG_LOOT_LIST)]
	private void HandleLootList(WorldPacket packet)
	{
		var list = new LootList();
		var creatureGuid = packet.ReadGuid();
		list.Owner = creatureGuid.To128(GetSession().GameState);
		list.LootObj = creatureGuid.ToLootGuid();

		var masterLooter = packet.ReadPackedGuid();
		if (!masterLooter.IsEmpty())
			list.Master = masterLooter.To128(GetSession().GameState);

		var roundRobinWinner = packet.ReadPackedGuid();
		if (!roundRobinWinner.IsEmpty())
			list.RoundRobinWinner = roundRobinWinner.To128(GetSession().GameState);

		SendPacketToClient(list);
	}

	[PacketHandler(Opcode.SMSG_LOOT_RESPONSE)]
	private void HandleLootResponse(WorldPacket packet)
	{
		var loot = new LootResponse();
		GetSession().GameState.LastLootTargetGuid = packet.ReadGuid();
		loot.Owner = GetSession().GameState.LastLootTargetGuid.To128(GetSession().GameState);
		loot.LootObj = GetSession().GameState.LastLootTargetGuid.ToLootGuid();
		loot.AcquireReason = (LootType)packet.ReadUInt8();
		if (loot.AcquireReason == LootType.None)
		{
			loot.FailureReason = (LootError)packet.ReadUInt8();
			return;
		}
		loot.LootMethod = GetSession().GameState.GetCurrentLootMethod();
		loot.Coins = packet.ReadUInt32();
		var itemsCount = packet.ReadUInt8();
		for (var i = 0; i < itemsCount; i++)
		{
			var lootItem = new LootItemData
			{
				LootListID = packet.ReadUInt8(),
				Loot =
				{
					ItemID = packet.ReadUInt32()
				},
				Quantity = packet.ReadUInt32()
			};
			packet.ReadUInt32();
			lootItem.Loot.RandomPropertiesSeed = packet.ReadUInt32();
			lootItem.Loot.RandomPropertiesID = packet.ReadUInt32();
			var uiType = (LootSlotTypeLegacy)packet.ReadUInt8();
			lootItem.UIType = (LootSlotTypeModern)Enum.Parse(typeof(LootSlotTypeModern), uiType.ToString());
			loot.Items.Add(lootItem);
		}
		SendPacketToClient(loot);
	}

	[PacketHandler(Opcode.SMSG_LOOT_RELEASE)]
	private void HandleLootRelease(WorldPacket packet)
	{
		var loot = new LootReleaseResponse();
		var owner = packet.ReadGuid();
		loot.Owner = owner.To128(GetSession().GameState);
		loot.LootObj = owner.ToLootGuid();
		packet.ReadBool();
		SendPacketToClient(loot);
	}

	[PacketHandler(Opcode.SMSG_LOOT_REMOVED)]
	private void HandleLootRemoved(WorldPacket packet)
	{
		var loot = new LootRemoved
		{
			Owner = GetSession().GameState.LastLootTargetGuid.To128(GetSession().GameState),
			LootObj = GetSession().GameState.LastLootTargetGuid.ToLootGuid(),
			LootListID = packet.ReadUInt8()
		};
		SendPacketToClient(loot);
	}

	[PacketHandler(Opcode.SMSG_LOOT_MONEY_NOTIFY)]
	private void HandleLootMoneyNotify(WorldPacket packet)
	{
		var loot = new LootMoneyNotify
		{
			Money = packet.ReadUInt32()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			loot.SoleLooter = packet.ReadBool();
		}
		SendPacketToClient(loot);
	}

	[PacketHandler(Opcode.SMSG_LOOT_CLEAR_MONEY)]
	private void HandleLootCelarMoney(WorldPacket packet)
	{
		var loot = new CoinRemoved
		{
			LootObj = GetSession().GameState.LastLootTargetGuid.ToLootGuid()
		};
		SendPacketToClient(loot);
	}

	[PacketHandler(Opcode.SMSG_LOOT_START_ROLL)]
	private void HandleLootStartRoll(WorldPacket packet)
	{
		var loot = new StartLootRoll();
		var owner = packet.ReadGuid();
		loot.LootObj = owner.ToLootGuid();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			loot.MapID = packet.ReadUInt32();
		}
		else
		{
			loot.MapID = GetSession().GameState.CurrentMapId.Value;
		}
		loot.Item.LootListID = (byte)packet.ReadUInt32();
		loot.Item.Loot.ItemID = packet.ReadUInt32();
		loot.Item.Loot.RandomPropertiesSeed = packet.ReadUInt32();
		loot.Item.Loot.RandomPropertiesID = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			loot.Item.Quantity = packet.ReadUInt32();
		}
		else
		{
			loot.Item.Quantity = 1u;
		}
		loot.RollTime = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			loot.ValidRolls = (RollMask)packet.ReadUInt8();
		}
		else
		{
			loot.ValidRolls = RollMask.AllNoDisenchant;
		}
		SendPacketToClient(loot);
		if (GetSession().GameState.IsPassingOnLoot)
		{
			var packet2 = new WorldPacket(Opcode.CMSG_LOOT_ROLL);
			packet2.WriteGuid(owner);
			packet2.WriteUInt32(loot.Item.LootListID);
			packet2.WriteUInt8(0);
			SendPacketToServer(packet2);
		}
	}

	[PacketHandler(Opcode.SMSG_LOOT_ROLL)]
	private void HandleLootRoll(WorldPacket packet)
	{
		var loot = new LootRollBroadcast();
		var owner = packet.ReadGuid();
		loot.LootObj = owner.ToLootGuid();
		loot.Item.LootListID = (byte)packet.ReadUInt32();
		loot.Player = packet.ReadGuid().To128(GetSession().GameState);
		loot.Item.Loot.ItemID = packet.ReadUInt32();
		loot.Item.Loot.RandomPropertiesSeed = packet.ReadUInt32();
		loot.Item.Loot.RandomPropertiesID = packet.ReadUInt32();
		loot.Item.Quantity = 1u;
		loot.Roll = packet.ReadUInt8();
		var rollType = packet.ReadUInt8();
		if (loot.Roll == 128 && rollType == 128)
		{
			loot.RollType = RollType.Pass;
		}
		else if (loot.Roll == 0 && rollType == 0)
		{
			loot.RollType = RollType.Need;
		}
		else
		{
			loot.RollType = (RollType)rollType;
		}
		if (loot.Roll == 128)
		{
			loot.Roll = 0;
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			loot.Autopassed = packet.ReadBool();
		}
		SendPacketToClient(loot);
	}

	[PacketHandler(Opcode.SMSG_LOOT_ROLL_WON)]
	private void HandleLootRollWon(WorldPacket packet)
	{
		var loot = new LootRollWon
		{
			LootObj = packet.ReadGuid().ToLootGuid(),
			Item =
			{
				LootListID = (byte)packet.ReadUInt32(),
				Loot =
				{
					ItemID = packet.ReadUInt32(),
					RandomPropertiesSeed = packet.ReadUInt32(),
					RandomPropertiesID = packet.ReadUInt32()
				},
				Quantity = 1u
			},
			Winner = packet.ReadGuid().To128(GetSession().GameState),
			Roll = packet.ReadUInt8(),
			RollType = (RollType)packet.ReadUInt8()
		};
		if (loot.RollType == RollType.Need)
		{
			loot.MainSpec = 128;
		}
		SendPacketToClient(loot);
		var complete = new LootRollsComplete
		{
			LootObj = loot.LootObj,
			LootListID = loot.Item.LootListID
		};
		SendPacketToClient(complete);
	}

	[PacketHandler(Opcode.SMSG_LOOT_ALL_PASSED)]
	private void HandleLootAllPassed(WorldPacket packet)
	{
		var loot = new LootAllPassed
		{
			LootObj = packet.ReadGuid().ToLootGuid(),
			Item =
			{
				LootListID = (byte)packet.ReadUInt32(),
				Loot =
				{
					ItemID = packet.ReadUInt32(),
					RandomPropertiesSeed = packet.ReadUInt32(),
					RandomPropertiesID = packet.ReadUInt32()
				},
				Quantity = 1u
			}
		};
		SendPacketToClient(loot);
		var complete = new LootRollsComplete
		{
			LootObj = loot.LootObj,
			LootListID = loot.Item.LootListID
		};
		SendPacketToClient(complete);
	}

	[PacketHandler(Opcode.SMSG_LOOT_MASTER_LIST)]
	private void HandleLootMasterList(WorldPacket packet)
	{
		if (!(GetSession().GameState.LastLootTargetGuid == null))
		{
			var list = new LootList
			{
				Owner = GetSession().GameState.LastLootTargetGuid.To128(GetSession().GameState),
				LootObj = GetSession().GameState.LastLootTargetGuid.ToLootGuid(),
				Master = GetSession().GameState.CurrentPlayerGuid
			};
			SendPacketToClient(list);
			var loot = new MasterLootCandidateList
			{
				LootObj = GetSession().GameState.LastLootTargetGuid.ToLootGuid()
			};
			var count = packet.ReadUInt8();
			for (byte i = 0; i < count; i++)
			{
				var guid = packet.ReadGuid().To128(GetSession().GameState);
				loot.Players.Add(guid);
			}
			SendPacketToClient(loot);
		}
	}

	[PacketHandler(Opcode.SMSG_NOTIFY_RECEIVED_MAIL)]
	private void HandleNotifyReceivedMail(WorldPacket packet)
	{
		var mail = new NotifyReceivedMail
		{
			Delay = packet.ReadFloat()
		};
		SendPacketToClient(mail);
	}

	[PacketHandler(Opcode.MSG_QUERY_NEXT_MAIL_TIME)]
	private void HandleQueryNextMailTime(WorldPacket packet)
	{
		var result = new MailQueryNextTimeResult
		{
			NextMailTime = packet.ReadFloat()
		};
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_3_0_7561))
		{
			if (result.NextMailTime == 0f)
			{
				var mail = new MailQueryNextTimeResult.MailNextTimeEntry
				{
					SenderGuid = GetSession().GameState.CurrentPlayerGuid,
					AltSenderID = 0,
					AltSenderType = 0,
					StationeryID = 41,
					TimeLeft = 3600f
				};
				result.Mails.Add(mail);
			}
		}
		else
		{
			var count = packet.ReadUInt32();
			for (var i = 0; i < count; i++)
			{
				var mail2 = new MailQueryNextTimeResult.MailNextTimeEntry
				{
					SenderGuid = packet.ReadGuid().To128(GetSession().GameState),
					AltSenderID = packet.ReadInt32(),
					AltSenderType = (sbyte)packet.ReadInt32(),
					StationeryID = packet.ReadInt32(),
					TimeLeft = packet.ReadFloat()
				};
				result.Mails.Add(mail2);
			}
		}
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.SMSG_MAIL_LIST_RESULT)]
	private void HandleMailListResult(WorldPacket packet)
	{
		var result = new MailListResult();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			result.TotalNumRecords = packet.ReadInt32();
		}
		var count = packet.ReadUInt8();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			result.TotalNumRecords = count;
		}
		for (var i = 0; i < count; i++)
		{
			var mail = new MailListEntry();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				packet.ReadUInt16();
			}
			mail.MailID = packet.ReadInt32();
			mail.SenderType = (MailType)packet.ReadUInt8();
			switch (mail.SenderType)
			{
			case MailType.Normal:
				mail.SenderCharacter = packet.ReadGuid().To128(GetSession().GameState);
				break;
			case MailType.Item:
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					mail.AltSenderID = packet.ReadUInt32();
				}
				break;
			default:
				mail.AltSenderID = packet.ReadUInt32();
				break;
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				mail.Cod = packet.ReadUInt32();
			}
			else
			{
				mail.Subject = packet.ReadCString();
			}
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_3_0_10958))
			{
				mail.ItemTextId = packet.ReadUInt32();
				if (mail.ItemTextId != 0 && !GetSession().GameState.ItemTexts.ContainsKey(mail.ItemTextId))
				{
					GetSession().GameState.RequestedItemTextIds.Add(mail.ItemTextId);
					var query = new WorldPacket(Opcode.CMSG_ITEM_TEXT_QUERY);
					query.WriteUInt32(mail.ItemTextId);
					query.WriteInt32(mail.MailID);
					query.WriteUInt32(0u);
					SendPacket(query);
				}
			}
			packet.ReadUInt32();
			mail.StationeryID = packet.ReadInt32();
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				var mailItem = ReadMailItem(packet);
				if (mailItem.Item.ItemID != 0)
				{
					mailItem.AttachID = 1;
					mail.Attachments.Add(mailItem);
				}
			}
			mail.SentMoney = packet.ReadUInt32();
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				mail.Cod = packet.ReadUInt32();
			}
			mail.Flags = packet.ReadUInt32();
			mail.DaysLeft = packet.ReadFloat();
			mail.MailTemplateID = packet.ReadInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				mail.Subject = packet.ReadCString();
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
			{
				mail.Body = packet.ReadCString();
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				var itemsCount = packet.ReadUInt8();
				for (var j = 0; j < itemsCount; j++)
				{
					var mailItem2 = ReadMailItem(packet);
					mail.Attachments.Add(mailItem2);
				}
			}
			result.Mails.Add(mail);
		}
		if (GetSession().GameState.RequestedItemTextIds.Count == 0)
		{
			foreach (var mail2 in result.Mails)
			{
				if (mail2.ItemTextId != 0)
				{
					mail2.Body = GetSession().GameState.ItemTexts[mail2.ItemTextId];
				}
			}
			SendPacketToClient(result);
		}
		else
		{
			GetSession().GameState.PendingMailListPacket = result;
		}
	}

	[PacketHandler(Opcode.SMSG_QUERY_ITEM_TEXT_RESPONSE)]
	private void HandleQueryItemTextResponse(WorldPacket packet)
	{
		var itemTextId = packet.ReadUInt32();
		var text = packet.ReadCString();
		if (GetSession().GameState.ItemTexts.ContainsKey(itemTextId))
		{
			GetSession().GameState.ItemTexts[itemTextId] = text;
		}
		else
		{
			GetSession().GameState.ItemTexts.Add(itemTextId, text);
		}
		if (GetSession().GameState.RequestedItemTextIds.Contains(itemTextId))
		{
			GetSession().GameState.RequestedItemTextIds.Remove(itemTextId);
		}
		if (GetSession().GameState.PendingMailListPacket == null || GetSession().GameState.RequestedItemTextIds.Count != 0)
		{
			return;
		}
		var result = GetSession().GameState.PendingMailListPacket;
		foreach (var mail in result.Mails)
		{
			if (mail.ItemTextId != 0)
			{
				mail.Body = GetSession().GameState.ItemTexts[mail.ItemTextId];
			}
		}
		SendPacketToClient(result);
	}

	private MailAttachedItem ReadMailItem(WorldPacket packet)
	{
		var mailItem = new MailAttachedItem();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			mailItem.Position = packet.ReadUInt8();
			mailItem.AttachID = packet.ReadInt32();
		}
		mailItem.Item.ItemID = packet.ReadUInt32();
		var enchantmentCount = (byte)(LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? 7 : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? 1 : 6));
		for (byte k = 0; k < enchantmentCount; k++)
		{
			var enchant = new ItemEnchantData
			{
				Slot = k
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				enchant.Charges = packet.ReadInt32();
				enchant.Expiration = packet.ReadUInt32();
			}
			enchant.ID = packet.ReadUInt32();
			if (enchant.ID != 0)
			{
				mailItem.Enchants.Add(enchant);
			}
		}
		mailItem.Item.RandomPropertiesID = packet.ReadUInt32();
		mailItem.Item.RandomPropertiesSeed = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			mailItem.Count = (byte)packet.ReadUInt32();
		}
		else
		{
			mailItem.Count = packet.ReadUInt8();
		}
		mailItem.Charges = packet.ReadInt32();
		mailItem.MaxDurability = packet.ReadUInt32();
		mailItem.Durability = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			mailItem.Unlocked = packet.ReadBool();
		}
		return mailItem;
	}

	[PacketHandler(Opcode.SMSG_MAIL_COMMAND_RESULT)]
	private void HandleMailCommandResult(WorldPacket packet)
	{
		var mail = new MailCommandResult
		{
			MailID = packet.ReadUInt32(),
			Command = (MailActionType)packet.ReadUInt32(),
			ErrorCode = (MailErrorType)packet.ReadUInt32()
		};
		if (mail.ErrorCode == MailErrorType.Equip)
		{
			mail.BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32());
		}
		else if (mail.Command == MailActionType.AttachmentExpired)
		{
			mail.AttachID = packet.ReadUInt32();
			mail.QtyInInventory = packet.ReadUInt32();
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				mail.AttachID = 1u;
			}
		}
		SendPacketToClient(mail);
	}

	[PacketHandler(Opcode.SMSG_PONG)]
	private void HandlePingResponse(WorldPacket packet)
	{
		var serial = packet.ReadUInt32();
		SendPacketToClient(new Pong(serial));
	}

	[PacketHandler(Opcode.SMSG_TUTORIAL_FLAGS)]
	private void HandleTutorialFlags(WorldPacket packet)
	{
		var tutorials = new TutorialFlags();
		for (byte i = 0; i < 8; i++)
		{
			tutorials.TutorialData[i] = packet.ReadUInt32();
		}
		SendPacketToClient(tutorials);
	}

	[PacketHandler(Opcode.SMSG_ACCOUNT_DATA_TIMES)]
	private void HandleAccountDataTimes(WorldPacket packet)
	{
		GetSession().RealmSocket.SendAccountDataTimes();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			GetSession().RealmSocket.SendFeatureSystemStatus();
			GetSession().RealmSocket.SendMotd();
			GetSession().RealmSocket.SendSetTimeZoneInformation();
			GetSession().RealmSocket.SendSeasonInfo();
		}
	}

	[PacketHandler(Opcode.SMSG_BIND_POINT_UPDATE)]
	private void HandleBindPointUpdate(WorldPacket packet)
	{
		var point = new BindPointUpdate
		{
			BindPosition = packet.ReadVector3(),
			BindMapID = packet.ReadUInt32(),
			BindAreaID = packet.ReadUInt32()
		};
		SendPacketToClient(point);
	}

	[PacketHandler(Opcode.SMSG_PLAYER_BOUND)]
	private void HandlePlayerBound(WorldPacket packet)
	{
		var bound = new PlayerBound
		{
			BinderGUID = packet.ReadGuid().To128(GetSession().GameState),
			AreaID = packet.ReadUInt32()
		};
		SendPacketToClient(bound);
	}

	[PacketHandler(Opcode.SMSG_DEATH_RELEASE_LOC)]
	private void HandleDeathReleaseLoc(WorldPacket packet)
	{
		var death = new DeathReleaseLoc
		{
			MapID = packet.ReadInt32(),
			Location = packet.ReadVector3()
		};
		Log.Print(LogType.Debug, $"[DeathReleaseLoc] MapID={death.MapID} Pos={death.Location}", "WorldClient.cs");
		SendPacketToClient(death);
	}

	[PacketHandler(Opcode.SMSG_PRE_RESSURECT)]
	private void HandlePreResurrect(WorldPacket packet)
	{
		var playerGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
		var preRes = new PreRessurect
		{
			PlayerGUID = playerGuid
		};
		SendPacketToClient(preRes);
	}

	[PacketHandler(Opcode.SMSG_CORPSE_RECLAIM_DELAY)]
	private void HandleCorpseReclaimDelay(WorldPacket packet)
	{
		var delay = new CorpseReclaimDelay
		{
			Remaining = packet.ReadUInt32()
		};
		SendPacketToClient(delay);
	}

	[PacketHandler(Opcode.SMSG_TIME_SYNC_REQUEST)]
	private void HandleTimeSyncRequest(WorldPacket packet)
	{
		var sync = new TimeSyncRequest
		{
			SequenceIndex = packet.ReadUInt32()
		};
		SendPacketToClient(sync);
	}

	[PacketHandler(Opcode.SMSG_WEATHER)]
	private void HandleWeather(WorldPacket packet)
	{
		var weather = new WeatherPkt();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var type = (WeatherType)packet.ReadUInt32();
			weather.Intensity = packet.ReadFloat();
			weather.WeatherID = Weather.ConvertWeatherTypeToWeatherState(type, weather.Intensity);
			packet.ReadUInt32();
			if (packet.CanRead())
			{
				weather.Abrupt = packet.ReadBool();
			}
		}
		else
		{
			weather.WeatherID = (WeatherState)packet.ReadUInt32();
			weather.Intensity = packet.ReadFloat();
			weather.Abrupt = packet.ReadBool();
		}
		SendPacketToClient(weather);
		SendPacketToClient(new StartLightningStorm());
	}

	[PacketHandler(Opcode.SMSG_LOGIN_SET_TIME_SPEED)]
	private void HandleLoginSetTimeSpeed(WorldPacket packet)
	{
		if (GetSession().GameState.IsFirstEnterWorld)
		{
			var login = new LoginSetTimeSpeed
			{
				ServerTime = packet.ReadUInt32()
			};
			login.GameTime = login.ServerTime;
			login.NewSpeed = packet.ReadFloat();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901))
			{
				login.ServerTimeHolidayOffset = packet.ReadInt32();
				login.GameTimeHolidayOffset = login.ServerTimeHolidayOffset;
			}
			SendPacketToClient(login);
		}
	}

	[PacketHandler(Opcode.SMSG_AREA_TRIGGER_MESSAGE)]
	private void HandleAreaTriggerMessage(WorldPacket packet)
	{
		var length = packet.ReadUInt32();
		var message = packet.ReadString(length);
		if (GetSession().GameState.LastEnteredAreaTrigger != 0)
		{
			var denied = new AreaTriggerMessage
			{
				AreaTriggerID = GetSession().GameState.LastEnteredAreaTrigger
			};
			SendPacketToClient(denied);
		}
		else
		{
			var chat = new ChatPkt(GetSession(), ChatMessageTypeModern.System, message);
			SendPacketToClient(chat);
		}
	}

	[PacketHandler(Opcode.MSG_CORPSE_QUERY)]
	private void HandleCorpseQuery(WorldPacket packet)
	{
		var corpse = new CorpseLocation
		{
			Player = GetSession().GameState.CurrentPlayerGuid,
			Transport = WowGuid128.Empty,
			Valid = packet.ReadBool()
		};
		if (corpse.Valid)
		{
			corpse.ActualMapID = packet.ReadInt32();
			corpse.Position = packet.ReadVector3();
			corpse.MapID = packet.ReadInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_2_10482))
			{
				packet.ReadInt32();
			}
		}
		else
		{
			corpse.MapID = (corpse.ActualMapID = (int)GetSession().GameState.CurrentMapId.Value);
		}
		SendPacketToClient(corpse);
	}

	[PacketHandler(Opcode.SMSG_STAND_STATE_UPDATE)]
	private void HandleStandStateUpdate(WorldPacket packet)
	{
		var state = new StandStateUpdate
		{
			StandState = packet.ReadUInt8()
		};
		SendPacketToClient(state);
	}

	[PacketHandler(Opcode.SMSG_EXPLORATION_EXPERIENCE)]
	private void HandleExplorationExperience(WorldPacket packet)
	{
		var explore = new ExplorationExperience
		{
			AreaID = packet.ReadUInt32(),
			Experience = packet.ReadUInt32()
		};
		SendPacketToClient(explore);
	}

	[PacketHandler(Opcode.SMSG_PLAY_MUSIC)]
	private void HandlePlayMusic(WorldPacket packet)
	{
		var music = new PlayMusic
		{
			SoundEntryID = packet.ReadUInt32()
		};
		SendPacketToClient(music);
	}

	[PacketHandler(Opcode.SMSG_PLAY_SOUND)]
	private void HandlePlaySound(WorldPacket packet)
	{
		var sound = new PlaySound
		{
			SoundEntryID = packet.ReadUInt32(),
			SourceObjectGuid = GetSession().GameState.CurrentPlayerGuid
		};
		SendPacketToClient(sound);
	}

	[PacketHandler(Opcode.SMSG_PLAY_OBJECT_SOUND)]
	private void HandlePlayObjectSound(WorldPacket packet)
	{
		var sound = new PlayObjectSound
		{
			SoundEntryID = packet.ReadUInt32(),
			SourceObjectGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		sound.TargetObjectGUID = sound.SourceObjectGUID;
		SendPacketToClient(sound);
	}

	[PacketHandler(Opcode.SMSG_TRIGGER_CINEMATIC)]
	private void HandleTriggerCinematic(WorldPacket packet)
	{
		var cinematic = new TriggerCinematic
		{
			CinematicID = packet.ReadUInt32()
		};
		SendPacketToClient(cinematic);
	}

	[PacketHandler(Opcode.SMSG_SPECIAL_MOUNT_ANIM)]
	private void HandleSpecialMountAnim(WorldPacket packet)
	{
		var mount = new SpecialMountAnim
		{
			UnitGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(mount);
	}

	[PacketHandler(Opcode.SMSG_START_MIRROR_TIMER)]
	private void HandleStartMirrorTimer(WorldPacket packet)
	{
		var timer = new StartMirrorTimer
		{
			Timer = (MirrorTimerType)packet.ReadUInt32(),
			Value = packet.ReadInt32(),
			MaxValue = packet.ReadInt32(),
			Scale = packet.ReadInt32(),
			Paused = packet.ReadBool(),
			SpellID = packet.ReadInt32()
		};
		SendPacketToClient(timer);
	}

	[PacketHandler(Opcode.SMSG_PAUSE_MIRROR_TIMER)]
	private void HandlePauseMirrorTimer(WorldPacket packet)
	{
		var timer = new PauseMirrorTimer
		{
			Timer = (MirrorTimerType)packet.ReadUInt32(),
			Paused = packet.ReadBool()
		};
		SendPacketToClient(timer);
	}

	[PacketHandler(Opcode.SMSG_STOP_MIRROR_TIMER)]
	private void HandleStopMirrorTimer(WorldPacket packet)
	{
		var timer = new StopMirrorTimer
		{
			Timer = (MirrorTimerType)packet.ReadUInt32()
		};
		SendPacketToClient(timer);
	}

	[PacketHandler(Opcode.SMSG_INVALIDATE_PLAYER)]
	private void HandleInvalidatePlayer(WorldPacket packet)
	{
		var invalidate = new InvalidatePlayer
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(invalidate);
		if (GetSession().GameState.CachedPlayers.ContainsKey(invalidate.Guid))
		{
			GetSession().GameState.CachedPlayers.Remove(invalidate.Guid);
		}
	}

	[PacketHandler(Opcode.SMSG_ZONE_UNDER_ATTACK)]
	private void HandleZoneUnderAttack(WorldPacket packet)
	{
		var zone = new ZoneUnderAttack
		{
			AreaID = packet.ReadInt32()
		};
		SendPacketToClient(zone);
	}

	[PacketHandler(Opcode.MSG_SET_DUNGEON_DIFFICULTY)]
	private void HandleSetDungeonDifficulty(WorldPacket packet)
	{
		var difficulty = new DungeonDifficultySet();
		var difficultyId = packet.ReadInt32();
		difficulty.DifficultyID = (byte)Enum.Parse(typeof(DifficultyModern), ((DifficultyLegacy)difficultyId/*cast due to .constrained prefix*/).ToString());
		packet.ReadInt32();
		packet.ReadInt32();
		SendPacketToClient(difficulty);
	}

	[PacketHandler(Opcode.SMSG_POWER_UPDATE)]
	private void HandlePowerUpdate(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
			var powerType = packet.ReadUInt8();
			var powerValue = packet.ReadInt32();
			var update = new PowerUpdate(guid);
			update.Powers.Add(new PowerUpdatePower(powerValue, powerType));
			SendPacketToClient(update);
		}
	}

	[PacketHandler(Opcode.SMSG_UPDATE_TALENT_DATA)]
	private void HandleUpdateTalentData(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var isPet = packet.ReadUInt8();
			if (isPet != 0)
			{
				// Pet talents - skip for now
				return;
			}

			var talentData = new UpdateTalentData
			{
				IsPetTalents = false,
				UnspentTalentPoints = packet.ReadUInt32()
			};
			var specsCount = packet.ReadUInt8();
			talentData.ActiveGroup = packet.ReadUInt8();

			for (byte spec = 0; spec < specsCount; spec++)
			{
				var group = new TalentGroupInfoData
				{
					SpecID = 4 // MAX_SPECIALIZATIONS - sentinel for "no spec" in WotLK
				};

				var talentCount = packet.ReadUInt8();
				for (byte t = 0; t < talentCount; t++)
				{
					var talent = new TalentInfoData
					{
						TalentID = packet.ReadUInt32(),
						Rank = packet.ReadUInt8()
					};
					group.Talents.Add(talent);
				}

				var glyphCount = packet.ReadUInt8();
				for (byte g = 0; g < glyphCount; g++)
				{
					group.GlyphIDs.Add(packet.ReadUInt16());
				}

				talentData.TalentGroups.Add(group);
			}

			// Compute total talent points (unspent + spent) and store for update fields
			var spentPoints = 0;
			foreach (var group2 in talentData.TalentGroups)
				foreach (var talent in group2.Talents)
					spentPoints += talent.Rank + 1; // rank is 0-based
			var totalPoints = (int)talentData.UnspentTalentPoints + spentPoints;
			GetSession().GameState.TotalTalentPoints = totalPoints;

			// Compute GlyphsEnabled from level (level = totalPoints + 9)
			var level = totalPoints + 9;
			byte glyphsEnabled = 0;
			if (level >= 15) glyphsEnabled |= 0x01 | 0x02; // Major slot 0 + Minor slot 1
			if (level >= 30) glyphsEnabled |= 0x08;         // Major slot 3
			if (level >= 50) glyphsEnabled |= 0x04;         // Major slot 2
			if (level >= 70) glyphsEnabled |= 0x10;         // Minor slot 4
			if (level >= 80) glyphsEnabled |= 0x20;         // Minor slot 5
			GetSession().GameState.GlyphsEnabled = glyphsEnabled;

			// Store active glyphs from active spec
			if (talentData.TalentGroups.Count > talentData.ActiveGroup)
			{
				var activeGroup = talentData.TalentGroups[talentData.ActiveGroup];
				for (var g = 0; g < activeGroup.GlyphIDs.Count && g < 6; g++)
					GetSession().GameState.ActiveGlyphs[g] = activeGroup.GlyphIDs[g];
			}

			SendPacketToClient(talentData);
		}
	}

	[PacketHandler(Opcode.SMSG_ALL_ACHIEVEMENT_DATA)]
	private void HandleAllAchievementData(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var data = new AllAchievementData();
			var realmAddress = GetSession().RealmId.GetAddress();
			var playerGuid = GetSession().GameState.CurrentPlayerGuid;

			// 3.3.5a format: earned achievements (terminated by -1), then criteria progress (terminated by -1)
			// Earned achievements
			while (true)
			{
				var achievementId = packet.ReadUInt32();
				if (achievementId == 0xFFFFFFFF) // -1 terminator
					break;
				var datePackedTime = packet.ReadUInt32();
				var dateUnix = Time.GetUnixTimeFromPackedTime(datePackedTime);

				var earned = new EarnedAchievement
				{
					Id = achievementId,
					Date = dateUnix,
					Owner = playerGuid,
					VirtualRealmAddress = realmAddress,
					NativeRealmAddress = realmAddress
				};
				data.Earned.Add(earned);
			}

			// Criteria progress
			while (true)
			{
				var criteriaId = packet.ReadUInt32();
				if (criteriaId == 0xFFFFFFFF) // -1 terminator
					break;
				var counter = packet.ReadPackedGuid().Low; // counter packed as guid
				var playerGuid64 = packet.ReadPackedGuid();
				var timedFlag = packet.ReadUInt32();
				var datePackedTime = packet.ReadUInt32();
				var dateUnix = Time.GetUnixTimeFromPackedTime(datePackedTime);
				var timeFromStart = packet.ReadUInt32();
				var timeFromCreate = packet.ReadUInt32();

				var progress = new CriteriaProgressPkt
				{
					Id = criteriaId,
					Quantity = counter,
					Player = playerGuid,
					Flags = timedFlag,
					Date = dateUnix,
					TimeFromStart = timeFromStart,
					TimeFromCreate = timeFromCreate
				};
				data.Progress.Add(progress);
			}

			SendPacketToClient(data);
		}
	}

	[PacketHandler(Opcode.SMSG_CRITERIA_UPDATE)]
	private void HandleCriteriaUpdate(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var update = new CriteriaUpdatePkt
			{
				CriteriaID = packet.ReadUInt32(),
				Quantity = packet.ReadPackedGuid().Low // counter packed as guid
			};
			var playerGuid64 = packet.ReadPackedGuid();
			update.Flags = packet.ReadUInt32(); // timed flag
			var datePackedTime = packet.ReadUInt32();
			update.CurrentTime = Time.GetUnixTimeFromPackedTime(datePackedTime);
			update.ElapsedTime = packet.ReadUInt32();
			update.CreationTime = packet.ReadUInt32();
			update.PlayerGUID = GetSession().GameState.CurrentPlayerGuid ?? WowGuid128.Empty;
			SendPacketToClient(update);
		}
	}

	[PacketHandler(Opcode.SMSG_ACHIEVEMENT_EARNED)]
	private void HandleAchievementEarned(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var earned = new AchievementEarnedPkt();
			var playerGuid64 = packet.ReadPackedGuid();
			earned.AchievementID = packet.ReadUInt32();
			var datePackedTime = packet.ReadUInt32();
			earned.Time = Time.GetUnixTimeFromPackedTime(datePackedTime);
			packet.ReadUInt32(); // unknown/reserved (0)

			var realmAddress = GetSession().RealmId.GetAddress();
			earned.Sender = GetSession().GameState.CurrentPlayerGuid;
			earned.Earner = playerGuid64.To128(GetSession().GameState);
			earned.EarnerNativeRealm = realmAddress;
			earned.EarnerVirtualRealm = realmAddress;
			earned.Initial = false;
			SendPacketToClient(earned);
		}
	}

	[PacketHandler(Opcode.SMSG_LOAD_EQUIPMENT_SET)]
	private void HandleLoadEquipmentSet(WorldPacket packet)
	{
	}

	[PacketHandler(Opcode.SMSG_INSTANCE_DIFFICULTY)]
	private void HandleInstanceDifficulty(WorldPacket packet)
	{
	}

	[PacketHandler(Opcode.SMSG_LFG_PLAYER_INFO)]
	private void HandleLfgPlayerInfo(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			var info = new LfgPlayerInfoPkt();

			// 3.3.5a format: random dungeons, then locked dungeons
			// Random dungeons (available)
			var randomCount = packet.ReadUInt8();
			for (var i = 0; i < randomCount; i++)
			{
				var dungeon = new LfgPlayerDungeonInfo
				{
					Slot = packet.ReadUInt32() // dungeon entry (id + type)
				};
				var isDone = packet.ReadUInt8() != 0;
				dungeon.FirstReward = !isDone;
				dungeon.CompletionQuantity = isDone ? 1 : 0;
				dungeon.CompletionLimit = 1;

				var rewards = new LfgPlayerQuestReward
				{
					Items = new List<LfgPlayerQuestRewardItem>(),
					Currency = new List<LfgPlayerQuestRewardCurrency>(),
					BonusCurrency = new List<LfgPlayerQuestRewardCurrency>(),
					RewardMoney = (int)packet.ReadUInt32(),
					RewardXP = (int)packet.ReadUInt32()
				};
				packet.ReadUInt32(); // unknown
				packet.ReadUInt32(); // unknown
				var itemCount = packet.ReadUInt8();
				for (var j = 0; j < itemCount; j++)
				{
					var item = new LfgPlayerQuestRewardItem
					{
						ItemID = (int)packet.ReadUInt32()
					};
					packet.ReadUInt32(); // displayInfo
					item.Quantity = (int)packet.ReadUInt32();
					rewards.Items.Add(item);
				}
				dungeon.Rewards = rewards;
				info.Dungeons.Add(dungeon);
			}

			// Locked dungeons (blacklist)
			var blackList = new LfgBlackList
			{
				Slots = new List<LfgBlackListSlot>()
			};
			var lockCount = packet.ReadUInt32();
			for (uint i = 0; i < lockCount; i++)
			{
				var slot = new LfgBlackListSlot
				{
					Slot = packet.ReadUInt32(), // dungeon entry
					Reason = packet.ReadUInt32() // lock status
				};
				blackList.Slots.Add(slot);
			}
			info.BlackList = blackList;

			SendPacketToClient(info);
		}
	}

	[PacketHandler(Opcode.SMSG_LFG_JOIN_RESULT)]
	private void HandleLfgJoinResult(WorldPacket packet)
	{
		var result = new DfJoinResult
		{
			Ticket =
			{
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid,
				Id = 1,
				Type = RideType.Lfg,
				Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			},
			Result = (byte)packet.ReadUInt32(), // joinData.result
			ResultDetail = (byte)packet.ReadUInt32() // joinData.state
		};
		if (packet.CanRead())
		{
			var partySize = packet.ReadUInt8();
			for (var p = 0; p < partySize; p++)
			{
				var blackList = new DfJoinBlackList
				{
					PlayerGuid = packet.ReadGuid().To128(GetSession().GameState)
				};
				var dungeonCount = packet.ReadUInt32();
				for (uint d = 0; d < dungeonCount; d++)
				{
					var slot = new DfJoinBlackListSlot
					{
						Slot = packet.ReadUInt32(),
						Reason = packet.ReadUInt32()
					};
					blackList.Slots.Add(slot);
				}
				result.BlackList.Add(blackList);
			}
		}
		SendPacketToClient(result);
	}

	[PacketHandler(Opcode.SMSG_LFG_UPDATE_PLAYER)]
	private void HandleLfgUpdatePlayer(WorldPacket packet)
	{
		static bool HasRemaining(WorldPacket p, long bytes)
		{
			return p.GetData().Length - p.GetCurrentStream().Position >= bytes;
		}

		var status = new DfUpdateStatus
		{
			Ticket =
			{
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid,
				Id = 1,
				Type = RideType.Lfg,
				Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			}
		};
		if (!HasRemaining(packet, 1))
		{
			SendPacketToClient(status);
			return;
		}
		var updateType = packet.ReadUInt8();
		status.SubType = updateType;
		if (!HasRemaining(packet, 1))
		{
			SendPacketToClient(status);
			return;
		}
		var hasExtraInfo = packet.ReadUInt8() != 0;
		if (hasExtraInfo)
		{
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			status.Queued = packet.ReadUInt8() != 0;
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			packet.ReadUInt8(); // unk
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			packet.ReadUInt8(); // unk
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			var dungeonCount = packet.ReadUInt8();
			for (var i = 0; i < dungeonCount; i++)
			{
				if (!HasRemaining(packet, 4))
				{
					break;
				}
				status.Slots.Add(packet.ReadUInt32());
			}
			status.Joined = true;
			status.LfgJoined = true;
			status.NotifyUI = true;
			if (packet.CanRead())
			{
				packet.ReadCString(); // comment - not used in modern
			}
		}
		SendPacketToClient(status);
	}

	[PacketHandler(Opcode.SMSG_LFG_UPDATE_PARTY)]
	private void HandleLfgUpdateParty(WorldPacket packet)
	{
		static bool HasRemaining(WorldPacket p, long bytes)
		{
			return p.GetData().Length - p.GetCurrentStream().Position >= bytes;
		}

		var status = new DfUpdateStatus
		{
			Ticket =
			{
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid,
				Id = 1,
				Type = RideType.Lfg,
				Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			}
		};
		if (!HasRemaining(packet, 1))
		{
			SendPacketToClient(status);
			return;
		}
		status.SubType = packet.ReadUInt8();
		status.IsParty = true;
		if (!HasRemaining(packet, 1))
		{
			SendPacketToClient(status);
			return;
		}
		var hasExtraInfo = packet.ReadUInt8() != 0;
		if (hasExtraInfo)
		{
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			status.Queued = packet.ReadUInt8() != 0;
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			packet.ReadUInt8(); // unk
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			packet.ReadUInt8(); // unk
			if (!HasRemaining(packet, 1))
			{
				SendPacketToClient(status);
				return;
			}
			var dungeonCount = packet.ReadUInt8();
			for (var i = 0; i < dungeonCount; i++)
			{
				if (!HasRemaining(packet, 4))
				{
					break;
				}
				status.Slots.Add(packet.ReadUInt32());
			}
			status.Joined = true;
			status.LfgJoined = true;
			status.NotifyUI = true;
			if (packet.CanRead())
			{
				packet.ReadCString(); // comment - not used in modern
			}
		}
		SendPacketToClient(status);
	}

	[PacketHandler(Opcode.SMSG_CALENDAR_SEND_NUM_PENDING)]
	private void HandleCalendarSendNumPending(WorldPacket packet)
	{
		var pending = new CalendarSendNumPendingPkt
		{
			NumPending = packet.CanRead() ? packet.ReadUInt32() : 0u
		};
		SendPacketToClient(pending);
	}

	[PacketHandler(Opcode.SMSG_LFG_QUEUE_STATUS)]
	private void HandleLfgQueueStatus(WorldPacket packet)
	{
		static bool HasRemaining(WorldPacket p, long bytes)
		{
			return p.GetData().Length - p.GetCurrentStream().Position >= bytes;
		}

		var status = new DfQueueStatus
		{
			Ticket =
			{
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid,
				Id = 1,
				Type = RideType.Lfg,
				Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			}
		};
		if (!HasRemaining(packet, 27))
		{
			SendPacketToClient(status);
			return;
		}
		status.Slot = packet.ReadUInt32();
		status.AvgWaitTime = (uint)packet.ReadInt32();
		status.AvgWaitTimeMe = (uint)packet.ReadInt32();
		status.AvgWaitTimeByRole[0] = (uint)packet.ReadInt32(); // Tank
		status.AvgWaitTimeByRole[1] = (uint)packet.ReadInt32(); // Healer
		status.AvgWaitTimeByRole[2] = (uint)packet.ReadInt32(); // DPS
		status.LastNeeded[0] = packet.ReadUInt8(); // Tanks needed
		status.LastNeeded[1] = packet.ReadUInt8(); // Healers needed
		status.LastNeeded[2] = packet.ReadUInt8(); // DPS needed
		status.QueuedTime = packet.ReadUInt32();
		SendPacketToClient(status);
	}

	[PacketHandler(Opcode.SMSG_LFG_PROPOSAL_UPDATE)]
	private void HandleLfgProposalUpdate(WorldPacket packet)
	{
		static bool HasRemaining(WorldPacket p, long bytes)
		{
			return p.GetData().Length - p.GetCurrentStream().Position >= bytes;
		}

		var proposal = new DfProposalUpdate
		{
			Ticket =
			{
				RequesterGuid = GetSession().GameState.CurrentPlayerGuid,
				Id = 1,
				Type = RideType.Lfg,
				Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			}
		};
		if (!HasRemaining(packet, 15))
		{
			SendPacketToClient(proposal);
			return;
		}
		var dungeonEntry = packet.ReadUInt32();
		proposal.Slot = dungeonEntry;
		proposal.State = (sbyte)packet.ReadUInt8();
		proposal.ProposalID = packet.ReadUInt32();
		proposal.CompletedMask = packet.ReadUInt32();
		var silent = packet.ReadUInt8() != 0;
		proposal.ProposalSilent = silent;
		var playerCount = packet.ReadUInt8();
		for (var i = 0; i < playerCount; i++)
		{
			if (!HasRemaining(packet, 9))
			{
				break;
			}
			var player = new DfProposalPlayer
			{
				Roles = (byte)packet.ReadUInt32(),
				Me = packet.ReadUInt8() != 0
			};
			var inDungeon = packet.ReadUInt8() != 0;
			var sameGroup = packet.ReadUInt8() != 0;
			player.SameParty = sameGroup;
			player.MyParty = inDungeon;
			player.Responded = packet.ReadUInt8() != 0;
			player.Accepted = packet.ReadUInt8() != 0;
			proposal.Players.Add(player);
		}
		SendPacketToClient(proposal);
	}

	[PacketHandler(Opcode.SMSG_LEARNED_DANCE_MOVES)]
	private void HandleLearnedDanceMoves(WorldPacket packet)
	{
	}

	[PacketHandler(Opcode.SMSG_CACHE_VERSION)]
	private void HandleCacheVersion(WorldPacket packet)
	{
	}

	[PacketHandler(Opcode.MSG_MOVE_START_FORWARD)]
	[PacketHandler(Opcode.MSG_MOVE_START_BACKWARD)]
	[PacketHandler(Opcode.MSG_MOVE_STOP)]
	[PacketHandler(Opcode.MSG_MOVE_START_STRAFE_LEFT)]
	[PacketHandler(Opcode.MSG_MOVE_START_STRAFE_RIGHT)]
	[PacketHandler(Opcode.MSG_MOVE_STOP_STRAFE)]
	[PacketHandler(Opcode.MSG_MOVE_START_ASCEND)]
	[PacketHandler(Opcode.MSG_MOVE_START_DESCEND)]
	[PacketHandler(Opcode.MSG_MOVE_STOP_ASCEND)]
	[PacketHandler(Opcode.MSG_MOVE_JUMP)]
	[PacketHandler(Opcode.MSG_MOVE_START_TURN_LEFT)]
	[PacketHandler(Opcode.MSG_MOVE_START_TURN_RIGHT)]
	[PacketHandler(Opcode.MSG_MOVE_STOP_TURN)]
	[PacketHandler(Opcode.MSG_MOVE_START_PITCH_UP)]
	[PacketHandler(Opcode.MSG_MOVE_START_PITCH_DOWN)]
	[PacketHandler(Opcode.MSG_MOVE_STOP_PITCH)]
	[PacketHandler(Opcode.MSG_MOVE_SET_RUN_MODE)]
	[PacketHandler(Opcode.MSG_MOVE_SET_WALK_MODE)]
	[PacketHandler(Opcode.MSG_MOVE_TELEPORT)]
	[PacketHandler(Opcode.MSG_MOVE_SET_FACING)]
	[PacketHandler(Opcode.MSG_MOVE_SET_PITCH)]
	[PacketHandler(Opcode.MSG_MOVE_TOGGLE_COLLISION_CHEAT)]
	[PacketHandler(Opcode.MSG_MOVE_GRAVITY_CHNG)]
	[PacketHandler(Opcode.MSG_MOVE_ROOT)]
	[PacketHandler(Opcode.MSG_MOVE_UNROOT)]
	[PacketHandler(Opcode.MSG_MOVE_START_SWIM)]
	[PacketHandler(Opcode.MSG_MOVE_STOP_SWIM)]
	[PacketHandler(Opcode.MSG_MOVE_START_SWIM_CHEAT)]
	[PacketHandler(Opcode.MSG_MOVE_STOP_SWIM_CHEAT)]
	[PacketHandler(Opcode.MSG_MOVE_HEARTBEAT)]
	[PacketHandler(Opcode.MSG_MOVE_FALL_LAND)]
	[PacketHandler(Opcode.MSG_MOVE_UPDATE_CAN_FLY)]
	[PacketHandler(Opcode.MSG_MOVE_UPDATE_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY)]
	[PacketHandler(Opcode.MSG_MOVE_HOVER)]
	[PacketHandler(Opcode.MSG_MOVE_FEATHER_FALL)]
	[PacketHandler(Opcode.MSG_MOVE_WATER_WALK)]
	private void HandleMovementMessages(WorldPacket packet)
	{
		var moveUpdate = new MoveUpdate
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			MoveInfo = new MovementInfo()
		};
		moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, GetSession().GameState);
		moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
		moveUpdate.MoveInfo.ValidateMovementInfo();
		SendPacketToClient(moveUpdate);
	}

	[PacketHandler(Opcode.MSG_MOVE_KNOCK_BACK)]
	private void HandleMoveKnockBack(WorldPacket packet)
	{
		var knockback = new MoveUpdateKnockBack
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			MoveInfo = new MovementInfo()
		};
		knockback.MoveInfo.ReadMovementInfoLegacy(packet, GetSession().GameState);
		knockback.MoveInfo.Flags = (uint)((MovementFlagWotLK)knockback.MoveInfo.Flags).CastFlags<MovementFlagModern>();
		knockback.MoveInfo.JumpSinAngle = packet.ReadFloat();
		knockback.MoveInfo.JumpCosAngle = packet.ReadFloat();
		knockback.MoveInfo.JumpHorizontalSpeed = packet.ReadFloat();
		knockback.MoveInfo.JumpVerticalSpeed = packet.ReadFloat();
		knockback.MoveInfo.ValidateMovementInfo();
		SendPacketToClient(knockback);
	}

	[PacketHandler(Opcode.SMSG_MOVE_KNOCK_BACK)]
	private void HandleMoveForceKnockBack(WorldPacket packet)
	{
		var knockback = new MoveKnockBack
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			MoveCounter = packet.ReadUInt32(),
			Direction = packet.ReadVector2(),
			HorizontalSpeed = packet.ReadFloat(),
			VerticalSpeed = packet.ReadFloat()
		};
		SendPacketToClient(knockback);
	}

	[PacketHandler(Opcode.SMSG_CONTROL_UPDATE)]
	private void HandleControlUpdate(WorldPacket packet)
	{
		var control = new ControlUpdate
		{
			Guid = packet.ReadPackedGuid().To128(GetSession().GameState),
			HasControl = packet.ReadBool()
		};
		SendPacketToClient(control);
	}

	[PacketHandler(Opcode.MSG_MOVE_TELEPORT_ACK)]
	private void HandleMoveTeleportAck(WorldPacket packet)
	{
		var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
		if (GetSession().GameState.IsInTaxiFlight && GetSession().GameState.CurrentPlayerGuid == guid)
		{
			var control = new ControlUpdate
			{
				Guid = guid,
				HasControl = true
			};
			SendPacketToClient(control);
			GetSession().GameState.IsInTaxiFlight = false;
		}
		var teleport = new MoveTeleport
		{
			MoverGUID = guid,
			MoveCounter = packet.ReadUInt32()
		};
		var moveInfo = new MovementInfo();
		moveInfo.ReadMovementInfoLegacy(packet, GetSession().GameState);
		moveInfo.Flags = (uint)((MovementFlagWotLK)moveInfo.Flags).CastFlags<MovementFlagModern>();
		moveInfo.ValidateMovementInfo();
		teleport.Position = moveInfo.Position;
		teleport.Orientation = moveInfo.Orientation;
		teleport.TransportGUID = moveInfo.TransportGuid;
		if (moveInfo.TransportSeat > 0)
		{
			teleport.Vehicle = new VehicleTeleport
			{
				VehicleSeatIndex = moveInfo.TransportSeat
			};
		}
		SendPacketToClient(teleport);
	}

	[PacketHandler(Opcode.SMSG_TRANSFER_PENDING)]
	private void HandleTransferPending(WorldPacket packet)
	{
		if (GetSession().GameState.IsWaitingForWorldPortAck)
		{
			Log.Print(LogType.Error, "Skipping SMSG_TRANSFER_PENDING, client is already being teleported.", "MovementHandler.cs");
			return;
		}
		var transfer = new TransferPending
		{
			MapID = (GetSession().GameState.PendingTransferMapId = packet.ReadUInt32()),
			OldMapPosition = Vector3.Zero
		};
		SendPacketToClient(transfer);
		GetSession().GameState.IsFirstEnterWorld = false;
		GetSession().GameState.IsWaitingForNewWorld = true;
		var suspend = new SuspendToken
		{
			SequenceIndex = 3u,
			Reason = 1u
		};
		SendPacketToClient(suspend);
	}

	[PacketHandler(Opcode.SMSG_TRANSFER_ABORTED)]
	private void HandleTransferAborted(WorldPacket packet)
	{
		var transfer = new TransferAborted();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			transfer.MapID = packet.ReadUInt32();
		}
		else
		{
			transfer.MapID = GetSession().GameState.PendingTransferMapId;
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			transfer.Reason = (TransferAbortReasonModern)packet.ReadUInt8();
		}
		else
		{
			var legacyReason = (TransferAbortReasonLegacy)packet.ReadUInt8();
			transfer.Reason = (TransferAbortReasonModern)Enum.Parse(typeof(TransferAbortReasonModern), legacyReason.ToString());
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			transfer.Arg = packet.ReadUInt8();
		}
		SendPacketToClient(transfer);
		GetSession().GameState.IsWaitingForNewWorld = false;
	}

	[PacketHandler(Opcode.SMSG_NEW_WORLD)]
	private void HandleNewWorld(WorldPacket packet)
	{
		var teleport = new NewWorld();
		GetSession().GameState.CurrentMapId = (teleport.MapID = packet.ReadUInt32());
		teleport.Position = packet.ReadVector3();
		teleport.Orientation = packet.ReadFloat();
		teleport.Reason = 4u;
		GetSession().GameState.IsFirstEnterWorld = false;
		if (!GetSession().GameState.IsWaitingForNewWorld)
		{
			return;
		}
		GetSession().GameState.IsWaitingForNewWorld = false;
		GetSession().GameState.IsWaitingForWorldPortAck = true;
		SendPacketToClient(teleport);
		if (teleport.MapID > 1)
		{
			var instance = new UpdateLastInstance
			{
				MapID = teleport.MapID
			};
			SendPacketToClient(instance);
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				SendPacketToClient(new TimeSyncRequest());
			}
			var resume = new ResumeToken
			{
				SequenceIndex = 3u,
				Reason = 1u
			};
			SendPacketToClient(resume);
		}
		var info = new WorldServerInfo();
		if (teleport.MapID > 1)
		{
			info.DifficultyID = 1u;
			info.InstanceGroupSize = 5u;
		}
		SendPacketToClient(info);
	}

	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_WALK_BACK_SPEED)]
	private void HandleMoveSplineSetWalkBackSpeed(WorldPacket packet)
	{
		// Walk back speed does not exist in WotLK Classic 3.4.3 - silently drop
	}

		[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_FLIGHT_BACK_SPEED)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_FLIGHT_SPEED)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_PITCH_RATE)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_RUN_BACK_SPEED)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_RUN_SPEED)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_SWIM_BACK_SPEED)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_SWIM_SPEED)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_TURN_RATE)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_WALK_SPEED)]
	private void HandleMoveSplineSetSpeed(WorldPacket packet)
	{
		var speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false))
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			Speed = packet.ReadFloat()
		};
		SendPacketToClient(speed);
	}

	[PacketHandler(Opcode.SMSG_FORCE_WALK_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_RUN_BACK_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_SWIM_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_SWIM_BACK_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_TURN_RATE_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_FLIGHT_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_FLIGHT_BACK_SPEED_CHANGE)]
	[PacketHandler(Opcode.SMSG_FORCE_PITCH_RATE_CHANGE)]
	private void HandleMoveForceSpeedChange(WorldPacket packet)
	{
		var opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
			.Replace("_CHANGE", "");
		var universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
		var speed = new MoveSetSpeed(universalOpcode)
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			MoveCounter = packet.ReadUInt32()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
		{
			packet.ReadUInt8();
		}
		speed.Speed = packet.ReadFloat();
		SendPacketToClient(speed);
		var flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
		if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
			var flySpeed = new MoveSetSpeed(flyOpcode)
			{
				MoverGUID = speed.MoverGUID,
				MoveCounter = speed.MoveCounter,
				Speed = speed.Speed
			};
			SendPacketToClient(flySpeed);
		}
	}

	[PacketHandler(Opcode.MSG_MOVE_SET_FLIGHT_BACK_SPEED)]
	[PacketHandler(Opcode.MSG_MOVE_SET_FLIGHT_SPEED)]
	[PacketHandler(Opcode.MSG_MOVE_SET_PITCH_RATE)]
	[PacketHandler(Opcode.MSG_MOVE_SET_RUN_BACK_SPEED)]
	[PacketHandler(Opcode.MSG_MOVE_SET_RUN_SPEED)]
	[PacketHandler(Opcode.MSG_MOVE_SET_SWIM_BACK_SPEED)]
	[PacketHandler(Opcode.MSG_MOVE_SET_SWIM_SPEED)]
	[PacketHandler(Opcode.MSG_MOVE_SET_TURN_RATE)]
	[PacketHandler(Opcode.MSG_MOVE_SET_WALK_SPEED)]
	private void HandleMoveUpdateSpeed(WorldPacket packet)
	{
		var opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
		var universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
		var speed = new MoveUpdateSpeed(universalOpcode)
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			MoveInfo = new MovementInfo()
		};
		speed.MoveInfo.ReadMovementInfoLegacy(packet, GetSession().GameState);
		var newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
		speed.MoveInfo.Flags = (uint)newFlags;
		speed.MoveInfo.ValidateMovementInfo();
		speed.Speed = packet.ReadFloat();
		SendPacketToClient(speed);
		var flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
		if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
			var flySpeed = new MoveUpdateSpeed(flyOpcode)
			{
				MoverGUID = speed.MoverGUID,
				MoveInfo = speed.MoveInfo,
				Speed = speed.Speed
			};
			SendPacketToClient(flySpeed);
		}
	}

	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_ROOT)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_UNROOT)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_ENABLE_GRAVITY)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_DISABLE_GRAVITY)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_FEATHER_FALL)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_NORMAL_FALL)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_HOVER)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_UNSET_HOVER)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_WATER_WALK)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_LAND_WALK)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_START_SWIM)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_STOP_SWIM)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_RUN_MODE)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_WALK_MODE)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_SET_FLYING)]
	[PacketHandler(Opcode.SMSG_MOVE_SPLINE_UNSET_FLYING)]
	private void HandleSplineMovementMessages(WorldPacket packet)
	{
		var spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false))
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(spline);
	}

	[PacketHandler(Opcode.SMSG_MOVE_ROOT)]
	[PacketHandler(Opcode.SMSG_MOVE_UNROOT)]
	[PacketHandler(Opcode.SMSG_MOVE_SET_WATER_WALK)]
	[PacketHandler(Opcode.SMSG_MOVE_SET_LAND_WALK)]
	[PacketHandler(Opcode.SMSG_MOVE_SET_HOVERING)]
	[PacketHandler(Opcode.SMSG_MOVE_UNSET_HOVERING)]
	[PacketHandler(Opcode.SMSG_MOVE_SET_CAN_FLY)]
	[PacketHandler(Opcode.SMSG_MOVE_UNSET_CAN_FLY)]
	[PacketHandler(Opcode.SMSG_MOVE_ENABLE_TRANSITION_BETWEEN_SWIM_AND_FLY)]
	[PacketHandler(Opcode.SMSG_MOVE_DISABLE_TRANSITION_BETWEEN_SWIM_AND_FLY)]
	[PacketHandler(Opcode.SMSG_MOVE_DISABLE_GRAVITY)]
	[PacketHandler(Opcode.SMSG_MOVE_ENABLE_GRAVITY)]
	[PacketHandler(Opcode.SMSG_MOVE_SET_FEATHER_FALL)]
	[PacketHandler(Opcode.SMSG_MOVE_SET_NORMAL_FALL)]
	private void HandleMoveForceFlagChange(WorldPacket packet)
	{
		var flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false))
		{
			MoverGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			MoveCounter = packet.ReadUInt32()
		};
		SendPacketToClient(flag);
	}

	[PacketHandler(Opcode.SMSG_COMPRESSED_MOVES)]
	private void HandleCompressedMoves(WorldPacket packet)
	{
		var uncompressedSize = packet.ReadInt32();
		var pkt = packet.Inflate(uncompressedSize);
		while (pkt.CanRead())
		{
			var size = pkt.ReadUInt8();
			var opc = pkt.ReadUInt16();
			var data = pkt.ReadBytes((uint)(size - 2));
			var pkt2 = new WorldPacket(opc, data);
			pkt2.SetReceiveTime(pkt.GetReceivedTime());
			HandlePacket(pkt2);
		}
	}

	[PacketHandler(Opcode.SMSG_ON_MONSTER_MOVE)]
	[PacketHandler(Opcode.SMSG_MONSTER_MOVE_TRANSPORT)]
	private void HandleMonsterMove(WorldPacket packet)
	{
		var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
		var moveSpline = new ServerSideMovement();
		if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_MONSTER_MOVE_TRANSPORT)
		{
			moveSpline.TransportGuid = packet.ReadPackedGuid().To128(GetSession().GameState);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
			{
				moveSpline.TransportSeat = packet.ReadInt8();
			}
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			packet.ReadBool();
		}
		moveSpline.StartPosition = packet.ReadVector3();
		moveSpline.SplineId = packet.ReadUInt32();
		var type = (SplineTypeLegacy)packet.ReadUInt8();
		switch (type)
		{
		case SplineTypeLegacy.FacingSpot:
			moveSpline.SplineType = SplineTypeModern.FacingSpot;
			moveSpline.FinalFacingSpot = packet.ReadVector3();
			break;
		case SplineTypeLegacy.FacingTarget:
			moveSpline.SplineType = SplineTypeModern.FacingTarget;
			moveSpline.FinalFacingGuid = packet.ReadGuid().To128(GetSession().GameState);
			break;
		case SplineTypeLegacy.FacingAngle:
			moveSpline.SplineType = SplineTypeModern.FacingAngle;
			moveSpline.FinalOrientation = packet.ReadFloat();
			MovementInfo.ClampOrientation(ref moveSpline.FinalOrientation);
			break;
		case SplineTypeLegacy.Stop:
		{
			moveSpline.SplineType = SplineTypeModern.None;
			var moveStop = new MonsterMove(guid, moveSpline);
			SendPacketToClient(moveStop);
			return;
		}
		}
		bool hasAnimTier;
		bool hasTrajectory;
		bool hasCatmullRom;
		bool hasTaxiFlightFlags;
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var splineFlags = (SplineFlagVanilla)packet.ReadUInt32();
			hasAnimTier = false;
			hasTrajectory = false;
			hasCatmullRom = splineFlags.HasAnyFlag(SplineFlagVanilla.Flying);
			hasTaxiFlightFlags = splineFlags == (SplineFlagVanilla.Runmode | SplineFlagVanilla.Flying);
			if (splineFlags == SplineFlagVanilla.Runmode)
			{
				moveSpline.SplineFlags = SplineFlagModern.Unknown5;
				var unitFlags = (UnitFlagsVanilla)GetSession().GameState.GetLegacyFieldValueUInt32(guid, UnitField.UNIT_FIELD_FLAGS);
				if (unitFlags.HasFlag(UnitFlagsVanilla.CanSwim))
				{
					moveSpline.SplineFlags |= SplineFlagModern.CanSwim;
				}
				if (type == SplineTypeLegacy.Normal && !unitFlags.HasFlag(UnitFlagsVanilla.InCombat))
				{
					moveSpline.SplineFlags |= SplineFlagModern.Steering | SplineFlagModern.Unknown10;
				}
			}
			else
			{
				moveSpline.SplineFlags = splineFlags.CastFlags<SplineFlagModern>();
			}
		}
		else if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var splineFlags2 = (SplineFlagTBC)packet.ReadUInt32();
			hasAnimTier = false;
			hasTrajectory = false;
			hasCatmullRom = splineFlags2.HasAnyFlag(SplineFlagTBC.Flying);
			hasTaxiFlightFlags = splineFlags2 == (SplineFlagTBC.Runmode | SplineFlagTBC.Flying);
			if (splineFlags2 == SplineFlagTBC.Runmode)
			{
				moveSpline.SplineFlags = SplineFlagModern.Unknown5;
				var unitFlags2 = (UnitFlags)GetSession().GameState.GetLegacyFieldValueUInt32(guid, UnitField.UNIT_FIELD_FLAGS);
				if (unitFlags2.HasFlag(UnitFlags.CanSwim))
				{
					moveSpline.SplineFlags |= SplineFlagModern.CanSwim;
				}
				if (type == SplineTypeLegacy.Normal && !unitFlags2.HasFlag(UnitFlags.InCombat))
				{
					moveSpline.SplineFlags |= SplineFlagModern.Steering | SplineFlagModern.Unknown10;
				}
			}
			else
			{
				moveSpline.SplineFlags = splineFlags2.CastFlags<SplineFlagModern>();
			}
		}
		else
		{
			var splineFlags3 = (SplineFlagWotLK)packet.ReadUInt32();
			hasAnimTier = splineFlags3.HasAnyFlag(SplineFlagWotLK.AnimationTier);
			hasTrajectory = splineFlags3.HasAnyFlag(SplineFlagWotLK.Trajectory);
			hasCatmullRom = splineFlags3.HasAnyFlag(SplineFlagWotLK.Flying | SplineFlagWotLK.CatmullRom);
			hasTaxiFlightFlags = splineFlags3 == (SplineFlagWotLK.WalkMode | SplineFlagWotLK.Flying);
			moveSpline.SplineFlags = splineFlags3.CastFlags<SplineFlagModern>();
		}
		if (hasAnimTier)
		{
			packet.ReadUInt8();
			packet.ReadInt32();
		}
		moveSpline.SplineTimeFull = packet.ReadUInt32();
		if (hasTrajectory)
		{
			packet.ReadFloat();
			packet.ReadInt32();
		}
		moveSpline.SplineCount = packet.ReadUInt32();
		if (hasCatmullRom)
		{
			for (var i = 0; i < moveSpline.SplineCount; i++)
			{
				var vec = packet.ReadVector3();
				moveSpline?.SplinePoints.Add(vec);
			}
			moveSpline.SplineFlags |= SplineFlagModern.UncompressedPath;
		}
		else
		{
			moveSpline.EndPosition = packet.ReadVector3();
			var mid = (moveSpline.StartPosition + moveSpline.EndPosition) * 0.5f;
			for (var j = 1; j < moveSpline.SplineCount; j++)
			{
				var vec2 = packet.ReadPackedVector3();
				vec2 = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? (moveSpline.EndPosition - vec2) : (mid - vec2));
				moveSpline.SplinePoints.Add(vec2);
			}
		}
		var isTaxiFlight = hasTaxiFlightFlags && (GetSession().GameState.IsWaitingForTaxiStart || Math.Abs(packet.GetReceivedTime() - GetSession().GameState.CurrentPlayerCreateTime) <= 1000) && GetSession().GameState.CurrentPlayerGuid == guid;
		if (isTaxiFlight)
		{
			var stopSpline = new ServerSideMovement
			{
				StartPosition = moveSpline.StartPosition,
				SplineId = moveSpline.SplineId - 2
			};
			var moveStop2 = new MonsterMove(guid, stopSpline);
			SendPacketToClient(moveStop2);
			var update = new ControlUpdate
			{
				Guid = guid,
				HasControl = false
			};
			SendPacketToClient(update);
			stopSpline.SplineId = moveSpline.SplineId - 1;
			moveStop2 = new MonsterMove(guid, stopSpline);
			SendPacketToClient(moveStop2);
			update = new ControlUpdate
			{
				Guid = guid,
				HasControl = false
			};
			SendPacketToClient(update);
			moveSpline.SplineFlags = SplineFlagModern.Flying | SplineFlagModern.CatmullRom | SplineFlagModern.CanSwim | SplineFlagModern.UncompressedPath | SplineFlagModern.Unknown5 | SplineFlagModern.Steering | SplineFlagModern.Unknown10;
			if (!hasCatmullRom && moveSpline.EndPosition != Vector3.Zero)
			{
				moveSpline.SplinePoints.Add(moveSpline.EndPosition);
			}
		}
		var monsterMove = new MonsterMove(guid, moveSpline);
		SendPacketToClient(monsterMove);
		if (isTaxiFlight)
		{
			if (GetSession().GameState.IsWaitingForTaxiStart)
			{
				var taxi = new ActivateTaxiReplyPkt
				{
					Reply = ActivateTaxiReply.Ok
				};
				SendPacketToClient(taxi);
				GetSession().GameState.IsWaitingForTaxiStart = false;
			}
			GetSession().GameState.IsInTaxiFlight = true;
		}
	}

	[PacketHandler(Opcode.SMSG_GOSSIP_MESSAGE)]
	private void HandleGossipmessage(WorldPacket packet)
	{
		var gossip = new GossipMessagePkt
		{
			GossipGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = gossip.GossipGUID;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
		{
			gossip.GossipID = packet.ReadInt32();
		}
		else
		{
			gossip.GossipID = (int)gossip.GossipGUID.GetEntry();
		}
		gossip.TextID = packet.ReadInt32();
		var optionsCount = packet.ReadUInt32();
		for (var i = 0u; i < optionsCount; i++)
		{
			var option = new ClientGossipOption
			{
				OptionIndex = packet.ReadInt32(),
				OptionIcon = packet.ReadUInt8(),
				OptionFlags = (byte)(packet.ReadBool() ? 1u : 0u)
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				option.OptionCost = packet.ReadInt32();
			}
			option.Text = packet.ReadCString();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				option.Confirm = packet.ReadCString();
			}
			gossip.GossipOptions.Add(option);
		}
		var questsCount = packet.ReadUInt32();
		for (var i2 = 0u; i2 < questsCount; i2++)
		{
			var quest = ReadGossipQuestOption(packet);
			gossip.GossipQuests.Add(quest);
		}
		SendPacketToClient(gossip);
	}

	[PacketHandler(Opcode.SMSG_GOSSIP_COMPLETE)]
	private void HandleGossipComplete(WorldPacket packet)
	{
		var gossip = new GossipComplete();
		SendPacketToClient(gossip);
	}

	[PacketHandler(Opcode.SMSG_GOSSIP_POI)]
	private void HandleGossipPoi(WorldPacket packet)
	{
		var poi = new GossipPOI
		{
			Flags = packet.ReadUInt32(),
			Pos = new Vector3(packet.ReadVector2()),
			Icon = packet.ReadUInt32(),
			Importance = packet.ReadUInt32(),
			Name = packet.ReadCString()
		};
		SendPacketToClient(poi);
	}

	[PacketHandler(Opcode.SMSG_BINDER_CONFIRM)]
	private void HandleBinderConfirm(WorldPacket packet)
	{
		var confirm = new BinderConfirm
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = confirm.Guid;
		SendPacketToClient(confirm);
	}

	[PacketHandler(Opcode.SMSG_VENDOR_INVENTORY)]
	private void HandleVendorInventory(WorldPacket packet)
	{
		var vendor = new VendorInventory
		{
			VendorGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = vendor.VendorGUID;
		var itemsCount = packet.ReadUInt8();
		if (itemsCount == 0)
		{
			vendor.Reason = packet.ReadUInt8();
			SendPacketToClient(vendor);
			return;
		}
		for (byte i = 0; i < itemsCount; i++)
		{
			var vendorItem = new VendorItem
			{
				Slot = packet.ReadInt32(),
				MuID = (uint)(i + 1),
				Item =
				{
					ItemID = packet.ReadUInt32()
				}
			};
			packet.ReadUInt32();
			vendorItem.Quantity = packet.ReadInt32();
			vendorItem.Price = packet.ReadUInt32();
			vendorItem.Durability = packet.ReadInt32();
			vendorItem.StackCount = packet.ReadUInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				vendorItem.ExtendedCostID = packet.ReadInt32();
			}
			GetSession().GameState.SetItemBuyCount(vendorItem.Item.ItemID, vendorItem.StackCount);
			vendor.Items.Add(vendorItem);
		}
		SendPacketToClient(vendor);
	}

	[PacketHandler(Opcode.SMSG_SHOW_BANK)]
	private void HandleShowBank(WorldPacket packet)
	{
		var bank = new ShowBank
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = bank.Guid;
		SendPacketToClient(bank);
	}

	[PacketHandler(Opcode.SMSG_TRAINER_LIST)]
	private void HandleTrainerList(WorldPacket packet)
	{
		var trainer = new TrainerList
		{
			TrainerGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = trainer.TrainerGUID;
		trainer.TrainerID = trainer.TrainerGUID.GetEntry();
		trainer.TrainerType = packet.ReadInt32();
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var spell = new TrainerListSpell();
			var spellId = packet.ReadUInt32();
			if (ModernVersion.ExpansionVersion > 1 && LegacyVersion.ExpansionVersion <= 1)
			{
				var realSpellId = GameData.GetRealSpell(spellId);
				if (realSpellId != spellId)
				{
					GetSession().GameState.StoreRealSpell(realSpellId, spellId);
					spellId = realSpellId;
				}
			}
			spell.SpellID = spellId;
			var stateOld = (TrainerSpellStateLegacy)packet.ReadUInt8();
			var stateNew = (TrainerSpellStateModern)Enum.Parse(typeof(TrainerSpellStateModern), stateOld.ToString());
			spell.Usable = stateNew;
			spell.MoneyCost = packet.ReadUInt32();
			packet.ReadInt32();
			packet.ReadInt32();
			spell.ReqLevel = packet.ReadUInt8();
			spell.ReqSkillLine = packet.ReadUInt32();
			spell.ReqSkillRank = packet.ReadUInt32();
			spell.ReqAbility[0] = packet.ReadUInt32();
			spell.ReqAbility[1] = packet.ReadUInt32();
			spell.ReqAbility[2] = packet.ReadUInt32();
			trainer.Spells.Add(spell);
		}
		trainer.Greeting = packet.ReadCString();
		SendPacketToClient(trainer);
	}

	[PacketHandler(Opcode.SMSG_TRAINER_BUY_FAILED)]
	private void HandleTrainerBuyFailed(WorldPacket packet)
	{
		var buy = new TrainerBuyFailed
		{
			TrainerGUID = packet.ReadGuid().To128(GetSession().GameState),
			SpellID = packet.ReadUInt32(),
			TrainerFailedReason = packet.ReadUInt32()
		};
		SendPacketToClient(buy);
		var chat = new ChatPkt(GetSession(), ChatMessageTypeModern.System, $"Failed to learn Spell {buy.SpellID} (Reason {buy.TrainerFailedReason}).");
		SendPacketToClient(chat);
	}

	[PacketHandler(Opcode.MSG_TALENT_WIPE_CONFIRM)]
	private void HandleTalentWipeConfirm(WorldPacket packet)
	{
		var respec = new RespecWipeConfirm
		{
			TrainerGUID = packet.ReadGuid().To128(GetSession().GameState),
			Cost = packet.ReadUInt32()
		};
		SendPacketToClient(respec);
	}

	[PacketHandler(Opcode.SMSG_SPIRIT_HEALER_CONFIRM)]
	private void HandleSpiritHealerConfirm(WorldPacket packet)
	{
		// 3.4.3 client has no SMSG_SPIRIT_HEALER_CONFIRM opcode — spirit healer works directly.
		// Auto-accept by sending CMSG_SPIRIT_HEALER_ACTIVATE back to the legacy server.
		var guid = packet.ReadGuid();
		var activate = new WorldPacket(Opcode.CMSG_SPIRIT_HEALER_ACTIVATE);
		activate.WriteGuid(guid);
		SendPacket(activate);
	}

	[PacketHandler(Opcode.SMSG_PET_SPELLS_MESSAGE)]
	private void HandlePetSpellsMessage(WorldPacket packet)
	{
		WowGuid guid = packet.ReadGuid();
		GetSession().GameState.CurrentPetGuid = guid.To128(GetSession().GameState);
		GetSession().GameState.CurrentClientPetCast = null;
		if (guid.IsEmpty())
		{
			var clear = new PetClearSpells();
			SendPacketToClient(clear);
			return;
		}
		var spells = new PetSpells
		{
			PetGUID = guid.To128(GetSession().GameState)
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			spells.CreatureFamily = packet.ReadUInt16();
		}
		spells.TimeLimit = packet.ReadUInt32();
		spells.ReactState = (ReactStates)packet.ReadUInt8();
		spells.CommandState = (CommandStates)packet.ReadUInt8();
		packet.ReadUInt8();
		spells.Flag = packet.ReadUInt8();
		for (var i = 0; i < 10; i++)
		{
			spells.ActionButtons[i] = packet.ReadUInt32();
		}
		var spellCount = packet.ReadUInt8();
		for (var j = 0; j < spellCount; j++)
		{
			spells.Actions.Add(packet.ReadUInt32());
		}
		var cdCount = packet.ReadUInt8();
		for (var k = 0; k < cdCount; k++)
		{
			var cooldown = new PetSpellCooldown();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
			{
				cooldown.SpellID = packet.ReadUInt32();
			}
			else
			{
				cooldown.SpellID = packet.ReadUInt16();
			}
			cooldown.Category = packet.ReadUInt16();
			cooldown.Duration = packet.ReadUInt32();
			cooldown.CategoryDuration = packet.ReadUInt32();
			spells.Cooldowns.Add(cooldown);
		}
		SendPacketToClient(spells);
	}

	[PacketHandler(Opcode.SMSG_PET_ACTION_SOUND)]
	private void HandlePetActionSound(WorldPacket packet)
	{
		var sound = new PetActionSound
		{
			UnitGUID = packet.ReadGuid().To128(GetSession().GameState),
			Action = packet.ReadUInt32()
		};
		SendPacketToClient(sound);
	}

	[PacketHandler(Opcode.SMSG_PET_BROKEN)]
	private void HandlePetBroken(WorldPacket packet)
	{
		var notify = new PrintNotification
		{
			NotifyText = "Your pet has run away"
		};
		SendPacketToClient(notify);
	}

	[PacketHandler(Opcode.SMSG_PET_UNLEARN_CONFIRM)]
	private void HandlePetUnlearnConfirm(WorldPacket packet)
	{
		var respec = new RespecWipeConfirm
		{
			TrainerGUID = packet.ReadGuid().To128(GetSession().GameState),
			Cost = packet.ReadUInt32(),
			RespecType = SpecResetType.PetTalents
		};
		SendPacketToClient(respec);
	}

	[PacketHandler(Opcode.MSG_LIST_STABLED_PETS)]
	private void HandleListStabledPets(WorldPacket packet)
	{
		var pets = new PetGuids();
		var updateFields = GetSession().GameState.GetCachedObjectFieldsLegacy(GetSession().GameState.CurrentPlayerGuid);
		var UNIT_FIELD_SUMMON = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_SUMMON);
		if (UNIT_FIELD_SUMMON >= 0 && updateFields.ContainsKey(UNIT_FIELD_SUMMON))
		{
			var guid = GetGuidValue(updateFields, UnitField.UNIT_FIELD_SUMMON).To128(GetSession().GameState);
			if (!guid.IsEmpty())
			{
				pets.Guids.Add(guid);
			}
		}
		SendPacketToClient(pets);
		var stable = new PetStableList
		{
			StableMaster = packet.ReadGuid().To128(GetSession().GameState)
		};
		var count = packet.ReadUInt8();
		stable.NumStableSlots = packet.ReadUInt8();
		for (byte i = 0; i < count; i++)
		{
			var pet = new PetStableInfo
			{
				PetNumber = packet.ReadUInt32(),
				CreatureID = packet.ReadUInt32(),
				ExperienceLevel = packet.ReadUInt32(),
				PetName = packet.ReadCString()
			};
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				pet.LoyaltyLevel = (byte)packet.ReadUInt32();
			}
			pet.PetFlags = packet.ReadUInt8();
			if (pet.PetFlags != 1)
			{
				pet.PetFlags = 3;
			}
			var template = GameData.GetCreatureTemplate(pet.CreatureID);
			if (template != null)
			{
				pet.DisplayID = template.Display.CreatureDisplay[0].CreatureDisplayID;
			}
			else
			{
				var query = new WorldPacket(Opcode.CMSG_QUERY_CREATURE);
				query.WriteUInt32(pet.CreatureID);
				query.WriteGuid(WowGuid64.Empty);
				SendPacket(query);
			}
			stable.Pets.Add(pet);
		}
		SendPacketToClient(stable);
	}

	[PacketHandler(Opcode.SMSG_PET_STABLE_RESULT)]
	private void HandlePetStableResult(WorldPacket packet)
	{
		var stable = new PetStableResult
		{
			Result = packet.ReadUInt8()
		};
		SendPacketToClient(stable);
	}

	[PacketHandler(Opcode.SMSG_PETITION_SHOW_LIST)]
	private void HandlePetitionShowList(WorldPacket packet)
	{
		var petitions = new ServerPetitionShowList
		{
			Unit = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = petitions.Unit;
		var count = packet.ReadUInt8();
		for (var i = 0; i < count; i++)
		{
			var petition = default(PetitionEntry);
			petition.Index = packet.ReadUInt32();
			petition.CharterEntry = packet.ReadUInt32();
			petition.IsArena = ((petition.CharterEntry != 5863) ? 1u : 0u);
			packet.ReadUInt32();
			petition.CharterCost = packet.ReadUInt32();
			packet.ReadUInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				petition.RequiredSignatures = packet.ReadUInt32();
			}
			else
			{
				petition.RequiredSignatures = 9u;
			}
			petitions.Petitions.Add(petition);
		}
		SendPacketToClient(petitions);
	}

	[PacketHandler(Opcode.SMSG_PETITION_SHOW_SIGNATURES)]
	private void HandlePetitionShowSignatures(WorldPacket packet)
	{
		var petition = new ServerPetitionShowSignatures
		{
			Item = packet.ReadGuid().To128(GetSession().GameState),
			Owner = packet.ReadGuid().To128(GetSession().GameState)
		};
		petition.OwnerAccountID = GetSession().GetGameAccountGuidForPlayer(petition.Owner);
		petition.PetitionID = packet.ReadInt32();
		var counter = packet.ReadUInt8();
		for (var i = 0; i < counter; i++)
		{
			var signature = new ServerPetitionShowSignatures.PetitionSignature
			{
				Signer = packet.ReadGuid().To128(GetSession().GameState),
				Choice = packet.ReadInt32()
			};
			petition.Signatures.Add(signature);
		}
		SendPacketToClient(petition);
	}

	[PacketHandler(Opcode.SMSG_QUERY_PETITION_RESPONSE)]
	private void HandlePetitionQueryResponse(WorldPacket packet)
	{
		QueryPetitionResponse petition = new QueryPetitionResponse();
		petition.PetitionID = packet.ReadUInt32();
		petition.Allow = true;
		petition.Info = new PetitionInfo();
		petition.Info.PetitionID = packet.ReadUInt32();
		petition.Info.Petitioner = packet.ReadGuid().To128(GetSession().GameState);
		petition.Info.Title = packet.ReadCString();
		petition.Info.BodyText = packet.ReadCString();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.ReadUInt32();
		}
		petition.Info.MinSignatures = packet.ReadUInt32();
		petition.Info.MaxSignatures = packet.ReadUInt32();
		petition.Info.DeadLine = packet.ReadInt32();
		petition.Info.IssueDate = packet.ReadInt32();
		petition.Info.AllowedGuildID = packet.ReadInt32();
		petition.Info.AllowedClasses = packet.ReadInt32();
		petition.Info.AllowedRaces = packet.ReadInt32();
		petition.Info.AllowedGender = packet.ReadInt16();
		petition.Info.AllowedMinLevel = packet.ReadInt32();
		petition.Info.AllowedMaxLevel = packet.ReadInt32();
		petition.Info.NumChoices = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			for (var i = 0; i < 10; i++)
			{
				petition.Info.Choicetext[i] = packet.ReadCString();
			}
		}
		petition.Info.Muid = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			petition.Info.StaticType = packet.ReadInt32();
		}
		SendPacketToClient(petition);
	}

	[PacketHandler(Opcode.MSG_PETITION_RENAME)]
	private void HandlePetitionRename(WorldPacket packet)
	{
		var petition = new PetitionRenameGuildResponse
		{
			PetitionGuid = packet.ReadGuid().To128(GetSession().GameState),
			NewGuildName = packet.ReadCString()
		};
		SendPacketToClient(petition);
	}

	[PacketHandler(Opcode.MSG_PETITION_DECLINE)]
	private void HandlePetitionDecline(WorldPacket packet)
	{
		var guid = packet.ReadGuid().To128(GetSession().GameState);
		var name = GetSession().GameState.GetPlayerName(guid);
		if (!string.IsNullOrEmpty(name))
		{
			var chat = new ChatPkt(GetSession(), ChatMessageTypeModern.System, name + " has declined your guild invitation.");
			SendPacketToClient(chat);
		}
	}

	[PacketHandler(Opcode.SMSG_PETITION_SIGN_RESULTS)]
	private void HandlePetitionSignResults(WorldPacket packet)
	{
		var petition = new PetitionSignResults
		{
			Item = packet.ReadGuid().To128(GetSession().GameState),
			Player = packet.ReadGuid().To128(GetSession().GameState),
			Error = (PetitionSignResult)packet.ReadUInt32()
		};
		SendPacketToClient(petition);
	}

	[PacketHandler(Opcode.SMSG_TURN_IN_PETITION_RESULT)]
	private void HandleTurnInPetitionResult(WorldPacket packet)
	{
		var petition = new TurnInPetitionResult
		{
			Result = (PetitionTurnResult)packet.ReadUInt32()
		};
		SendPacketToClient(petition);
	}

	[PacketHandler(Opcode.SMSG_QUERY_TIME_RESPONSE)]
	private void HandleQueryTimeResponse(WorldPacket packet)
	{
		var response = new QueryTimeResponse
		{
			CurrentTime = packet.ReadInt32()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.CanRead())
		{
			packet.ReadInt32();
		}
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUERY_QUEST_INFO_RESPONSE)]
	private void HandleQueryQuestInfoResponse(WorldPacket packet)
	{
		var response = new QueryQuestInfoResponse();
		var id = packet.ReadEntry();
		response.QuestID = (uint)id.Key;
		if (id.Value)
		{
			response.Allow = false;
			SendPacketToClient(response);
			return;
		}
		response.Allow = true;
		response.Info = new QuestTemplate();
		var quest = response.Info;
		quest.QuestID = response.QuestID;
		quest.QuestType = packet.ReadInt32();
		quest.QuestLevel = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.MinLevel = packet.ReadInt32();
		}
		else
		{
			quest.MinLevel = 1;
		}
		quest.QuestSortID = packet.ReadInt32();
		quest.QuestInfoID = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			quest.SuggestedGroupNum = packet.ReadUInt32();
		}
		sbyte objectiveCounter = 0;
		for (var i = 0; i < 2; i++)
		{
			var factionId = packet.ReadInt32();
			var factionValue = packet.ReadInt32();
			if (factionId != 0 && factionValue != 0)
			{
				var objective = new QuestObjective
				{
					QuestID = response.QuestID,
					Id = QuestObjective.QuestObjectiveCounter++,
					StorageIndex = objectiveCounter++,
					Type = QuestObjectiveType.MinReputation,
					ObjectID = factionId,
					Amount = factionValue
				};
				quest.Objectives.Add(objective);
			}
		}
		quest.RewardNextQuest = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.RewardXPDifficulty = packet.ReadUInt32();
		}
		var rewOrReqMoney = packet.ReadInt32();
		if (rewOrReqMoney >= 0)
		{
			quest.RewardMoney = rewOrReqMoney;
		}
		else
		{
			var objective2 = new QuestObjective
			{
				QuestID = response.QuestID,
				Id = QuestObjective.QuestObjectiveCounter++,
				StorageIndex = objectiveCounter++,
				Type = QuestObjectiveType.Money,
				ObjectID = 0,
				Amount = -rewOrReqMoney
			};
			quest.Objectives.Add(objective2);
		}
		quest.RewardBonusMoney = packet.ReadUInt32();
		quest.RewardDisplaySpell[0] = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			quest.RewardSpell = packet.ReadUInt32();
			quest.RewardHonor = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.RewardKillHonor = packet.ReadFloat();
		}
		quest.StartItem = packet.ReadUInt32();
		quest.Flags = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
		{
			quest.RewardTitle = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var requiredPlayerKills = packet.ReadInt32();
			if (requiredPlayerKills != 0)
			{
				var objective3 = new QuestObjective
				{
					QuestID = response.QuestID,
					Id = QuestObjective.QuestObjectiveCounter++,
					StorageIndex = objectiveCounter++,
					Type = QuestObjectiveType.PlayerKills,
					ObjectID = 0,
					Amount = requiredPlayerKills
				};
				quest.Objectives.Add(objective3);
			}
			packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.RewardArenaPoints = packet.ReadInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			packet.ReadInt32();
		}
		for (var j = 0; j < 4; j++)
		{
			quest.RewardItems[j] = packet.ReadUInt32();
			quest.RewardAmount[j] = packet.ReadUInt32();
		}
		for (var k = 0; k < 6; k++)
		{
			var choiceItem = new QuestInfoChoiceItem
			{
				ItemID = packet.ReadUInt32(),
				Quantity = packet.ReadUInt32()
			};
			var displayId = GameData.GetItemDisplayId(choiceItem.ItemID);
			if (displayId != 0)
			{
				choiceItem.DisplayID = displayId;
			}
			quest.UnfilteredChoiceItems[k] = choiceItem;
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			for (var l = 0; l < 5; l++)
			{
				quest.RewardFactionID[l] = packet.ReadUInt32();
			}
			for (var m = 0; m < 5; m++)
			{
				quest.RewardFactionValue[m] = packet.ReadInt32();
			}
			for (var n = 0; n < 5; n++)
			{
				quest.RewardFactionOverride[n] = (int)packet.ReadUInt32();
			}
		}
		quest.POIContinent = packet.ReadUInt32();
		quest.POIx = packet.ReadFloat();
		quest.POIy = packet.ReadFloat();
		quest.POIPriority = packet.ReadUInt32();
		quest.LogTitle = packet.ReadCString();
		quest.LogDescription = packet.ReadCString();
		quest.QuestDescription = packet.ReadCString();
		quest.AreaDescription = packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.QuestCompletionLog = packet.ReadCString();
		}
		var reqId = new KeyValuePair<int, bool>[4];
		var reqItemFieldCount = 4;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
		{
			reqItemFieldCount = 5;
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			reqItemFieldCount = 6;
		}
		var requiredItemID = new int[reqItemFieldCount];
		var requiredItemCount = new int[reqItemFieldCount];
		for (var num = 0; num < 4; num++)
		{
			reqId[num] = packet.ReadEntry();
			var isGo = reqId[num].Value;
			var creatureOrGoId = reqId[num].Key;
			var creatureOrGoAmount = packet.ReadInt32();
			if (creatureOrGoId != 0 && creatureOrGoAmount != 0)
			{
				var objective4 = new QuestObjective
				{
					QuestID = response.QuestID,
					Id = QuestObjective.QuestObjectiveCounter++,
					StorageIndex = objectiveCounter++,
					Type = (isGo ? QuestObjectiveType.GameObject : QuestObjectiveType.Monster),
					ObjectID = creatureOrGoId,
					Amount = creatureOrGoAmount
				};
				quest.Objectives.Add(objective4);
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				requiredItemID[num] = packet.ReadInt32();
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
			{
				requiredItemCount[num] = packet.ReadInt32();
			}
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_8_9464))
			{
				requiredItemID[num] = packet.ReadInt32();
				requiredItemCount[num] = packet.ReadInt32();
			}
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
		{
			for (var num2 = 0; num2 < reqItemFieldCount; num2++)
			{
				requiredItemID[num2] = packet.ReadInt32();
				requiredItemCount[num2] = packet.ReadInt32();
			}
		}
		for (var num3 = 0; num3 < reqItemFieldCount; num3++)
		{
			if (requiredItemID[num3] != 0 && requiredItemCount[num3] != 0)
			{
				var objective5 = new QuestObjective
				{
					QuestID = response.QuestID,
					Id = QuestObjective.QuestObjectiveCounter++,
					StorageIndex = objectiveCounter++,
					Type = QuestObjectiveType.Item,
					ObjectID = requiredItemID[num3],
					Amount = requiredItemCount[num3]
				};
				quest.Objectives.Add(objective5);
			}
		}
		for (var num4 = 0; num4 < 4; num4++)
		{
			var objectiveText = packet.ReadCString();
			if (quest.Objectives.Count > num4)
			{
				quest.Objectives[num4].Description = objectiveText;
			}
		}
		quest.QuestMaxScalingLevel = 255;
		quest.RewardXPMultiplier = 1f;
		quest.RewardMoneyMultiplier = 1f;
		quest.RewardArtifactXPMultiplier = 1f;
		for (var num5 = 0; num5 < 5; num5++)
		{
			quest.RewardFactionCapIn[num5] = 7;
		}
		quest.AllowableRaces = 511L;
		quest.AcceptedSoundKitID = 890u;
		quest.CompleteSoundKitID = 878u;
		GameData.StoreQuestTemplate(response.QuestID, quest);
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUERY_CREATURE_RESPONSE)]
	private void HandleQueryCreatureResponse(WorldPacket packet)
	{
		var response = new QueryCreatureResponse();
		var id = packet.ReadEntry();
		response.CreatureID = (uint)id.Key;
		if (id.Value)
		{
			response.Allow = false;
			SendPacketToClient(response);
			return;
		}
		response.Allow = true;
		response.Stats = new CreatureTemplate();
		var creature = response.Stats;
		for (var i = 0; i < 4; i++)
		{
			creature.Name[i] = packet.ReadCString();
		}
		creature.Title = packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			creature.CursorName = packet.ReadCString();
		}
		creature.Flags[0] = packet.ReadUInt32();
		creature.Type = packet.ReadInt32();
		creature.Family = packet.ReadInt32();
		creature.Classification = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			for (var j = 0; j < 2; j++)
			{
				creature.ProxyCreatureID[j] = packet.ReadUInt32();
			}
		}
		else
		{
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				packet.ReadInt32();
			}
			creature.PetSpellDataId = packet.ReadUInt32();
		}
		var displayIdCount = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? 1 : 4);
		for (var k = 0; k < displayIdCount; k++)
		{
			var displayId = packet.ReadUInt32();
			if (displayId != 0)
			{
				creature.Display.CreatureDisplay.Add(new CreatureXDisplay(displayId, 1f, 0f));
			}
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			creature.HpMulti = packet.ReadFloat();
			creature.EnergyMulti = packet.ReadFloat();
		}
		else
		{
			creature.HpMulti = 1f;
			creature.EnergyMulti = 1f;
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			creature.Civilian = packet.ReadBool();
		}
		creature.Leader = packet.ReadBool();
		var questItems = (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192) ? 6 : 4);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			for (var i2 = 0u; i2 < questItems; i2++)
			{
				var itemId = packet.ReadUInt32();
				if (itemId != 0)
				{
					creature.QuestItems.Add(itemId);
				}
			}
			packet.ReadUInt32();
		}
		creature.Flags[0] |= 134217728u;
		creature.MovementInfoID = 1693u;
		creature.Class = 1;
		GameData.StoreCreatureTemplate(response.CreatureID, creature);
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUERY_GAME_OBJECT_RESPONSE)]
	private void HandleQueryGameObjectResponse(WorldPacket packet)
	{
		var response = new QueryGameObjectResponse();
		var id = packet.ReadEntry();
		response.GameObjectID = (uint)id.Key;
		response.Guid = WowGuid128.Empty;
		if (id.Value)
		{
			response.Allow = false;
			SendPacketToClient(response);
			return;
		}
		response.Allow = true;
		response.Stats = new GameObjectStats();
		var gameObject = response.Stats;
		gameObject.Type = packet.ReadUInt32();
		gameObject.DisplayID = packet.ReadUInt32();
		for (var i = 0; i < 4; i++)
		{
			gameObject.Name[i] = packet.ReadCString();
		}
		gameObject.IconName = packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			gameObject.CastBarCaption = packet.ReadCString();
			gameObject.UnkString = packet.ReadCString();
		}
		for (var j = 0; j < 24; j++)
		{
			gameObject.Data[j] = packet.ReadInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			gameObject.Size = packet.ReadFloat();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			var count = (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192) ? 6u : 4u);
			for (var i2 = 0u; i2 < count; i2++)
			{
				var itemId = packet.ReadUInt32();
				if (itemId != 0)
				{
					gameObject.QuestItems.Add(itemId);
				}
			}
		}
		// Cache for pre-sending before transport CreateObjects
		GetSession().GameState.GameObjectQueryCache[response.GameObjectID] = response;
		if (gameObject.Type == 15) // MO_TRANSPORT — log template data for debugging
			Log.Print(LogType.Debug, $"[GOQuery] Entry={response.GameObjectID} Type={gameObject.Type} DisplayID={gameObject.DisplayID} Name={gameObject.Name[0]} data0(pathID)={gameObject.Data[0]} data1(speed)={gameObject.Data[1]} data2(accel)={gameObject.Data[2]} Size={gameObject.Size}");
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUERY_PAGE_TEXT_RESPONSE)]
	private void HandleQueryPageTextResponse(WorldPacket packet)
	{
		var response = new QueryPageTextResponse
		{
			PageTextID = packet.ReadUInt32(),
			Allow = true
		};
		var page = new QueryPageTextResponse.PageTextInfo
		{
			Id = response.PageTextID,
			Text = packet.ReadCString(),
			NextPageID = packet.ReadUInt32()
		};
		response.Pages.Add(page);
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUERY_NPC_TEXT_RESPONSE)]
	private void HandleQueryNpcTextResponse(WorldPacket packet)
	{
		var response = new QueryNPCTextResponse();
		var id = packet.ReadEntry();
		response.TextID = (uint)id.Key;
		if (id.Value)
		{
			response.Allow = false;
			SendPacketToClient(response);
			return;
		}
		response.Allow = true;
		for (var i = 0; i < 8; i++)
		{
			response.Probabilities[i] = packet.ReadFloat();
			var maleText = packet.ReadCString().TrimEnd().Replace("\0", "");
			var femaleText = packet.ReadCString().TrimEnd().Replace("\0", "");
			var language = packet.ReadUInt32();
			var emoteDelays = new ushort[3];
			var emotes = new ushort[3];
			for (var j = 0; j < 3; j++)
			{
				emoteDelays[j] = (ushort)packet.ReadUInt32();
				emotes[j] = (ushort)packet.ReadUInt32();
			}
			if ((string.IsNullOrEmpty(maleText) && string.IsNullOrEmpty(femaleText)) || (maleText.Equals("Greetings $N") && femaleText.Equals("Greetings $N") && i != 0))
			{
				response.BroadcastTextID[i] = 0u;
			}
			else
			{
				response.BroadcastTextID[i] = GameData.GetBroadcastTextId(maleText, femaleText, language, emoteDelays, emotes);
			}
		}
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_ITEM_QUERY_SINGLE_RESPONSE)]
	private void HandleItemQueryResponse(WorldPacket packet)
	{
		var entry = packet.ReadEntry();
		if (entry.Value)
		{
			// Server doesn't have this item - remove from requested sets but do NOT send
			// Invalid DBReply, as that would poison the client's hotfix cache before a
			// valid hotfix can arrive from a different query
			GetSession().GameState.RequestedItemHotfixes.Remove((uint)entry.Key);
			GetSession().GameState.RequestedItemSparseHotfixes.Remove((uint)entry.Key);
			Log.Print(LogType.Debug, $"Item #{entry.Key} not found on legacy server, skipping Invalid DBReply.", "QueryHandler.cs");
		}
		else
		{
			var item = new ItemTemplate();
			item.ReadFromLegacyPacket((uint)entry.Key, packet);
			SendItemUpdatesIfNeeded(item);
			GameData.StoreItemTemplate((uint)entry.Key, item);

			// Flush any buffered item CreateObjects that were waiting for this template
			var itemEntryKey = (uint)entry.Key;
			if (GetSession().GameState.PendingItemCreates.TryGetValue(itemEntryKey, out var pendingUpdates))
			{
				GetSession().GameState.PendingItemCreates.Remove(itemEntryKey);
				var updateObject = new UpdateObject(GetSession().GameState);
				foreach (var pending in pendingUpdates)
				{
					updateObject.ObjectUpdates.Add(pending);
					Log.Print(LogType.Debug, $"Flushing buffered item CreateObject {pending.Guid} entry={itemEntryKey} after template arrived.", "");
				}
				if (updateObject.ObjectUpdates.Count > 0)
				{
					SendPacketToClient(updateObject);
				}
			}
		}
	}

	private void SendItemUpdatesIfNeeded(ItemTemplate item)
	{
		// Skip hotfix for glyph items (class=16) - the 3.4.3 client already has correct
		// data for these in CASC. Our hotfix would override it with incomplete data,
		// breaking the icon and preventing the item from being used.
		if (item.Class == 16)
			return;

		var reply = GameData.GenerateItemUpdateIfNeeded(item);
		if (reply != null)
		{
			SendPacketToClient(reply);
		}
		reply = GameData.GenerateItemSparseUpdateIfNeeded(item);
		if (reply != null)
		{
			SendPacketToClient(reply);
			var replyA = new DBReply
			{
				Status = HotfixStatus.Valid,
				Timestamp = (uint)Time.UnixTime,
				RecordID = reply.Hotfixes[0].RecordId,
				TableHash = reply.Hotfixes[0].TableHash,
				Data = reply.Hotfixes[0].HotfixContent
			};
			SendPacketToClient(replyA);
		}
		// Skip ItemEffect hotfix for mount items (class 15, subclass 5) — the DB2 already has
		// the correct mount summon spell; overwriting with the learn spell (55884) breaks the
		// 3.4.3 client's mount item recognition.
		if (item.Class != 15 || item.SubClass != 5)
		{
			for (byte i = 0; i < 5; i++)
			{
				reply = GameData.GenerateItemEffectUpdateIfNeeded(item, i);
				if (reply != null)
				{
					SendPacketToClient(reply);
				}
			}
		}
		if (GameData.ItemCanHaveModel(item))
		{
			reply = GameData.GenerateItemAppearanceUpdateIfNeeded(item);
			if (reply != null)
			{
				SendPacketToClient(reply);
			}
			reply = GameData.GenerateItemModifiedAppearanceUpdateIfNeeded(item);
			if (reply != null)
			{
				SendPacketToClient(reply);
			}
		}
	}

	[PacketHandler(Opcode.SMSG_QUERY_PET_NAME_RESPONSE)]
	private void HandleQueryPetNameResponse(WorldPacket packet)
	{
		var petNumber = packet.ReadUInt32();
		var guid = GetSession().GameState.GetPetGuidByNumber(petNumber);
		if (guid == null)
		{
			Log.Print(LogType.Error, $"Pet name query response for unknown pet {petNumber}!", "QueryHandler.cs");
			return;
		}
		var response = new QueryPetNameResponse
		{
			UnitGUID = guid,
			Name = packet.ReadCString()
		};
		if (response.Name.Length == 0)
		{
			response.Allow = false;
			packet.ReadBytes(7u);
			return;
		}
		response.Allow = true;
		response.Timestamp = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool())
		{
			for (var i = 0; i < 5; i++)
			{
				response.DeclinedNames.name[i] = packet.ReadCString();
			}
		}
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_ITEM_NAME_QUERY_RESPONSE)]
	private void HandleItemNameQueryResponse(WorldPacket packet)
	{
		var entry = packet.ReadUInt32();
		var name = packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.ReadUInt32();
		}
		GameData.StoreItemName(entry, name);
	}

	[PacketHandler(Opcode.SMSG_WHO)]
	private void HandleWhoResponse(WorldPacket packet)
	{
		var response = new WhoResponsePkt
		{
			RequestID = GetSession().GameState.LastWhoRequestId
		};
		var count = packet.ReadUInt32();
		packet.ReadUInt32();
		for (var i = 0; i < count; i++)
		{
			var player = new WhoEntry
			{
				PlayerData =
				{
					Name = packet.ReadCString(),
					Level = (byte)packet.ReadUInt32(),
					ClassID = (Class)packet.ReadUInt32(),
					RaceID = (Race)packet.ReadUInt32()
				},
				GuildName = packet.ReadCString()
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				player.PlayerData.Sex = (Gender)packet.ReadUInt8();
			}
			player.AreaID = packet.ReadInt32();
			player.PlayerData.GuidActual = GetSession().GameState.GetPlayerGuidByName(player.PlayerData.Name);
			if (player.PlayerData.GuidActual == null)
			{
				player.PlayerData.GuidActual = WowGuid128.CreateUnknownPlayerGuid();
			}
			player.PlayerData.AccountID = GetSession().GetGameAccountGuidForPlayer(player.PlayerData.GuidActual);
			player.PlayerData.BnetAccountID = GetSession().GetBnetAccountGuidForPlayer(player.PlayerData.GuidActual);
			player.PlayerData.VirtualRealmAddress = GetSession().RealmId.GetAddress();
			if (!string.IsNullOrEmpty(player.GuildName))
			{
				player.GuildGUID = GetSession().GetGuildGuid(player.GuildName);
				player.GuildVirtualRealmAddress = player.PlayerData.VirtualRealmAddress;
			}
			response.Players.Add(player);
			Session.GameState.UpdatePlayerCache(player.PlayerData.GuidActual, new PlayerCache
			{
				Name = player.PlayerData.Name,
				RaceId = player.PlayerData.RaceID,
				ClassId = player.PlayerData.ClassID,
				SexId = player.PlayerData.Sex,
				Level = player.PlayerData.Level
			});
		}
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_QUEST_DETAILS)]
	private void HandleQuestGiverQuestDetails(WorldPacket packet)
	{
		var quest = new QuestGiverQuestDetails
		{
			QuestGiverGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = quest.QuestGiverGUID;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			quest.InformUnit = packet.ReadGuid().To128(GetSession().GameState);
		}
		else
		{
			quest.InformUnit = quest.QuestGiverGUID;
		}
		quest.QuestID = packet.ReadUInt32();
		quest.QuestTitle = packet.ReadCString();
		quest.DescriptionText = packet.ReadCString();
		quest.LogDescription = packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.AutoLaunched = packet.ReadBool();
		}
		else
		{
			quest.AutoLaunched = packet.ReadUInt32() != 0;
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
		{
			quest.QuestFlags[0] = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			quest.SuggestedPartyMembers = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt8();
		}
		if (LegacyVersion.InVersion(ClientVersionBuild.V3_1_0_9767, ClientVersionBuild.V3_3_3a_11723))
		{
			quest.StartCheat = packet.ReadBool();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_2_11403))
			{
				quest.DisplayPopup = packet.ReadBool();
			}
		}
		if (quest.QuestFlags[0].HasAnyFlag(QuestFlags.HiddenRewards) && LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_3_5a_12340))
		{
			packet.ReadUInt32();
			packet.ReadUInt32();
			quest.Rewards.Money = packet.ReadUInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_2_10482))
			{
				quest.Rewards.XP = packet.ReadUInt32();
			}
		}
		ReadExtraQuestInfo(packet, quest.Rewards, readFlags: false);
		var emoteCount = packet.ReadUInt32();
		for (var i = 0; i < emoteCount; i++)
		{
			quest.DescEmotes[i].Type = packet.ReadUInt32();
			quest.DescEmotes[i].Delay = packet.ReadUInt32();
		}
		SendPacketToClient(quest);
	}

	private void ReadExtraQuestInfo(WorldPacket packet, QuestRewards rewards, bool readFlags)
	{
		rewards.ChoiceItemCount = packet.ReadUInt32();
		for (var i = 0; i < rewards.ChoiceItemCount; i++)
		{
			rewards.ChoiceItems[i].Item.ItemID = packet.ReadUInt32();
			rewards.ChoiceItems[i].Quantity = packet.ReadUInt32();
			packet.ReadUInt32();
		}
		var rewardCount = packet.ReadUInt32();
		for (var j = 0; j < rewardCount; j++)
		{
			rewards.ItemID[j] = packet.ReadUInt32();
			rewards.ItemQty[j] = packet.ReadUInt32();
			packet.ReadUInt32();
		}
		rewards.Money = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_2_10482))
		{
			rewards.XP = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
		{
			rewards.Honor = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			packet.ReadFloat();
		}
		if (readFlags)
		{
			packet.ReadUInt32();
		}
		rewards.SpellCompletionID = packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
		{
			rewards.Title = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			rewards.NumSkillUps = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			packet.ReadUInt32();
			packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			for (var k = 0; k < 5; k++)
			{
				rewards.FactionID[k] = packet.ReadUInt32();
			}
			for (var l = 0; l < 5; l++)
			{
				rewards.FactionValue[l] = packet.ReadInt32();
			}
			for (var m = 0; m < 5; m++)
			{
				packet.ReadInt32();
			}
		}
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_STATUS)]
	private void HandleQuestGiverStatus(WorldPacket packet)
	{
		var response = new QuestGiverStatusPkt
		{
			QuestGiver =
			{
				Guid = packet.ReadGuid().To128(GetSession().GameState),
				Status = LegacyVersion.ConvertQuestGiverStatus(packet.ReadUInt8())
			}
		};
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_STATUS_MULTIPLE)]
	private void HandleQuestGiverStatusMultple(WorldPacket packet)
	{
		var response = new QuestGiverStatusMultiple();
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var info = new QuestGiverInfo
			{
				Guid = packet.ReadGuid().To128(GetSession().GameState),
				Status = LegacyVersion.ConvertQuestGiverStatus(packet.ReadUInt8())
			};
			response.QuestGivers.Add(info);
		}
		SendPacketToClient(response);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_QUEST_LIST_MESSAGE)]
	private void HandleQuestGiverQuestListMessage(WorldPacket packet)
	{
		var quests = new QuestGiverQuestListMessage
		{
			QuestGiverGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = quests.QuestGiverGUID;
		quests.Greeting = packet.ReadCString();
		quests.GreetEmoteDelay = packet.ReadUInt32();
		quests.GreetEmoteType = packet.ReadUInt32();
		var count = packet.ReadUInt8();
		for (var i = 0; i < count; i++)
		{
			var quest = ReadGossipQuestOption(packet);
			quests.QuestOptions.Add(quest);
		}
		SendPacketToClient(quests);
	}

	private ClientGossipQuest ReadGossipQuestOption(WorldPacket packet)
	{
		var quest = new ClientGossipQuest
		{
			QuestID = packet.ReadUInt32()
		};
		// Icon value from server: 0=autocomplete, 2=available, 4=completable
		// Use directly as QuestType - do NOT convert through QuestGiverStatus enum
		var questIcon = packet.ReadInt32();
		quest.QuestType = questIcon;
		quest.QuestLevel = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
		{
			quest.QuestFlags = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
		{
			quest.Repeatable = packet.ReadBool();
		}
		quest.QuestTitle = packet.ReadCString();
		return quest;
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_REQUEST_ITEMS)]
	private void HandleQuestGiverRequestItems(WorldPacket packet)
	{
		var quest = new QuestGiverRequestItems
		{
			QuestGiverGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		GetSession().GameState.CurrentInteractedWithNPC = quest.QuestGiverGUID;
		quest.QuestGiverCreatureID = quest.QuestGiverGUID.GetEntry();
		quest.QuestID = packet.ReadUInt32();
		quest.QuestTitle = packet.ReadCString();
		quest.CompletionText = packet.ReadCString();
		quest.CompEmoteDelay = packet.ReadUInt32();
		quest.CompEmoteType = packet.ReadUInt32();
		quest.AutoLaunched = packet.ReadUInt32() != 0;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
		{
			quest.QuestFlags[0] = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			quest.SuggestPartyMembers = packet.ReadUInt32();
		}
		quest.MoneyToGet = packet.ReadInt32();
		var itemsCount = packet.ReadUInt32();
		for (var i = 0; i < itemsCount; i++)
		{
			var item = new QuestObjectiveCollect
			{
				ObjectID = packet.ReadUInt32(),
				Amount = packet.ReadUInt32()
			};
			packet.ReadUInt32();
			quest.Collect.Add(item);
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.ReadUInt32();
		}
		var statusFlags = packet.ReadUInt32();
		if ((statusFlags & 3) != 0)
		{
			quest.StatusFlags = 223u;
		}
		else
		{
			quest.StatusFlags = 219u;
		}
		packet.ReadUInt32();
		packet.ReadUInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.ReadUInt32();
		}
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_OFFER_REWARD_MESSAGE)]
	private void HandleQuestGiverOfferRewardMessage(WorldPacket packet)
	{
		var quest = new QuestGiverOfferRewardMessage
		{
			QuestData =
			{
				QuestGiverGUID = packet.ReadGuid().To128(GetSession().GameState)
			}
		};
		GetSession().GameState.CurrentInteractedWithNPC = quest.QuestData.QuestGiverGUID;
		quest.QuestData.QuestGiverCreatureID = quest.QuestData.QuestGiverGUID.GetEntry();
		quest.QuestGiverCreatureID = (int)quest.QuestData.QuestGiverGUID.GetEntry();
		quest.QuestData.QuestID = packet.ReadUInt32();
		quest.QuestTitle = packet.ReadCString();
		quest.RewardText = packet.ReadCString();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
		{
			quest.QuestData.AutoLaunched = packet.ReadBool();
		}
		else
		{
			quest.QuestData.AutoLaunched = packet.ReadUInt32() != 0;
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
		{
			quest.QuestData.QuestFlags[0] = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			quest.QuestData.SuggestedPartyMembers = packet.ReadUInt32();
		}
		var emotesCount = packet.ReadUInt32();
		for (var i = 0; i < emotesCount; i++)
		{
			var emote = new QuestDescEmote
			{
				Delay = packet.ReadUInt32(),
				Type = packet.ReadUInt32()
			};
		}
		ReadExtraQuestInfo(packet, quest.QuestData.Rewards, readFlags: true);
		// Cache quest template for reward selection (HandleQuestGiverChooseReward needs it)
		if (GameData.GetQuestTemplate(quest.QuestData.QuestID) == null)
		{
			var cached = new QuestTemplate();
			for (var ci = 0; ci < quest.QuestData.Rewards.ChoiceItemCount && ci < 6; ci++)
			{
				cached.UnfilteredChoiceItems[ci].ItemID = quest.QuestData.Rewards.ChoiceItems[ci].Item.ItemID;
				cached.UnfilteredChoiceItems[ci].Quantity = quest.QuestData.Rewards.ChoiceItems[ci].Quantity;
			}
			GameData.StoreQuestTemplate(quest.QuestData.QuestID, cached);
		}
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_QUEST_COMPLETE)]
	private void HandleQuestGiverQuestComplete(WorldPacket packet)
	{
		var quest = new QuestGiverQuestComplete
		{
			QuestID = packet.ReadUInt32()
		};
		GetSession().GameState.CurrentPlayerStorage.CompletedQuests.MarkQuestAsCompleted(quest.QuestID);
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt32();
		}
		quest.XPReward = packet.ReadUInt32();
		quest.MoneyReward = packet.ReadInt32();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
		{
			packet.ReadInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadInt32();
			packet.ReadInt32();
		}
		var itemId = 0u;
		var itemCount = 0u;
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var itemsCount = packet.ReadUInt32();
			for (var i = 0u; i < itemsCount; i++)
			{
				var itemId2 = packet.ReadUInt32();
				var itemCount2 = packet.ReadUInt32();
				if (itemId2 != 0 && itemCount2 != 0)
				{
					itemId = itemId2;
					itemCount = itemCount2;
				}
			}
		}
		quest.ItemReward.ItemID = itemId;
		var questTemplate = GameData.GetQuestTemplate(quest.QuestID);
		if (questTemplate != null && questTemplate.RewardNextQuest == 0)
		{
			quest.LaunchQuest = false;
			if (GetSession().GameState.CurrentInteractedWithNPC != null)
			{
				var npcFlags = GetSession().GameState.GetLegacyFieldValueUInt32(GetSession().GameState.CurrentInteractedWithNPC, UnitField.UNIT_NPC_FLAGS);
				if (npcFlags.HasAnyFlag(NPCFlags.Gossip))
				{
					quest.LaunchGossip = true;
				}
			}
		}
		SendPacketToClient(quest);
		var toast = new DisplayToast
		{
			QuestID = quest.QuestID
		};
		if (itemId != 0 && itemCount != 0)
		{
			toast.Quantity = 1uL;
			toast.Type = 0;
			toast.ItemReward.ItemID = itemId;
		}
		else
		{
			toast.Quantity = 60uL;
			toast.Type = 2;
		}
		SendPacketToClient(toast);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_QUEST_FAILED)]
	private void HandleQuestGiverQuestFailed(WorldPacket packet)
	{
		var quest = new QuestGiverQuestFailed
		{
			QuestID = packet.ReadUInt32(),
			Reason = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32())
		};
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.SMSG_QUEST_GIVER_INVALID_QUEST)]
	private void HandleQuestGiverInvalidQuest(WorldPacket packet)
	{
		var quest = new QuestGiverInvalidQuest
		{
			Reason = (QuestFailedReasons)packet.ReadUInt32()
		};
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.SMSG_QUEST_UPDATE_COMPLETE)]
	[PacketHandler(Opcode.SMSG_QUEST_UPDATE_FAILED)]
	[PacketHandler(Opcode.SMSG_QUEST_UPDATE_FAILED_TIMER)]
	private void HandleQuestUpdateStatus(WorldPacket packet)
	{
		var quest = new QuestUpdateStatus(packet.GetUniversalOpcode(isModern: false))
		{
			QuestID = packet.ReadUInt32()
		};
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.SMSG_QUEST_UPDATE_ADD_ITEM)]
	private void HandleQuestUpdateAddItem(WorldPacket packet)
	{
		var itemId = packet.ReadUInt32();
		var count = packet.ReadUInt32();
		var objective = GameData.GetQuestObjectiveForItem(itemId);
		if (objective != null)
		{
			return;
		}
		var updateFields = GetSession().GameState.GetCachedObjectFieldsLegacy(GetSession().GameState.CurrentPlayerGuid);
		var questsCount = LegacyVersion.GetQuestLogSize();
		for (var i = 0; i < questsCount; i++)
		{
			var logEntry = ReadQuestLogEntry(i, null, updateFields);
			if (logEntry != null && logEntry.QuestID.HasValue && GameData.GetQuestTemplate((uint)logEntry.QuestID.Value) == null)
			{
				var packet2 = new WorldPacket(Opcode.CMSG_QUERY_QUEST_INFO);
				packet2.WriteUInt32((uint)logEntry.QuestID.Value);
				SendPacketToServer(packet2);
			}
		}
	}

	[PacketHandler(Opcode.SMSG_QUEST_UPDATE_ADD_KILL)]
	private void HandleQuestUpdateAddKill(WorldPacket packet)
	{
		var credit = new QuestUpdateAddCredit
		{
			QuestID = packet.ReadUInt32()
		};
		var entry = packet.ReadEntry();
		credit.ObjectID = entry.Key;
		credit.ObjectiveType = (entry.Value ? QuestObjectiveType.GameObject : QuestObjectiveType.Monster);
		credit.Count = (ushort)packet.ReadUInt32();
		credit.Required = (ushort)packet.ReadUInt32();
		credit.VictimGUID = packet.ReadGuid().To128(GetSession().GameState);
		SendPacketToClient(credit);
	}

	[PacketHandler(Opcode.SMSG_QUEST_CONFIRM_ACCEPT)]
	private void HandleQuestConfirmAccept(WorldPacket packet)
	{
		var quest = new QuestConfirmAccept
		{
			QuestID = packet.ReadUInt32(),
			QuestTitle = packet.ReadCString(),
			InitiatedBy = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.MSG_QUEST_PUSH_RESULT)]
	private void HandleQuestPushResult(WorldPacket packet)
	{
		var quest = new QuestPushResult
		{
			SenderGUID = packet.ReadGuid().To128(GetSession().GameState),
			Result = (QuestPushReason)packet.ReadUInt8()
		};
		SendPacketToClient(quest);
	}

	[PacketHandler(Opcode.SMSG_INITIALIZE_FACTIONS)]
	private void HandleInitializeFactions(WorldPacket packet)
	{
		if (GetSession().GameState.IsFirstEnterWorld)
		{
			var factions = new InitializeFactions();
			var count = packet.ReadUInt32();
			for (var i = 0u; i < count; i++)
			{
				factions.FactionFlags[i] = (ReputationFlags)packet.ReadUInt8();
				factions.FactionStandings[i] = packet.ReadInt32();
			}
			SendPacketToClient(factions);
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				SendPacketToClient(new TimeSyncRequest());
			}
		}
	}

	[PacketHandler(Opcode.SMSG_SET_FACTION_STANDING)]
	private void HandleSetFactionStanding(WorldPacket packet)
	{
		var standing = new SetFactionStanding();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
		{
			packet.ReadFloat();
		}
		var showVisual = true;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			showVisual = packet.ReadBool();
		}
		standing.ShowVisual = showVisual;
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var faction = new FactionStandingData
			{
				Index = packet.ReadInt32(),
				Standing = packet.ReadInt32()
			};
			standing.Factions.Add(faction);
		}
		SendPacketToClient(standing);
	}

	[PacketHandler(Opcode.SMSG_SET_FORCED_REACTIONS)]
	private void HandleSetForcedReaction(WorldPacket packet)
	{
		var reactions = new SetForcedReactions();
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var reaction = new ForcedReaction
			{
				Faction = packet.ReadInt32(),
				Reaction = packet.ReadInt32()
			};
			reactions.Reactions.Add(reaction);
		}
		SendPacketToClient(reactions);
	}

	[PacketHandler(Opcode.SMSG_SET_FACTION_VISIBLE)]
	private void HandleSetFactionVisible(WorldPacket packet)
	{
		var faction = new SetFactionVisible(visible: true)
		{
			FactionIndex = packet.ReadUInt32()
		};
		SendPacketToClient(faction);
	}

	[PacketHandler(Opcode.SMSG_FRIEND_LIST)]
	private void HandleFriendList(WorldPacket packet)
	{
		var contacts = new ContactList
		{
			Flags = SocialFlag.Friend
		};
		var count = packet.ReadUInt8();
		for (var i = 0; i < count; i++)
		{
			var contact = new ContactInfo
			{
				TypeFlags = SocialFlag.Friend,
				Guid = packet.ReadGuid().To128(GetSession().GameState)
			};
			contact.WowAccountGuid = GetSession().GetGameAccountGuidForPlayer(contact.Guid);
			contact.NativeRealmAddr = GetSession().RealmId.GetAddress();
			contact.VirtualRealmAddr = GetSession().RealmId.GetAddress();
			contact.Status = (FriendStatus)packet.ReadUInt8();
			if (contact.Status != FriendStatus.Offline)
			{
				contact.AreaID = packet.ReadUInt32();
				contact.Level = packet.ReadUInt32();
				contact.ClassID = (Class)packet.ReadUInt32();
			}
			contacts.Contacts.Add(contact);
		}
		SendPacketToClient(contacts);
	}

	[PacketHandler(Opcode.SMSG_IGNORE_LIST)]
	private void HandleIgnoreList(WorldPacket packet)
	{
		var contacts = new ContactList
		{
			Flags = SocialFlag.Ignored
		};
		var count = packet.ReadUInt8();
		var ignoredPlayers = new HashSet<WowGuid128>();
		for (var i = 0; i < count; i++)
		{
			var contact = new ContactInfo
			{
				TypeFlags = SocialFlag.Ignored,
				Guid = packet.ReadGuid().To128(GetSession().GameState)
			};
			contact.WowAccountGuid = GetSession().GetGameAccountGuidForPlayer(contact.Guid);
			contact.NativeRealmAddr = GetSession().RealmId.GetAddress();
			contact.VirtualRealmAddr = GetSession().RealmId.GetAddress();
			contacts.Contacts.Add(contact);
			ignoredPlayers.Add(contact.Guid);
		}
		Session.GameState.IgnoredPlayers = ignoredPlayers;
		SendPacketToClient(contacts);
	}

	[PacketHandler(Opcode.SMSG_CONTACT_LIST)]
	private void HandleContactList(WorldPacket packet)
	{
		var contacts = new ContactList
		{
			Flags = (SocialFlag)packet.ReadUInt32()
		};
		var count = packet.ReadUInt32();
		for (var i = 0; i < count; i++)
		{
			var contact = new ContactInfo
			{
				Guid = packet.ReadGuid().To128(GetSession().GameState)
			};
			contact.WowAccountGuid = GetSession().GetGameAccountGuidForPlayer(contact.Guid);
			contact.NativeRealmAddr = GetSession().RealmId.GetAddress();
			contact.VirtualRealmAddr = GetSession().RealmId.GetAddress();
			contact.TypeFlags = (SocialFlag)packet.ReadUInt32();
			contact.Note = packet.ReadCString();
			if (contact.TypeFlags.HasAnyFlag(SocialFlag.Friend))
			{
				contact.Status = (FriendStatus)packet.ReadUInt8();
				if (contact.Status != FriendStatus.Offline)
				{
					contact.AreaID = packet.ReadUInt32();
					contact.Level = packet.ReadUInt32();
					contact.ClassID = (Class)packet.ReadUInt32();
				}
			}
			contacts.Contacts.Add(contact);
		}
		SendPacketToClient(contacts);
	}

	[PacketHandler(Opcode.SMSG_FRIEND_STATUS)]
	private void HandleFriendStatus(WorldPacket packet)
	{
		var friend = new FriendStatusPkt
		{
			FriendResult = (FriendsResult)packet.ReadUInt8(),
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		friend.WowAccountGuid = GetSession().GetGameAccountGuidForPlayer(friend.Guid);
		friend.VirtualRealmAddress = GetSession().RealmId.GetAddress();
		switch (friend.FriendResult)
		{
		case FriendsResult.AddedOffline:
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				friend.Notes = packet.ReadCString();
			}
			break;
		case FriendsResult.AddedOnline:
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				friend.Notes = packet.ReadCString();
			}
			friend.Status = (FriendStatus)packet.ReadUInt8();
			friend.AreaID = packet.ReadUInt32();
			friend.Level = packet.ReadUInt32();
			friend.ClassID = (Class)packet.ReadUInt32();
			break;
		case FriendsResult.Online:
			friend.Status = (FriendStatus)packet.ReadUInt8();
			friend.AreaID = packet.ReadUInt32();
			friend.Level = packet.ReadUInt32();
			friend.ClassID = (Class)packet.ReadUInt32();
			break;
		}
		SendPacketToClient(friend);
		if (friend.FriendResult == FriendsResult.IgnoreAdded)
		{
			Session.GameState.IgnoredPlayers.Add(friend.Guid);
		}
		else if (friend.FriendResult == FriendsResult.IgnoreRemoved)
		{
			Session.GameState.IgnoredPlayers.Remove(friend.Guid);
		}
	}

	[PacketHandler(Opcode.SMSG_SEND_KNOWN_SPELLS)]
	private void HandleSendKnownSpells(WorldPacket packet)
	{
		var spells = new SendKnownSpells
		{
			InitialLogin = packet.ReadBool()
		};
		var spellCount = packet.ReadUInt16();
		for (ushort i = 0; i < spellCount; i++)
		{
			if (!packet.CanRead()) break;
			var spellId = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt16() : packet.ReadUInt32());
			spells.KnownSpells.Add(spellId);
			GetSession().GameState.KnownSpells.Add(spellId);
			if (!packet.CanRead()) break;
			packet.ReadInt16();
		}
		SendPacketToClient(spells);
		// Send mount collection based on known mount spells
		SendAccountMountUpdate();
		if (!packet.CanRead())
			return;
		var cooldownCount = packet.ReadUInt16();
		if (cooldownCount != 0)
		{
			var histories = new SendSpellHistory();
			for (ushort i2 = 0; i2 < cooldownCount; i2++)
			{
				if (!packet.CanRead()) break;
				var history = new SpellHistoryEntry();
				var spellId2 = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt16() : packet.ReadUInt32());
				history.SpellID = spellId2;
				var itemId = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V4_2_2_14545)) ? packet.ReadUInt16() : packet.ReadUInt32());
				history.ItemID = itemId;
				history.Category = packet.ReadUInt16();
				history.RecoveryTime = packet.ReadInt32();
				history.CategoryRecoveryTime = packet.ReadInt32();
				histories.Entries.Add(history);
			}
			SendPacketToClient(histories, Opcode.SMSG_SEND_UNLEARN_SPELLS);
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			SendPacketToClient(new SendUnlearnSpells());
			SendPacketToClient(new SendSpellCharges());
		}
	}

	[PacketHandler(Opcode.SMSG_SUPERCEDED_SPELLS)]
	private void HandleSupercededSpells(WorldPacket packet)
	{
		var spells = new SupercededSpells();
		uint supercededId;
		uint spellId;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			supercededId = packet.ReadUInt32();
			spellId = packet.ReadUInt32();
		}
		else
		{
			supercededId = packet.ReadUInt16();
			spellId = packet.ReadUInt16();
		}
		spells.SpellID.Add(spellId);
		spells.Superceded.Add(supercededId);
		SendPacketToClient(spells);
	}

	[PacketHandler(Opcode.SMSG_LEARNED_SPELL)]
	private void HandleLearnedSpell(WorldPacket packet)
	{
		var spells = new LearnedSpells();
		var spellId = packet.ReadUInt32();
		spells.ClientLearnedSpellData.Add(new LearnedSpellInfo
		{
			SpellID = (int)spellId,
			IsFavorite = false,
			Superceded = null
		});
		GetSession().GameState.KnownSpells.Add(spellId);
		SendPacketToClient(spells);
		// If this is a mount spell, update the mount collection
		if (GameData.MountSpells.Contains(spellId))
			SendAccountMountUpdate();
	}

	/// <summary>
	/// Sends SMSG_ACCOUNT_MOUNT_UPDATE with all known mount spells.
	/// TC343 format: WriteBit IsFullUpdate, uint32 count, then (int32 spellId + 4-bit flags) per mount.
	/// </summary>
	private void SendAccountMountUpdate()
	{
		var update = new AccountMountUpdate();
		foreach (var spellId in GetSession().GameState.KnownSpells)
		{
			if (GameData.MountSpells.Contains(spellId))
				update.MountSpellIDs.Add(spellId);
		}
		SendPacketToClient(update);
		Log.Print(LogType.Debug, $"[MountUpdate] Sent {update.MountSpellIDs.Count} mounts to client", "");
	}

	[PacketHandler(Opcode.SMSG_SEND_UNLEARN_SPELLS)]
	private void HandleSendUnlearnSpells(WorldPacket packet)
	{
		var spells = new SendUnlearnSpells();
		var spellCount = packet.ReadUInt32();
		for (var i = 0u; i < spellCount; i++)
		{
			var spellId = packet.ReadUInt32();
			spells.Spells.Add(spellId);
		}
		SendPacketToClient(spells);
	}

	[PacketHandler(Opcode.SMSG_UNLEARNED_SPELLS)]
	private void HandleUnlearnedSpells(WorldPacket packet)
	{
		var spells = new UnlearnedSpells();
		var spellId = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt16() : packet.ReadUInt32());
		spells.Spells.Add(spellId);
		SendPacketToClient(spells);
	}

	[PacketHandler(Opcode.SMSG_CAST_FAILED)]
	private void HandleCastFailed(WorldPacket packet)
	{
		if (Settings.ClientSpellDelay > 0)
		{
			Thread.Sleep(Settings.ClientSpellDelay);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt8();
		}
		var spellId = packet.ReadUInt32();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var status = packet.ReadUInt8();
			if (status != 2)
			{
				return;
			}
		}
		uint reason = packet.ReadUInt8();
		Log.Print(LogType.Debug, $"[CastFailed] SpellID={spellId} Reason={reason}", "");
		if (LegacyVersion.InVersion(ClientVersionBuild.V2_0_1_6180, ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt8();
		}
		var arg1 = 0;
		var arg2 = 0;
		if (packet.CanRead())
		{
			arg1 = packet.ReadInt32();
		}
		if (packet.CanRead())
		{
			arg2 = packet.ReadInt32();
		}
		if (GetSession().GameState.CurrentClientSpecialCast != null && GetSession().GameState.CurrentClientSpecialCast.SpellId == spellId)
		{
			var failed = new CastFailed
			{
				SpellID = GetSession().GameState.CurrentClientSpecialCast.SpellId,
				SpellXSpellVisualID = GetSession().GameState.CurrentClientSpecialCast.SpellXSpellVisualId,
				Reason = LegacyVersion.ConvertSpellCastResult(reason),
				CastID = GetSession().GameState.CurrentClientSpecialCast.ServerGUID,
				FailedArg1 = arg1,
				FailedArg2 = arg2
			};
			SendPacketToClient(failed);
			GetSession().GameState.CurrentClientSpecialCast = null;
		}
		else
		{
			if (GetSession().GameState.CurrentClientNormalCast == null || GetSession().GameState.CurrentClientNormalCast.SpellId != spellId)
			{
				return;
			}
			if (!GetSession().GameState.CurrentClientNormalCast.HasStarted)
			{
				var prepare2 = new SpellPrepare
				{
					ClientCastID = GetSession().GameState.CurrentClientNormalCast.ClientGUID,
					ServerCastID = GetSession().GameState.CurrentClientNormalCast.ServerGUID
				};
				SendPacketToClient(prepare2);
			}
			var failed2 = new CastFailed
			{
				SpellID = GetSession().GameState.CurrentClientNormalCast.SpellId,
				SpellXSpellVisualID = GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId,
				Reason = LegacyVersion.ConvertSpellCastResult(reason),
				CastID = GetSession().GameState.CurrentClientNormalCast.ServerGUID,
				FailedArg1 = arg1,
				FailedArg2 = arg2
			};
			SendPacketToClient(failed2);
			GetSession().GameState.CurrentClientNormalCast = null;
			foreach (var pending in GetSession().GameState.PendingClientCasts)
			{
				GetSession().InstanceSocket.SendCastRequestFailed(pending, isPet: false);
			}
			GetSession().GameState.PendingClientCasts.Clear();
		}
	}

	[PacketHandler(Opcode.SMSG_PET_CAST_FAILED, ClientVersionBuild.Zero, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePetCastFailed(WorldPacket packet)
	{
		if (Settings.ClientSpellDelay > 0)
		{
			Thread.Sleep(Settings.ClientSpellDelay);
		}
		var spellId = packet.ReadUInt32();
		var status = packet.ReadUInt8();
		if (status != 2 || GetSession().GameState.CurrentClientPetCast == null || GetSession().GameState.CurrentClientPetCast.SpellId != spellId)
		{
			return;
		}
		if (!GetSession().GameState.CurrentClientPetCast.HasStarted)
		{
			var prepare2 = new SpellPrepare
			{
				ClientCastID = GetSession().GameState.CurrentClientPetCast.ClientGUID,
				ServerCastID = GetSession().GameState.CurrentClientPetCast.ServerGUID
			};
			SendPacketToClient(prepare2);
		}
		var spell = new PetCastFailed
		{
			SpellID = spellId
		};
		uint reason = packet.ReadUInt8();
		spell.Reason = LegacyVersion.ConvertSpellCastResult(reason);
		spell.CastID = GetSession().GameState.CurrentClientPetCast.ServerGUID;
		SendPacketToClient(spell);
		foreach (var pending in GetSession().GameState.PendingClientPetCasts)
		{
			GetSession().InstanceSocket.SendCastRequestFailed(pending, isPet: true);
		}
		GetSession().GameState.PendingClientPetCasts.Clear();
	}

	[PacketHandler(Opcode.SMSG_PET_CAST_FAILED, ClientVersionBuild.V2_0_1_6180)]
	private void HandlePetCastFailedTBC(WorldPacket packet)
	{
		if (Settings.ClientSpellDelay > 0)
		{
			Thread.Sleep(Settings.ClientSpellDelay);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt8();
		}
		var spellId = packet.ReadUInt32();
		if (GetSession().GameState.CurrentClientPetCast == null || GetSession().GameState.CurrentClientPetCast.SpellId != spellId)
		{
			return;
		}
		if (!GetSession().GameState.CurrentClientPetCast.HasStarted)
		{
			var prepare2 = new SpellPrepare
			{
				ClientCastID = GetSession().GameState.CurrentClientPetCast.ClientGUID,
				ServerCastID = GetSession().GameState.CurrentClientPetCast.ServerGUID
			};
			SendPacketToClient(prepare2);
		}
		var failed = new PetCastFailed
		{
			SpellID = spellId
		};
		uint reason = packet.ReadUInt8();
		failed.Reason = LegacyVersion.ConvertSpellCastResult(reason);
		failed.CastID = GetSession().GameState.CurrentClientPetCast.ServerGUID;
		if (packet.CanRead())
		{
			failed.FailedArg1 = packet.ReadInt32();
		}
		if (packet.CanRead())
		{
			failed.FailedArg2 = packet.ReadInt32();
		}
		SendPacketToClient(failed);
		foreach (var pending in GetSession().GameState.PendingClientPetCasts)
		{
			GetSession().InstanceSocket.SendCastRequestFailed(pending, isPet: true);
		}
		GetSession().GameState.PendingClientPetCasts.Clear();
	}

	[PacketHandler(Opcode.SMSG_SPELL_FAILURE)]
	private void HandleSpellFailure(WorldPacket packet)
	{
		// Consumed — SpellFailure is generated from SMSG_SPELL_FAILED_OTHER handler
	}

	[PacketHandler(Opcode.SMSG_SPELL_FAILED_OTHER)]
	private void HandleSpellFailedOther(WorldPacket packet)
	{
		var casterUnit = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? packet.ReadGuid().To128(GetSession().GameState) : packet.ReadPackedGuid().To128(GetSession().GameState));
		if (casterUnit == GetSession().GameState.CurrentPlayerGuid && Settings.ClientSpellDelay > 0)
		{
			Thread.Sleep(Settings.ClientSpellDelay);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt8();
		}
		var spellId = packet.ReadUInt32();
		byte reason = 61;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			reason = (byte)LegacyVersion.ConvertSpellCastResult(packet.ReadUInt8());
		}
		WowGuid128 castId;
		uint spellVisual;
		if (GetSession().GameState.CurrentPlayerGuid == casterUnit && GetSession().GameState.CurrentClientNormalCast != null && GetSession().GameState.CurrentClientNormalCast.SpellId == spellId)
		{
			castId = GetSession().GameState.CurrentClientNormalCast.ServerGUID;
			spellVisual = GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
		}
		else if (GetSession().GameState.CurrentPetGuid == casterUnit && GetSession().GameState.CurrentClientPetCast != null && GetSession().GameState.CurrentClientPetCast.SpellId == spellId)
		{
			castId = GetSession().GameState.CurrentClientPetCast.ServerGUID;
			spellVisual = GetSession().GameState.CurrentClientPetCast.SpellXSpellVisualId;
		}
		else if (casterUnit == GetSession().GameState.CurrentPlayerGuid && GetSession().GameState.CurrentChanneledSpellId == spellId && GetSession().GameState.CurrentChanneledCastId != null)
		{
			// Channeled spell failure (e.g. fishing cancel) — use stored cast info
			castId = GetSession().GameState.CurrentChanneledCastId;
			spellVisual = GetSession().GameState.CurrentChanneledSpellVisualId;
			GetSession().GameState.CurrentChanneledSpellId = 0;
			GetSession().GameState.CurrentChanneledCastId = null;
		}
		else
		{
			castId = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, spellId, spellId + casterUnit.GetCounter());
			spellVisual = GameData.GetSpellVisual(spellId);
		}
		var spell = new SpellFailure
		{
			CasterUnit = casterUnit,
			CastID = castId,
			SpellID = spellId,
			SpellXSpellVisualID = spellVisual,
			Reason = reason
		};
		SendPacketToClient(spell);
		var spell2 = new SpellFailedOther
		{
			CasterUnit = casterUnit,
			CastID = castId,
			SpellID = spellId,
			SpellXSpellVisualID = spellVisual,
			Reason = reason
		};
		SendPacketToClient(spell2);
	}

	[PacketHandler(Opcode.SMSG_SPELL_START)]
	private void HandleSpellStart(WorldPacket packet)
	{
		if (!GetSession().GameState.CurrentMapId.HasValue)
		{
			return;
		}
		var spell = new SpellStart
		{
			Cast = HandleSpellStartOrGo(packet, isSpellGo: false)
		};
		byte failPending = 0;
		if (GetSession().GameState.CurrentPlayerGuid == spell.Cast.CasterUnit && GetSession().GameState.CurrentClientNormalCast != null && GetSession().GameState.CurrentClientNormalCast.SpellId == spell.Cast.SpellID)
		{
			spell.Cast.CastID = GetSession().GameState.CurrentClientNormalCast.ServerGUID;
			spell.Cast.SpellXSpellVisualID = GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
			GetSession().GameState.CurrentClientNormalCast.HasStarted = true;
			var prepare = new SpellPrepare
			{
				ClientCastID = GetSession().GameState.CurrentClientNormalCast.ClientGUID,
				ServerCastID = spell.Cast.CastID
			};
			SendPacketToClient(prepare);
			failPending = 1;
		}
		else if (GetSession().GameState.CurrentPetGuid == spell.Cast.CasterUnit && GetSession().GameState.CurrentClientPetCast != null && GetSession().GameState.CurrentClientPetCast.SpellId == spell.Cast.SpellID)
		{
			spell.Cast.CastID = GetSession().GameState.CurrentClientPetCast.ServerGUID;
			spell.Cast.SpellXSpellVisualID = GetSession().GameState.CurrentClientPetCast.SpellXSpellVisualId;
			GetSession().GameState.CurrentClientPetCast.HasStarted = true;
			var prepare2 = new SpellPrepare
			{
				ClientCastID = GetSession().GameState.CurrentClientPetCast.ClientGUID,
				ServerCastID = spell.Cast.CastID
			};
			SendPacketToClient(prepare2);
			failPending = 2;
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180) && GameData.DispellSpells.Contains((uint)spell.Cast.SpellID))
		{
			GetSession().GameState.LastDispellSpellId = (uint)spell.Cast.SpellID;
		}
		SendPacketToClient(spell);
		switch (failPending)
		{
		case 1:
			foreach (var pending2 in GetSession().GameState.PendingClientCasts)
			{
				GetSession().InstanceSocket.SendCastRequestFailed(pending2, isPet: false);
			}
			GetSession().GameState.PendingClientCasts.Clear();
			break;
		case 2:
			foreach (var pending in GetSession().GameState.PendingClientPetCasts)
			{
				GetSession().InstanceSocket.SendCastRequestFailed(pending, isPet: true);
			}
			GetSession().GameState.PendingClientPetCasts.Clear();
			break;
		}
	}

	[PacketHandler(Opcode.SMSG_SPELL_GO)]
	private void HandleSpellGo(WorldPacket packet)
	{
		if (!GetSession().GameState.CurrentMapId.HasValue)
		{
			return;
		}
		var spell = new SpellGo
		{
			Cast = HandleSpellStartOrGo(packet, isSpellGo: true)
		};
		// 3.3.5a SpellGo doesn't set CAST_FLAG_HAS_TRAJECTORY but 3.4.3 always expects it
		spell.Cast.CastFlags |= (uint)CastFlag.HasTrajectory;
		if (GetSession().GameState.CurrentPlayerGuid == spell.Cast.CasterUnit && GetSession().GameState.CurrentClientNormalCast != null && GetSession().GameState.CurrentClientNormalCast.SpellId == spell.Cast.SpellID)
		{
			spell.Cast.CastID = GetSession().GameState.CurrentClientNormalCast.ServerGUID;
			spell.Cast.SpellXSpellVisualID = GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
			// Save cast info for channeled spells — needed for cancel/failure after channel starts
			GetSession().GameState.CurrentChanneledCastId = GetSession().GameState.CurrentClientNormalCast.ServerGUID;
			GetSession().GameState.CurrentChanneledSpellVisualId = GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
			GetSession().GameState.CurrentClientNormalCast = null;
		}
		else if (GetSession().GameState.CurrentPlayerGuid == spell.Cast.CasterUnit && GetSession().GameState.CurrentClientSpecialCast != null && GetSession().GameState.CurrentClientSpecialCast.SpellId == spell.Cast.SpellID)
		{
			spell.Cast.CastID = GetSession().GameState.CurrentClientSpecialCast.ServerGUID;
			spell.Cast.SpellXSpellVisualID = GetSession().GameState.CurrentClientSpecialCast.SpellXSpellVisualId;
			GetSession().GameState.CurrentClientSpecialCast = null;
		}
		else if (GetSession().GameState.CurrentPetGuid == spell.Cast.CasterUnit && GetSession().GameState.CurrentClientPetCast != null && GetSession().GameState.CurrentClientPetCast.SpellId == spell.Cast.SpellID)
		{
			spell.Cast.CastID = GetSession().GameState.CurrentClientPetCast.ServerGUID;
			spell.Cast.SpellXSpellVisualID = GetSession().GameState.CurrentClientPetCast.SpellXSpellVisualId;
			GetSession().GameState.CurrentClientPetCast = null;
		}
		if (!spell.Cast.CasterUnit.IsEmpty() && GameData.AuraSpells.Contains((uint)spell.Cast.SpellID))
		{
			foreach (var target in spell.Cast.HitTargets)
			{
				GetSession().GameState.StoreLastAuraCasterOnTarget(target, (uint)spell.Cast.SpellID, spell.Cast.CasterUnit);
			}
		}
		SendPacketToClient(spell);
	}

	private SpellCastData HandleSpellStartOrGo(WorldPacket packet, bool isSpellGo)
	{
		var dbdata = new SpellCastData
		{
			CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			CasterUnit = packet.ReadPackedGuid().To128(GetSession().GameState)
		};
		if (dbdata.CasterUnit == GetSession().GameState.CurrentPlayerGuid && Settings.ClientSpellDelay > 0)
		{
			Thread.Sleep(Settings.ClientSpellDelay);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadUInt8();
		}
		dbdata.SpellID = packet.ReadInt32();
		dbdata.SpellXSpellVisualID = GameData.GetSpellVisual((uint)dbdata.SpellID);
		dbdata.CastID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, (uint)dbdata.SpellID, (ulong)dbdata.SpellID + dbdata.CasterUnit.GetCounter());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056) && !isSpellGo)
		{
			packet.ReadUInt8();
		}
		var flags = (dbdata.CastFlags = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056)) ? packet.ReadUInt16() : packet.ReadUInt32()));
		if (!isSpellGo || LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			dbdata.CastTime = packet.ReadUInt32();
		}
		if (isSpellGo)
		{
			var hitCount = packet.ReadUInt8();
			for (var i = 0; i < hitCount; i++)
			{
				var hitTarget = packet.ReadGuid().To128(GetSession().GameState);
				dbdata.HitTargets.Add(hitTarget);
			}
			var missCount = packet.ReadUInt8();
			for (var j = 0; j < missCount; j++)
			{
				var missTarget = packet.ReadGuid().To128(GetSession().GameState);
				var missType = (SpellMissInfo)packet.ReadUInt8();
				var reflectType = SpellMissInfo.None;
				if (missType == SpellMissInfo.Reflect)
				{
					reflectType = (SpellMissInfo)packet.ReadUInt8();
				}
				dbdata.MissTargets.Add(missTarget);
				dbdata.MissStatus.Add(new SpellMissStatus(missType, reflectType));
			}
		}
		var targetFlags = (SpellCastTargetFlags)(LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? packet.ReadUInt32() : packet.ReadUInt16());
		dbdata.Target.Flags = targetFlags;
		var unitTarget = WowGuid128.Empty;
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.CorpseMask | SpellCastTargetFlags.Unit | SpellCastTargetFlags.GameObject | SpellCastTargetFlags.UnitMinipet))
		{
			unitTarget = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		dbdata.Target.Unit = unitTarget;
		var itemTarget = WowGuid128.Empty;
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.Item | SpellCastTargetFlags.TradeItem))
		{
			itemTarget = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		dbdata.Target.Item = itemTarget;
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.SourceLocation))
		{
			dbdata.Target.SrcLocation = new TargetLocation
			{
				Transport = WowGuid128.Empty
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
			{
				dbdata.Target.SrcLocation.Transport = packet.ReadPackedGuid().To128(GetSession().GameState);
			}
			dbdata.Target.SrcLocation.Location = packet.ReadVector3();
		}
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.DestLocation))
		{
			dbdata.Target.DstLocation = new TargetLocation
			{
				Transport = WowGuid128.Empty
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
			{
				dbdata.Target.DstLocation.Transport = packet.ReadPackedGuid().To128(GetSession().GameState);
			}
			dbdata.Target.DstLocation.Location = packet.ReadVector3();
		}
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.String))
		{
			dbdata.Target.Name = packet.ReadCString();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			if (flags.HasAnyFlag(CastFlag.PredictedPower))
			{
				packet.ReadInt32();
			}
			if (flags.HasAnyFlag(CastFlag.RuneInfo))
			{
				var spellRuneState = packet.ReadUInt8();
				var playerRuneState = packet.ReadUInt8();
				for (var k = 0; k < 6; k++)
				{
					var mask = 1 << k;
					if ((mask & spellRuneState) != 0 && (mask & playerRuneState) == 0)
					{
						packet.ReadUInt8();
					}
				}
			}
			if (isSpellGo && flags.HasAnyFlag(CastFlag.AdjustMissile))
			{
				dbdata.MissileTrajectory.Pitch = packet.ReadFloat();
				dbdata.MissileTrajectory.TravelTime = packet.ReadUInt32();
			}
		}
		if (flags.HasAnyFlag(CastFlag.Projectile))
		{
			dbdata.AmmoDisplayId = packet.ReadInt32();
			dbdata.AmmoInventoryType = packet.ReadInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			if (isSpellGo)
			{
				if (flags.HasAnyFlag(CastFlag.VisualChain))
				{
					packet.ReadInt32();
					packet.ReadInt32();
				}
				if (targetFlags.HasAnyFlag(SpellCastTargetFlags.DestLocation))
				{
					packet.ReadInt8();
				}
				if (targetFlags.HasAnyFlag(SpellCastTargetFlags.ExtraTargets))
				{
					var targetCount = packet.ReadInt32();
					if (targetCount > 0)
					{
						var location = new TargetLocation();
						for (var l = 0; l < targetCount; l++)
						{
							location.Location = packet.ReadVector3();
							location.Transport = packet.ReadGuid().To128(GetSession().GameState);
						}
						dbdata.TargetPoints.Add(location);
					}
				}
			}
			else
			{
				if (flags.HasAnyFlag(CastFlag.Immunity))
				{
					dbdata.Immunities.School = packet.ReadUInt32();
					dbdata.Immunities.Value = packet.ReadUInt32();
				}
				if (flags.HasAnyFlag(CastFlag.HealPrediction))
				{
					packet.ReadInt32();
					if (packet.ReadUInt8() == 2)
					{
						packet.ReadPackedGuid();
					}
				}
			}
		}
		return dbdata;
	}

	[PacketHandler(Opcode.SMSG_CANCEL_AUTO_REPEAT)]
	private void HandleCancelAutoRepeat(WorldPacket packet)
	{
		if (Settings.ClientSpellDelay > 0)
		{
			Thread.Sleep(Settings.ClientSpellDelay);
		}
		if (GetSession().GameState.CurrentClientSpecialCast != null && GameData.AutoRepeatSpells.Contains(GetSession().GameState.CurrentClientSpecialCast.SpellId))
		{
			GetSession().GameState.CurrentClientSpecialCast = null;
		}
		var cancel = new CancelAutoRepeat();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			cancel.Guid = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		else
		{
			cancel.Guid = GetSession().GameState.CurrentPlayerGuid;
		}
		SendPacketToClient(cancel);
	}

	[PacketHandler(Opcode.SMSG_SPELL_COOLDOWN)]
	private void HandleSpellCooldown(WorldPacket packet)
	{
		var cooldown = new SpellCooldownPkt();
		try
		{
			cooldown.Caster = packet.ReadGuid().To128(GetSession().GameState);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				cooldown.Flags = packet.ReadUInt8();
			}
			while (packet.CanRead())
			{
				var cd = new SpellCooldownStruct
				{
					SpellID = packet.ReadUInt32(),
					ForcedCooldown = packet.ReadUInt32()
				};
				cooldown.SpellCooldowns.Add(cd);
			}
		}
		catch (ArgumentOutOfRangeException)
		{
			packet.ResetReadPos();
			var cd2 = new SpellCooldownStruct
			{
				SpellID = packet.ReadUInt32()
			};
			cooldown.Caster = packet.ReadPackedGuid().To128(GetSession().GameState);
			cd2.ForcedCooldown = packet.ReadUInt32();
			cooldown.SpellCooldowns.Add(cd2);
		}
		SendPacketToClient(cooldown);
	}

	[PacketHandler(Opcode.SMSG_COOLDOWN_EVENT)]
	private void HandleCooldownEvent(WorldPacket packet)
	{
		var cooldown = new CooldownEvent
		{
			SpellID = packet.ReadUInt32()
		};
		WowGuid guid = packet.ReadGuid();
		cooldown.IsPet = guid.GetHighType() == HighGuidType.Pet;
		SendPacketToClient(cooldown);
	}

	[PacketHandler(Opcode.SMSG_CLEAR_COOLDOWN)]
	private void HandleClearCooldown(WorldPacket packet)
	{
		var cooldown = new ClearCooldown
		{
			SpellID = packet.ReadUInt32()
		};
		WowGuid guid = packet.ReadGuid();
		cooldown.IsPet = guid.GetHighType() == HighGuidType.Pet;
		SendPacketToClient(cooldown);
	}

	[PacketHandler(Opcode.SMSG_COOLDOWN_CHEAT)]
	private void HandleCooldownCheat(WorldPacket packet)
	{
		var cooldown = new CooldownCheat
		{
			Guid = packet.ReadGuid().To128(GetSession().GameState)
		};
		SendPacketToClient(cooldown);
	}

	[PacketHandler(Opcode.SMSG_SPELL_NON_MELEE_DAMAGE_LOG)]
	private void HandleSpellNonMeleeDamageLog(WorldPacket packet)
	{
		var spell = new SpellNonMeleeDamageLog
		{
			TargetGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			SpellID = packet.ReadUInt32()
		};
		spell.SpellXSpellVisualID = GameData.GetSpellVisual(spell.SpellID);
		spell.CastID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, spell.SpellID, spell.SpellID + spell.CasterGUID.GetCounter());
		spell.Damage = packet.ReadInt32();
		spell.OriginalDamage = spell.Damage;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
		{
			spell.Overkill = packet.ReadInt32();
		}
		else
		{
			spell.Overkill = -1;
		}
		var school = packet.ReadUInt8();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			school = (byte)(1 << school);
		}
		spell.SchoolMask = school;
		spell.Absorbed = packet.ReadInt32();
		spell.Resisted = packet.ReadInt32();
		spell.Periodic = packet.ReadBool();
		packet.ReadUInt8();
		spell.ShieldBlock = packet.ReadInt32();
		spell.Flags = (SpellHitType)packet.ReadUInt32();
		if (packet.ReadBool() && !spell.Flags.HasAnyFlag(SpellHitType.Split))
		{
			if (spell.Flags.HasAnyFlag(SpellHitType.CritDebug))
			{
				packet.ReadFloat();
				packet.ReadFloat();
			}
			if (spell.Flags.HasAnyFlag(SpellHitType.HitDebug))
			{
				packet.ReadFloat();
				packet.ReadFloat();
			}
			if (spell.Flags.HasAnyFlag(SpellHitType.AttackTableDebug))
			{
				packet.ReadFloat();
				packet.ReadFloat();
				packet.ReadFloat();
				packet.ReadFloat();
				packet.ReadFloat();
				packet.ReadFloat();
			}
		}
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_SPELL_HEAL_LOG)]
	private void HandleSpellHealLog(WorldPacket packet)
	{
		var spell = new SpellHealLog
		{
			TargetGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			SpellID = packet.ReadUInt32(),
			HealAmount = packet.ReadInt32()
		};
		spell.OriginalHealAmount = spell.HealAmount;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
		{
			spell.OverHeal = packet.ReadUInt32();
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			spell.Absorbed = packet.ReadUInt32();
		}
		spell.Crit = packet.ReadBool();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool())
		{
			spell.CritRollMade = packet.ReadFloat();
			spell.CritRollNeeded = packet.ReadFloat();
		}
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_SPELL_PERIODIC_AURA_LOG)]
	private void HandleSpellPeriodicAuraLog(WorldPacket packet)
	{
		var spell = new SpellPeriodicAuraLog
		{
			TargetGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			SpellID = packet.ReadUInt32()
		};
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var aura = (AuraType)packet.ReadUInt32();
			switch (aura)
			{
			case AuraType.PeriodicDamage:
			case AuraType.PeriodicDamagePercent:
			{
				var effect4 = new SpellPeriodicAuraLog.SpellLogEffect
				{
					Effect = (uint)aura,
					Amount = packet.ReadInt32()
				};
				effect4.OriginalDamage = effect4.Amount;
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					effect4.OverHealOrKill = packet.ReadUInt32();
				}
				var school = packet.ReadUInt32();
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					school = (uint)(1 << (byte)school);
				}
				effect4.SchoolMaskOrPower = school;
				effect4.AbsorbedOrAmplitude = packet.ReadUInt32();
				effect4.Resisted = packet.ReadUInt32();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901))
				{
					effect4.Crit = packet.ReadBool();
				}
				spell.Effects.Add(effect4);
				break;
			}
			case AuraType.PeriodicHeal:
			case AuraType.ObsModHealth:
			{
				var effect3 = new SpellPeriodicAuraLog.SpellLogEffect
				{
					Effect = (uint)aura,
					Amount = packet.ReadInt32()
				};
				effect3.OriginalDamage = effect3.Amount;
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					effect3.OverHealOrKill = packet.ReadUInt32();
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
				{
					effect3.AbsorbedOrAmplitude = packet.ReadUInt32();
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901))
				{
					effect3.Crit = packet.ReadBool();
				}
				spell.Effects.Add(effect3);
				break;
			}
			case AuraType.ObsModPower:
			case AuraType.PeriodicEnergize:
			{
				var effect2 = new SpellPeriodicAuraLog.SpellLogEffect
				{
					Effect = (uint)aura,
					SchoolMaskOrPower = packet.ReadUInt32(),
					Amount = packet.ReadInt32()
				};
				spell.Effects.Add(effect2);
				break;
			}
			case AuraType.PeriodicManaLeech:
			{
				var effect = new SpellPeriodicAuraLog.SpellLogEffect
				{
					Effect = (uint)aura,
					SchoolMaskOrPower = packet.ReadUInt32(),
					Amount = packet.ReadInt32()
				};
				packet.ReadFloat();
				spell.Effects.Add(effect);
				break;
			}
			}
		}
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_SPELL_ENERGIZE_LOG)]
	private void HandleSpellEnergizeLog(WorldPacket packet)
	{
		var spell = new SpellEnergizeLog
		{
			TargetGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			SpellID = packet.ReadUInt32(),
			Type = (PowerType)packet.ReadUInt32(),
			Amount = packet.ReadInt32()
		};
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_SPELL_DELAYED)]
	private void HandleSpellDelayed(WorldPacket packet)
	{
		var delay = new SpellDelayed();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			delay.CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		else
		{
			delay.CasterGUID = packet.ReadGuid().To128(GetSession().GameState);
		}
		delay.Delay = packet.ReadInt32();
		SendPacketToClient(delay);
	}

	[PacketHandler(Opcode.MSG_CHANNEL_START)]
	private void HandleSpellChannelStart(WorldPacket packet)
	{
		var channel = new SpellChannelStart();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			channel.CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		else
		{
			channel.CasterGUID = GetSession().GameState.CurrentPlayerGuid;
		}
		channel.SpellID = packet.ReadUInt32();
		channel.SpellXSpellVisualID = GameData.GetSpellVisual(channel.SpellID);
		channel.Duration = packet.ReadUInt32();
		// Store channeled spell ID so cancel can use the real ID
		if (channel.CasterGUID == GetSession().GameState.CurrentPlayerGuid)
		{
			GetSession().GameState.CurrentChanneledSpellId = channel.SpellID;
		}
		SendPacketToClient(channel);
	}

	[PacketHandler(Opcode.MSG_CHANNEL_UPDATE)]
	private void HandleSpellChannelUpdate(WorldPacket packet)
	{
		var channel = new SpellChannelUpdate();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			channel.CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		else
		{
			channel.CasterGUID = GetSession().GameState.CurrentPlayerGuid;
		}
		channel.TimeRemaining = packet.ReadInt32();
		if (channel.TimeRemaining == 0 && channel.CasterGUID == GetSession().GameState.CurrentPlayerGuid)
		{
			GetSession().GameState.CurrentChanneledSpellId = 0;
			Log.Print(LogType.Debug, "[ChannelUpdate] Channel ended (TimeRemaining=0)", "");
		}
		SendPacketToClient(channel);
	}

	[PacketHandler(Opcode.SMSG_SPELL_DAMAGE_SHIELD)]
	private void HandleSpellDamageShield(WorldPacket packet)
	{
		var spell = new SpellDamageShield
		{
			VictimGUID = packet.ReadGuid().To128(GetSession().GameState),
			CasterGUID = packet.ReadGuid().To128(GetSession().GameState)
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			spell.SpellID = packet.ReadUInt32();
		}
		else
		{
			spell.SpellID = 7294u;
		}
		spell.Damage = packet.ReadInt32();
		spell.OriginalDamage = spell.Damage;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			spell.OverKill = packet.ReadUInt32();
		}
		var school = packet.ReadUInt32();
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			school = (uint)(1 << (byte)school);
		}
		spell.SchoolMask = school;
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_ENVIRONMENTAL_DAMAGE_LOG)]
	private void HandleEnvironmentalDamageLog(WorldPacket packet)
	{
		var damage = new EnvironmentalDamageLog
		{
			Victim = packet.ReadGuid().To128(GetSession().GameState),
			Type = (EnvironmentalDamage)packet.ReadUInt8(),
			Amount = packet.ReadInt32(),
			Absorbed = packet.ReadInt32(),
			Resisted = packet.ReadInt32()
		};
		SendPacketToClient(damage);
	}

	[PacketHandler(Opcode.SMSG_SPELL_INSTAKILL_LOG)]
	private void HandleSpellInstakillLog(WorldPacket packet)
	{
		var spell = new SpellInstakillLog();
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			spell.CasterGUID = packet.ReadGuid().To128(GetSession().GameState);
			spell.TargetGUID = packet.ReadGuid().To128(GetSession().GameState);
		}
		else
		{
			spell.CasterGUID = (spell.TargetGUID = packet.ReadGuid().To128(GetSession().GameState));
		}
		spell.SpellID = packet.ReadUInt32();
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_SPELL_DISPELL_LOG)]
	private void HandleSpellDispellLog(WorldPacket packet)
	{
		var spell = new SpellDispellLog
		{
			TargetGUID = packet.ReadPackedGuid().To128(GetSession().GameState),
			CasterGUID = packet.ReadPackedGuid().To128(GetSession().GameState)
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			spell.DispelledBySpellID = packet.ReadUInt32();
		}
		else
		{
			spell.DispelledBySpellID = GetSession().GameState.LastDispellSpellId;
		}
		var hasDebug = LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool();
		var count = packet.ReadInt32();
		for (var i = 0; i < count; i++)
		{
			var dispel = new SpellDispellData
			{
				SpellID = packet.ReadUInt32()
			};
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				dispel.Harmful = packet.ReadBool();
			}
			spell.DispellData.Add(dispel);
		}
		if (hasDebug)
		{
			packet.ReadInt32();
			packet.ReadInt32();
		}
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_PLAY_SPELL_VISUAL)]
	private void HandlePlaySpellVisualKit(WorldPacket packet)
	{
		var spell = new PlaySpellVisualKit
		{
			Unit = packet.ReadGuid().To128(GetSession().GameState),
			KitRecID = packet.ReadUInt32()
		};
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_PLAY_SPELL_IMPACT)]
	private void HandlePlaySpellImpact(WorldPacket packet)
	{
		var spell = new PlaySpellVisualKit
		{
			Unit = packet.ReadGuid().To128(GetSession().GameState),
			KitRecID = packet.ReadUInt32()
		};
		SendPacketToClient(spell);
	}

	[PacketHandler(Opcode.SMSG_UPDATE_AURA_DURATION)]
	private void HandleUpdateAuraDuration(WorldPacket packet)
	{
		var slot = packet.ReadUInt8();
		var duration = packet.ReadInt32();
		var guid = GetSession().GameState.CurrentPlayerGuid;
		if (guid == null)
		{
			return;
		}
		GetSession().GameState.StoreAuraDurationLeft(guid, slot, duration, (int)packet.GetReceivedTime());
		if (duration <= 0)
		{
			return;
		}
		var updateFields = GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
		if (updateFields != null)
		{
			var aura = new AuraInfo
			{
				Slot = slot,
				AuraData = ReadAuraSlot(slot, guid, updateFields)
			};
			if (aura.AuraData != null)
			{
				aura.AuraData.Flags |= AuraFlagsModern.Duration;
				aura.AuraData.Duration = duration;
				aura.AuraData.Remaining = duration;
				var update = new AuraUpdate(guid, all: false);
				update.Auras.Add(aura);
				SendPacketToClient(update);
			}
		}
	}

	[PacketHandler(Opcode.SMSG_SET_EXTRA_AURA_INFO)]
	[PacketHandler(Opcode.SMSG_SET_EXTRA_AURA_INFO_NEED_UPDATE)]
	private void HandleSetExtraAuraInfo(WorldPacket packet)
	{
		var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
		if (!packet.CanRead())
		{
			return;
		}
		var slot = packet.ReadUInt8();
		var spellId = packet.ReadUInt32();
		var durationFull = packet.ReadInt32();
		var durationLeft = packet.ReadInt32();
		GetSession().GameState.StoreAuraDurationFull(guid, slot, durationFull);
		GetSession().GameState.StoreAuraDurationLeft(guid, slot, durationLeft, (int)packet.GetReceivedTime());
		if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_SET_EXTRA_AURA_INFO_NEED_UPDATE)
		{
			GetSession().GameState.StoreAuraCaster(guid, slot, GetSession().GameState.CurrentPlayerGuid);
		}
		if (durationFull <= 0 && durationLeft <= 0)
		{
			return;
		}
		var updateFields = GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
		if (updateFields != null)
		{
			var aura = new AuraInfo
			{
				Slot = slot,
				AuraData = ReadAuraSlot(slot, guid, updateFields)
			};
			if (aura.AuraData != null && aura.AuraData.SpellID == spellId)
			{
				aura.AuraData.CastUnit = GetSession().GameState.GetAuraCaster(guid, slot, spellId);
				aura.AuraData.Flags |= AuraFlagsModern.Duration;
				aura.AuraData.Duration = durationFull;
				aura.AuraData.Remaining = durationLeft;
				var update = new AuraUpdate(guid, all: false);
				update.Auras.Add(aura);
				SendPacketToClient(update);
			}
		}
	}

	[PacketHandler(Opcode.SMSG_AURA_UPDATE)]
	private void HandleAuraUpdate(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
			var update = new AuraUpdate(guid, all: false);
			ReadSingleAura(packet, guid, update);
			if (update.Auras.Count > 0)
			{
					SendPacketToClient(update);
			}
		}
	}

	[PacketHandler(Opcode.SMSG_AURA_UPDATE_ALL)]
	private void HandleAuraUpdateAll(WorldPacket packet)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
			var update = new AuraUpdate(guid, all: true);
			while (packet.CanRead())
			{
				ReadSingleAura(packet, guid, update);
			}
			if (update.Auras.Count > 0)
			{
				SendPacketToClient(update);
			}
		}
	}

	private void ReadSingleAura(WorldPacket packet, WowGuid128 guid, AuraUpdate update)
	{
		var slot = packet.ReadUInt8();
		var spellId = packet.ReadUInt32();
		var aura = new AuraInfo
		{
			Slot = slot
		};
		if (spellId == 0)
		{
			aura.AuraData = null;
			update.Auras.Add(aura);
			if (guid == GetSession().GameState.CurrentPlayerGuid)
				Log.Print(LogType.Debug, $"[AuraUpdate] REMOVE slot={slot} for player", "");
			return;
		}
		if (guid == GetSession().GameState.CurrentPlayerGuid)
			Log.Print(LogType.Debug, $"[AuraUpdate] SET slot={slot} spellId={spellId} for player", "");
		var data = new AuraDataInfo
		{
			SpellID = spellId,
			CastID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Aura, GetSession().GameState.CurrentMapId.Value, spellId, guid.GetCounter()),
			SpellXSpellVisualID = GameData.GetSpellVisual(spellId)
		};
		var flags = packet.ReadUInt8();
		data.CastLevel = packet.ReadUInt8();
		data.Applications = packet.ReadUInt8();
		data.Flags = AuraFlagsModern.None;
		data.ActiveFlags = 0u;
		if ((flags & 0x10) != 0)
		{
			data.Flags |= AuraFlagsModern.Positive;
		}
		if ((flags & 0x20) != 0)
		{
			data.Flags |= AuraFlagsModern.Duration;
		}
		if ((flags & 1) != 0)
		{
			data.ActiveFlags |= 1u;
		}
		if ((flags & 2) != 0)
		{
			data.ActiveFlags |= 2u;
		}
		if ((flags & 4) != 0)
		{
			data.ActiveFlags |= 4u;
		}
		if ((flags & 8) == 0)
		{
			data.CastUnit = packet.ReadPackedGuid().To128(GetSession().GameState);
		}
		else
		{
			data.CastUnit = guid;
		}
		if ((flags & 0x20) != 0)
		{
			data.Duration = packet.ReadInt32();
			data.Remaining = packet.ReadInt32();
		}
		if ((flags & 0x40) != 0)
		{
			if ((flags & 1) != 0)
			{
				data.Points.Add(packet.ReadFloat());
			}
			if ((flags & 2) != 0)
			{
				data.Points.Add(packet.ReadFloat());
			}
			if ((flags & 4) != 0)
			{
				data.Points.Add(packet.ReadFloat());
			}
		}
		aura.AuraData = data;
		update.Auras.Add(aura);
	}

	[PacketHandler(Opcode.SMSG_RESURRECT_REQUEST)]
	private void HandleResurrectRequest(WorldPacket packet)
	{
		var revive = new ResurrectRequest
		{
			CasterGUID = packet.ReadGuid().To128(GetSession().GameState),
			CasterVirtualRealmAddress = GetSession().RealmId.GetAddress()
		};
		packet.ReadUInt32();
		revive.Name = packet.ReadCString();
		revive.Sickness = packet.ReadBool();
		revive.UseTimer = packet.ReadBool();
		SendPacketToClient(revive);
	}

	[PacketHandler(Opcode.SMSG_TOTEM_CREATED)]
	private void HandleTotemCreated(WorldPacket packet)
	{
		var totem = new TotemCreated
		{
			Slot = packet.ReadUInt8(),
			Totem = packet.ReadGuid().To128(GetSession().GameState),
			Duration = packet.ReadUInt32(),
			SpellId = packet.ReadUInt32()
		};
		SendPacketToClient(totem);
	}

	[PacketHandler(Opcode.SMSG_SET_FLAT_SPELL_MODIFIER)]
	[PacketHandler(Opcode.SMSG_SET_PCT_SPELL_MODIFIER)]
	private void HandleSetSpellModifier(WorldPacket packet)
	{
		var classIndex = packet.ReadUInt8();
		var modIndex = packet.ReadUInt8();
		var modValue = packet.ReadInt32();
		if (GetSession().GameState.CurrentPlayerCreateTime != 0)
		{
			var spell = new SetSpellModifier(packet.GetUniversalOpcode(isModern: false));
			var mod = new SpellModifierInfo();
			var data = new SpellModifierData
			{
				ClassIndex = classIndex
			};
			mod.ModIndex = modIndex;
			data.ModifierValue = modValue;
			mod.ModifierData.Add(data);
			spell.Modifiers.Add(mod);
			SendPacketToClient(spell);
		}
		if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_SET_FLAT_SPELL_MODIFIER)
		{
			GetSession().GameState.SetFlatSpellMod(modIndex, classIndex, modValue);
		}
		else
		{
			GetSession().GameState.SetPctSpellMod(modIndex, classIndex, modValue);
		}
	}

	[PacketHandler(Opcode.SMSG_GM_TICKET_CREATE)]
	private void HandleGmTicketCreate(WorldPacket packet)
	{
		var response = (LegacyGmTicketResponse)packet.ReadUInt32();
		var flag = ((response == LegacyGmTicketResponse.CreateSuccess || response == LegacyGmTicketResponse.UpdateSuccess) ? true : false);
		var isError = !flag;
		Session.SendHermesTextMessage($"GM Ticket Status: {response}", isError);
	}

	[PacketHandler(Opcode.SMSG_FEATURE_SYSTEM_STATUS)]
	private void HandleFeatureSystemStatus(WorldPacket packet)
	{
		GetSession().RealmSocket.SendFeatureSystemStatus();
	}

	[PacketHandler(Opcode.SMSG_MOTD)]
	private void HandleMotd(WorldPacket packet)
	{
		var motd = new MOTD();
		var count = packet.ReadUInt32();
		for (var i = 0u; i < count; i++)
		{
			motd.Text.Add(packet.ReadCString());
		}
		SendPacketToClient(motd);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			GetSession().RealmSocket.SendSetTimeZoneInformation();
			GetSession().RealmSocket.SendSeasonInfo();
		}
	}

	[PacketHandler(Opcode.SMSG_TAXI_NODE_STATUS)]
	private void HandleTaxiNodeStatus(WorldPacket packet)
	{
		var taxi = new TaxiNodeStatusPkt
		{
			FlightMaster = packet.ReadGuid().To128(GetSession().GameState)
		};
		var learned = packet.ReadBool();
		taxi.Status = (learned ? TaxiNodeStatus.Learned : TaxiNodeStatus.Unlearned);
		SendPacketToClient(taxi);
	}

	[PacketHandler(Opcode.SMSG_SHOW_TAXI_NODES)]
	private void HandleShowTaxiNodes(WorldPacket packet)
	{
		var playerFlags = GetSession().GameState.GetLegacyFieldValueUInt32(GetSession().GameState.CurrentPlayerGuid, PlayerField.PLAYER_FLAGS);
		if (playerFlags.HasAnyFlag(PlayerFlags.GM))
		{
			var chat = new ChatPkt(GetSession(), ChatMessageTypeModern.System, "Disable GM mode before talking to taxi master or your game will freeze.");
			SendPacketToClient(chat);
			return;
		}
		var taxi = new ShowTaxiNodes();
		if (packet.ReadUInt32() != 0)
		{
			taxi.WindowInfo = new ShowTaxiNodesWindowInfo
			{
				UnitGUID = packet.ReadGuid().To128(GetSession().GameState),
				CurrentNode = (GetSession().GameState.CurrentTaxiNode = packet.ReadUInt32())
			};
		}
		while (packet.CanRead())
		{
			var nodesMask = packet.ReadUInt8();
			taxi.CanLandNodes.Add(nodesMask);
			taxi.CanUseNodes.Add(nodesMask);
		}
		GetSession().GameState.UsableTaxiNodes = taxi.CanUseNodes;
		SendPacketToClient(taxi);
	}

	[PacketHandler(Opcode.SMSG_NEW_TAXI_PATH)]
	private void HandleNewTaxiPath(WorldPacket packet)
	{
		var taxi = new NewTaxiPath();
		SendPacketToClient(taxi);
	}

	[PacketHandler(Opcode.SMSG_ACTIVATE_TAXI_REPLY)]
	private void HandleActivateTaxiReply(WorldPacket packet)
	{
		var reply = (ActivateTaxiReply)packet.ReadUInt32();
		if (reply != ActivateTaxiReply.Ok)
		{
			var taxi = new ActivateTaxiReplyPkt
			{
				Reply = reply
			};
			SendPacketToClient(taxi);
			GetSession().GameState.IsWaitingForTaxiStart = false;
		}
	}

	[PacketHandler(Opcode.SMSG_TRADE_STATUS)]
	private void HandleTradeStatus(WorldPacket packet)
	{
		var trade = new TradeStatusPkt
		{
			Status = (TradeStatus)packet.ReadUInt32()
		};
		var tradeSession = GetSession().GameState.CurrentTrade;
		if (tradeSession == null)
		{
			var status = trade.Status;
			var tradeStatus = status;
			if ((uint)(tradeStatus - 1) > 1u)
			{
				Log.Print(LogType.Error, $"Got SMSG_TRADE_STATUS without trade session (status: {trade.Status})", "TradeHandler.cs");
				SendPacketToClient(new TradeStatusPkt
				{
					Status = TradeStatus.Cancelled
				});
				return;
			}
			tradeSession = new TradeSession();
			GetSession().GameState.CurrentTrade = tradeSession;
		}
		switch (trade.Status)
		{
		case TradeStatus.Proposed:
			trade.Partner = (tradeSession.Partner = packet.ReadGuid().To128(GetSession().GameState));
			trade.PartnerAccount = (tradeSession.PartnerAccount = GetSession().GetGameAccountGuidForPlayer(trade.Partner));
			break;
		case TradeStatus.Initiated:
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				trade.Id = packet.ReadUInt32();
			}
			else
			{
				trade.Id = TradeSession.GlobalTradeIdCounter++;
			}
			tradeSession.TradeId = trade.Id;
			break;
		case TradeStatus.Failed:
			trade.BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32());
			trade.FailureForYou = packet.ReadBool();
			trade.ItemID = packet.ReadUInt32();
			break;
		case TradeStatus.WrongRealm:
		case TradeStatus.NotOnTaplist:
			trade.TradeSlot = packet.ReadUInt8();
			break;
		}
		bool flag;
		switch (trade.Status)
		{
		case TradeStatus.Proposed:
		case TradeStatus.Initiated:
		case TradeStatus.Accepted:
		case TradeStatus.Unaccepted:
		case TradeStatus.StateChanged:
		case TradeStatus.WrongRealm:
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (!flag)
		{
			GetSession().GameState.CurrentTrade = null;
		}
		SendPacketToClient(trade);
	}

	[PacketHandler(Opcode.SMSG_TRADE_STATUS_EXTENDED)]
	private void HandleTradeStatusExtended(WorldPacket packet)
	{
		var tradeSession = GetSession().GameState.CurrentTrade;
		if (tradeSession == null)
		{
			Log.Print(LogType.Error, "Got SMSG_TRADE_STATUS_EXTENDED without trade session", "TradeHandler.cs");
			return;
		}
		tradeSession.ServerStateIndex++;
		var trade = new TradeUpdated
		{
			WhichPlayer = packet.ReadUInt8()
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var actualTradeId = packet.ReadUInt32();
			if (actualTradeId != trade.Id)
			{
				Log.Print(LogType.Error, $"Got SMSG_TRADE_STATUS_EXTENDED with wrong tradeId (expected {trade.Id} but got {actualTradeId})", "TradeHandler.cs");
				return;
			}
		}
		trade.Id = tradeSession.TradeId;
		packet.ReadUInt32();
		packet.ReadUInt32();
		trade.ClientStateIndex = tradeSession.ClientStateIndex;
		trade.CurrentStateIndex = tradeSession.ServerStateIndex;
		trade.Gold = packet.ReadUInt32();
		trade.ProposedEnchantment = packet.ReadInt32();
		while (packet.CanRead())
		{
			var item = new TradeUpdated.TradeItem
			{
				Unwrapped = new TradeUpdated.UnwrappedTradeItem(),
				Slot = packet.ReadUInt8(),
				Item =
				{
					ItemID = packet.ReadUInt32()
				}
			};
			packet.ReadUInt32();
			item.StackCount = packet.ReadInt32();
			packet.ReadUInt32();
			item.GiftCreator = packet.ReadGuid().To128(GetSession().GameState);
			item.Unwrapped.EnchantID = packet.ReadInt32();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				for (var i = 0; i < 3; i++)
				{
					packet.ReadUInt32();
				}
			}
			item.Unwrapped.Creator = packet.ReadGuid().To128(GetSession().GameState);
			item.Unwrapped.Charges = packet.ReadInt32();
			item.Item.RandomPropertiesSeed = packet.ReadUInt32();
			item.Item.RandomPropertiesID = packet.ReadUInt32();
			item.Unwrapped.Lock = packet.ReadUInt32() != 0;
			item.Unwrapped.MaxDurability = packet.ReadUInt32();
			item.Unwrapped.Durability = packet.ReadUInt32();
			trade.Items.Add(item);
		}
		SendPacketToClient(trade);
	}

	[PacketHandler(Opcode.SMSG_DESTROY_OBJECT)]
	private void HandleDestroyObject(WorldPacket packet)
	{
		var guid = packet.ReadGuid().To128(GetSession().GameState);
		Log.Print(LogType.Debug, $"[DestroyObject] Destroying {guid} type={guid.GetHighType()}", "");
		GetSession().GameState.ObjectCacheMutex.WaitOne();
		GetSession().GameState.ObjectCacheLegacy.Remove(guid);
		GetSession().GameState.ObjectCacheModern.Remove(guid);
		GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
		GetSession().GameState.LastAuraCasterOnTarget.Remove(guid);
		// Send both DestroyObject (for 3.4.3 GO cleanup) and UpdateObject (for compatibility)
		if (ModernVersion.GetCurrentOpcode(Opcode.SMSG_DESTROY_OBJECT) != 0)
		{
			SendPacketToClient(new DestroyObject(guid));
		}
		var updateObject = new UpdateObject(GetSession().GameState);
		updateObject.DestroyedGuids.Add(guid);
		SendPacketToClient(updateObject);
	}

	[PacketHandler(Opcode.SMSG_COMPRESSED_UPDATE_OBJECT)]
	private void HandleCompressedUpdateObject(WorldPacket packet)
	{
		using var packet2 = packet.Inflate(packet.ReadInt32());
		HandleUpdateObject(packet2);
	}

	[PacketHandler(Opcode.SMSG_UPDATE_OBJECT)]
	private void HandleUpdateObject(WorldPacket packet)
	{
		var count = packet.ReadUInt32();
		PrintString($"Updates Count = {count}");
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.ReadBool();
		}
		var missingItemTemplates = new HashSet<uint>();
		var auraUpdates = new List<AuraUpdate>();
		var updateObject = new UpdateObject(GetSession().GameState);
		for (var i = 0; i < count; i++)
		{
			var type = (UpdateTypeLegacy)packet.ReadUInt8();
			PrintString($"Update Type = {type}", i);
			switch (type)
			{
			case UpdateTypeLegacy.Values:
			{
				var guid3 = packet.ReadPackedGuid().To128(GetSession().GameState);
				PrintString("Guid = " + guid3, i);
				var updateData2 = new ObjectUpdate(guid3, UpdateTypeModern.Values, GetSession());
				var auraUpdate2 = new AuraUpdate(guid3, all: false);
				var powerUpdate = new PowerUpdate(guid3);
				ReadValuesUpdateBlock(packet, guid3, updateData2, auraUpdate2, powerUpdate, i);
				if (powerUpdate.Powers.Count != 0)
				{
					SendPacketToClient(powerUpdate);
				}
				if (guid3 == GetSession().GameState.CurrentPlayerGuid)
				{
					// 3.4.3 client is unstable when legacy player Values updates carry
					// Object/Player/ActivePlayer sections (loot/inventory/quest state churn).
					// Keep only Unit/Aura/Power data for self updates.
					updateData2.ObjectData = null;
					updateData2.PlayerData = null;
					updateData2.ActivePlayerData = null;
				}
				// DestroyObject + CreateObject2 on revive: ghost→alive transition
				if (guid3 == GetSession().GameState.CurrentPlayerGuid && GetSession().GameState.NeedPlayerRecreate)
				{
					GetSession().GameState.NeedPlayerRecreate = false;
					Log.Print(LogType.Debug, "[DeathRevive] Performing DestroyObject + CreateObject2 for revive", "");

					// 1. Send DestroyObject to remove stale ghost-flagged player from client
					var destroyPacket = new UpdateObject(GetSession().GameState);
					destroyPacket.DestroyedGuids.Add(guid3);
					SendPacketToClient(destroyPacket);

					// 2. Clear modern cache so CreateObject2 starts fresh
					GetSession().GameState.ObjectCacheMutex.WaitOne();
					GetSession().GameState.ObjectCacheModern.Remove(guid3);
					GetSession().GameState.ObjectCacheMutex.ReleaseMutex();

					// 3. Build full CreateObject2 from cached legacy fields
					var cachedFields = GetSession().GameState.GetCachedObjectFieldsLegacy(guid3);
					if (cachedFields != null && GetSession().GameState.LastSelfPlayerMoveInfo != null)
					{
						var createUpdate = new ObjectUpdate(guid3, UpdateTypeModern.CreateObject2, GetSession())
							{
								CreateData =
								{
									ObjectType = ObjectType.ActivePlayer,
									ThisIsYou = true,
									MoveInfo = GetSession().GameState.LastSelfPlayerMoveInfo.CopyFromMe()
								}
							};

						// Build full update mask from all cached fields
						var maxKey = 0;
						foreach (var key in cachedFields.Keys)
							if (key > maxKey) maxKey = key;
						var fullMask = new BitArray(maxKey + 1, false);
						foreach (var key in cachedFields.Keys)
							fullMask.Set(key, true);

						// Populate all field values via StoreObjectUpdate
						var createAuraUpdate = new AuraUpdate(guid3, all: true);
						StoreObjectUpdate(guid3, ObjectType.ActivePlayer, fullMask, cachedFields, createAuraUpdate, null, true, createUpdate, fullMask);

						// Also apply completed quests
						GetSession().GameState.CurrentPlayerStorage.CompletedQuests.WriteAllCompletedIntoArray(createUpdate.ActivePlayerData.QuestCompleted);

						updateObject.ObjectUpdates.Add(createUpdate);
						if (createAuraUpdate.Auras.Count != 0)
						{
							auraUpdates.Add(createAuraUpdate);
						}
						Log.Print(LogType.Debug, $"[DeathRevive] Created full CreateObject2 with {cachedFields.Count} fields, pos={createUpdate.CreateData.MoveInfo.Position}", "");
					}
					else
					{
						Log.Print(LogType.Error, "[DeathRevive] Cannot recreate player — missing cached fields or MoveInfo, sending Values update instead", "");
						// Fall through to normal Values update as fallback
						goto normalValuesUpdate;
					}
					// Send any aura updates and skip normal Values path
					if (auraUpdate2.Auras.Count != 0)
					{
						auraUpdates.Add(auraUpdate2);
					}
					break;
				}
				normalValuesUpdate:
				if (guid3 == GetSession().GameState.CurrentPlayerGuid)
				{
					// 3.4.3 client can disconnect/crash on player Values packets containing
					// Object/Player/ActivePlayer blocks or Unit blocks 0+1 fields.
					// Keep only Unit power fields (block 4), which are known-safe.
					updateData2.ObjectData = null;
					updateData2.PlayerData = null;
					updateData2.ActivePlayerData = null;

					if (updateData2.UnitData != null)
					{
						var sanitizedUnitData = new UnitData();
						if (updateData2.UnitData.Power != null)
						{
							sanitizedUnitData.Power = (int?[])updateData2.UnitData.Power.Clone();
						}
						if (updateData2.UnitData.MaxPower != null)
						{
							sanitizedUnitData.MaxPower = (int?[])updateData2.UnitData.MaxPower.Clone();
						}
						updateData2.UnitData = sanitizedUnitData;
					}
				}

				// Check if the update has any actual data to send.
				// Empty Values updates (changedMask=0) crash the 3.4.3 client.
				var hasAnythingToSend = false;
				if (updateData2.ObjectData != null && (updateData2.ObjectData.EntryID.HasValue || updateData2.ObjectData.DynamicFlags.HasValue || updateData2.ObjectData.Scale.HasValue))
					hasAnythingToSend = true;
				if (updateData2.UnitData != null && (updateData2.UnitData.Health.HasValue || updateData2.UnitData.MaxHealth.HasValue ||
					updateData2.UnitData.DisplayID.HasValue || updateData2.UnitData.Target != null ||
					updateData2.UnitData.Flags.HasValue || updateData2.UnitData.Flags2.HasValue ||
					updateData2.UnitData.Level.HasValue || updateData2.UnitData.FactionTemplate.HasValue ||
					updateData2.UnitData.AuraState.HasValue || updateData2.UnitData.NativeDisplayID.HasValue))
					hasAnythingToSend = true;
				if (updateData2.UnitData != null && updateData2.UnitData.Power != null)
					for (var p = 0; p < updateData2.UnitData.Power.Length; p++)
						if (updateData2.UnitData.Power[p].HasValue) { hasAnythingToSend = true; break; }
				if (updateData2.UnitData != null && updateData2.UnitData.MaxPower != null)
					for (var p = 0; p < updateData2.UnitData.MaxPower.Length; p++)
						if (updateData2.UnitData.MaxPower[p].HasValue) { hasAnythingToSend = true; break; }
				// Check stat/resistance/combat fields
				if (updateData2.UnitData != null)
				{
					var u = updateData2.UnitData;
					if (u.AttackPower.HasValue || u.RangedAttackPower.HasValue ||
						u.AttackPowerModPos.HasValue || u.AttackPowerModNeg.HasValue ||
						u.ShapeshiftForm.HasValue || u.BaseMana.HasValue || u.BaseHealth.HasValue ||
						u.EmoteState.HasValue || u.SheatheState.HasValue ||
						u.ModCastSpeed.HasValue || u.ModCastHaste.HasValue ||
						u.MinDamage.HasValue || u.MaxDamage.HasValue ||
						u.MountDisplayID.HasValue || u.GuildGUID != null)
						hasAnythingToSend = true;
					if (u.Stats != null)
						for (var s = 0; s < u.Stats.Length; s++)
							if (u.Stats[s].HasValue) { hasAnythingToSend = true; break; }
					if (u.Resistances != null)
						for (var r = 0; r < 7; r++)
							if (u.Resistances[r].HasValue) { hasAnythingToSend = true; break; }
					if (u.ResistanceBuffModsPositive != null)
						for (var r = 0; r < 7; r++)
							if (u.ResistanceBuffModsPositive[r].HasValue) { hasAnythingToSend = true; break; }
					if (u.ResistanceBuffModsNegative != null)
						for (var r = 0; r < 7; r++)
							if (u.ResistanceBuffModsNegative[r].HasValue) { hasAnythingToSend = true; break; }
				}
				// Skip Item-only Values updates - sends corrupt data that breaks client state
				if (guid3.IsItem())
					hasAnythingToSend = false;
				if (updateData2.ActivePlayerData != null)
				{
					var a = updateData2.ActivePlayerData;
					if (a.Coinage.HasValue || a.XP.HasValue || a.NextLevelXP.HasValue)
						hasAnythingToSend = true;
					if (a.InvSlots != null)
						for (var s = 0; s < a.InvSlots.Length; s++)
							if (a.InvSlots[s] != null) { hasAnythingToSend = true; break; }
					if (a.PackSlots != null)
						for (var s = 0; s < a.PackSlots.Length; s++)
							if (a.PackSlots[s] != null) { hasAnythingToSend = true; break; }
				}
				if (updateData2.PlayerData != null)
				{
					var pd = updateData2.PlayerData;
					if (pd.PlayerFlags.HasValue || pd.PlayerFlagsEx.HasValue || pd.ChosenTitle.HasValue || pd.GuildTimeStamp.HasValue)
						hasAnythingToSend = true;
					if (pd.QuestLog != null)
						for (var q = 0; q < pd.QuestLog.Length; q++)
							if (pd.QuestLog[q] != null && pd.QuestLog[q].QuestID.HasValue) { hasAnythingToSend = true; break; }
					if (pd.VisibleItems != null)
						for (var v = 0; v < pd.VisibleItems.Length; v++)
							if (pd.VisibleItems[v] != null) { hasAnythingToSend = true; break; }
				}
				if (hasAnythingToSend)
				{
					// Debug: log stat/resistance updates for the player
					if (guid3 == GetSession().GameState.CurrentPlayerGuid && updateData2.UnitData != null)
					{
						var u = updateData2.UnitData;
						var statInfo = "";
						if (u.Stats != null)
							for (var si = 0; si < u.Stats.Length; si++)
								if (u.Stats[si].HasValue) statInfo += $" Stat{si}={u.Stats[si].Value}";
						if (u.Resistances != null)
							for (var ri = 0; ri < 7; ri++)
								if (u.Resistances[ri].HasValue) statInfo += $" Res{ri}={u.Resistances[ri].Value}";
						if (u.AttackPower.HasValue) statInfo += $" AP={u.AttackPower.Value}";
						if (u.BaseMana.HasValue) statInfo += $" BaseMana={u.BaseMana.Value}";
						if (u.BaseHealth.HasValue) statInfo += $" BaseHP={u.BaseHealth.Value}";
						if (statInfo.Length > 0)
							Log.Print(LogType.Debug, $"[PlayerUpdate] SENDING stats:{statInfo}", "");
						if (updateData2.PlayerData?.VisibleItems != null)
						{
							var visInfo = "";
							for (var vi = 0; vi < updateData2.PlayerData.VisibleItems.Length; vi++)
								if (updateData2.PlayerData.VisibleItems[vi] != null)
									visInfo += $" Slot{vi}=ItemID:{updateData2.PlayerData.VisibleItems[vi].ItemID}";
							if (visInfo.Length > 0)
								Log.Print(LogType.Debug, $"[PlayerUpdate] SENDING visItems:{visInfo}", "");
						}
					}
					updateObject.ObjectUpdates.Add(updateData2);
				}
				else if (guid3 == GetSession().GameState.CurrentPlayerGuid)
				{
					Log.Print(LogType.Debug, "[PlayerUpdate] DROPPED - no sendable data", "");
				}
				if (auraUpdate2.Auras.Count != 0)
				{
					auraUpdates.Add(auraUpdate2);
				}
				break;
			}
			case UpdateTypeLegacy.Movement:
			{
				var guid2 = (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901) ? packet.ReadPackedGuid() : packet.ReadGuid());
				PrintString("Guid = " + guid2, i);
				ReadMovementUpdateBlock(packet, guid2, null, i);
				break;
			}
			case UpdateTypeLegacy.CreateObject1:
			{
				var oldGuid2 = packet.ReadPackedGuid();
				if (oldGuid2.GetHighType() == HighGuidType.Creature || oldGuid2.GetHighType() == HighGuidType.GameObject)
				{
					if (!GetSession().GameState.ObjectSpawnCount.ContainsKey(oldGuid2))
					{
						GetSession().GameState.ObjectSpawnCount.Add(oldGuid2, 0);
					}
					else if (oldGuid2.GetHighType() == HighGuidType.GameObject && GetSession().GameState.DespawnedGameObjects.Contains(oldGuid2))
					{
						GetSession().GameState.IncrementObjectSpawnCounter(oldGuid2);
					}
				}
				var guid4 = oldGuid2.To128(GetSession().GameState);
				PrintString("Guid = " + guid4, i);
				if (guid4 == GetSession().GameState.CurrentPlayerGuid && GetSession().GameState.IsInFarSight)
				{
					var updateObject2 = new UpdateObject(GetSession().GameState);
					var updateData3 = new ObjectUpdate(guid4, UpdateTypeModern.Values, GetSession())
						{
							ActivePlayerData =
							{
								FarsightObject = WowGuid128.Empty
							}
						};
					updateObject2.ObjectUpdates.Add(updateData3);
					SendPacketToClient(updateObject2);
				}
				var updateData4 = new ObjectUpdate(guid4, UpdateTypeModern.CreateObject1, GetSession());
				var auraUpdate3 = new AuraUpdate(guid4, all: true);
				ReadCreateObjectBlock(packet, guid4, updateData4, auraUpdate3, i);
				if (updateData4.Guid == GetSession().GameState.CurrentPlayerGuid)
				{
					GetSession().GameState.CurrentPlayerStorage.CompletedQuests.WriteAllCompletedIntoArray(updateData4.ActivePlayerData.QuestCompleted);
				}
				if (guid4.IsItem() && updateData4.ObjectData.EntryID.HasValue && !GameData.ItemTemplates.ContainsKey((uint)updateData4.ObjectData.EntryID.Value))
				{
					var entryId4 = (uint)updateData4.ObjectData.EntryID.Value;
					missingItemTemplates.Add(entryId4);
					// Buffer this item create until its template arrives via hotfix
					if (!GetSession().GameState.PendingItemCreates.ContainsKey(entryId4))
						GetSession().GameState.PendingItemCreates[entryId4] = new List<ObjectUpdate>();
					GetSession().GameState.PendingItemCreates[entryId4].Add(updateData4);
					Log.Print(LogType.Debug, $"Buffering item CreateObject {guid4} entry={entryId4} until template arrives.", "");
				}
				else if (updateData4.CreateData.MoveInfo != null || !guid4.IsWorldObject())
				{
					updateObject.ObjectUpdates.Add(updateData4);
					if (auraUpdate3.Auras.Count != 0)
					{
						auraUpdates.Add(auraUpdate3);
					}
				}
				else
				{
					Log.Print(LogType.Error, $"Broken create1 without position for {guid4}", "UpdateHandler.cs");
				}
				break;
			}
			case UpdateTypeLegacy.CreateObject2:
			{
				var oldGuid = packet.ReadPackedGuid();
				if (oldGuid.GetHighType() == HighGuidType.Creature || oldGuid.GetHighType() == HighGuidType.GameObject)
				{
					GetSession().GameState.IncrementObjectSpawnCounter(oldGuid);
				}
				var guid = oldGuid.To128(GetSession().GameState);
				PrintString("Guid = " + guid, i);
				// In 3.4.3, CreateObject2 is ONLY for the self-player.
				// Legacy 3.3.5a uses CreateObject2 for all nearby objects, so downgrade non-self objects to CreateObject1.
				var createType = (guid == GetSession().GameState.CurrentPlayerGuid)
					? UpdateTypeModern.CreateObject2
					: UpdateTypeModern.CreateObject1;
				var updateData = new ObjectUpdate(guid, createType, GetSession());
				var auraUpdate = new AuraUpdate(guid, all: true);
				ReadCreateObjectBlock(packet, guid, updateData, auraUpdate, i);
				// Cache MoveInfo for self player — needed for DestroyObject+CreateObject2 on revive
				if (guid == GetSession().GameState.CurrentPlayerGuid && updateData.CreateData?.MoveInfo != null)
				{
					GetSession().GameState.LastSelfPlayerMoveInfo = updateData.CreateData.MoveInfo.CopyFromMe();
					Log.Print(LogType.Debug, $"[DeathRevive] Cached self player MoveInfo pos={updateData.CreateData.MoveInfo.Position}", "");
				}
				if (guid.IsItem() && updateData.ObjectData.EntryID.HasValue && !GameData.ItemTemplates.ContainsKey((uint)updateData.ObjectData.EntryID.Value))
				{
					var entryId2 = (uint)updateData.ObjectData.EntryID.Value;
					missingItemTemplates.Add(entryId2);
					// Buffer this item create until its template arrives via hotfix
					if (!GetSession().GameState.PendingItemCreates.ContainsKey(entryId2))
						GetSession().GameState.PendingItemCreates[entryId2] = new List<ObjectUpdate>();
					GetSession().GameState.PendingItemCreates[entryId2].Add(updateData);
					Log.Print(LogType.Debug, $"Buffering item CreateObject {guid} entry={entryId2} until template arrives.", "");
				}
				else if (updateData.CreateData.MoveInfo != null || !guid.IsWorldObject())
				{
					updateObject.ObjectUpdates.Add(updateData);
					if (auraUpdate.Auras.Count != 0)
					{
						auraUpdates.Add(auraUpdate);
					}
				}
				else
				{
					Log.Print(LogType.Error, $"Broken create2 without position for {guid}", "UpdateHandler.cs");
				}
				break;
			}
			case UpdateTypeLegacy.NearObjects:
				ReadNearObjectsBlock(packet, i);
				break;
			case UpdateTypeLegacy.FarObjects:
				ReadFarObjectsBlock(packet, updateObject, i);
				break;
			}
		}
		if (updateObject.ObjectUpdates.Count == 0 && GetSession().GameState.IsWaitingForNewWorld)
		{
			return;
		}
		foreach (var itemId in missingItemTemplates)
		{
			var packet2 = new WorldPacket(Opcode.CMSG_ITEM_QUERY_SINGLE);
			packet2.WriteUInt32(itemId);
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				packet2.WriteGuid(WowGuid64.Empty);
			}
			SendPacketToServer(packet2);
		}
		var activePlayerUpdateIndex = -1;
		for (var j = 0; j < updateObject.ObjectUpdates.Count; j++)
		{
			if (updateObject.ObjectUpdates[j].CreateData != null && updateObject.ObjectUpdates[j].CreateData.ThisIsYou)
			{
				activePlayerUpdateIndex = j;
				break;
			}
		}
		if (activePlayerUpdateIndex >= 0)
		{
			if (GetSession().GameState.FlatSpellMods.Count > 0)
			{
				var spell = new SetSpellModifier(Opcode.SMSG_SET_FLAT_SPELL_MODIFIER);
				foreach (var modItr in GetSession().GameState.FlatSpellMods)
				{
					var mod = new SpellModifierInfo
					{
						ModIndex = modItr.Key
					};
					foreach (var dataItr in modItr.Value)
					{
						var data = new SpellModifierData
						{
							ClassIndex = dataItr.Key,
							ModifierValue = dataItr.Value
						};
						mod.ModifierData.Add(data);
					}
					spell.Modifiers.Add(mod);
				}
				SendPacketToClient(spell);
			}
			if (GetSession().GameState.PctSpellMods.Count > 0)
			{
				var spell2 = new SetSpellModifier(Opcode.SMSG_SET_PCT_SPELL_MODIFIER);
				foreach (var modItr2 in GetSession().GameState.PctSpellMods)
				{
					var mod2 = new SpellModifierInfo
					{
						ModIndex = modItr2.Key
					};
					foreach (var dataItr2 in modItr2.Value)
					{
						var data2 = new SpellModifierData
						{
							ClassIndex = dataItr2.Key,
							ModifierValue = dataItr2.Value
						};
						mod2.ModifierData.Add(data2);
					}
					spell2.Modifiers.Add(mod2);
				}
				SendPacketToClient(spell2);
			}
		}
		if (activePlayerUpdateIndex > 0)
		{
			var tmp = updateObject.ObjectUpdates[0];
			updateObject.ObjectUpdates[0] = updateObject.ObjectUpdates[activePlayerUpdateIndex];
			updateObject.ObjectUpdates[activePlayerUpdateIndex] = tmp;
		}
		if (GetSession().GameState.CurrentMapId == 489)
		{
			var resetBgPlayerPositions = false;
			foreach (var guid5 in updateObject.OutOfRangeGuids)
			{
				if (guid5.IsPlayer() && GetSession().GameState.FlagCarrierGuids.Contains(guid5))
				{
					resetBgPlayerPositions = true;
					break;
				}
			}
			if (resetBgPlayerPositions)
			{
				var bglist = new BattlegroundPlayerPositions();
				SendPacketToClient(bglist);
			}
		}
		// Split player Values updates into a separate packet from creature updates
		var playerGuid = GetSession().GameState.CurrentPlayerGuid;
		var playerValuesUpdates = new List<ObjectUpdate>();
		var otherUpdates = new List<ObjectUpdate>();
		foreach (var upd in updateObject.ObjectUpdates)
		{
			if (upd.Guid == playerGuid && upd.Type == UpdateTypeModern.Values)
				playerValuesUpdates.Add(upd);
			else
				otherUpdates.Add(upd);
		}
		if (otherUpdates.Count != 0 || updateObject.DestroyedGuids.Count != 0 || updateObject.OutOfRangeGuids.Count != 0)
		{
			updateObject.ObjectUpdates.Clear();
			updateObject.ObjectUpdates.AddRange(otherUpdates);
			SendPacketToClient(updateObject);
		}
		if (playerValuesUpdates.Count != 0)
		{
			var playerUpdateObject = new UpdateObject(GetSession().GameState);
			playerUpdateObject.ObjectUpdates.AddRange(playerValuesUpdates);
			SendPacketToClient(playerUpdateObject);
		}
		foreach (var auraUpdate4 in auraUpdates)
		{
			SendPacketToClient(auraUpdate4);
		}
	}

	public void ReadNearObjectsBlock(WorldPacket packet, object index)
	{
		var objCount = packet.ReadInt32();
		PrintString($"NearObjectsCount = {objCount}", index);
		for (var j = 0; j < objCount; j++)
		{
			var guid = packet.ReadPackedGuid();
			PrintString($"Guid = {objCount}", index, j);
		}
	}

	public void ReadFarObjectsBlock(WorldPacket packet, UpdateObject updateObject, object index)
	{
		var objCount = packet.ReadInt32();
		PrintString($"FarObjectsCount = {objCount}", index);
		for (var j = 0; j < objCount; j++)
		{
			var guid = packet.ReadPackedGuid().To128(GetSession().GameState);
			if (!(guid == GetSession().GameState.CurrentPlayerGuid))
			{
				PrintString($"Guid = {objCount}", index, j);
				GetSession().GameState.ObjectCacheMutex.WaitOne();
				GetSession().GameState.ObjectCacheLegacy.Remove(guid);
				GetSession().GameState.ObjectCacheModern.Remove(guid);
				GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
				GetSession().GameState.LastAuraCasterOnTarget.Remove(guid);
				if (GetSession().GameState.CurrentPetGuid == guid)
				{
					var updateObject2 = new UpdateObject(GetSession().GameState);
					var updateData2 = new ObjectUpdate(guid, UpdateTypeModern.Values, GetSession());
					updateObject2.ObjectUpdates.Add(updateData2);
					SendPacketToClient(updateObject2);
				}
				updateObject.OutOfRangeGuids.Add(guid);
			}
		}
	}

	private void ReadCreateObjectBlock(WorldPacket packet, WowGuid128 guid, ObjectUpdate updateData, AuraUpdate auraUpdate, object index)
	{
		updateData.CreateData.ObjectType = ObjectTypeConverter.Convert((ObjectTypeLegacy)packet.ReadUInt8());
		GetSession().GameState.StoreOriginalObjectType(guid, updateData.CreateData.ObjectType);
		ReadMovementUpdateBlock(packet, guid, updateData, index);
		ReadValuesUpdateBlockOnCreate(packet, guid, updateData.CreateData.ObjectType, updateData, auraUpdate, index);
	}

	public void ReadValuesUpdateBlockOnCreate(WorldPacket packet, WowGuid128 guid, ObjectType type, ObjectUpdate updateData, AuraUpdate auraUpdate, object index)
	{
		BitArray updateMaskArray = null;
		BitArray actuallyChangedValuesMaskArray;
		var updates = ReadValuesUpdateBlock(packet, ref type, index, isCreating: true, null, out updateMaskArray, out actuallyChangedValuesMaskArray);
		StoreObjectUpdate(guid, type, updateMaskArray, updates, auraUpdate, null, isCreate: true, updateData, actuallyChangedValuesMaskArray);
		GetSession().GameState.ObjectCacheMutex.WaitOne();
		if (!GetSession().GameState.ObjectCacheLegacy.ContainsKey(guid))
		{
			GetSession().GameState.ObjectCacheLegacy.Add(guid, updates);
		}
		else
		{
			GetSession().GameState.ObjectCacheLegacy[guid] = updates;
		}
		GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
	}

	public void ReadValuesUpdateBlock(WorldPacket packet, WowGuid128 guid, ObjectUpdate updateData, AuraUpdate auraUpdate, PowerUpdate powerUpdate, int index)
	{
		BitArray updateMaskArray = null;
		var type = GetSession().GameState.GetOriginalObjectType(guid);
		BitArray actuallyChangedValuesMaskArray;
		var updates = ReadValuesUpdateBlock(packet, ref type, index, isCreating: false, GetSession().GameState.GetCachedObjectFieldsLegacy(guid), out updateMaskArray, out actuallyChangedValuesMaskArray);
		StoreObjectUpdate(guid, type, updateMaskArray, updates, auraUpdate, powerUpdate, isCreate: false, updateData, actuallyChangedValuesMaskArray);

		// Merge changed fields back into ObjectCacheLegacy so inventory slot
		// GUIDs and other values stay current for subsequent lookups
		// (e.g. HandleItemPushResult reading GetInventorySlotItem).
		GetSession().GameState.ObjectCacheMutex.WaitOne();
		if (GetSession().GameState.ObjectCacheLegacy.TryGetValue(guid, out var cached))
		{
			foreach (var kvp in updates)
				cached[kvp.Key] = kvp.Value;
		}
		else
		{
			GetSession().GameState.ObjectCacheLegacy[guid] = updates;
		}
		GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
	}

	private string GetIndexString(params object[] values)
	{
		var list = values.Flatten();
		return list.Where(value => value != null).Aggregate(string.Empty, delegate(string current, object value)
		{
			var text = ((value is string) ? "()" : "[]");
			return current + text[0] + value + text[1] + " ";
		});
	}

	private void PrintString(string txt, params object[] indexes)
	{
	}

	private T PrintValue<T>(string name, T obj, params object[] indexes)
	{
		return obj;
	}

	private Dictionary<int, UpdateField> ReadValuesUpdateBlock(WorldPacket packet, ref ObjectType type, object index, bool isCreating, Dictionary<int, UpdateField>? oldValues, out BitArray outUpdateMaskArray, out BitArray outActuallyChangedValuesMaskArray)
	{
		var missingCreateObject = !isCreating && oldValues == null;
		var maskSize = packet.ReadUInt8();
		var updateMask = new int[maskSize];
		for (var i = 0; i < maskSize; i++)
		{
			updateMask[i] = packet.ReadInt32();
		}
		var mask = (outUpdateMaskArray = new BitArray(updateMask));
		outActuallyChangedValuesMaskArray = new BitArray(new int[maskSize]);
		var dict = oldValues ?? new Dictionary<int, UpdateField>();
		if (missingCreateObject)
		{
			switch (type)
			{
			case ObjectType.Item:
				if (mask.Count >= LegacyVersion.GetUpdateField(ItemField.ITEM_END) && maskSize == Convert.ToInt32((LegacyVersion.GetUpdateField(ContainerField.CONTAINER_END) + 32) / 32))
				{
					type = ObjectType.Container;
				}
				break;
			case ObjectType.Player:
				if (mask.Count >= LegacyVersion.GetUpdateField(PlayerField.PLAYER_END) && maskSize == Convert.ToInt32((LegacyVersion.GetUpdateField(ActivePlayerField.ACTIVE_PLAYER_END) + 32) / 32))
				{
					type = ObjectType.ActivePlayer;
				}
				break;
			}
		}
		else
		{
			switch (type)
			{
			case ObjectType.Item:
			{
				var ITEM_END = LegacyVersion.GetUpdateField(ItemField.ITEM_END);
				if (mask.Length < ITEM_END)
				{
					mask.Length = ITEM_END;
				}
				break;
			}
			case ObjectType.Container:
			{
				var CONTAINER_END = LegacyVersion.GetUpdateField(ContainerField.CONTAINER_END);
				if (mask.Length < CONTAINER_END)
				{
					mask.Length = CONTAINER_END;
				}
				break;
			}
			case ObjectType.Unit:
			{
				var UNIT_END = LegacyVersion.GetUpdateField(UnitField.UNIT_END);
				if (mask.Length < UNIT_END)
				{
					mask.Length = UNIT_END;
				}
				break;
			}
			case ObjectType.Player:
			{
				var PLAYER_END = LegacyVersion.GetUpdateField(PlayerField.PLAYER_END);
				if (mask.Length < PLAYER_END)
				{
					mask.Length = PLAYER_END;
				}
				break;
			}
			case ObjectType.GameObject:
			{
				var GAMEOBJECT_END = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_END);
				if (mask.Length < GAMEOBJECT_END)
				{
					mask.Length = GAMEOBJECT_END;
				}
				break;
			}
			case ObjectType.DynamicObject:
			{
				var DYNAMICOBJECT_END = LegacyVersion.GetUpdateField(DynamicObjectField.DYNAMICOBJECT_END);
				if (mask.Length < DYNAMICOBJECT_END)
				{
					mask.Length = DYNAMICOBJECT_END;
				}
				break;
			}
			case ObjectType.Corpse:
			{
				var CORPSE_END = LegacyVersion.GetUpdateField(CorpseField.CORPSE_END);
				if (mask.Length < CORPSE_END)
				{
					mask.Length = CORPSE_END;
				}
				break;
			}
			}
		}
		var objectEnd = LegacyVersion.GetUpdateField(ObjectField.OBJECT_END);
		for (var j = 0; j < mask.Count; j++)
		{
			if (!mask[j])
			{
				continue;
			}
			var blockVal = packet.ReadUpdateField();
			var key = "Block Value " + j;
			var value = blockVal.UInt32Value + "/" + blockVal.FloatValue;
			UpdateFieldInfo fieldInfo = null;
			if (j < objectEnd)
			{
				fieldInfo = LegacyVersion.GetUpdateFieldInfo<ObjectField>(j);
			}
			else
			{
				switch (type)
				{
				case ObjectType.Container:
					if (j < LegacyVersion.GetUpdateField(ItemField.ITEM_END))
					{
						goto case ObjectType.Item;
					}
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<ContainerField>(j);
					break;
				case ObjectType.Item:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<ItemField>(j);
					break;
				case ObjectType.AzeriteEmpoweredItem:
					if (j < LegacyVersion.GetUpdateField(ItemField.ITEM_END))
					{
						goto case ObjectType.Item;
					}
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<AzeriteEmpoweredItemField>(j);
					break;
				case ObjectType.AzeriteItem:
					if (j < LegacyVersion.GetUpdateField(ItemField.ITEM_END))
					{
						goto case ObjectType.Item;
					}
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<AzeriteItemField>(j);
					break;
				case ObjectType.Player:
					if (j < LegacyVersion.GetUpdateField(UnitField.UNIT_END) || j < LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_END))
					{
						goto case ObjectType.Unit;
					}
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<PlayerField>(j);
					break;
				case ObjectType.ActivePlayer:
					if (j < LegacyVersion.GetUpdateField(PlayerField.PLAYER_END))
					{
						goto case ObjectType.Player;
					}
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<ActivePlayerField>(j);
					break;
				case ObjectType.Unit:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<UnitField>(j);
					break;
				case ObjectType.GameObject:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<GameObjectField>(j);
					break;
				case ObjectType.DynamicObject:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<DynamicObjectField>(j);
					break;
				case ObjectType.Corpse:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<CorpseField>(j);
					break;
				case ObjectType.AreaTrigger:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<AreaTriggerField>(j);
					break;
				case ObjectType.SceneObject:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<SceneObjectField>(j);
					break;
				case ObjectType.Conversation:
					fieldInfo = LegacyVersion.GetUpdateFieldInfo<ConversationField>(j);
					break;
				}
			}
			var start = j;
			var size = 1;
			var updateFieldType = UpdateFieldType.Default;
			if (fieldInfo != null)
			{
				key = fieldInfo.Name;
				size = fieldInfo.Size;
				start = fieldInfo.Value;
				updateFieldType = fieldInfo.Format;
			}
			var fieldData = new List<UpdateField>();
			for (var k = start; k < j; k++)
			{
				if (oldValues == null || !oldValues.TryGetValue(k, out var updateField))
				{
					updateField = new UpdateField(0);
				}
				fieldData.Add(updateField);
			}
			fieldData.Add(blockVal);
			for (var l = j - start + 1; l < size; l++)
			{
				var currentPosition = ++j;
				UpdateField updateField2;
				if (mask[currentPosition])
				{
					updateField2 = packet.ReadUpdateField();
				}
				else if (oldValues == null || !oldValues.TryGetValue(currentPosition, out updateField2))
				{
					updateField2 = new UpdateField(0);
				}
				fieldData.Add(updateField2);
			}
			switch (updateFieldType)
			{
			case UpdateFieldType.Guid:
			{
				var guidSize = (LegacyVersion.AddedInVersion(ClientVersionBuild.V6_0_2_19033) ? 4 : 2);
				var guidCount = size / guidSize;
				for (var guidI = 0; guidI < guidCount; guidI++)
				{
					var hasGuidValue = false;
					for (var guidPart = 0; guidPart < guidSize; guidPart++)
					{
						if (mask[start + guidI * guidSize + guidPart])
						{
							hasGuidValue = true;
						}
					}
					if (!hasGuidValue)
					{
						continue;
					}
					if (!LegacyVersion.AddedInVersion(ClientVersionBuild.V6_0_2_19033))
					{
						ulong guid = fieldData[guidI * guidSize + 1].UInt32Value;
						guid <<= 32;
						guid |= fieldData[guidI * guidSize].UInt32Value;
						if (!isCreating || guid != 0)
						{
							PrintValue(key + ((guidCount > 1) ? (" + " + guidI) : ""), new WowGuid64(guid), index);
						}
						continue;
					}
					ulong low = fieldData[guidI * guidSize + 1].UInt32Value;
					low <<= 32;
					low |= fieldData[guidI * guidSize].UInt32Value;
					ulong high = fieldData[guidI * guidSize + 3].UInt32Value;
					high <<= 32;
					high |= fieldData[guidI * guidSize + 2].UInt32Value;
					if (!isCreating || high != 0L || low != 0)
					{
						PrintValue(key + ((guidCount > 1) ? (" + " + guidI) : ""), new WowGuid128(low, high), index);
					}
				}
				break;
			}
			case UpdateFieldType.Quaternion:
			{
				var quaternionCount = size / 4;
				for (var quatI = 0; quatI < quaternionCount; quatI++)
				{
					var hasQuatValue = false;
					for (var num3 = 0; num3 < 4; num3++)
					{
						if (mask[start + quatI * 4 + num3])
						{
							hasQuatValue = true;
						}
					}
					if (hasQuatValue)
					{
						PrintValue(key + ((quaternionCount > 1) ? (" + " + quatI) : ""), new Quaternion(fieldData[quatI * 4].FloatValue, fieldData[quatI * 4 + 1].FloatValue, fieldData[quatI * 4 + 2].FloatValue, fieldData[quatI * 4 + 3].FloatValue), index);
					}
				}
				break;
			}
			case UpdateFieldType.PackedQuaternion:
			{
				var quaternionCount2 = size / 2;
				for (var num5 = 0; num5 < quaternionCount2; num5++)
				{
					var hasQuatValue2 = false;
					for (var num6 = 0; num6 < 2; num6++)
					{
						if (mask[start + num5 * 2 + num6])
						{
							hasQuatValue2 = true;
						}
					}
					if (hasQuatValue2)
					{
						long quat = fieldData[num5 * 2 + 1].UInt32Value;
						quat <<= 32;
						quat |= fieldData[num5 * 2].UInt32Value;
						PrintValue(key + ((quaternionCount2 > 1) ? (" + " + num5) : ""), new Quaternion(quat), index);
					}
				}
				break;
			}
			case UpdateFieldType.Uint:
			{
				for (var num = 0; num < fieldData.Count; num++)
				{
					if (mask[start + num] && (!isCreating || fieldData[num].UInt32Value != 0))
					{
						PrintValue((num > 0) ? (key + " + " + num) : key, fieldData[num].UInt32Value, index);
					}
				}
				break;
			}
			case UpdateFieldType.Int:
			{
				for (var num7 = 0; num7 < fieldData.Count; num7++)
				{
					if (mask[start + num7] && (!isCreating || fieldData[num7].UInt32Value != 0))
					{
						PrintValue((num7 > 0) ? (key + " + " + num7) : key, fieldData[num7].Int32Value, index);
					}
				}
				break;
			}
			case UpdateFieldType.Float:
			{
				for (var num4 = 0; num4 < fieldData.Count; num4++)
				{
					if (mask[start + num4] && (!isCreating || fieldData[num4].UInt32Value != 0))
					{
						PrintValue((num4 > 0) ? (key + " + " + num4) : key, fieldData[num4].FloatValue, index);
					}
				}
				break;
			}
			case UpdateFieldType.Bytes:
			{
				for (var num2 = 0; num2 < fieldData.Count; num2++)
				{
					if (mask[start + num2] && (!isCreating || fieldData[num2].UInt32Value != 0))
					{
						var intBytes = BitConverter.GetBytes(fieldData[num2].UInt32Value);
						PrintValue((num2 > 0) ? (key + " + " + num2) : key, intBytes[0] + "/" + intBytes[1] + "/" + intBytes[2] + "/" + intBytes[3], index);
					}
				}
				break;
			}
			case UpdateFieldType.Short:
			{
				for (var n = 0; n < fieldData.Count; n++)
				{
					if (mask[start + n] && (!isCreating || fieldData[n].UInt32Value != 0))
					{
						PrintValue((n > 0) ? (key + " + " + n) : key, (short)(fieldData[n].UInt32Value & 0xFFFF) + "/" + (short)(fieldData[n].UInt32Value >> 16), index);
					}
				}
				break;
			}
			default:
			{
				for (var m = 0; m < fieldData.Count; m++)
				{
					if (mask[start + m] && (!isCreating || fieldData[m].UInt32Value != 0))
					{
						PrintValue((m > 0) ? (key + " + " + m) : key, fieldData[m].UInt32Value + "/" + fieldData[m].FloatValue, index);
					}
				}
				break;
			}
			}
			for (var num8 = 0; num8 < fieldData.Count; num8++)
			{
				if (!dict.ContainsKey(start + num8))
				{
					outActuallyChangedValuesMaskArray.Set(start + num8, value: true);
					dict.Add(start + num8, fieldData[num8]);
					continue;
				}
				if (dict[start + num8] != fieldData[num8])
				{
					outActuallyChangedValuesMaskArray.Set(start + num8, value: true);
				}
				dict[start + num8] = fieldData[num8];
			}
		}
		return dict;
	}

	private void ReadMovementUpdateBlock(WorldPacket packet, WowGuid guid, ObjectUpdate updateData, object index)
	{
		MovementInfo moveInfo = null;
		var flags = (UpdateFlag)((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt8() : packet.ReadUInt16());
		if (flags.HasAnyFlag(UpdateFlag.Self))
		{
			if (updateData != null)
			{
				updateData.CreateData.ThisIsYou = true;
			}
			GetSession().GameState.CurrentPlayerCreateTime = packet.GetReceivedTime();
		}
		if (flags.HasAnyFlag(UpdateFlag.Living))
		{
			moveInfo = new MovementInfo();
			moveInfo.ReadMovementInfoLegacy(packet, GetSession().GameState);
			var moveFlags = moveInfo.Flags;
			moveInfo.WalkSpeed = packet.ReadFloat();
			moveInfo.RunSpeed = packet.ReadFloat();
			moveInfo.RunBackSpeed = packet.ReadFloat();
			moveInfo.SwimSpeed = packet.ReadFloat();
			moveInfo.SwimBackSpeed = packet.ReadFloat();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				moveInfo.FlightSpeed = packet.ReadFloat();
				moveInfo.FlightBackSpeed = packet.ReadFloat();
			}
			else
			{
				moveInfo.FlightSpeed = moveInfo.SwimSpeed;
				moveInfo.FlightBackSpeed = moveInfo.SwimBackSpeed;
			}
			moveInfo.TurnRate = packet.ReadFloat();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				moveInfo.PitchRate = packet.ReadFloat();
			}
			if (moveFlags.HasAnyFlag(MovementFlagWotLK.SplineEnabled))
			{
				moveInfo.HasSplineData = true;
				var monsterMove = new ServerSideMovement();
				if (moveInfo.TransportGuid != null)
				{
					monsterMove.TransportGuid = moveInfo.TransportGuid;
				}
				monsterMove.TransportSeat = moveInfo.TransportSeat;
				monsterMove.SplineFlags = SplineFlagModern.None;
				monsterMove.SplineType = SplineTypeModern.None;
				bool hasTaxiFlightFlags;
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					var splineFlags = (SplineFlagWotLK)packet.ReadUInt32();
					monsterMove.SplineFlags = splineFlags.CastFlags<SplineFlagModern>();
					hasTaxiFlightFlags = splineFlags == (SplineFlagWotLK.WalkMode | SplineFlagWotLK.Flying);
					if (splineFlags.HasAnyFlag(SplineFlagWotLK.FinalTarget))
					{
						monsterMove.FinalFacingGuid = packet.ReadGuid().To128(GetSession().GameState);
						monsterMove.SplineType = SplineTypeModern.FacingTarget;
					}
					else if (splineFlags.HasAnyFlag(SplineFlagWotLK.FinalOrientation))
					{
						monsterMove.FinalOrientation = packet.ReadFloat();
						MovementInfo.ClampOrientation(ref monsterMove.FinalOrientation);
						monsterMove.SplineType = SplineTypeModern.FacingAngle;
					}
					else if (splineFlags.HasAnyFlag(SplineFlagWotLK.FinalPoint))
					{
						monsterMove.FinalFacingSpot = packet.ReadVector3();
						monsterMove.SplineType = SplineTypeModern.FacingSpot;
					}
				}
				else if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					var splineFlags2 = (SplineFlagTBC)packet.ReadUInt32();
					monsterMove.SplineFlags = splineFlags2.CastFlags<SplineFlagModern>();
					hasTaxiFlightFlags = splineFlags2 == (SplineFlagTBC.Runmode | SplineFlagTBC.Flying);
					if (splineFlags2.HasAnyFlag(SplineFlagTBC.FinalTarget))
					{
						monsterMove.FinalFacingGuid = packet.ReadGuid().To128(GetSession().GameState);
						monsterMove.SplineType = SplineTypeModern.FacingTarget;
					}
					else if (splineFlags2.HasAnyFlag(SplineFlagTBC.FinalOrientation))
					{
						monsterMove.FinalOrientation = packet.ReadFloat();
						MovementInfo.ClampOrientation(ref monsterMove.FinalOrientation);
						monsterMove.SplineType = SplineTypeModern.FacingAngle;
					}
					else if (splineFlags2.HasAnyFlag(SplineFlagTBC.FinalPoint))
					{
						monsterMove.FinalFacingSpot = packet.ReadVector3();
						monsterMove.SplineType = SplineTypeModern.FacingSpot;
					}
				}
				else
				{
					var splineFlags3 = (SplineFlagVanilla)packet.ReadUInt32();
					monsterMove.SplineFlags = splineFlags3.CastFlags<SplineFlagModern>();
					hasTaxiFlightFlags = splineFlags3 == (SplineFlagVanilla.Runmode | SplineFlagVanilla.Flying);
					if (splineFlags3.HasAnyFlag(SplineFlagVanilla.FinalTarget))
					{
						monsterMove.FinalFacingGuid = packet.ReadGuid().To128(GetSession().GameState);
						monsterMove.SplineType = SplineTypeModern.FacingTarget;
					}
					else if (splineFlags3.HasAnyFlag(SplineFlagVanilla.FinalOrientation))
					{
						monsterMove.FinalOrientation = packet.ReadFloat();
						MovementInfo.ClampOrientation(ref monsterMove.FinalOrientation);
						monsterMove.SplineType = SplineTypeModern.FacingAngle;
					}
					else if (splineFlags3.HasAnyFlag(SplineFlagVanilla.FinalPoint))
					{
						monsterMove.FinalFacingSpot = packet.ReadVector3();
						monsterMove.SplineType = SplineTypeModern.FacingSpot;
					}
				}
				if (hasTaxiFlightFlags && guid.IsPlayer() && flags.HasAnyFlag(UpdateFlag.Self))
				{
					monsterMove.SplineFlags = SplineFlagModern.Flying | SplineFlagModern.CatmullRom | SplineFlagModern.CanSwim | SplineFlagModern.UncompressedPath | SplineFlagModern.Unknown5 | SplineFlagModern.Steering | SplineFlagModern.Unknown10;
				}
				monsterMove.SplineTime = packet.ReadUInt32();
				monsterMove.SplineTimeFull = packet.ReadUInt32();
				monsterMove.SplineId = packet.ReadUInt32();
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
				{
					packet.ReadFloat();
					packet.ReadFloat();
					packet.ReadInt32();
					packet.ReadInt32();
				}
				var splineCount = (monsterMove.SplineCount = packet.ReadUInt32());
				monsterMove.SplinePoints = new List<Vector3>();
				for (var i = 0; i < splineCount; i++)
				{
					var vec = packet.ReadVector3();
					monsterMove.SplinePoints.Add(vec);
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
				{
					monsterMove.SplineMode = packet.ReadUInt8();
				}
				monsterMove.EndPosition = packet.ReadVector3();
				if (updateData != null)
				{
					updateData.CreateData.MoveSpline = monsterMove;
				}
			}
		}
		else if (flags.HasAnyFlag(UpdateFlag.GOPosition))
		{
			moveInfo = new MovementInfo
			{
				TransportGuid = packet.ReadPackedGuid().To128(GetSession().GameState),
				Position = packet.ReadVector3(),
				TransportOffset = packet.ReadVector3(),
				Orientation = packet.ReadFloat()
			};
			moveInfo.TransportOrientation = moveInfo.Orientation;
			moveInfo.CorpseOrientation = packet.ReadFloat();
		}
		else if (flags.HasAnyFlag(UpdateFlag.StationaryObject))
		{
			moveInfo = new MovementInfo
			{
				Position = packet.ReadVector3(),
				Orientation = packet.ReadFloat()
			};
		}
		if (flags.HasAnyFlag(UpdateFlag.LowGuid))
		{
			packet.ReadUInt32();
		}
		if (flags.HasAnyFlag(UpdateFlag.HighGuid))
		{
			packet.ReadUInt32();
		}
		if (flags.HasAnyFlag(UpdateFlag.AttackingTarget))
		{
			var attackGuid = packet.ReadPackedGuid();
			if (updateData != null)
			{
				updateData.CreateData.AutoAttackVictim = attackGuid.To128(GetSession().GameState);
			}
		}
		if (flags.HasAnyFlag(UpdateFlag.Transport))
		{
			var transportPathTimer = packet.ReadUInt32();
			if (moveInfo != null)
			{
				moveInfo.TransportPathTimer = transportPathTimer;
			}
		}
		if (flags.HasAnyFlag(UpdateFlag.Vehicle))
		{
			var vehicleId = packet.ReadUInt32();
			var vehicleOrientation = packet.ReadFloat();
			if (moveInfo != null)
			{
				moveInfo.VehicleId = vehicleId;
				moveInfo.VehicleOrientation = vehicleOrientation;
			}
		}
		if (flags.HasAnyFlag(UpdateFlag.GORotation))
		{
			var rotation = packet.ReadPackedQuaternion();
			if (moveInfo != null)
			{
				moveInfo.Rotation = rotation;
			}
		}
		if (updateData != null && moveInfo != null)
		{
			moveInfo.Flags = (uint)((MovementFlagWotLK)moveInfo.Flags).CastFlags<MovementFlagModern>();
			moveInfo.ValidateMovementInfo();
			updateData.CreateData.MoveInfo = moveInfo;
		}
	}

	private static WowGuid GetGuidValue<T>(Dictionary<int, UpdateField> UpdateFields, T field) where T : Enum
	{
		if (!LegacyVersion.AddedInVersion(ClientVersionBuild.V6_0_2_19033))
		{
			var parts = UpdateFields.GetArray<T, uint>(field, 2);
			return new WowGuid64(MathFunctions.MakePair64(parts[0], parts[1]));
		}
		var parts2 = UpdateFields.GetArray<T, uint>(field, 4);
		return new WowGuid128(MathFunctions.MakePair64(parts2[0], parts2[1]), MathFunctions.MakePair64(parts2[2], parts2[3]));
	}

	private static WowGuid GetGuidValue(Dictionary<int, UpdateField> UpdateFields, int field)
	{
		if (!LegacyVersion.AddedInVersion(ClientVersionBuild.V6_0_2_19033))
		{
			var parts = UpdateFields.GetArray<uint>(field, 2);
			return new WowGuid64(MathFunctions.MakePair64(parts[0], parts[1]));
		}
		var parts2 = UpdateFields.GetArray<uint>(field, 4);
		return new WowGuid128(MathFunctions.MakePair64(parts2[0], parts2[1]), MathFunctions.MakePair64(parts2[2], parts2[3]));
	}

	public QuestLog ReadQuestLogEntry(int i, BitArray updateMaskArray, Dictionary<int, UpdateField> updates)
	{
		var PLAYER_QUEST_LOG_1_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_QUEST_LOG_1_1);
		// 3.3.5a quest log: 5 fields per entry (QuestID, StateFlags, Progress, [gap], Timer)
		// Fields: _1=QuestID(+0), _2=StateFlags(+1), _3=Progress(+2), skip(+3), _4/_5=Timer(+4)
		var sizePerEntry = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089) ? 5 : 3);
		var stateOffset = 1;
		var progressOffset = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089) ? 2 : (-1));
		var timerOffset = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089) ? 4 : 2);
		QuestLog questLog = null;
		var index = PLAYER_QUEST_LOG_1_1 + i * sizePerEntry;
		if ((updateMaskArray != null && updateMaskArray[index]) || (updateMaskArray == null && updates.ContainsKey(index)))
		{
			if (questLog == null)
			{
				questLog = new QuestLog();
			}
			questLog.QuestID = updates[index].Int32Value;
			// Cache the QuestID for this slot
			GetSession().GameState.QuestLogQuestIDs[i] = questLog.QuestID.Value;
			Log.Print(LogType.Debug, $"[QuestLogRead] slot={i} QuestID={questLog.QuestID.Value} fieldIndex={index}", "");
		}
		if ((updateMaskArray != null && updateMaskArray[index + stateOffset]) || (updateMaskArray == null && updates.ContainsKey(index + stateOffset)))
		{
			if (questLog == null)
			{
				questLog = new QuestLog();
			}
			if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_4_0_8089))
			{
				var rawValue = updates[index + stateOffset].UInt32Value;
				questLog.ObjectiveProgress[0] = (byte)(rawValue & 0x3F);
				questLog.ObjectiveProgress[1] = (byte)((rawValue & 0xFC0) >> 6);
				questLog.ObjectiveProgress[2] = (byte)((rawValue & 0x3F000) >> 12);
				questLog.ObjectiveProgress[3] = (byte)((rawValue & 0xFC0000) >> 18);
				questLog.StateFlags = (rawValue >> 24) & 0xFF;
			}
			else
			{
				questLog.StateFlags = updates[index + stateOffset].UInt32Value;
			}
		}
		if (progressOffset != -1 && ((updateMaskArray != null && updateMaskArray[index + progressOffset]) || (updateMaskArray == null && updates.ContainsKey(index + progressOffset))))
		{
			if (questLog == null)
			{
				questLog = new QuestLog();
			}
			// In 3.3.5a, objective counts are 16-bit each, stored as uint64 across fields +2 and +3
			// Field +2: objective 0 (low 16 bits) | objective 1 (high 16 bits)
			// Field +3: objective 2 (low 16 bits) | objective 3 (high 16 bits)
			var progressField0 = updates[index + progressOffset].UInt32Value;
			questLog.ObjectiveProgress[0] = (short)(progressField0 & 0xFFFF);
			questLog.ObjectiveProgress[1] = (short)((progressField0 >> 16) & 0xFFFF);
			var progressOffset2 = progressOffset + 1;
			if (updates.ContainsKey(index + progressOffset2))
			{
				var progressField1 = updates[index + progressOffset2].UInt32Value;
				questLog.ObjectiveProgress[2] = (short)(progressField1 & 0xFFFF);
				questLog.ObjectiveProgress[3] = (short)((progressField1 >> 16) & 0xFFFF);
			}
		}
		// Also handle when only field +3 updates (objectives 2-3 change without 0-1 changing)
		if (progressOffset != -1)
		{
			var progressOffset2 = progressOffset + 1;
			var field3Updated = (updateMaskArray != null && updateMaskArray[index + progressOffset2]) || (updateMaskArray == null && updates.ContainsKey(index + progressOffset2));
			var field2Updated = (updateMaskArray != null && updateMaskArray[index + progressOffset]) || (updateMaskArray == null && updates.ContainsKey(index + progressOffset));
			if (field3Updated && !field2Updated)
			{
				if (questLog == null)
				{
					questLog = new QuestLog();
				}
				var progressField1 = updates[index + progressOffset2].UInt32Value;
				questLog.ObjectiveProgress[2] = (short)(progressField1 & 0xFFFF);
				questLog.ObjectiveProgress[3] = (short)((progressField1 >> 16) & 0xFFFF);
			}
		}
		if ((updateMaskArray != null && updateMaskArray[index + timerOffset]) || (updateMaskArray == null && updates.ContainsKey(index + timerOffset)))
		{
			if (questLog == null)
			{
				questLog = new QuestLog();
			}
			questLog.EndTime = updates[index + timerOffset].UInt32Value;
		}
		// If we have quest data (StateFlags/Progress) but no QuestID in this update,
		// fill QuestID from the cache (set during CreateObject or earlier Values update)
		if (questLog != null && !questLog.QuestID.HasValue)
		{
			var cachedId = GetSession().GameState.QuestLogQuestIDs[i];
			if (cachedId != 0)
			{
				questLog.QuestID = cachedId;
			}
		}
		// If QuestID was explicitly set to 0, clear the cache (quest abandoned/completed)
		if (questLog != null && questLog.QuestID.HasValue && questLog.QuestID.Value == 0)
		{
			GetSession().GameState.QuestLogQuestIDs[i] = 0;
		}
		return questLog;
	}

	public AuraDataInfo ReadAuraSlot(byte i, WowGuid128 guid, Dictionary<int, UpdateField> updates)
	{
		var UNIT_FIELD_AURA = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURA);
		var UNIT_FIELD_AURAFLAGS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURAFLAGS);
		var UNIT_FIELD_AURALEVELS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURALEVELS);
		var UNIT_FIELD_AURAAPPLICATIONS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURAAPPLICATIONS);
		if (!updates.ContainsKey(UNIT_FIELD_AURA + i))
		{
			return null;
		}
		var spellId = updates[UNIT_FIELD_AURA + i].UInt32Value;
		if (spellId == 0)
		{
			return null;
		}
		var data = new AuraDataInfo
		{
			CastID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Aura, GetSession().GameState.CurrentMapId.Value, spellId, guid.GetCounter()),
			SpellID = spellId,
			SpellXSpellVisualID = GameData.GetSpellVisual(spellId)
		};
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var flagsIndex = UNIT_FIELD_AURAFLAGS + i / 4;
			if (updates.ContainsKey(flagsIndex))
			{
				var flags = (ushort)((updates[flagsIndex].UInt32Value >> i % 4 * 8) & 0xFF);
				ModernVersion.ConvertAuraFlags(flags, i, out data.Flags, out data.ActiveFlags);
			}
		}
		else
		{
			var flagsIndex2 = UNIT_FIELD_AURAFLAGS + i / 8;
			if (updates.ContainsKey(flagsIndex2))
			{
				var flags2 = (ushort)((updates[flagsIndex2].UInt32Value >> i % 8 * 4) & 0xF);
				ModernVersion.ConvertAuraFlags(flags2, i, out data.Flags, out data.ActiveFlags);
			}
		}
		var levelsIndex = UNIT_FIELD_AURALEVELS + i / 4;
		if (updates.ContainsKey(levelsIndex))
		{
			data.CastLevel = (ushort)((updates[levelsIndex].UInt32Value >> i % 4 * 8) & 0xFF);
		}
		else
		{
			data.CastLevel = 0;
		}
		var stacksIndex = UNIT_FIELD_AURAAPPLICATIONS + i / 4;
		if (updates.ContainsKey(stacksIndex))
		{
			data.Applications = (byte)((updates[stacksIndex].UInt32Value >> i % 4 * 8) & 0xFF);
		}
		else
		{
			data.Applications = 0;
		}
		if (GameData.StackableAuras.Contains(spellId))
		{
			data.Applications++;
		}
		if (GameData.SpellEffectPoints.TryGetValue(spellId, out var basePoints))
		{
			data.Points = basePoints;
		}
		return data;
	}

	public byte ReadPvPFlags(Dictionary<int, UpdateField> updates)
	{
		byte flags = 0;
		var UNIT_FIELD_FLAGS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_FLAGS);
		if (UNIT_FIELD_FLAGS >= 0 && updates.ContainsKey(UNIT_FIELD_FLAGS) && updates[UNIT_FIELD_FLAGS].UInt32Value.HasAnyFlag(UnitFlags.Pvp))
		{
			flags |= 1;
		}
		var PLAYER_FLAGS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FLAGS);
		if (PLAYER_FLAGS >= 0 && updates.ContainsKey(PLAYER_FLAGS))
		{
			if (updates[PLAYER_FLAGS].UInt32Value.HasAnyFlag(PlayerFlagsLegacy.FreeForAllPvP))
			{
				flags |= 4;
			}
			if (updates[PLAYER_FLAGS].UInt32Value.HasAnyFlag(PlayerFlagsLegacy.Sanctuary))
			{
				flags |= 8;
			}
		}
		return flags;
	}

	public void StoreObjectUpdate(WowGuid128 guid, ObjectType objectType, BitArray updateMaskArray, Dictionary<int, UpdateField> updates, AuraUpdate auraUpdate, PowerUpdate powerUpdate, bool isCreate, ObjectUpdate updateData, BitArray actuallyChangedValuesMaskArray)
	{
		StoreObjectUpdateInternal(guid, objectType, updateMaskArray, updates, auraUpdate, powerUpdate, isCreate, updateData);
		AfterStoreObjectUpdateHook(guid, objectType, updateMaskArray, updates, auraUpdate, powerUpdate, isCreate, updateData, actuallyChangedValuesMaskArray);
	}

	private void AfterStoreObjectUpdateHook(WowGuid128 guid, ObjectType objectType, BitArray updateMaskArray, Dictionary<int, UpdateField> updates, AuraUpdate auraUpdate, PowerUpdate powerUpdate, bool isCreate, ObjectUpdate updateData, BitArray changedValuesMask)
	{
		if (objectType != ObjectType.Player && objectType != ObjectType.ActivePlayer)
		{
			return;
		}
		var UNIT_FIELD_NATIVEDISPLAYID = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_NATIVEDISPLAYID);
		var UNIT_FIELD_MOUNTDISPLAYID = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MOUNTDISPLAYID);
		var OBJECT_FIELD_SCALE_X = LegacyVersion.GetUpdateField(ObjectField.OBJECT_FIELD_SCALE_X);
		if (UNIT_FIELD_NATIVEDISPLAYID < 0 || UNIT_FIELD_MOUNTDISPLAYID < 0 || OBJECT_FIELD_SCALE_X < 0 || (!changedValuesMask.Get(UNIT_FIELD_NATIVEDISPLAYID) && !changedValuesMask.Get(UNIT_FIELD_MOUNTDISPLAYID) && !changedValuesMask.Get(OBJECT_FIELD_SCALE_X)))
		{
			return;
		}
		var nativeDisplayId = Session.GameState.GetLegacyFieldValueInt32(guid, UnitField.UNIT_FIELD_DISPLAYID);
		var mountDisplayId = Session.GameState.GetLegacyFieldValueInt32(guid, UnitField.UNIT_FIELD_MOUNTDISPLAYID);
		var rawScaleX = Session.GameState.GetLegacyFieldValueFloat(guid, ObjectField.OBJECT_FIELD_SCALE_X);
		if (rawScaleX != 0f)
		{
			var regularNativeDisplaySize = GameData.GetUnitCompleteDisplayScale((uint)nativeDisplayId);
			var scale = rawScaleX / regularNativeDisplaySize;
			var ourDisplayInfo = GameData.GetDisplayInfo((uint)nativeDisplayId);
			var ourModel = GameData.GetModelData(ourDisplayInfo.ModelId);
			float calculatedBaseHeight;
			if (mountDisplayId != 0 && LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				var mountDisplayInfo = GameData.GetDisplayInfo((uint)mountDisplayId);
				var mountModel = GameData.GetModelData(mountDisplayInfo.ModelId);
				calculatedBaseHeight = mountModel.MountHeight * mountDisplayInfo.DisplayScale + ourModel.Height * ourModel.ModelScale * ourDisplayInfo.DisplayScale * 0.5f;
			}
			else
			{
				calculatedBaseHeight = ourDisplayInfo.DisplayScale * ourModel.Height * ourModel.ModelScale;
			}
			if (calculatedBaseHeight == 0f)
			{
				calculatedBaseHeight = ((mountDisplayId != 0) ? 3.081099f : 2.438083f);
			}
			var heightScale = Math.Max(scale, regularNativeDisplaySize);
			var scaledHeight = heightScale * calculatedBaseHeight;
			var displayScale = regularNativeDisplaySize * scale;
			var reason = (changedValuesMask.Get(UNIT_FIELD_MOUNTDISPLAYID) ? MoveSetCollisionHeight.UpdateCollisionHeightReason.Mount : MoveSetCollisionHeight.UpdateCollisionHeightReason.Force);
			var height = new MoveSetCollisionHeight
			{
				MoverGUID = guid,
				Height = scaledHeight,
				Scale = displayScale,
				Reason = reason,
				MountDisplayID = (uint)mountDisplayId
			};
			SendPacketToClient(height, Opcode.SMSG_UPDATE_OBJECT);
		}
	}

	private void StoreObjectUpdateInternal(WowGuid128 guid, ObjectType objectType, BitArray updateMaskArray, Dictionary<int, UpdateField> updates, AuraUpdate auraUpdate, PowerUpdate powerUpdate, bool isCreate, ObjectUpdate updateData)
	{
		var OBJECT_FIELD_GUID = LegacyVersion.GetUpdateField(ObjectField.OBJECT_FIELD_GUID);
		if (OBJECT_FIELD_GUID >= 0 && updateMaskArray[OBJECT_FIELD_GUID])
		{
			updateData.ObjectData.Guid = GetGuidValue(updates, ObjectField.OBJECT_FIELD_GUID).To128(GetSession().GameState);
		}
		var OBJECT_FIELD_ENTRY = LegacyVersion.GetUpdateField(ObjectField.OBJECT_FIELD_ENTRY);
		if (OBJECT_FIELD_ENTRY >= 0 && updateMaskArray[OBJECT_FIELD_ENTRY])
		{
			updateData.ObjectData.EntryID = updates[OBJECT_FIELD_ENTRY].Int32Value;
		}
		var OBJECT_FIELD_SCALE_X = LegacyVersion.GetUpdateField(ObjectField.OBJECT_FIELD_SCALE_X);
		if (OBJECT_FIELD_SCALE_X >= 0 && updateMaskArray[OBJECT_FIELD_SCALE_X])
		{
			updateData.ObjectData.Scale = updates[OBJECT_FIELD_SCALE_X].FloatValue;
		}
		if (objectType == ObjectType.Item || objectType == ObjectType.Container)
		{
			var ITEM_FIELD_OWNER = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_OWNER);
			if (ITEM_FIELD_OWNER >= 0 && updateMaskArray[ITEM_FIELD_OWNER])
			{
				updateData.ItemData.Owner = GetGuidValue(updates, ItemField.ITEM_FIELD_OWNER).To128(GetSession().GameState);
			}
			var ITEM_FIELD_CONTAINED = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_CONTAINED);
			if (ITEM_FIELD_CONTAINED >= 0 && updateMaskArray[ITEM_FIELD_CONTAINED])
			{
				updateData.ItemData.ContainedIn = GetGuidValue(updates, ItemField.ITEM_FIELD_CONTAINED).To128(GetSession().GameState);
			}
			var ITEM_FIELD_CREATOR = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_CREATOR);
			if (ITEM_FIELD_CREATOR >= 0 && updateMaskArray[ITEM_FIELD_CREATOR])
			{
				updateData.ItemData.Creator = GetGuidValue(updates, ItemField.ITEM_FIELD_CREATOR).To128(GetSession().GameState);
			}
			var ITEM_FIELD_GIFTCREATOR = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_GIFTCREATOR);
			if (ITEM_FIELD_GIFTCREATOR >= 0 && updateMaskArray[ITEM_FIELD_GIFTCREATOR])
			{
				updateData.ItemData.GiftCreator = GetGuidValue(updates, ItemField.ITEM_FIELD_GIFTCREATOR).To128(GetSession().GameState);
			}
			var ITEM_FIELD_STACK_COUNT = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_STACK_COUNT);
			if (ITEM_FIELD_STACK_COUNT >= 0 && updateMaskArray[ITEM_FIELD_STACK_COUNT])
			{
				updateData.ItemData.StackCount = updates[ITEM_FIELD_STACK_COUNT].UInt32Value;
			}
			var ITEM_FIELD_DURATION = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_DURATION);
			if (ITEM_FIELD_DURATION >= 0 && updateMaskArray[ITEM_FIELD_DURATION])
			{
				updateData.ItemData.Duration = updates[ITEM_FIELD_DURATION].UInt32Value;
			}
			var ITEM_FIELD_SPELL_CHARGES = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_SPELL_CHARGES);
			if (ITEM_FIELD_SPELL_CHARGES >= 0)
			{
				for (var i = 0; i < 5; i++)
				{
					if (updateMaskArray[ITEM_FIELD_SPELL_CHARGES + i])
					{
						updateData.ItemData.SpellCharges[i] = updates[ITEM_FIELD_SPELL_CHARGES + i].Int32Value;
					}
				}
			}
			var ITEM_FIELD_FLAGS = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_FLAGS);
			if (ITEM_FIELD_FLAGS >= 0 && updateMaskArray[ITEM_FIELD_FLAGS])
			{
				updateData.ItemData.Flags = updates[ITEM_FIELD_FLAGS].UInt32Value;
			}
			var ITEM_FIELD_ENCHANTMENT = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_ENCHANTMENT);
			if (ITEM_FIELD_ENCHANTMENT >= 0)
			{
				var sizePerEntry = 3;
				var ReadEnchantData = delegate(int num2)
				{
					ItemEnchantment itemEnchantment = null;
					var num = ITEM_FIELD_ENCHANTMENT + num2 * sizePerEntry;
					var num3 = num + 1;
					var num4 = num3 + 1;
					if (updateMaskArray[num])
					{
						if (itemEnchantment == null)
						{
							itemEnchantment = new ItemEnchantment();
						}
						itemEnchantment.ID = updates[num].Int32Value;
					}
					if (updateMaskArray[num3])
					{
						if (itemEnchantment == null)
						{
							itemEnchantment = new ItemEnchantment();
						}
						itemEnchantment.Duration = updates[num3].UInt32Value;
					}
					if (updateMaskArray[num4])
					{
						if (itemEnchantment == null)
						{
							itemEnchantment = new ItemEnchantment();
						}
						itemEnchantment.Charges = (ushort)updates[num4].UInt32Value;
					}
					return itemEnchantment;
				};
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					updateData.ItemData.Enchantment[EnchantmentSlot.Perm] = ReadEnchantData(Enums.Vanilla.EnchantmentSlot.Perm);
					updateData.ItemData.Enchantment[EnchantmentSlot.Temp] = ReadEnchantData(Enums.Vanilla.EnchantmentSlot.Temp);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop0] = ReadEnchantData(Enums.Vanilla.EnchantmentSlot.Prop0);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop1] = ReadEnchantData(Enums.Vanilla.EnchantmentSlot.Prop1);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop2] = ReadEnchantData(Enums.Vanilla.EnchantmentSlot.Prop2);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop3] = ReadEnchantData(Enums.Vanilla.EnchantmentSlot.Prop3);
				}
				else if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					updateData.ItemData.Enchantment[EnchantmentSlot.Perm] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Perm);
					updateData.ItemData.Enchantment[EnchantmentSlot.Temp] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Temp);
					updateData.ItemData.Enchantment[EnchantmentSlot.Sock1] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Sock1);
					updateData.ItemData.Enchantment[EnchantmentSlot.Sock2] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Sock2);
					updateData.ItemData.Enchantment[EnchantmentSlot.Sock3] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Sock3);
					updateData.ItemData.Enchantment[EnchantmentSlot.Bonus] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Bonus);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop0] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Prop0);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop1] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Prop1);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop2] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Prop2);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop3] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Prop3);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop4] = ReadEnchantData(Enums.TBC.EnchantmentSlot.Prop4);
				}
				else
				{
					updateData.ItemData.Enchantment[EnchantmentSlot.Perm] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Perm);
					updateData.ItemData.Enchantment[EnchantmentSlot.Temp] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Temp);
					updateData.ItemData.Enchantment[EnchantmentSlot.Sock1] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Sock1);
					updateData.ItemData.Enchantment[EnchantmentSlot.Sock2] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Sock2);
					updateData.ItemData.Enchantment[EnchantmentSlot.Sock3] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Sock3);
					updateData.ItemData.Enchantment[EnchantmentSlot.Bonus] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Bonus);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prismatic] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Prismatic);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop0] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Prop0);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop1] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Prop1);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop2] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Prop2);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop3] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Prop3);
					updateData.ItemData.Enchantment[EnchantmentSlot.Prop4] = ReadEnchantData(Enums.WotLK.EnchantmentSlot.Prop4);
				}
				var gems = new uint?[3];
				for (var i2 = 0; i2 < 3; i2++)
				{
					var slot = EnchantmentSlot.Sock1 + i2;
					if (updateData.ItemData.Enchantment[slot] != null && updateData.ItemData.Enchantment[slot].ID.HasValue)
					{
						var itemId = GameData.GetGemFromEnchantId((uint)updateData.ItemData.Enchantment[slot].ID.Value);
						if (itemId != 0 || updateData.ItemData.Enchantment[slot].ID == 0)
						{
							gems[i2] = itemId;
							updateData.ItemData.HasGemsUpdate = true;
						}
					}
				}
				if (updateData.ItemData.HasGemsUpdate)
				{
					GetSession().GameState.SaveGemsForItem(guid, gems);
				}
			}
			var ITEM_FIELD_PROPERTY_SEED = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_PROPERTY_SEED);
			if (ITEM_FIELD_PROPERTY_SEED >= 0 && updateMaskArray[ITEM_FIELD_PROPERTY_SEED])
			{
				updateData.ItemData.PropertySeed = updates[ITEM_FIELD_PROPERTY_SEED].UInt32Value;
			}
			var ITEM_FIELD_RANDOM_PROPERTIES_ID = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_RANDOM_PROPERTIES_ID);
			if (ITEM_FIELD_RANDOM_PROPERTIES_ID >= 0 && updateMaskArray[ITEM_FIELD_RANDOM_PROPERTIES_ID])
			{
				updateData.ItemData.RandomProperty = updates[ITEM_FIELD_RANDOM_PROPERTIES_ID].UInt32Value;
			}
			var ITEM_FIELD_DURABILITY = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_DURABILITY);
			if (ITEM_FIELD_DURABILITY >= 0 && updateMaskArray[ITEM_FIELD_DURABILITY])
			{
				updateData.ItemData.Durability = updates[ITEM_FIELD_DURABILITY].UInt32Value;
			}
			var ITEM_FIELD_MAXDURABILITY = LegacyVersion.GetUpdateField(ItemField.ITEM_FIELD_MAXDURABILITY);
			if (ITEM_FIELD_MAXDURABILITY >= 0 && updateMaskArray[ITEM_FIELD_MAXDURABILITY])
			{
				updateData.ItemData.MaxDurability = updates[ITEM_FIELD_MAXDURABILITY].UInt32Value;
			}
		}
		if (objectType == ObjectType.Container)
		{
			var CONTAINER_FIELD_NUM_SLOTS = LegacyVersion.GetUpdateField(ContainerField.CONTAINER_FIELD_NUM_SLOTS);
			if (CONTAINER_FIELD_NUM_SLOTS >= 0 && updateMaskArray[CONTAINER_FIELD_NUM_SLOTS])
			{
				updateData.ContainerData.NumSlots = updates[CONTAINER_FIELD_NUM_SLOTS].UInt32Value;
			}
			var CONTAINER_FIELD_SLOT_1 = LegacyVersion.GetUpdateField(ContainerField.CONTAINER_FIELD_SLOT_1);
			if (CONTAINER_FIELD_SLOT_1 >= 0)
			{
				for (var i3 = 0; i3 < 36; i3++)
				{
					if (updateMaskArray[CONTAINER_FIELD_SLOT_1 + i3 * 2])
					{
						updateData.ContainerData.Slots[i3] = GetGuidValue(updates, CONTAINER_FIELD_SLOT_1 + i3 * 2).To128(GetSession().GameState);
					}
				}
			}
		}
		if (objectType == ObjectType.Unit || objectType == ObjectType.Player || objectType == ObjectType.ActivePlayer)
		{
			var UNIT_FIELD_CHARM = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_CHARM);
			if (UNIT_FIELD_CHARM >= 0 && updateMaskArray[UNIT_FIELD_CHARM])
			{
				updateData.UnitData.Charm = GetGuidValue(updates, UnitField.UNIT_FIELD_CHARM).To128(GetSession().GameState);
			}
			var UNIT_FIELD_SUMMON = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_SUMMON);
			if (UNIT_FIELD_SUMMON >= 0 && updateMaskArray[UNIT_FIELD_SUMMON])
			{
				updateData.UnitData.Summon = GetGuidValue(updates, UnitField.UNIT_FIELD_SUMMON).To128(GetSession().GameState);
			}
			var UNIT_FIELD_CHARMEDBY = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_CHARMEDBY);
			if (UNIT_FIELD_CHARMEDBY >= 0 && updateMaskArray[UNIT_FIELD_CHARMEDBY])
			{
				updateData.UnitData.CharmedBy = GetGuidValue(updates, UnitField.UNIT_FIELD_CHARMEDBY).To128(GetSession().GameState);
			}
			var UNIT_FIELD_SUMMONEDBY = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_SUMMONEDBY);
			if (UNIT_FIELD_SUMMONEDBY >= 0 && updateMaskArray[UNIT_FIELD_SUMMONEDBY])
			{
				updateData.UnitData.SummonedBy = GetGuidValue(updates, UnitField.UNIT_FIELD_SUMMONEDBY).To128(GetSession().GameState);
			}
			var UNIT_FIELD_CREATEDBY = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_CREATEDBY);
			if (UNIT_FIELD_CREATEDBY >= 0 && updateMaskArray[UNIT_FIELD_CREATEDBY])
			{
				updateData.UnitData.CreatedBy = GetGuidValue(updates, UnitField.UNIT_FIELD_CREATEDBY).To128(GetSession().GameState);
			}
			var UNIT_FIELD_TARGET = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_TARGET);
			if (UNIT_FIELD_TARGET >= 0 && updateMaskArray[UNIT_FIELD_TARGET])
			{
				updateData.UnitData.Target = GetGuidValue(updates, UnitField.UNIT_FIELD_TARGET).To128(GetSession().GameState);
			}
			var UNIT_FIELD_CHANNEL_OBJECT = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_CHANNEL_OBJECT);
			if (UNIT_FIELD_CHANNEL_OBJECT >= 0 && updateMaskArray[UNIT_FIELD_CHANNEL_OBJECT])
			{
				updateData.UnitData.ChannelObject = GetGuidValue(updates, UnitField.UNIT_FIELD_CHANNEL_OBJECT).To128(GetSession().GameState);
			}
			var UNIT_FIELD_HEALTH = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_HEALTH);
			if (UNIT_FIELD_HEALTH >= 0 && updateMaskArray[UNIT_FIELD_HEALTH])
			{
				updateData.UnitData.Health = updates[UNIT_FIELD_HEALTH].Int32Value;
			}
			var UNIT_FIELD_MAXHEALTH = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MAXHEALTH);
			if (UNIT_FIELD_MAXHEALTH >= 0 && updateMaskArray[UNIT_FIELD_MAXHEALTH])
			{
				updateData.UnitData.MaxHealth = updates[UNIT_FIELD_MAXHEALTH].Int32Value;
			}
			var UNIT_FIELD_LEVEL = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_LEVEL);
			if (UNIT_FIELD_LEVEL >= 0 && updateMaskArray[UNIT_FIELD_LEVEL])
			{
				updateData.UnitData.Level = updates[UNIT_FIELD_LEVEL].Int32Value;
				// Compute GlyphsEnabled for current player based on level
				if (guid == GetSession().GameState.CurrentPlayerGuid)
				{
					var lvl = updates[UNIT_FIELD_LEVEL].Int32Value;
					byte ge = 0;
					if (lvl >= 15) ge |= 0x01 | 0x02;
					if (lvl >= 30) ge |= 0x08;
					if (lvl >= 50) ge |= 0x04;
					if (lvl >= 70) ge |= 0x10;
					if (lvl >= 80) ge |= 0x20;
					GetSession().GameState.GlyphsEnabled = ge;
				}
			}
			var UNIT_FIELD_FACTIONTEMPLATE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_FACTIONTEMPLATE);
			if (UNIT_FIELD_FACTIONTEMPLATE >= 0 && updateMaskArray[UNIT_FIELD_FACTIONTEMPLATE])
			{
				updateData.UnitData.FactionTemplate = updates[UNIT_FIELD_FACTIONTEMPLATE].Int32Value;
			}
			var UNIT_FIELD_BYTES_0 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BYTES_0);
			if (UNIT_FIELD_BYTES_0 >= 0 && updateMaskArray[UNIT_FIELD_BYTES_0])
			{
				updateData.UnitData.RaceId = (byte)(updates[UNIT_FIELD_BYTES_0].UInt32Value & 0xFF);
				updateData.UnitData.ClassId = (byte)((updates[UNIT_FIELD_BYTES_0].UInt32Value >> 8) & 0xFF);
				updateData.UnitData.SexId = (byte)((updates[UNIT_FIELD_BYTES_0].UInt32Value >> 16) & 0xFF);
				updateData.UnitData.DisplayPower = (byte)((updates[UNIT_FIELD_BYTES_0].UInt32Value >> 24) & 0xFF);
				if (guid.GetHighType() == HighGuidType.Pet && updateData.UnitData.DisplayPower == 2)
				{
					GetSession().GameState.HunterPetGuids.Add(guid);
				}
				if (objectType == ObjectType.Unit)
				{
					GetSession().GameState.StoreCreatureClass(guid.GetEntry(), (Class)updateData.UnitData.ClassId.Value);
				}
				else
				{
					updateData.PlayerData.ArenaFaction = (byte)(GameData.IsAllianceRace((Race)updateData.UnitData.RaceId.Value) ? 1u : 0u);
				}
			}
			var UNIT_FIELD_POWER1 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_POWER1);
			if (UNIT_FIELD_POWER1 >= 0)
			{
				for (var i4 = 0; i4 < LegacyVersion.GetPowersCount(); i4++)
				{
					if (updateMaskArray[UNIT_FIELD_POWER1 + i4])
					{
						if (powerUpdate != null && (guid == GetSession().GameState.CurrentPlayerGuid || guid == GetSession().GameState.CurrentPetGuid))
						{
							powerUpdate.Powers.Add(new PowerUpdatePower(updates[UNIT_FIELD_POWER1 + i4].Int32Value, (byte)i4));
						}
						sbyte powerSlot;
						if (GetSession().GameState.HunterPetGuids.Contains(guid))
						{
							powerSlot = ClassPowerTypes.GetPowerSlotForPet((PowerType)i4);
						}
						else
						{
							var classId = ((!updateData.UnitData.ClassId.HasValue) ? GetSession().GameState.GetUnitClass(guid.To128(GetSession().GameState)) : ((Class)updateData.UnitData.ClassId.Value));
							powerSlot = ClassPowerTypes.GetPowerSlotForClass(classId, (PowerType)i4);
						}
						if (powerSlot >= 0)
						{
							updateData.UnitData.Power[powerSlot] = updates[UNIT_FIELD_POWER1 + i4].Int32Value;
						}
					}
				}
			}
			var UNIT_FIELD_MAXPOWER1 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER1);
			if (UNIT_FIELD_MAXPOWER1 >= 0)
			{
				for (var i5 = 0; i5 < LegacyVersion.GetPowersCount(); i5++)
				{
					if (!updateMaskArray[UNIT_FIELD_MAXPOWER1 + i5])
					{
						continue;
					}
					var classId2 = ((!updateData.UnitData.ClassId.HasValue) ? GetSession().GameState.GetUnitClass(guid.To128(GetSession().GameState)) : ((Class)updateData.UnitData.ClassId.Value));
					var powerSlot2 = ((!GetSession().GameState.HunterPetGuids.Contains(guid)) ? ClassPowerTypes.GetPowerSlotForClass(classId2, (PowerType)i5) : ClassPowerTypes.GetPowerSlotForPet((PowerType)i5));
					if (powerSlot2 >= 0)
					{
						updateData.UnitData.MaxPower[powerSlot2] = updates[UNIT_FIELD_MAXPOWER1 + i5].Int32Value;
					}
					if (i5 == 3)
					{
						powerSlot2 = ClassPowerTypes.GetPowerSlotForClass(classId2, PowerType.ComboPoints);
						if (powerSlot2 >= 0)
						{
							updateData.UnitData.MaxPower[powerSlot2] = 5;
						}
					}
				}
			}
			var UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER);
		if (UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER >= 0)
		{
			for (var iPR = 0; iPR < 7; iPR++)
			{
				if (updateMaskArray[UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER + iPR])
				{
					updateData.UnitData.ModPowerRegen[iPR] = updates[UNIT_FIELD_POWER_REGEN_FLAT_MODIFIER + iPR].FloatValue;
				}
			}
		}
		var UNIT_VIRTUAL_ITEM_SLOT_DISPLAY = LegacyVersion.GetUpdateField(UnitField.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY);
			if (UNIT_VIRTUAL_ITEM_SLOT_DISPLAY >= 0)
			{
				for (var i6 = 0; i6 < 3; i6++)
				{
					if (updateMaskArray[UNIT_VIRTUAL_ITEM_SLOT_DISPLAY + i6])
					{
						var itemDisplayId = updates[UNIT_VIRTUAL_ITEM_SLOT_DISPLAY + i6].UInt32Value;
						var itemId2 = GameData.GetItemIdWithDisplayId(itemDisplayId);
						if (itemId2 != 0)
						{
							var visibleItem = new VisibleItem
							{
								ItemID = (int)itemId2
							};
							updateData.UnitData.VirtualItems[i6] = visibleItem;
						}
					}
				}
			}
			var UNIT_VIRTUAL_ITEM_SLOT_ID = LegacyVersion.GetUpdateField(UnitField.UNIT_VIRTUAL_ITEM_SLOT_ID);
			if (UNIT_VIRTUAL_ITEM_SLOT_ID >= 0)
			{
				for (var i7 = 0; i7 < 3; i7++)
				{
					if (updateMaskArray[UNIT_VIRTUAL_ITEM_SLOT_ID + i7])
					{
						var visibleItem2 = new VisibleItem
						{
							ItemID = updates[UNIT_VIRTUAL_ITEM_SLOT_ID + i7].Int32Value
						};
						updateData.UnitData.VirtualItems[i7] = visibleItem2;
					}
				}
			}
			var UNIT_FIELD_FLAGS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_FLAGS);
			if (UNIT_FIELD_FLAGS >= 0 && updateMaskArray[UNIT_FIELD_FLAGS])
			{
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					var vanillaFlags = (UnitFlagsVanilla)updates[UNIT_FIELD_FLAGS].UInt32Value;
					updateData.UnitData.Flags = (uint)vanillaFlags.CastFlags<UnitFlags>();
					if (vanillaFlags.HasAnyFlag(UnitFlagsVanilla.PetRename))
					{
						if (!updateData.UnitData.PetFlags.HasValue)
						{
							updateData.UnitData.PetFlags = 1;
						}
						else
						{
							var unitData = updateData.UnitData;
							unitData.PetFlags |= 1;
						}
					}
					if (vanillaFlags.HasAnyFlag(UnitFlagsVanilla.PetAbandon))
					{
						if (!updateData.UnitData.PetFlags.HasValue)
						{
							updateData.UnitData.PetFlags = 2;
						}
						else
						{
							var unitData = updateData.UnitData;
							unitData.PetFlags |= 2;
						}
					}
				}
				else
				{
					updateData.UnitData.Flags = updates[UNIT_FIELD_FLAGS].UInt32Value;
				}
				if (updateData.UnitData.Flags.HasAnyFlag(UnitFlags.ServerControlled) && isCreate && guid == GetSession().GameState.CurrentPlayerGuid && updateData.CreateData.MoveSpline == null)
				{
					var unitData = updateData.UnitData;
					unitData.Flags &= 4294967294u;
				}
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056) && !updateData.UnitData.PvpFlags.HasValue)
				{
					updateData.UnitData.PvpFlags = ReadPvPFlags(updates);
				}
			}
			var UNIT_FIELD_FLAGS_2 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_FLAGS_2);
			if (UNIT_FIELD_FLAGS_2 >= 0 && updateMaskArray[UNIT_FIELD_FLAGS_2])
			{
				updateData.UnitData.Flags2 = updates[UNIT_FIELD_FLAGS_2].UInt32Value;
			}
			var UNIT_FIELD_AURASTATE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURASTATE);
			if (UNIT_FIELD_AURASTATE >= 0 && updateMaskArray[UNIT_FIELD_AURASTATE])
			{
				updateData.UnitData.AuraState = updates[UNIT_FIELD_AURASTATE].UInt32Value;
			}
			var UNIT_FIELD_BASEATTACKTIME = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BASEATTACKTIME);
			if (UNIT_FIELD_BASEATTACKTIME >= 0)
			{
				for (var i8 = 0; i8 < 2; i8++)
				{
					if (updateMaskArray[UNIT_FIELD_BASEATTACKTIME + i8])
					{
						updateData.UnitData.AttackRoundBaseTime[i8] = updates[UNIT_FIELD_BASEATTACKTIME + i8].UInt32Value;
					}
				}
			}
			var UNIT_FIELD_RANGEDATTACKTIME = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RANGEDATTACKTIME);
			if (UNIT_FIELD_RANGEDATTACKTIME >= 0 && updateMaskArray[UNIT_FIELD_RANGEDATTACKTIME])
			{
				updateData.UnitData.RangedAttackRoundBaseTime = updates[UNIT_FIELD_RANGEDATTACKTIME].UInt32Value;
				Log.Print(LogType.Debug, $"[UnitField] RangedAttackRoundBaseTime = {updates[UNIT_FIELD_RANGEDATTACKTIME].UInt32Value}", "HandleUpdateObject", "");
			}
			var UNIT_FIELD_BOUNDINGRADIUS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BOUNDINGRADIUS);
			if (UNIT_FIELD_BOUNDINGRADIUS >= 0 && updateMaskArray[UNIT_FIELD_BOUNDINGRADIUS])
			{
				updateData.UnitData.BoundingRadius = updates[UNIT_FIELD_BOUNDINGRADIUS].FloatValue;
			}
			var UNIT_FIELD_COMBATREACH = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_COMBATREACH);
			if (UNIT_FIELD_COMBATREACH >= 0 && updateMaskArray[UNIT_FIELD_COMBATREACH])
			{
				updateData.UnitData.CombatReach = updates[UNIT_FIELD_COMBATREACH].FloatValue;
			}
			var UNIT_FIELD_DISPLAYID = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_DISPLAYID);
			if (UNIT_FIELD_DISPLAYID >= 0 && updateMaskArray[UNIT_FIELD_DISPLAYID])
			{
				updateData.UnitData.DisplayID = updates[UNIT_FIELD_DISPLAYID].Int32Value;
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					updateData.UnitData.DisplayScale = 1f / GameData.GetUnitCompleteDisplayScale((uint)updateData.UnitData.DisplayID.Value);
				}
			}
			var UNIT_FIELD_NATIVEDISPLAYID = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_NATIVEDISPLAYID);
			if (UNIT_FIELD_NATIVEDISPLAYID >= 0 && updateMaskArray[UNIT_FIELD_NATIVEDISPLAYID])
			{
				updateData.UnitData.NativeDisplayID = updates[UNIT_FIELD_NATIVEDISPLAYID].Int32Value;
			}
			var UNIT_FIELD_MOUNTDISPLAYID = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MOUNTDISPLAYID);
			if (UNIT_FIELD_MOUNTDISPLAYID >= 0 && updateMaskArray[UNIT_FIELD_MOUNTDISPLAYID])
			{
				updateData.UnitData.MountDisplayID = updates[UNIT_FIELD_MOUNTDISPLAYID].Int32Value;
			}
			var UNIT_FIELD_MINDAMAGE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MINDAMAGE);
			if (UNIT_FIELD_MINDAMAGE >= 0 && updateMaskArray[UNIT_FIELD_MINDAMAGE])
			{
				updateData.UnitData.MinDamage = updates[UNIT_FIELD_MINDAMAGE].FloatValue;
			}
			var UNIT_FIELD_MAXDAMAGE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MAXDAMAGE);
			if (UNIT_FIELD_MAXDAMAGE >= 0 && updateMaskArray[UNIT_FIELD_MAXDAMAGE])
			{
				updateData.UnitData.MaxDamage = updates[UNIT_FIELD_MAXDAMAGE].FloatValue;
			}
			var UNIT_FIELD_MINOFFHANDDAMAGE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MINOFFHANDDAMAGE);
			if (UNIT_FIELD_MINOFFHANDDAMAGE >= 0 && updateMaskArray[UNIT_FIELD_MINOFFHANDDAMAGE])
			{
				updateData.UnitData.MinOffHandDamage = updates[UNIT_FIELD_MINOFFHANDDAMAGE].FloatValue;
			}
			var UNIT_FIELD_MAXOFFHANDDAMAGE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MAXOFFHANDDAMAGE);
			if (UNIT_FIELD_MAXOFFHANDDAMAGE >= 0 && updateMaskArray[UNIT_FIELD_MAXOFFHANDDAMAGE])
			{
				updateData.UnitData.MaxOffHandDamage = updates[UNIT_FIELD_MAXOFFHANDDAMAGE].FloatValue;
			}
			var UNIT_FIELD_BYTES_1 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BYTES_1);
			if (UNIT_FIELD_BYTES_1 >= 0 && updateMaskArray[UNIT_FIELD_BYTES_1])
			{
				updateData.UnitData.StandState = (byte)(updates[UNIT_FIELD_BYTES_1].UInt32Value & 0xFF);
				var petLoyaltyIndex = (byte)((updates[UNIT_FIELD_BYTES_1].UInt32Value >> 8) & 0xFF);
				if (petLoyaltyIndex != 238)
				{
					updateData.UnitData.PetLoyaltyIndex = petLoyaltyIndex;
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
				{
					updateData.UnitData.VisFlags = (byte)((updates[UNIT_FIELD_BYTES_1].UInt32Value >> 16) & 0xFF);
					updateData.UnitData.AnimTier = (byte)((updates[UNIT_FIELD_BYTES_1].UInt32Value >> 24) & 0xFF);
				}
				else
				{
					updateData.UnitData.ShapeshiftForm = (byte)((updates[UNIT_FIELD_BYTES_1].UInt32Value >> 16) & 0xFF);
					updateData.UnitData.VisFlags = (byte)((updates[UNIT_FIELD_BYTES_1].UInt32Value >> 24) & 0xFF);
				}
			}
			var UNIT_FIELD_PETNUMBER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_PETNUMBER);
			if (UNIT_FIELD_PETNUMBER >= 0 && updateMaskArray[UNIT_FIELD_PETNUMBER])
			{
				updateData.UnitData.PetNumber = updates[UNIT_FIELD_PETNUMBER].UInt32Value;
			}
			var UNIT_FIELD_PET_NAME_TIMESTAMP = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_PET_NAME_TIMESTAMP);
			if (UNIT_FIELD_PET_NAME_TIMESTAMP >= 0 && updateMaskArray[UNIT_FIELD_PET_NAME_TIMESTAMP])
			{
				updateData.UnitData.PetNameTimestamp = updates[UNIT_FIELD_PET_NAME_TIMESTAMP].UInt32Value;
			}
			var UNIT_FIELD_PETEXPERIENCE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_PETEXPERIENCE);
			if (UNIT_FIELD_PETEXPERIENCE >= 0 && updateMaskArray[UNIT_FIELD_PETEXPERIENCE])
			{
				updateData.UnitData.PetExperience = updates[UNIT_FIELD_PETEXPERIENCE].UInt32Value;
			}
			var UNIT_FIELD_PETNEXTLEVELEXP = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_PETNEXTLEVELEXP);
			if (UNIT_FIELD_PETNEXTLEVELEXP >= 0 && updateMaskArray[UNIT_FIELD_PETNEXTLEVELEXP])
			{
				updateData.UnitData.PetNextLevelExperience = updates[UNIT_FIELD_PETNEXTLEVELEXP].UInt32Value;
			}
			var UNIT_DYNAMIC_FLAGS = LegacyVersion.GetUpdateField(UnitField.UNIT_DYNAMIC_FLAGS);
			if (UNIT_DYNAMIC_FLAGS >= 0 && updateMaskArray[UNIT_DYNAMIC_FLAGS])
			{
				var flags = (UnitDynamicFlagsLegacy)updates[UNIT_DYNAMIC_FLAGS].UInt32Value;
				if (flags.HasFlag(UnitDynamicFlagsLegacy.Tapped) && flags.HasFlag(UnitDynamicFlagsLegacy.TappedByPlayer))
				{
					flags = (UnitDynamicFlagsLegacy)((uint)flags & 0xFFFFFFF3u);
				}
				updateData.ObjectData.DynamicFlags = (uint)flags.CastFlags<UnitDynamicFlagsModern>();
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					if (!updateData.UnitData.Flags2.HasValue)
					{
						updateData.UnitData.Flags2 = 2048u;
					}
					if (flags.HasAnyFlag(UnitDynamicFlagsLegacy.AppearDead))
					{
						var unitData = updateData.UnitData;
						unitData.Flags2 |= 1u;
					}
				}
			}
			var UNIT_CHANNEL_SPELL = LegacyVersion.GetUpdateField(UnitField.UNIT_CHANNEL_SPELL);
			if (UNIT_CHANNEL_SPELL >= 0 && updateMaskArray[UNIT_CHANNEL_SPELL])
			{
				var channelSpellId = updates[UNIT_CHANNEL_SPELL].Int32Value;
				if (channelSpellId == 0)
				{
					GetSession().GameState.CurrentChanneledSpellId = 0;
					// Don't write ChannelData with SpellID=0 — SMSG_SPELL_CHANNEL_UPDATE
					// handles channel end. Writing it causes the 3.4.3 client to get stuck.
				}
				else
				{
					// Write ChannelData for active channels — the client needs ChannelObject
					// (bobber GUID) to identify the fishing bobber for interaction.
					var channel = new UnitChannel
					{
						SpellID = channelSpellId,
						SpellXSpellVisualID = (int)GameData.GetSpellVisual((uint)channelSpellId)
					};
					updateData.UnitData.ChannelData = channel;
				}
			}
			var UNIT_MOD_CAST_SPEED = LegacyVersion.GetUpdateField(UnitField.UNIT_MOD_CAST_SPEED);
			if (UNIT_MOD_CAST_SPEED >= 0 && updateMaskArray[UNIT_MOD_CAST_SPEED])
			{
				updateData.UnitData.ModCastSpeed = updates[UNIT_MOD_CAST_SPEED].FloatValue;
			}
			var UNIT_CREATED_BY_SPELL = LegacyVersion.GetUpdateField(UnitField.UNIT_CREATED_BY_SPELL);
			if (UNIT_CREATED_BY_SPELL >= 0 && updateMaskArray[UNIT_CREATED_BY_SPELL])
			{
				updateData.UnitData.CreatedBySpell = updates[UNIT_CREATED_BY_SPELL].Int32Value;
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180) && isCreate && updateData.UnitData.CreatedBy == GetSession().GameState.CurrentPlayerGuid)
				{
					var totemSlot = GameData.GetTotemSlotForSpell((uint)updateData.UnitData.CreatedBySpell.Value);
					if (totemSlot >= 0)
					{
						var totem = new TotemCreated
						{
							Slot = (byte)totemSlot,
							Totem = guid,
							Duration = 120000u,
							SpellId = (uint)updateData.UnitData.CreatedBySpell.Value,
							CannotDismiss = true
						};
						SendPacketToClient(totem);
					}
				}
			}
			var UNIT_NPC_FLAGS = LegacyVersion.GetUpdateField(UnitField.UNIT_NPC_FLAGS);
			if (UNIT_NPC_FLAGS >= 0 && updateMaskArray[UNIT_NPC_FLAGS])
			{
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					var vanillaFlags2 = (NPCFlagsVanilla)updates[UNIT_NPC_FLAGS].UInt32Value;
					updateData.UnitData.NpcFlags[0] = (uint)vanillaFlags2.CastFlags<NPCFlags>();
				}
				else
				{
					updateData.UnitData.NpcFlags[0] = updates[UNIT_NPC_FLAGS].UInt32Value;
				}
			}
			var UNIT_NPC_EMOTESTATE = LegacyVersion.GetUpdateField(UnitField.UNIT_NPC_EMOTESTATE);
			if (UNIT_NPC_EMOTESTATE >= 0 && updateMaskArray[UNIT_NPC_EMOTESTATE])
			{
				updateData.UnitData.EmoteState = updates[UNIT_NPC_EMOTESTATE].Int32Value;
			}
			var UNIT_TRAINING_POINTS = LegacyVersion.GetUpdateField(UnitField.UNIT_TRAINING_POINTS);
			if (UNIT_TRAINING_POINTS >= 0 && updateMaskArray[UNIT_TRAINING_POINTS])
			{
				updateData.UnitData.TrainingPointsUsed = (ushort)(updates[UNIT_TRAINING_POINTS].UInt32Value & 0xFFFF);
				updateData.UnitData.TrainingPointsTotal = (ushort)((updates[UNIT_TRAINING_POINTS].UInt32Value >> 16) & 0xFFFF);
			}
			var UNIT_FIELD_STAT0 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_STAT0);
			if (UNIT_FIELD_STAT0 >= 0)
			{
				for (var i9 = 0; i9 < 5; i9++)
				{
					if (updateMaskArray[UNIT_FIELD_STAT0 + i9])
					{
						updateData.UnitData.Stats[i9] = updates[UNIT_FIELD_STAT0 + i9].Int32Value;
					}
				}
			}
			var UNIT_FIELD_POSSTAT0 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_POSSTAT0);
			if (UNIT_FIELD_POSSTAT0 >= 0)
			{
				for (var i10 = 0; i10 < 5; i10++)
				{
					if (updateMaskArray[UNIT_FIELD_POSSTAT0 + i10])
					{
						updateData.UnitData.StatPosBuff[i10] = updates[UNIT_FIELD_POSSTAT0 + i10].Int32Value;
					}
				}
			}
			var UNIT_FIELD_NEGSTAT0 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_NEGSTAT0);
			if (UNIT_FIELD_NEGSTAT0 >= 0)
			{
				for (var i11 = 0; i11 < 5; i11++)
				{
					if (updateMaskArray[UNIT_FIELD_NEGSTAT0 + i11])
					{
						updateData.UnitData.StatNegBuff[i11] = updates[UNIT_FIELD_NEGSTAT0 + i11].Int32Value;
					}
				}
			}
			// 3.3.5a uses individual names (RESISTANCES_ARMOR=99..RESISTANCES_ARCANE=105)
			// not the generic UNIT_FIELD_RESISTANCES. Fall back to _ARMOR as base.
			var UNIT_FIELD_RESISTANCES = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RESISTANCES);
			if (UNIT_FIELD_RESISTANCES < 0)
				UNIT_FIELD_RESISTANCES = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RESISTANCES_ARMOR);
			if (UNIT_FIELD_RESISTANCES >= 0)
			{
				for (var i12 = 0; i12 < 7; i12++)
				{
					if (updateMaskArray[UNIT_FIELD_RESISTANCES + i12])
					{
						updateData.UnitData.Resistances[i12] = updates[UNIT_FIELD_RESISTANCES + i12].Int32Value;
					}
				}
			}
			var UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE);
			if (UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE < 0)
				UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_ARMOR);
			if (UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE >= 0)
			{
				for (var i13 = 0; i13 < 7; i13++)
				{
					if (updateMaskArray[UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + i13])
					{
						updateData.UnitData.ResistanceBuffModsPositive[i13] = updates[UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE + i13].Int32Value;
					}
				}
			}
			var UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE);
			if (UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE < 0)
				UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_ARMOR);
			if (UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE >= 0)
			{
				for (var i14 = 0; i14 < 7; i14++)
				{
					if (updateMaskArray[UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + i14])
					{
						updateData.UnitData.ResistanceBuffModsNegative[i14] = updates[UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE + i14].Int32Value;
					}
				}
			}
			var UNIT_FIELD_BASE_MANA = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BASE_MANA);
			if (UNIT_FIELD_BASE_MANA >= 0 && updateMaskArray[UNIT_FIELD_BASE_MANA])
			{
				updateData.UnitData.BaseMana = updates[UNIT_FIELD_BASE_MANA].Int32Value;
			}
			var UNIT_FIELD_BASE_HEALTH = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BASE_HEALTH);
			if (UNIT_FIELD_BASE_HEALTH >= 0 && updateMaskArray[UNIT_FIELD_BASE_HEALTH])
			{
				updateData.UnitData.BaseHealth = updates[UNIT_FIELD_BASE_HEALTH].Int32Value;
			}
			var UNIT_FIELD_BYTES_2 = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_BYTES_2);
			if (UNIT_FIELD_BYTES_2 >= 0 && updateMaskArray[UNIT_FIELD_BYTES_2])
			{
				updateData.UnitData.SheatheState = (byte)(updates[UNIT_FIELD_BYTES_2].UInt32Value & 0xFF);
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
				{
					updateData.UnitData.PvpFlags = (byte)((updates[UNIT_FIELD_BYTES_2].UInt32Value >> 8) & 0xFF);
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					updateData.UnitData.PetFlags = (byte)((updates[UNIT_FIELD_BYTES_2].UInt32Value >> 16) & 0xFF);
				}
				if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
				{
					updateData.UnitData.ShapeshiftForm = (byte)((updates[UNIT_FIELD_BYTES_2].UInt32Value >> 24) & 0xFF);
				}
			}
			var UNIT_FIELD_ATTACK_POWER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_ATTACK_POWER);
			if (UNIT_FIELD_ATTACK_POWER >= 0 && updateMaskArray[UNIT_FIELD_ATTACK_POWER])
			{
				updateData.UnitData.AttackPower = updates[UNIT_FIELD_ATTACK_POWER].Int32Value;
			}
			var UNIT_FIELD_ATTACK_POWER_MODS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_ATTACK_POWER_MODS);
			if (UNIT_FIELD_ATTACK_POWER_MODS >= 0 && updateMaskArray[UNIT_FIELD_ATTACK_POWER_MODS])
			{
				updateData.UnitData.AttackPowerModNeg = updates[UNIT_FIELD_ATTACK_POWER_MODS].Int32Value & 0xFFFF;
				updateData.UnitData.AttackPowerModPos = (updates[UNIT_FIELD_ATTACK_POWER_MODS].Int32Value >> 16) & 0xFFFF;
			}
			var UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER);
			if (UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER >= 0 && updateMaskArray[UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER])
			{
				updateData.UnitData.AttackPowerMultiplier = updates[UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER].FloatValue;
			}
			var UNIT_FIELD_MINRANGEDDAMAGE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MINRANGEDDAMAGE);
			if (UNIT_FIELD_MINRANGEDDAMAGE >= 0 && updateMaskArray[UNIT_FIELD_MINRANGEDDAMAGE])
			{
				updateData.UnitData.MinRangedDamage = updates[UNIT_FIELD_MINRANGEDDAMAGE].FloatValue;
			}
			var UNIT_FIELD_MAXRANGEDDAMAGE = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MAXRANGEDDAMAGE);
			if (UNIT_FIELD_MAXRANGEDDAMAGE >= 0 && updateMaskArray[UNIT_FIELD_MAXRANGEDDAMAGE])
			{
				updateData.UnitData.MaxRangedDamage = updates[UNIT_FIELD_MAXRANGEDDAMAGE].FloatValue;
			}
			var UNIT_FIELD_POWER_COST_MODIFIER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_POWER_COST_MODIFIER);
			if (UNIT_FIELD_POWER_COST_MODIFIER >= 0)
			{
				for (var i15 = 0; i15 < 7; i15++)
				{
					if (updateMaskArray[UNIT_FIELD_POWER_COST_MODIFIER + i15])
					{
						updateData.UnitData.PowerCostModifier[i15] = updates[UNIT_FIELD_POWER_COST_MODIFIER + i15].Int32Value;
					}
				}
			}
			var UNIT_FIELD_POWER_COST_MULTIPLIER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_POWER_COST_MULTIPLIER);
			if (UNIT_FIELD_POWER_COST_MULTIPLIER >= 0)
			{
				for (var i16 = 0; i16 < 7; i16++)
				{
					if (updateMaskArray[UNIT_FIELD_POWER_COST_MULTIPLIER + i16])
					{
						updateData.UnitData.PowerCostMultiplier[i16] = updates[UNIT_FIELD_POWER_COST_MULTIPLIER + i16].FloatValue;
					}
				}
			}
			var UNIT_FIELD_MAXHEALTHMODIFIER = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_MAXHEALTHMODIFIER);
			if (UNIT_FIELD_MAXHEALTHMODIFIER >= 0 && updateMaskArray[UNIT_FIELD_MAXHEALTHMODIFIER])
			{
				updateData.UnitData.MaxHealthModifier = updates[UNIT_FIELD_MAXHEALTHMODIFIER].FloatValue;
			}
			var UNIT_FIELD_AURA = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURA);
			var UNIT_FIELD_AURAFLAGS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURAFLAGS);
			var UNIT_FIELD_AURALEVELS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURALEVELS);
			var UNIT_FIELD_AURAAPPLICATIONS = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_AURAAPPLICATIONS);
			if (UNIT_FIELD_AURA > 0 && UNIT_FIELD_AURAFLAGS > 0 && UNIT_FIELD_AURALEVELS > 0 && UNIT_FIELD_AURAAPPLICATIONS > 0)
			{
				var aurasCount = LegacyVersion.GetAuraSlotsCount();
				for (byte i17 = 0; i17 < aurasCount; i17++)
				{
					if (!updateMaskArray[UNIT_FIELD_AURA + i17] && !updateMaskArray[UNIT_FIELD_AURALEVELS + i17 / 4] && !updateMaskArray[UNIT_FIELD_AURAAPPLICATIONS + i17 / 4])
					{
						continue;
					}
					var aura = new AuraInfo
					{
						Slot = i17,
						AuraData = ReadAuraSlot(i17, guid, updates)
					};
					if (aura.AuraData != null)
					{
						GetSession().GameState.GetAuraDuration(guid, i17, out var durationLeft, out var durationFull);
						if (durationLeft > 0 && durationFull > 0)
						{
							var auraData = aura.AuraData;
							auraData.Flags |= AuraFlagsModern.Duration;
							aura.AuraData.Duration = durationFull;
							aura.AuraData.Remaining = durationLeft;
						}
						aura.AuraData.CastUnit = GetSession().GameState.GetAuraCaster(guid, i17, aura.AuraData.SpellID);
					}
					else if (updateMaskArray[UNIT_FIELD_AURA + i17])
					{
						GetSession().GameState.ClearAuraDuration(guid, i17);
						GetSession().GameState.ClearAuraCaster(guid, i17);
					}
					if (aura.AuraData != null || updateMaskArray[UNIT_FIELD_AURA + i17])
					{
						auraUpdate.Auras.Add(aura);
					}
				}
			}
		}
		if (objectType == ObjectType.Player || objectType == ObjectType.ActivePlayer)
		{
			var PLAYER_DUEL_ARBITER = LegacyVersion.GetUpdateField(PlayerField.PLAYER_DUEL_ARBITER);
			if (PLAYER_DUEL_ARBITER >= 0 && updateMaskArray[PLAYER_DUEL_ARBITER])
			{
				updateData.PlayerData.DuelArbiter = GetGuidValue(updates, PlayerField.PLAYER_DUEL_ARBITER).To128(GetSession().GameState);
			}
			var PLAYER_FLAGS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FLAGS);
			if (PLAYER_FLAGS >= 0 && updateMaskArray[PLAYER_FLAGS])
			{
				var legacyFlags = (PlayerFlagsLegacy)updates[PLAYER_FLAGS].UInt32Value;
				var flags2 = legacyFlags.CastFlags<PlayerFlags>();
				if (updateData.Guid == GetSession().GameState.CurrentPlayerGuid)
				{
					GetSession().GameState.CurrentPlayerStorage.Settings.PatchFlags(ref flags2);
					// Detect ghost→alive transition for DestroyObject+CreateObject2 revive fix
					var isGhostNow = legacyFlags.HasAnyFlag(PlayerFlagsLegacy.Ghost);
					if (GetSession().GameState.IsPlayerGhost && !isGhostNow)
					{
						Log.Print(LogType.Debug, "[DeathRevive] Ghost→Alive transition detected, will recreate player object", "StoreObjectUpdate", "");
						GetSession().GameState.NeedPlayerRecreate = true;
					}
					GetSession().GameState.IsPlayerGhost = isGhostNow;
				}
				// Ghost flag now sent through — DestroyObject+CreateObject2 on revive clears the grey overlay
				updateData.PlayerData.PlayerFlags = (uint)flags2;
				if (!updateData.PlayerData.PlayerFlagsEx.HasValue)
				{
					updateData.PlayerData.PlayerFlagsEx = 0u;
				}
				// 3.4.3 uses ActivePlayerData.LocalFlags for death UI (RELEASE_TIMER=0x08)
				// 3.3.5a doesn't have this — only inject when actually ghost
				// Setting LocalFlags=0 on every update floods ActivePlayerData and may break bag updates
				if (updateData.Guid == GetSession().GameState.CurrentPlayerGuid
					&& legacyFlags.HasAnyFlag(PlayerFlagsLegacy.Ghost))
				{
					updateData.ActivePlayerData.LocalFlags = 0x08u; // PLAYER_LOCAL_FLAG_RELEASE_TIMER
				}
				if (legacyFlags.HasAnyFlag(PlayerFlagsLegacy.HideHelm))
				{
					var playerData = updateData.PlayerData;
					playerData.PlayerFlagsEx |= 128u;
				}
				if (legacyFlags.HasAnyFlag(PlayerFlagsLegacy.HideCloak))
				{
					var playerData = updateData.PlayerData;
					playerData.PlayerFlagsEx |= 256u;
				}
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056) && !updateData.UnitData.PvpFlags.HasValue)
				{
					updateData.UnitData.PvpFlags = ReadPvPFlags(updates);
				}
			}
			else if (updateData.Guid == GetSession().GameState.CurrentPlayerGuid && GetSession().GameState.CurrentPlayerStorage.Settings.NeedToForcePatchFlags)
			{
				var flags3 = GetSession().GameState.CurrentPlayerStorage.Settings.CreateNewFlags();
				updateData.PlayerData.PlayerFlags = (uint)flags3;
			}
			var PLAYER_GUILDID = LegacyVersion.GetUpdateField(PlayerField.PLAYER_GUILDID);
			if (PLAYER_GUILDID >= 0 && updateMaskArray[PLAYER_GUILDID])
			{
				GetSession().GameState.StorePlayerGuildId(guid, updates[PLAYER_GUILDID].UInt32Value);
				updateData.UnitData.GuildGUID = WowGuid128.Create(HighGuidType703.Guild, updates[PLAYER_GUILDID].UInt32Value);
			}
			var PLAYER_GUILDRANK = LegacyVersion.GetUpdateField(PlayerField.PLAYER_GUILDRANK);
			if (PLAYER_GUILDRANK >= 0 && updateMaskArray[PLAYER_GUILDRANK])
			{
				updateData.PlayerData.GuildLevel = 25;
				updateData.PlayerData.GuildRankID = updates[PLAYER_GUILDRANK].UInt32Value;
			}
			var PLAYER_GUILD_TIMESTAMP = LegacyVersion.GetUpdateField(PlayerField.PLAYER_GUILD_TIMESTAMP);
			if (PLAYER_GUILD_TIMESTAMP >= 0 && updateMaskArray[PLAYER_GUILD_TIMESTAMP])
			{
				updateData.PlayerData.GuildTimeStamp = updates[PLAYER_GUILD_TIMESTAMP].Int32Value;
			}
			var PLAYER_QUEST_LOG_1_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_QUEST_LOG_1_1);
			if (PLAYER_QUEST_LOG_1_1 >= 0)
			{
				var questsCount = LegacyVersion.GetQuestLogSize();
				for (var i18 = 0; i18 < questsCount; i18++)
				{
					updateData.PlayerData.QuestLog[i18] = ReadQuestLogEntry(i18, updateMaskArray, updates);
				}
			}
			var PLAYER_CHOSEN_TITLE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_CHOSEN_TITLE);
			if (PLAYER_CHOSEN_TITLE >= 0 && updateMaskArray[PLAYER_CHOSEN_TITLE])
			{
				updateData.PlayerData.ChosenTitle = updates[PLAYER_CHOSEN_TITLE].Int32Value;
			}
			var PLAYER_VISIBLE_ITEM_1_0 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_0);
			if (PLAYER_VISIBLE_ITEM_1_0 >= 0)
			{
				var offset = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 16 : 12);
				for (var i19 = 0; i19 < 19; i19++)
				{
					var itemIdIndex = PLAYER_VISIBLE_ITEM_1_0 + i19 * offset;
					var enchantIdIndex = PLAYER_VISIBLE_ITEM_1_0 + 1 + i19 * offset;
					if (updateMaskArray[itemIdIndex] || updateMaskArray[enchantIdIndex])
					{
						updateData.PlayerData.VisibleItems[i19] = new VisibleItem();
						if (updates.ContainsKey(itemIdIndex))
						{
							updateData.PlayerData.VisibleItems[i19].ItemID = updates[itemIdIndex].Int32Value;
						}
						if (updates.ContainsKey(enchantIdIndex))
						{
							updateData.PlayerData.VisibleItems[i19].ItemVisual = (ushort)GameData.GetItemEnchantVisual(updates[enchantIdIndex].UInt32Value);
						}
					}
				}
			}
			var PLAYER_VISIBLE_ITEM_1_ENTRYID = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_ENTRYID);
			if (PLAYER_VISIBLE_ITEM_1_ENTRYID >= 0)
			{
				var offset2 = 2;
				for (var i20 = 0; i20 < 19; i20++)
				{
					if (updateMaskArray[PLAYER_VISIBLE_ITEM_1_ENTRYID + i20 * offset2])
					{
						updateData.PlayerData.VisibleItems[i20] = new VisibleItem
						{
							ItemID = updates[PLAYER_VISIBLE_ITEM_1_ENTRYID + i20 * offset2].Int32Value
						};
						if (i20 >= 15 && i20 <= 18)
							Log.Print(LogType.Debug, $"[VisibleItem] Slot {i20} ItemID={updateData.PlayerData.VisibleItems[i20].ItemID}", "HandleUpdateObject", "");
					}
				}
			}
			var PLAYER_FIELD_INV_SLOT_HEAD = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_INV_SLOT_HEAD);
			if (PLAYER_FIELD_INV_SLOT_HEAD >= 0)
			{
				for (var i21 = 0; i21 < 23; i21++)
				{
					if (updateMaskArray[PLAYER_FIELD_INV_SLOT_HEAD + i21 * 2])
					{
						updateData.ActivePlayerData.InvSlots[i21] = GetGuidValue(updates, PLAYER_FIELD_INV_SLOT_HEAD + i21 * 2).To128(GetSession().GameState);
						if (i21 >= 15 && i21 <= 18)
							Log.Print(LogType.Debug, $"[InvSlot] Slot {i21} = {updateData.ActivePlayerData.InvSlots[i21]}", "HandleUpdateObject", "");
					}
				}
			}
			var PLAYER_FIELD_PACK_SLOT_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_PACK_SLOT_1);
			if (PLAYER_FIELD_PACK_SLOT_1 >= 0)
			{
				for (var i22 = 0; i22 < 16; i22++)
				{
					if (updateMaskArray[PLAYER_FIELD_PACK_SLOT_1 + i22 * 2])
					{
						updateData.ActivePlayerData.PackSlots[i22] = GetGuidValue(updates, PLAYER_FIELD_PACK_SLOT_1 + i22 * 2).To128(GetSession().GameState);
						Log.Print(LogType.Debug, $"[InvUpdate] PackSlot[{i22}] = {updateData.ActivePlayerData.PackSlots[i22]} (modern idx {35 + i22})", "HandleUpdateObject", "");
					}
				}
			}
			var PLAYER_FIELD_BANK_SLOT_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BANK_SLOT_1);
			if (PLAYER_FIELD_BANK_SLOT_1 >= 0)
			{
				var bankSlots = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 28 : 24);
				for (var i23 = 0; i23 < bankSlots; i23++)
				{
					if (updateMaskArray[PLAYER_FIELD_BANK_SLOT_1 + i23 * 2])
					{
						updateData.ActivePlayerData.BankSlots[i23] = GetGuidValue(updates, PLAYER_FIELD_BANK_SLOT_1 + i23 * 2).To128(GetSession().GameState);
						Log.Print(LogType.Debug, $"[InvUpdate] BankSlot[{i23}] = {updateData.ActivePlayerData.BankSlots[i23]} (modern idx {59 + i23})", "HandleUpdateObject", "");
					}
				}
			}
			var PLAYER_FIELD_BANKBAG_SLOT_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BANKBAG_SLOT_1);
			if (PLAYER_FIELD_BANKBAG_SLOT_1 >= 0)
			{
				var bankBagSlots = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 7 : 6);
				for (var i24 = 0; i24 < bankBagSlots; i24++)
				{
					if (updateMaskArray[PLAYER_FIELD_BANKBAG_SLOT_1 + i24 * 2])
					{
						updateData.ActivePlayerData.BankBagSlots[i24] = GetGuidValue(updates, PLAYER_FIELD_BANKBAG_SLOT_1 + i24 * 2).To128(GetSession().GameState);
					}
				}
			}
			var PLAYER_FIELD_VENDORBUYBACK_SLOT_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_VENDORBUYBACK_SLOT_1);
			if (PLAYER_FIELD_VENDORBUYBACK_SLOT_1 >= 0)
			{
				for (var i25 = 0; i25 < 12; i25++)
				{
					if (updateMaskArray[PLAYER_FIELD_VENDORBUYBACK_SLOT_1 + i25 * 2])
					{
						updateData.ActivePlayerData.BuyBackSlots[i25] = GetGuidValue(updates, PLAYER_FIELD_VENDORBUYBACK_SLOT_1 + i25 * 2).To128(GetSession().GameState);
					}
				}
			}
			var PLAYER_FIELD_KEYRING_SLOT_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_KEYRING_SLOT_1);
			if (PLAYER_FIELD_KEYRING_SLOT_1 >= 0)
			{
				for (var i26 = 0; i26 < 32; i26++)
				{
					if (updateMaskArray[PLAYER_FIELD_KEYRING_SLOT_1 + i26 * 2])
					{
						updateData.ActivePlayerData.KeyringSlots[i26] = GetGuidValue(updates, PLAYER_FIELD_KEYRING_SLOT_1 + i26 * 2).To128(GetSession().GameState);
					}
				}
			}
			byte? skin = null;
			byte? face = null;
			byte? hairStyle = null;
			byte? hairColor = null;
			byte? facialHair = null;
			var PLAYER_BYTES = LegacyVersion.GetUpdateField(PlayerField.PLAYER_BYTES);
			if (PLAYER_BYTES >= 0 && updateMaskArray[PLAYER_BYTES])
			{
				skin = (byte)(updates[PLAYER_BYTES].UInt32Value & 0xFF);
				face = (byte)((updates[PLAYER_BYTES].UInt32Value >> 8) & 0xFF);
				hairStyle = (byte)((updates[PLAYER_BYTES].UInt32Value >> 16) & 0xFF);
				hairColor = (byte)((updates[PLAYER_BYTES].UInt32Value >> 24) & 0xFF);
			}
			var restInfo = ((isCreate && guid == GetSession().GameState.CurrentPlayerGuid) ? new RestInfo() : null);
			if (restInfo != null)
			{
				restInfo.StateID = 2u;
			}
			var PLAYER_BYTES_2 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_BYTES_2);
			if (PLAYER_BYTES_2 >= 0 && updateMaskArray[PLAYER_BYTES_2])
			{
				facialHair = (byte)(updates[PLAYER_BYTES_2].UInt32Value & 0xFF);
				updateData.PlayerData.NumBankSlots = (byte)((updates[PLAYER_BYTES_2].UInt32Value >> 16) & 0xFF);
				if (restInfo == null && guid == GetSession().GameState.CurrentPlayerGuid)
				{
					restInfo = new RestInfo();
				}
				if (restInfo != null)
				{
					restInfo.StateID = (byte)((updates[PLAYER_BYTES_2].UInt32Value >> 24) & 0xFF);
				}
			}
			if (skin.HasValue && face.HasValue && hairStyle.HasValue && hairColor.HasValue && facialHair.HasValue)
			{
				var raceId = Race.None;
				var sexId = Gender.None;
				if (updateData.UnitData.RaceId.HasValue)
				{
					raceId = (Race)updateData.UnitData.RaceId.Value;
				}
				if (updateData.UnitData.SexId.HasValue)
				{
					sexId = (Gender)updateData.UnitData.SexId.Value;
				}
				if ((raceId == Race.None || sexId == Gender.None) && GetSession().GameState.CachedPlayers.TryGetValue(guid.To128(GetSession().GameState), out var cache))
				{
					raceId = cache.RaceId;
					sexId = cache.SexId;
				}
				if (raceId != Race.None && sexId != Gender.None)
				{
					var customizations = CharacterCustomizations.ConvertLegacyCustomizationsToModern(raceId, sexId, skin.Value, face.Value, hairStyle.Value, hairColor.Value, facialHair.Value);
					for (var i27 = 0; i27 < 5; i27++)
					{
						updateData.PlayerData.Customizations[i27] = customizations[i27];
					}
				}
			}
			var PLAYER_REST_STATE_EXPERIENCE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_REST_STATE_EXPERIENCE);
			if (PLAYER_REST_STATE_EXPERIENCE >= 0 && updateMaskArray[PLAYER_REST_STATE_EXPERIENCE])
			{
				if (restInfo == null && guid == GetSession().GameState.CurrentPlayerGuid)
				{
					restInfo = new RestInfo();
				}
				if (restInfo != null)
				{
					restInfo.Threshold = updates[PLAYER_REST_STATE_EXPERIENCE].UInt32Value;
				}
			}
			if (restInfo != null)
			{
				updateData.ActivePlayerData.RestInfo[0] = restInfo;
			}
			var PLAYER_BYTES_3 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_BYTES_3);
			if (PLAYER_BYTES_3 >= 0 && updateMaskArray[PLAYER_BYTES_3])
			{
				var genderAndInebriation = (ushort)(updates[PLAYER_BYTES_3].UInt32Value & 0xFFFF);
				updateData.PlayerData.NativeSex = (byte)(genderAndInebriation & 1);
				updateData.PlayerData.Inebriation = (byte)(genderAndInebriation & 0xFFFE);
				updateData.PlayerData.PvpTitle = (byte)((updates[PLAYER_BYTES_3].UInt32Value >> 16) & 0xFF);
				updateData.PlayerData.PvPRank = (byte)((updates[PLAYER_BYTES_3].UInt32Value >> 24) & 0xFF);
			}
			var PLAYER_DUEL_TEAM = LegacyVersion.GetUpdateField(PlayerField.PLAYER_DUEL_TEAM);
			if (PLAYER_DUEL_TEAM >= 0 && updateMaskArray[PLAYER_DUEL_TEAM])
			{
				updateData.PlayerData.DuelTeam = updates[PLAYER_DUEL_TEAM].UInt32Value;
			}
			var PLAYER_FARSIGHT = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FARSIGHT);
			if (PLAYER_FARSIGHT >= 0 && updateMaskArray[PLAYER_FARSIGHT])
			{
				updateData.ActivePlayerData.FarsightObject = GetGuidValue(updates, PlayerField.PLAYER_FARSIGHT).To128(GetSession().GameState);
			}
			var PLAYER_FIELD_COMBO_TARGET = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_COMBO_TARGET);
			if (PLAYER_FIELD_COMBO_TARGET >= 0 && updateMaskArray[PLAYER_FIELD_COMBO_TARGET])
			{
				updateData.ActivePlayerData.ComboTarget = GetGuidValue(updates, PlayerField.PLAYER_FIELD_COMBO_TARGET).To128(GetSession().GameState);
			}
			var PLAYER_FIELD_KNOWN_TITLES = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_KNOWN_TITLES);
			if (PLAYER_FIELD_KNOWN_TITLES >= 0)
			{
				var count = (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? 3 : 2);
				for (var i28 = 0; i28 < count; i28++)
				{
					if (updateMaskArray[PLAYER_FIELD_KNOWN_TITLES + i28])
					{
						updateData.ActivePlayerData.KnownTitles[i28] = updates[PLAYER_FIELD_KNOWN_TITLES + i28].UInt32Value;
					}
				}
			}
			var PLAYER_XP = LegacyVersion.GetUpdateField(PlayerField.PLAYER_XP);
			if (PLAYER_XP >= 0 && updateMaskArray[PLAYER_XP])
			{
				updateData.ActivePlayerData.XP = updates[PLAYER_XP].Int32Value;
			}
			var PLAYER_NEXT_LEVEL_XP = LegacyVersion.GetUpdateField(PlayerField.PLAYER_NEXT_LEVEL_XP);
			if (PLAYER_NEXT_LEVEL_XP >= 0 && updateMaskArray[PLAYER_NEXT_LEVEL_XP])
			{
				updateData.ActivePlayerData.NextLevelXP = updates[PLAYER_NEXT_LEVEL_XP].Int32Value;
			}
			var PLAYER_SKILL_INFO_1_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_SKILL_INFO_1_1);
			if (PLAYER_SKILL_INFO_1_1 >= 0)
			{
				for (var i29 = 0; i29 < 128; i29++)
				{
					var idIndex = PLAYER_SKILL_INFO_1_1 + i29 * 3;
					if (updateMaskArray[idIndex])
					{
						updateData.ActivePlayerData.Skill.SkillLineID[i29] = (ushort)(updates[idIndex].UInt32Value & 0xFFFF);
						updateData.ActivePlayerData.Skill.SkillStep[i29] = (ushort)((updates[idIndex].UInt32Value >> 16) & 0xFFFF);
					}
					var valueIndex = idIndex + 1;
					if (updateMaskArray[valueIndex])
					{
						updateData.ActivePlayerData.Skill.SkillRank[i29] = (ushort)(updates[valueIndex].UInt32Value & 0xFFFF);
						updateData.ActivePlayerData.Skill.SkillMaxRank[i29] = (ushort)((updates[valueIndex].UInt32Value >> 16) & 0xFFFF);
					}
					var bonusIndex = valueIndex + 1;
					if (updateMaskArray[bonusIndex])
					{
						updateData.ActivePlayerData.Skill.SkillTempBonus[i29] = (short)(updates[bonusIndex].Int32Value & 0xFFFF);
						updateData.ActivePlayerData.Skill.SkillPermBonus[i29] = (ushort)((updates[bonusIndex].UInt32Value >> 16) & 0xFFFF);
					}
				}
			}
			var PLAYER_CHARACTER_POINTS1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_CHARACTER_POINTS1);
			if (PLAYER_CHARACTER_POINTS1 >= 0 && updateMaskArray[PLAYER_CHARACTER_POINTS1])
			{
				updateData.ActivePlayerData.CharacterPoints = updates[PLAYER_CHARACTER_POINTS1].Int32Value;
				// MaxTalentTiers = total talent points (from SMSG_UPDATE_TALENT_DATA)
				var totalTalentPoints = GetSession().GameState.TotalTalentPoints;
				if (totalTalentPoints > 0)
					updateData.ActivePlayerData.MaxTalentTiers = totalTalentPoints;
				else
					updateData.ActivePlayerData.MaxTalentTiers = updates[PLAYER_CHARACTER_POINTS1].Int32Value;
			}
			var PLAYER_TRACK_CREATURES = LegacyVersion.GetUpdateField(PlayerField.PLAYER_TRACK_CREATURES);
			if (PLAYER_TRACK_CREATURES >= 0 && updateMaskArray[PLAYER_TRACK_CREATURES])
			{
				updateData.ActivePlayerData.TrackCreatureMask = updates[PLAYER_TRACK_CREATURES].UInt32Value;
			}
			var PLAYER_TRACK_RESOURCES = LegacyVersion.GetUpdateField(PlayerField.PLAYER_TRACK_RESOURCES);
			if (PLAYER_TRACK_RESOURCES >= 0 && updateMaskArray[PLAYER_TRACK_RESOURCES])
			{
				updateData.ActivePlayerData.TrackResourceMask[0] = updates[PLAYER_TRACK_RESOURCES].UInt32Value;
			}
			var PLAYER_BLOCK_PERCENTAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_BLOCK_PERCENTAGE);
			if (PLAYER_BLOCK_PERCENTAGE >= 0 && updateMaskArray[PLAYER_BLOCK_PERCENTAGE])
			{
				updateData.ActivePlayerData.BlockPercentage = updates[PLAYER_BLOCK_PERCENTAGE].FloatValue;
			}
			var PLAYER_DODGE_PERCENTAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_DODGE_PERCENTAGE);
			if (PLAYER_DODGE_PERCENTAGE >= 0 && updateMaskArray[PLAYER_DODGE_PERCENTAGE])
			{
				updateData.ActivePlayerData.DodgePercentage = updates[PLAYER_DODGE_PERCENTAGE].FloatValue;
			}
			var PLAYER_PARRY_PERCENTAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_PARRY_PERCENTAGE);
			if (PLAYER_PARRY_PERCENTAGE >= 0 && updateMaskArray[PLAYER_PARRY_PERCENTAGE])
			{
				updateData.ActivePlayerData.ParryPercentage = updates[PLAYER_PARRY_PERCENTAGE].FloatValue;
			}
			var PLAYER_EXPERTISE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_EXPERTISE);
			if (PLAYER_EXPERTISE >= 0 && updateMaskArray[PLAYER_EXPERTISE])
			{
				updateData.ActivePlayerData.MainhandExpertise = updates[PLAYER_EXPERTISE].Int32Value;
			}
			var PLAYER_OFFHAND_EXPERTISE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_OFFHAND_EXPERTISE);
			if (PLAYER_OFFHAND_EXPERTISE >= 0 && updateMaskArray[PLAYER_OFFHAND_EXPERTISE])
			{
				updateData.ActivePlayerData.OffhandExpertise = updates[PLAYER_OFFHAND_EXPERTISE].Int32Value;
			}
			var PLAYER_CRIT_PERCENTAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_CRIT_PERCENTAGE);
			if (PLAYER_CRIT_PERCENTAGE >= 0 && updateMaskArray[PLAYER_CRIT_PERCENTAGE])
			{
				updateData.ActivePlayerData.CritPercentage = updates[PLAYER_CRIT_PERCENTAGE].FloatValue;
			}
			var PLAYER_RANGED_CRIT_PERCENTAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_RANGED_CRIT_PERCENTAGE);
			if (PLAYER_RANGED_CRIT_PERCENTAGE >= 0 && updateMaskArray[PLAYER_RANGED_CRIT_PERCENTAGE])
			{
				updateData.ActivePlayerData.RangedCritPercentage = updates[PLAYER_RANGED_CRIT_PERCENTAGE].FloatValue;
			}
			var PLAYER_OFFHAND_CRIT_PERCENTAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_OFFHAND_CRIT_PERCENTAGE);
			if (PLAYER_OFFHAND_CRIT_PERCENTAGE >= 0 && updateMaskArray[PLAYER_OFFHAND_CRIT_PERCENTAGE])
			{
				updateData.ActivePlayerData.OffhandCritPercentage = updates[PLAYER_OFFHAND_CRIT_PERCENTAGE].FloatValue;
			}
			var PLAYER_SPELL_CRIT_PERCENTAGE1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_SPELL_CRIT_PERCENTAGE1);
			if (PLAYER_SPELL_CRIT_PERCENTAGE1 >= 0)
			{
				for (var i30 = 0; i30 < 7; i30++)
				{
					if (updateMaskArray[PLAYER_SPELL_CRIT_PERCENTAGE1 + i30])
					{
						updateData.ActivePlayerData.SpellCritPercentage[i30] = updates[PLAYER_SPELL_CRIT_PERCENTAGE1 + i30].FloatValue;
					}
				}
			}
			var PLAYER_SHIELD_BLOCK = LegacyVersion.GetUpdateField(PlayerField.PLAYER_SHIELD_BLOCK);
			if (PLAYER_SHIELD_BLOCK >= 0 && updateMaskArray[PLAYER_SHIELD_BLOCK])
			{
				updateData.ActivePlayerData.ShieldBlock = updates[PLAYER_SHIELD_BLOCK].Int32Value;
			}
			var PLAYER_EXPLORED_ZONES_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_EXPLORED_ZONES_1);
			if (PLAYER_EXPLORED_ZONES_1 >= 0)
			{
				var maxZones = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 128 : 64);
				for (var i31 = 0; i31 < maxZones; i31++)
				{
					if (updateMaskArray[PLAYER_EXPLORED_ZONES_1 + i31])
					{
						if ((i31 & 1) != 0)
						{
							var oldValue = (updateData.ActivePlayerData.ExploredZones[i31 / 2].HasValue ? updateData.ActivePlayerData.ExploredZones[i31 / 2].Value : 0);
							updateData.ActivePlayerData.ExploredZones[i31 / 2] = oldValue | ((ulong)updates[PLAYER_EXPLORED_ZONES_1 + i31].UInt32Value << 32);
						}
						else
						{
							updateData.ActivePlayerData.ExploredZones[i31 / 2] = updates[PLAYER_EXPLORED_ZONES_1 + i31].UInt32Value;
						}
					}
				}
			}
			var PLAYER_FIELD_COINAGE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_COINAGE);
			if (PLAYER_FIELD_COINAGE >= 0 && updateMaskArray[PLAYER_FIELD_COINAGE])
			{
				updateData.ActivePlayerData.Coinage = updates[PLAYER_FIELD_COINAGE].UInt32Value;
			}
			var PLAYER_FIELD_POSSTAT0 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_POSSTAT0);
			if (PLAYER_FIELD_POSSTAT0 >= 0)
			{
				for (var i32 = 0; i32 < 5; i32++)
				{
					if (updateMaskArray[PLAYER_FIELD_POSSTAT0 + i32])
					{
						updateData.UnitData.StatPosBuff[i32] = updates[PLAYER_FIELD_POSSTAT0 + i32].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_NEGSTAT0 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_NEGSTAT0);
			if (PLAYER_FIELD_NEGSTAT0 >= 0)
			{
				for (var i33 = 0; i33 < 5; i33++)
				{
					if (updateMaskArray[PLAYER_FIELD_NEGSTAT0 + i33])
					{
						updateData.UnitData.StatNegBuff[i33] = updates[PLAYER_FIELD_NEGSTAT0 + i33].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_RESISTANCEBUFFMODSPOSITIVE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_RESISTANCEBUFFMODSPOSITIVE);
			if (PLAYER_FIELD_RESISTANCEBUFFMODSPOSITIVE >= 0)
			{
				for (var i34 = 0; i34 < 7; i34++)
				{
					if (updateMaskArray[PLAYER_FIELD_RESISTANCEBUFFMODSPOSITIVE + i34])
					{
						updateData.UnitData.ResistanceBuffModsPositive[i34] = updates[PLAYER_FIELD_RESISTANCEBUFFMODSPOSITIVE + i34].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_RESISTANCEBUFFMODSNEGATIVE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_RESISTANCEBUFFMODSNEGATIVE);
			if (PLAYER_FIELD_RESISTANCEBUFFMODSNEGATIVE >= 0)
			{
				for (var i35 = 0; i35 < 7; i35++)
				{
					if (updateMaskArray[PLAYER_FIELD_RESISTANCEBUFFMODSNEGATIVE + i35])
					{
						updateData.UnitData.ResistanceBuffModsNegative[i35] = updates[PLAYER_FIELD_RESISTANCEBUFFMODSNEGATIVE + i35].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_MOD_DAMAGE_DONE_POS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_DAMAGE_DONE_POS);
			if (PLAYER_FIELD_MOD_DAMAGE_DONE_POS >= 0)
			{
				for (var i36 = 0; i36 < 7; i36++)
				{
					if (updateMaskArray[PLAYER_FIELD_MOD_DAMAGE_DONE_POS + i36])
					{
						updateData.ActivePlayerData.ModDamageDonePos[i36] = updates[PLAYER_FIELD_MOD_DAMAGE_DONE_POS + i36].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_MOD_DAMAGE_DONE_NEG = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG);
			if (PLAYER_FIELD_MOD_DAMAGE_DONE_NEG >= 0)
			{
				for (var i37 = 0; i37 < 7; i37++)
				{
					if (updateMaskArray[PLAYER_FIELD_MOD_DAMAGE_DONE_NEG + i37])
					{
						updateData.ActivePlayerData.ModDamageDoneNeg[i37] = updates[PLAYER_FIELD_MOD_DAMAGE_DONE_NEG + i37].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_MOD_DAMAGE_DONE_PCT = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT);
			if (PLAYER_FIELD_MOD_DAMAGE_DONE_PCT >= 0)
			{
				for (var i38 = 0; i38 < 7; i38++)
				{
					if (updateMaskArray[PLAYER_FIELD_MOD_DAMAGE_DONE_PCT + i38])
					{
						updateData.ActivePlayerData.ModDamageDonePercent[i38] = updates[PLAYER_FIELD_MOD_DAMAGE_DONE_PCT + i38].FloatValue;
					}
				}
			}
			var PLAYER_FIELD_MOD_HEALING_DONE_POS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_HEALING_DONE_POS);
			if (PLAYER_FIELD_MOD_HEALING_DONE_POS >= 0 && updateMaskArray[PLAYER_FIELD_MOD_HEALING_DONE_POS])
			{
				updateData.ActivePlayerData.ModHealingDonePos = updates[PLAYER_FIELD_MOD_HEALING_DONE_POS].Int32Value;
			}
			var PLAYER_FIELD_MOD_TARGET_RESISTANCE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_TARGET_RESISTANCE);
			if (PLAYER_FIELD_MOD_TARGET_RESISTANCE >= 0 && updateMaskArray[PLAYER_FIELD_MOD_TARGET_RESISTANCE])
			{
				updateData.ActivePlayerData.ModTargetResistance = updates[PLAYER_FIELD_MOD_TARGET_RESISTANCE].Int32Value;
			}
			var PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE);
			if (PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE >= 0 && updateMaskArray[PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE])
			{
				updateData.ActivePlayerData.ModTargetPhysicalResistance = updates[PLAYER_FIELD_MOD_TARGET_PHYSICAL_RESISTANCE].Int32Value;
			}
			var PLAYER_FIELD_BYTES = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BYTES);
			if (PLAYER_FIELD_BYTES >= 0 && updateMaskArray[PLAYER_FIELD_BYTES])
			{
				updateData.ActivePlayerData.LocalFlags = (byte)(updates[PLAYER_FIELD_BYTES].UInt32Value & 0xFF);
				if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
				{
					var comboPoints = (byte)((updates[PLAYER_FIELD_BYTES].UInt32Value >> 8) & 0xFF);
					var classId3 = Class.None;
					classId3 = ((!updateData.UnitData.ClassId.HasValue) ? GetSession().GameState.GetUnitClass(guid.To128(GetSession().GameState)) : ((Class)updateData.UnitData.ClassId.Value));
					var powerSlot3 = ClassPowerTypes.GetPowerSlotForClass(classId3, PowerType.ComboPoints);
					if (powerSlot3 >= 0)
					{
						if (powerUpdate != null && guid == GetSession().GameState.CurrentPlayerGuid)
						{
							powerUpdate.Powers.Add(new PowerUpdatePower(comboPoints, 14));
						}
						updateData.UnitData.Power[powerSlot3] = comboPoints;
					}
				}
				else
				{
					updateData.ActivePlayerData.GrantableLevels = (byte)((updates[PLAYER_FIELD_BYTES].UInt32Value >> 8) & 0xFF);
				}
				updateData.ActivePlayerData.MultiActionBars = (byte)((updates[PLAYER_FIELD_BYTES].UInt32Value >> 16) & 0xFF);
				updateData.ActivePlayerData.LifetimeMaxRank = (byte)((updates[PLAYER_FIELD_BYTES].UInt32Value >> 24) & 0xFF);
			}
			var PLAYER_AMMO_ID = LegacyVersion.GetUpdateField(PlayerField.PLAYER_AMMO_ID);
			if (PLAYER_AMMO_ID >= 0 && updateMaskArray[PLAYER_AMMO_ID])
			{
				updateData.ActivePlayerData.AmmoID = updates[PLAYER_AMMO_ID].UInt32Value;
			}
			var PLAYER_SELF_RES_SPELL = LegacyVersion.GetUpdateField(PlayerField.PLAYER_SELF_RES_SPELL);
			if (PLAYER_SELF_RES_SPELL >= 0 && updateMaskArray[PLAYER_SELF_RES_SPELL])
			{
				var spellId = updates[PLAYER_SELF_RES_SPELL].UInt32Value;
				updateData.ActivePlayerData.SelfResSpells = new List<uint>();
				updateData.ActivePlayerData.SelfResSpells.Add(spellId);
			}
			var PLAYER_FIELD_PVP_MEDALS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_PVP_MEDALS);
			if (PLAYER_FIELD_PVP_MEDALS >= 0 && updateMaskArray[PLAYER_FIELD_PVP_MEDALS])
			{
				updateData.ActivePlayerData.PvpMedals = updates[PLAYER_FIELD_PVP_MEDALS].UInt32Value;
			}
			var PLAYER_FIELD_BUYBACK_PRICE_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BUYBACK_PRICE_1);
			if (PLAYER_FIELD_BUYBACK_PRICE_1 >= 0)
			{
				for (var i39 = 0; i39 < 12; i39++)
				{
					if (updateMaskArray[PLAYER_FIELD_BUYBACK_PRICE_1 + i39])
					{
						updateData.ActivePlayerData.BuybackPrice[i39] = updates[PLAYER_FIELD_BUYBACK_PRICE_1 + i39].UInt32Value;
					}
				}
			}
			var PLAYER_FIELD_BUYBACK_TIMESTAMP_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BUYBACK_TIMESTAMP_1);
			if (PLAYER_FIELD_BUYBACK_TIMESTAMP_1 >= 0)
			{
				for (var i40 = 0; i40 < 12; i40++)
				{
					if (updateMaskArray[PLAYER_FIELD_BUYBACK_TIMESTAMP_1 + i40])
					{
						updateData.ActivePlayerData.BuybackTimestamp[i40] = updates[PLAYER_FIELD_BUYBACK_TIMESTAMP_1 + i40].UInt32Value;
					}
				}
			}
			var PLAYER_FIELD_SESSION_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_SESSION_KILLS);
			if (PLAYER_FIELD_SESSION_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_SESSION_KILLS])
			{
				updateData.ActivePlayerData.TodayHonorableKills = (ushort)(updates[PLAYER_FIELD_SESSION_KILLS].UInt32Value & 0xFFFF);
				updateData.ActivePlayerData.TodayDishonorableKills = (ushort)((updates[PLAYER_FIELD_SESSION_KILLS].UInt32Value >> 16) & 0xFFFF);
			}
			var PLAYER_FIELD_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_KILLS);
			if (PLAYER_FIELD_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_KILLS])
			{
				updateData.ActivePlayerData.TodayHonorableKills = (ushort)(updates[PLAYER_FIELD_KILLS].UInt32Value & 0xFFFF);
				updateData.ActivePlayerData.YesterdayHonorableKills = (ushort)((updates[PLAYER_FIELD_KILLS].UInt32Value >> 16) & 0xFFFF);
			}
			var PLAYER_FIELD_YESTERDAY_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_YESTERDAY_KILLS);
			if (PLAYER_FIELD_YESTERDAY_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_YESTERDAY_KILLS])
			{
				updateData.ActivePlayerData.YesterdayHonorableKills = (ushort)(updates[PLAYER_FIELD_YESTERDAY_KILLS].UInt32Value & 0xFFFF);
				updateData.ActivePlayerData.YesterdayDishonorableKills = (ushort)((updates[PLAYER_FIELD_YESTERDAY_KILLS].UInt32Value >> 16) & 0xFFFF);
			}
			var PLAYER_FIELD_LAST_WEEK_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_LAST_WEEK_KILLS);
			if (PLAYER_FIELD_LAST_WEEK_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_LAST_WEEK_KILLS])
			{
				updateData.ActivePlayerData.LastWeekHonorableKills = (ushort)(updates[PLAYER_FIELD_LAST_WEEK_KILLS].UInt32Value & 0xFFFF);
				updateData.ActivePlayerData.LastWeekDishonorableKills = (ushort)((updates[PLAYER_FIELD_LAST_WEEK_KILLS].UInt32Value >> 16) & 0xFFFF);
			}
			var PLAYER_FIELD_THIS_WEEK_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_THIS_WEEK_KILLS);
			if (PLAYER_FIELD_THIS_WEEK_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_THIS_WEEK_KILLS])
			{
				updateData.ActivePlayerData.ThisWeekHonorableKills = (ushort)(updates[PLAYER_FIELD_THIS_WEEK_KILLS].UInt32Value & 0xFFFF);
				updateData.ActivePlayerData.ThisWeekDishonorableKills = (ushort)((updates[PLAYER_FIELD_THIS_WEEK_KILLS].UInt32Value >> 16) & 0xFFFF);
			}
			var PLAYER_FIELD_THIS_WEEK_CONTRIBUTION = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_THIS_WEEK_CONTRIBUTION);
			if (PLAYER_FIELD_THIS_WEEK_CONTRIBUTION < 0)
			{
				PLAYER_FIELD_THIS_WEEK_CONTRIBUTION = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_TODAY_CONTRIBUTION);
			}
			if (PLAYER_FIELD_THIS_WEEK_CONTRIBUTION >= 0 && updateMaskArray[PLAYER_FIELD_THIS_WEEK_CONTRIBUTION])
			{
				updateData.ActivePlayerData.ThisWeekContribution = updates[PLAYER_FIELD_THIS_WEEK_CONTRIBUTION].UInt32Value;
			}
			var PLAYER_FIELD_LIFETIME_HONORABLE_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_LIFETIME_HONORABLE_KILLS);
			if (PLAYER_FIELD_LIFETIME_HONORABLE_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_LIFETIME_HONORABLE_KILLS])
			{
				updateData.ActivePlayerData.LifetimeHonorableKills = updates[PLAYER_FIELD_LIFETIME_HONORABLE_KILLS].UInt32Value;
			}
			var PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS);
			if (PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS >= 0 && updateMaskArray[PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS])
			{
				updateData.ActivePlayerData.LifetimeDishonorableKills = updates[PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS].UInt32Value;
			}
			var PLAYER_FIELD_YESTERDAY_CONTRIBUTION = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_YESTERDAY_CONTRIBUTION);
			if (PLAYER_FIELD_YESTERDAY_CONTRIBUTION >= 0 && updateMaskArray[PLAYER_FIELD_YESTERDAY_CONTRIBUTION])
			{
				updateData.ActivePlayerData.YesterdayContribution = updates[PLAYER_FIELD_YESTERDAY_CONTRIBUTION].UInt32Value;
			}
			var PLAYER_FIELD_LAST_WEEK_CONTRIBUTION = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_LAST_WEEK_CONTRIBUTION);
			if (PLAYER_FIELD_LAST_WEEK_CONTRIBUTION >= 0 && updateMaskArray[PLAYER_FIELD_LAST_WEEK_CONTRIBUTION])
			{
				updateData.ActivePlayerData.LastWeekContribution = updates[PLAYER_FIELD_LAST_WEEK_CONTRIBUTION].UInt32Value;
			}
			var PLAYER_FIELD_LAST_WEEK_RANK = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_LAST_WEEK_RANK);
			if (PLAYER_FIELD_LAST_WEEK_RANK >= 0 && updateMaskArray[PLAYER_FIELD_LAST_WEEK_RANK])
			{
				updateData.ActivePlayerData.LastWeekRank = updates[PLAYER_FIELD_LAST_WEEK_RANK].UInt32Value;
			}
			var PLAYER_FIELD_BYTES2 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_BYTES2);
			if (PLAYER_FIELD_BYTES2 >= 0 && updateMaskArray[PLAYER_FIELD_BYTES2])
			{
				updateData.ActivePlayerData.PvPRankProgress = (byte)(updates[PLAYER_FIELD_BYTES2].UInt32Value & 0xFF);
				updateData.ActivePlayerData.AuraVision = (byte)((updates[PLAYER_FIELD_BYTES2].UInt32Value >> 8) & 0xFF);
			}
			var PLAYER_FIELD_WATCHED_FACTION_INDEX = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_WATCHED_FACTION_INDEX);
			if (PLAYER_FIELD_WATCHED_FACTION_INDEX >= 0 && updateMaskArray[PLAYER_FIELD_WATCHED_FACTION_INDEX])
			{
				updateData.ActivePlayerData.WatchedFactionIndex = updates[PLAYER_FIELD_WATCHED_FACTION_INDEX].Int32Value;
			}
			var PLAYER_FIELD_COMBAT_RATING_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_COMBAT_RATING_1);
			if (PLAYER_FIELD_COMBAT_RATING_1 >= 0)
			{
				for (var i41 = 0; i41 < 20; i41++)
				{
					if (updateMaskArray[PLAYER_FIELD_COMBAT_RATING_1 + i41])
					{
						updateData.ActivePlayerData.CombatRatings[i41] = updates[PLAYER_FIELD_COMBAT_RATING_1 + i41].Int32Value;
					}
				}
			}
			var PLAYER_FIELD_ARENA_TEAM_INFO_1_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_ARENA_TEAM_INFO_1_1);
			if (PLAYER_FIELD_ARENA_TEAM_INFO_1_1 >= 0)
			{
				var teamIdOffset = 0;
				var teamGamesWeekOffset = 2;
				var teamGamesSeasonOffset = 3;
				var teamWinsSeasonOffset = 4;
				var teamPersonalRatingOffset = 5;
				var sizePerEntry2 = 6;
				for (var i42 = 0; i42 < 3; i42++)
				{
					var startOffset = PLAYER_FIELD_ARENA_TEAM_INFO_1_1 + i42 * sizePerEntry2;
					if (updateMaskArray[startOffset + teamIdOffset] && guid == GetSession().GameState.CurrentPlayerGuid)
					{
						var teamId = (GetSession().GameState.CurrentArenaTeamIds[i42] = updates[startOffset + teamIdOffset].UInt32Value);
						if (teamId != 0)
						{
							var packet = new WorldPacket(Opcode.CMSG_ARENA_TEAM_QUERY);
							packet.WriteUInt32(teamId);
							SendPacketToServer(packet);
							var packet2 = new WorldPacket(Opcode.CMSG_ARENA_TEAM_ROSTER);
							packet2.WriteUInt32(teamId);
							SendPacketToServer(packet2);
						}
						else
						{
							var response = new ArenaTeamRosterResponse
							{
								TeamSize = ModernVersion.GetArenaTeamSizeFromIndex((uint)i42)
							};
							SendPacketToClient(response);
						}
					}
					if (updateMaskArray[startOffset + teamGamesWeekOffset])
					{
						if (updateData.ActivePlayerData.PvpInfo[i42] == null)
						{
							updateData.ActivePlayerData.PvpInfo[i42] = new PVPInfo();
						}
						updateData.ActivePlayerData.PvpInfo[i42].WeeklyPlayed = updates[startOffset + teamGamesWeekOffset].UInt32Value;
					}
					if (updateMaskArray[startOffset + teamGamesSeasonOffset])
					{
						if (updateData.ActivePlayerData.PvpInfo[i42] == null)
						{
							updateData.ActivePlayerData.PvpInfo[i42] = new PVPInfo();
						}
						updateData.ActivePlayerData.PvpInfo[i42].SeasonPlayed = updates[startOffset + teamGamesSeasonOffset].UInt32Value;
					}
					if (updateMaskArray[startOffset + teamWinsSeasonOffset])
					{
						if (updateData.ActivePlayerData.PvpInfo[i42] == null)
						{
							updateData.ActivePlayerData.PvpInfo[i42] = new PVPInfo();
						}
						updateData.ActivePlayerData.PvpInfo[i42].SeasonWon = updates[startOffset + teamWinsSeasonOffset].UInt32Value;
					}
					if (updateMaskArray[startOffset + teamPersonalRatingOffset])
					{
						if (updateData.ActivePlayerData.PvpInfo[i42] == null)
						{
							updateData.ActivePlayerData.PvpInfo[i42] = new PVPInfo();
						}
						updateData.ActivePlayerData.PvpInfo[i42].Rating = updates[startOffset + teamPersonalRatingOffset].UInt32Value;
					}
				}
			}
			if (guid == GetSession().GameState.CurrentPlayerGuid && ModernVersion.ExpansionVersion > 1)
			{
				var PLAYER_FIELD_HONOR_CURRENCY = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_HONOR_CURRENCY);
				var PLAYER_FIELD_ARENA_CURRENCY = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_ARENA_CURRENCY);
				if (PLAYER_FIELD_HONOR_CURRENCY >= 0 && PLAYER_FIELD_ARENA_CURRENCY >= 0 && (updateMaskArray[PLAYER_FIELD_HONOR_CURRENCY] || updateMaskArray[PLAYER_FIELD_ARENA_CURRENCY]))
				{
					var currencies = new SetupCurrency();
					if (updates.ContainsKey(PLAYER_FIELD_ARENA_CURRENCY))
					{
						var honor = new SetupCurrency.Record
						{
							Type = 1900u,
							Quantity = updates[PLAYER_FIELD_ARENA_CURRENCY].UInt32Value
						};
						currencies.Data.Add(honor);
					}
					if (updates.ContainsKey(PLAYER_FIELD_HONOR_CURRENCY))
					{
						var honor2 = new SetupCurrency.Record
						{
							Type = 1901u,
							Quantity = updates[PLAYER_FIELD_HONOR_CURRENCY].UInt32Value
						};
						currencies.Data.Add(honor2);
					}
					SendPacketToClient(currencies);
				}
			}
			var PLAYER_FIELD_MOD_MANA_REGEN = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MOD_MANA_REGEN);
			if (PLAYER_FIELD_MOD_MANA_REGEN >= 0 && updateMaskArray[PLAYER_FIELD_MOD_MANA_REGEN])
			{
				updateData.UnitData.ModPowerRegen[0] = updates[PLAYER_FIELD_MOD_MANA_REGEN].FloatValue;
			}
			var PLAYER_FIELD_MAX_LEVEL = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_MAX_LEVEL);
			if (PLAYER_FIELD_MAX_LEVEL >= 0 && updateMaskArray[PLAYER_FIELD_MAX_LEVEL])
			{
				updateData.ActivePlayerData.MaxLevel = updates[PLAYER_FIELD_MAX_LEVEL].Int32Value;
			}
			var PLAYER_FIELD_DAILY_QUESTS_1 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_FIELD_DAILY_QUESTS_1);
			if (PLAYER_FIELD_DAILY_QUESTS_1 >= 0 && guid == GetSession().GameState.CurrentPlayerGuid)
			{
				for (var i43 = 0; i43 < 25; i43++)
				{
					if (updateMaskArray[PLAYER_FIELD_DAILY_QUESTS_1 + i43])
					{
						GetSession().GameState.SetDailyQuestSlot((uint)i43, updates[PLAYER_FIELD_DAILY_QUESTS_1 + i43].UInt32Value);
						updateData.ActivePlayerData.HasDailyQuestsUpdate = true;
					}
				}
			}
		}
		if (objectType == ObjectType.GameObject)
		{
			var GAMEOBJECT_FIELD_CREATED_BY = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_FIELD_CREATED_BY);
			if (GAMEOBJECT_FIELD_CREATED_BY >= 0 && updateMaskArray[GAMEOBJECT_FIELD_CREATED_BY])
			{
				updateData.GameObjectData.CreatedBy = GetGuidValue(updates, GameObjectField.GAMEOBJECT_FIELD_CREATED_BY).To128(GetSession().GameState);
			}
			var GAMEOBJECT_DISPLAYID = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_DISPLAYID);
			if (GAMEOBJECT_DISPLAYID >= 0 && updateMaskArray[GAMEOBJECT_DISPLAYID])
			{
				updateData.GameObjectData.DisplayID = updates[GAMEOBJECT_DISPLAYID].Int32Value;
			}
			var GAMEOBJECT_FLAGS = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_FLAGS);
			if (GAMEOBJECT_FLAGS >= 0 && updateMaskArray[GAMEOBJECT_FLAGS])
			{
				updateData.GameObjectData.Flags = updates[GAMEOBJECT_FLAGS].UInt32Value;
			}
			var GAMEOBJECT_ROTATION = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_ROTATION);
			if (GAMEOBJECT_ROTATION >= 0 && updateData.CreateData != null && updateData.CreateData.MoveInfo != null)
			{
				for (var i44 = 0; i44 < 4; i44++)
				{
					if (updateMaskArray[GAMEOBJECT_ROTATION + i44])
					{
						updateData.CreateData.MoveInfo.Rotation[i44] = updates[GAMEOBJECT_ROTATION + i44].FloatValue;
					}
				}
				switch (updateData.ObjectData.EntryID)
				{
				case 176080:
				case 176084:
				case 176085:
				{
					var rot = updateData.CreateData.MoveInfo.Rotation.AsEulerAngles();
					rot.Yaw *= -1.0;
					updateData.CreateData.MoveInfo.Rotation = rot.AsQuaternion();
					break;
				}
				}
				switch (updateData.ObjectData.EntryID)
				{
				case 176081:
				case 176082:
				case 176083:
				case 176085:
					updateData.GameObjectData.ParentRotation = new float?[4] { -4.371139E-08f, 0f, 1f, 0f };
					break;
				case 183177:
					updateData.GameObjectData.ParentRotation = new float?[4] { 0f, 0f, -0.69465846f, 0.7193397f };
					break;
				}
			}
			var GAMEOBJECT_STATE = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_STATE);
			if (GAMEOBJECT_STATE >= 0 && updateMaskArray[GAMEOBJECT_STATE])
			{
				updateData.GameObjectData.State = (sbyte)updates[GAMEOBJECT_STATE].Int32Value;
			}
			// Handle GO dynamic flags - try GAMEOBJECT_DYN_FLAGS first (newer expansions),
			// then fall back to GAMEOBJECT_DYNAMIC (3.3.5a packs dyn flags in low 16 bits)
			var GAMEOBJECT_DYN_FLAGS = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_DYN_FLAGS);
			var GAMEOBJECT_DYNAMIC = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_DYNAMIC);
			uint legacyDynFlags = 0;
			var hasDynFlags = false;
			if (GAMEOBJECT_DYN_FLAGS >= 0 && updateMaskArray[GAMEOBJECT_DYN_FLAGS])
			{
				legacyDynFlags = updates[GAMEOBJECT_DYN_FLAGS].UInt32Value;
				hasDynFlags = true;
			}
			else if (GAMEOBJECT_DYNAMIC >= 0 && updateMaskArray[GAMEOBJECT_DYNAMIC])
			{
				// In 3.3.5a, GAMEOBJECT_DYNAMIC low 16 bits = dynamic flags, high 16 bits = path progress
				legacyDynFlags = updates[GAMEOBJECT_DYNAMIC].UInt32Value & 0xFFFF;
				hasDynFlags = true;
			}
			if (hasDynFlags)
			{
				var oldValue2 = 0u;
				if (updateData.ObjectData.DynamicFlags.HasValue)
				{
					oldValue2 = updateData.ObjectData.DynamicFlags.Value;
				}
				else if (!guid.IsTransport())
				{
					oldValue2 = 4294901760u;
				}
				var flags4 = (GameObjectDynamicFlagsLegacy)legacyDynFlags;
				updateData.ObjectData.DynamicFlags = oldValue2 | (uint)flags4.CastFlags<GameObjectDynamicFlagsModern>();
			}
			// Fishing bobbers need Activate flag to be clickable in 3.4.3
			if (updateData.ObjectData.EntryID == 35591)
			{
				var dynVal = updateData.ObjectData.DynamicFlags.GetValueOrDefault(0xFFFF0000u);
				dynVal |= (uint)GameObjectDynamicFlagsModern.Activate;
				updateData.ObjectData.DynamicFlags = dynVal;
			}
			var GAMEOBJECT_FACTION = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_FACTION);
			if (GAMEOBJECT_FACTION >= 0 && updateMaskArray[GAMEOBJECT_FACTION])
			{
				updateData.GameObjectData.FactionTemplate = updates[GAMEOBJECT_FACTION].Int32Value;
			}
			var GAMEOBJECT_TYPE_ID = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_TYPE_ID);
			if (GAMEOBJECT_TYPE_ID >= 0 && updateMaskArray[GAMEOBJECT_TYPE_ID])
			{
				updateData.GameObjectData.TypeID = (sbyte)updates[GAMEOBJECT_TYPE_ID].Int32Value;
			}
			var GAMEOBJECT_LEVEL = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_LEVEL);
			if (GAMEOBJECT_LEVEL >= 0 && updateMaskArray[GAMEOBJECT_LEVEL])
			{
				updateData.GameObjectData.Level = updates[GAMEOBJECT_LEVEL].Int32Value;
			}
			var GAMEOBJECT_ARTKIT = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_ARTKIT);
			if (GAMEOBJECT_ARTKIT >= 0 && updateMaskArray[GAMEOBJECT_ARTKIT])
			{
				updateData.GameObjectData.ArtKit = (byte)updates[GAMEOBJECT_ARTKIT].UInt32Value;
			}
			// 3.3.5a packs State, TypeID, ArtKit, AnimProgress into GAMEOBJECT_BYTES_1
			var GAMEOBJECT_BYTES_1 = LegacyVersion.GetUpdateField(GameObjectField.GAMEOBJECT_BYTES_1);
			if (GAMEOBJECT_BYTES_1 >= 0 && updateMaskArray[GAMEOBJECT_BYTES_1])
			{
				var packed = updates[GAMEOBJECT_BYTES_1].UInt32Value;
				updateData.GameObjectData.State = (sbyte)(packed & 0xFF);
				updateData.GameObjectData.TypeID = (sbyte)((packed >> 8) & 0xFF);
				updateData.GameObjectData.ArtKit = (byte)((packed >> 16) & 0xFF);
				updateData.GameObjectData.PercentHealth = (byte)((packed >> 24) & 0xFF);
			}
			// Fishing bobbers: force State=0 (READY) so 3.4.3 client allows interaction
			if (updateData.ObjectData.EntryID == 35591)
			{
				updateData.GameObjectData.State = 0;
			}
		}
		if (objectType == ObjectType.DynamicObject)
		{
			var DYNAMICOBJECT_CASTER = LegacyVersion.GetUpdateField(DynamicObjectField.DYNAMICOBJECT_CASTER);
			if (DYNAMICOBJECT_CASTER >= 0 && updateMaskArray[DYNAMICOBJECT_CASTER])
			{
				updateData.DynamicObjectData.Caster = GetGuidValue(updates, DynamicObjectField.DYNAMICOBJECT_CASTER).To128(GetSession().GameState);
			}
			var DYNAMICOBJECT_SPELLID = LegacyVersion.GetUpdateField(DynamicObjectField.DYNAMICOBJECT_SPELLID);
			if (DYNAMICOBJECT_SPELLID >= 0 && updateMaskArray[DYNAMICOBJECT_SPELLID])
			{
				updateData.DynamicObjectData.SpellID = updates[DYNAMICOBJECT_SPELLID].Int32Value;
				updateData.DynamicObjectData.SpellXSpellVisualID = (int)GameData.GetSpellVisual((uint)updateData.DynamicObjectData.SpellID.Value);
			}
			var DYNAMICOBJECT_RADIUS = LegacyVersion.GetUpdateField(DynamicObjectField.DYNAMICOBJECT_RADIUS);
			if (DYNAMICOBJECT_RADIUS >= 0 && updateMaskArray[DYNAMICOBJECT_RADIUS])
			{
				updateData.DynamicObjectData.Radius = updates[DYNAMICOBJECT_RADIUS].FloatValue;
			}
		}
		if (objectType != ObjectType.Corpse)
		{
			return;
		}
		var CORPSE_FIELD_OWNER = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_OWNER);
		if (CORPSE_FIELD_OWNER >= 0 && updateMaskArray[CORPSE_FIELD_OWNER])
		{
			updateData.CorpseData.Owner = GetGuidValue(updates, CorpseField.CORPSE_FIELD_OWNER).To128(GetSession().GameState);
		}
		var CORPSE_FIELD_DISPLAY_ID = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_DISPLAY_ID);
		if (CORPSE_FIELD_DISPLAY_ID >= 0 && updateMaskArray[CORPSE_FIELD_DISPLAY_ID])
		{
			updateData.CorpseData.DisplayID = updates[CORPSE_FIELD_DISPLAY_ID].UInt32Value;
		}
		var CORPSE_FIELD_ITEM = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_ITEM);
		if (CORPSE_FIELD_ITEM >= 0)
		{
			for (var i45 = 0; i45 < 19; i45++)
			{
				if (updateMaskArray[CORPSE_FIELD_ITEM + i45])
				{
					updateData.CorpseData.Items[i45] = updates[CORPSE_FIELD_ITEM + i45].UInt32Value;
				}
			}
		}
		var CORPSE_FIELD_BYTES_1 = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_BYTES_1);
		if (CORPSE_FIELD_BYTES_1 >= 0 && updateMaskArray[CORPSE_FIELD_BYTES_1])
		{
			updateData.CorpseData.RaceId = (byte)((updates[CORPSE_FIELD_BYTES_1].UInt32Value >> 8) & 0xFF);
			updateData.CorpseData.SexId = (byte)((updates[CORPSE_FIELD_BYTES_1].UInt32Value >> 16) & 0xFF);
			var skin2 = (byte)((updates[CORPSE_FIELD_BYTES_1].UInt32Value >> 24) & 0xFF);
			var CORPSE_FIELD_BYTES_2 = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_BYTES_2);
			if (CORPSE_FIELD_BYTES_2 >= 0 && updateMaskArray[CORPSE_FIELD_BYTES_2])
			{
				var face2 = (byte)(updates[CORPSE_FIELD_BYTES_2].UInt32Value & 0xFF);
				var hairStyle2 = (byte)((updates[CORPSE_FIELD_BYTES_2].UInt32Value >> 8) & 0xFF);
				var hairColor2 = (byte)((updates[CORPSE_FIELD_BYTES_2].UInt32Value >> 16) & 0xFF);
				var facialHair2 = (byte)((updates[CORPSE_FIELD_BYTES_2].UInt32Value >> 24) & 0xFF);
				var customizations2 = CharacterCustomizations.ConvertLegacyCustomizationsToModern((Race)updateData.CorpseData.RaceId.Value, (Gender)updateData.CorpseData.SexId.Value, skin2, face2, hairStyle2, hairColor2, facialHair2);
				for (var i46 = 0; i46 < 5; i46++)
				{
					updateData.CorpseData.Customizations[i46] = customizations2[i46];
				}
			}
		}
		var CORPSE_FIELD_GUILD = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_GUILD);
		if (CORPSE_FIELD_GUILD >= 0 && updateMaskArray[CORPSE_FIELD_GUILD])
		{
			updateData.CorpseData.GuildGUID = WowGuid128.Create(HighGuidType703.Guild, updates[CORPSE_FIELD_GUILD].UInt32Value);
		}
		var CORPSE_FIELD_FLAGS = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_FLAGS);
		if (CORPSE_FIELD_FLAGS >= 0 && updateMaskArray[CORPSE_FIELD_FLAGS])
		{
			updateData.CorpseData.Flags = updates[CORPSE_FIELD_FLAGS].UInt32Value;
			if (updateData.CorpseData.Flags.HasAnyFlag(CorpseFlags.HideHelm))
			{
				var corpseData = updateData.CorpseData;
				corpseData.Flags &= 4294967287u;
				updateData.CorpseData.Items[0] = null;
			}
			if (updateData.CorpseData.Flags.HasAnyFlag(CorpseFlags.HideCloak))
			{
				var corpseData = updateData.CorpseData;
				corpseData.Flags &= 4294967279u;
				updateData.CorpseData.Items[14] = null;
			}
		}
		var CORPSE_FIELD_DYNAMIC_FLAGS = LegacyVersion.GetUpdateField(CorpseField.CORPSE_FIELD_DYNAMIC_FLAGS);
		if (CORPSE_FIELD_DYNAMIC_FLAGS >= 0 && updateMaskArray[CORPSE_FIELD_DYNAMIC_FLAGS])
		{
			updateData.CorpseData.DynamicFlags = updates[CORPSE_FIELD_DYNAMIC_FLAGS].UInt32Value;
		}
	}

	[PacketHandler(Opcode.SMSG_INIT_WORLD_STATES)]
	private void HandleInitWorldStates(WorldPacket packet)
	{
		var states = new InitWorldStates
		{
			MapID = packet.ReadUInt32()
		};
		GetSession().GameState.CurrentMapId = states.MapID;
		states.ZoneID = packet.ReadUInt32();
		states.AreaID = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_1_0_6692) ? packet.ReadUInt32() : states.ZoneID);
		GetSession().GameState.HasWsgAllyFlagCarrier = false;
		GetSession().GameState.HasWsgHordeFlagCarrier = false;
		var count = packet.ReadUInt16();
		for (ushort i = 0; i < count; i++)
		{
			var variable = packet.ReadUInt32();
			var value = packet.ReadInt32();
			if (variable != 0 || value != 0)
			{
				states.AddState(variable, value);
			}
			switch (variable)
			{
			case 2339u:
				GetSession().GameState.HasWsgAllyFlagCarrier = value == 2;
				break;
			case 2338u:
				GetSession().GameState.HasWsgHordeFlagCarrier = value == 2;
				break;
			}
		}
		states.AddClassicStates();
		SendPacketToClient(states);
		if (LegacyVersion.ExpansionVersion <= 1 || ModernVersion.ExpansionVersion <= 1)
		{
			SendPacketToClient(new SetupCurrency());
		}
		// AllAccountCriteria removed — was sending empty criteria after real SMSG_ALL_ACHIEVEMENT_DATA
		if (GetSession().GameState.HasWsgHordeFlagCarrier || GetSession().GameState.HasWsgAllyFlagCarrier)
		{
			var packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet2);
		}
		if (GetSession().GameState.CurrentZoneId == states.ZoneID)
		{
			return;
		}
		var oldZoneName = GameData.GetAreaName(GetSession().GameState.CurrentZoneId);
		var newZoneName = GameData.GetAreaName(states.ZoneID);
		GetSession().GameState.CurrentZoneId = states.ZoneID;
		if (string.IsNullOrEmpty(oldZoneName) || string.IsNullOrEmpty(newZoneName))
		{
			return;
		}
		foreach (var channel in GameData.GetChatChannelsWithFlags(ChannelFlags.AutoJoin | ChannelFlags.ZoneBased))
		{
			SendChatLeaveChannel(1, channel.Name + " - " + oldZoneName);
			SendChatJoinChannel(1, channel.Name + " - " + newZoneName, "");
		}
	}

	[PacketHandler(Opcode.SMSG_UPDATE_WORLD_STATE)]
	private void HandleUpdateWorldState(WorldPacket packet)
	{
		var update = new UpdateWorldState
		{
			VariableID = packet.ReadUInt32(),
			Value = packet.ReadInt32()
		};
		SendPacketToClient(update);
		if (update.VariableID == 2339)
		{
			var packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet2);
			GetSession().GameState.HasWsgAllyFlagCarrier = update.Value == 2;
		}
		else if (update.VariableID == 2338)
		{
			var packet3 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
			SendPacket(packet3);
			GetSession().GameState.HasWsgHordeFlagCarrier = update.Value == 2;
		}
	}

	public WorldClient()
	{
		InitializePacketHandlers();
	}

	public GlobalSessionData GetSession()
	{
		return _globalSession;
	}

	public bool ConnectToWorldServer(Realm realm, GlobalSessionData globalSession)
	{
		_worldCrypt = null;
		_realm = realm;
		_globalSession = globalSession;
		_username = globalSession.Username;
		_isSuccessful = null;
		_delayedPacketsToServer = new Dictionary<Opcode, List<WorldPacket>>();
		_delayedPacketsToClient = new Dictionary<Opcode, List<ServerPacket>>();
		Log.Print(LogType.Network, "Connecting to world server...", "WorldClient.cs");
		try
		{
			var ip = NetworkUtils.ResolveOrDirectIPv4(realm.ExternalAddress);
			Log.Print(LogType.Network, $"World Server address {realm.ExternalAddress}:{realm.Port} resolved as {ip}:{realm.Port}", "WorldClient.cs");
			_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var endPoint = new IPEndPoint(ip, realm.Port);
			_clientSocket.BeginConnect(endPoint, ConnectCallback, null);
		}
		catch (Exception ex)
		{
			Log.Print(LogType.Error, "Socket Error: " + ex.Message, "WorldClient.cs");
			_isSuccessful = false;
		}
		while (!_isSuccessful.HasValue)
		{
			Thread.Sleep(100);
		}
		return _isSuccessful.Value;
	}

	public bool IsAuthenticated()
	{
		return _isSuccessful == true;
	}

	private void InitializeEncryption(byte[] sessionKey)
	{
		switch (Settings.ServerBuild)
		{
		case ClientVersionBuild.V1_12_1_5875:
		case ClientVersionBuild.V1_12_2_6005:
		case ClientVersionBuild.V1_12_3_6141:
			_worldCrypt = new VanillaWorldCrypt();
			break;
		case ClientVersionBuild.V2_4_3_8606:
			_worldCrypt = new TbcWorldCrypt();
			break;
		case ClientVersionBuild.V3_3_5a_12340:
			_worldCrypt = new WotlkWorldCrypt();
			break;
		}
		if (_worldCrypt != null)
		{
			_worldCrypt.Initialize(sessionKey);
		}
	}

	public void Disconnect()
	{
		if (IsConnected())
		{
			_clientSocket.Shutdown(SocketShutdown.Both);
			_clientSocket.Disconnect(reuseSocket: false);
			if (GetSession().WorldClient == this)
			{
				GetSession().WorldClient = null;
			}
		}
	}

	public bool IsConnected()
	{
		return _clientSocket != null && _clientSocket.Connected;
	}

	public uint GetQueuePosition()
	{
		return _queuePosition;
	}

	private void ConnectCallback(IAsyncResult AR)
	{
		try
		{
			Log.Print(LogType.Network, "Connection established!", "WorldClient.cs");
			_clientSocket.EndConnect(AR);
			_clientSocket.ReceiveBufferSize = 65535;
			Task.Run((Func<Task?>)ReceiveLoop);
		}
		catch (Exception ex)
		{
			Log.Print(LogType.Error, "Connect Error: " + ex.Message, "WorldClient.cs");
			if (!_isSuccessful.HasValue)
			{
				_isSuccessful = false;
			}
		}
	}

	private async Task<bool> ReceiveBufferFully(ArraySegment<byte> bufferToFill)
	{
		int receive;
		for (var alreadyReceived = 0; alreadyReceived < bufferToFill.Count; alreadyReceived += receive)
		{
			var tmpArrayBuffer = new ArraySegment<byte>(bufferToFill.Array, alreadyReceived + bufferToFill.Offset, bufferToFill.Count - alreadyReceived);
			receive = await _clientSocket.ReceiveAsync(tmpArrayBuffer, SocketFlags.None);
			if (receive == 0)
			{
				return false;
			}
		}
		return true;
	}

	private async Task ReceiveLoop()
	{
		try
		{
			while (true)
			{
				var headerBuffer = new byte[4];
				if (!(await ReceiveBufferFully(headerBuffer)))
				{
					Log.PrintNet(LogType.Error, LogNetDir.S2P, "Socket Closed By GameWorldServer (header)", "WorldClient.cs");
					if (!_isSuccessful.HasValue)
					{
						_isSuccessful = false;
					}
					else if (GetSession().WorldClient == this)
					{
						GetSession().OnDisconnect();
					}
					return;
				}
				if (_worldCrypt != null)
				{
					_worldCrypt.Decrypt(headerBuffer, 4);
				}
				var header = new LegacyServerPacketHeader();
				header.Read(headerBuffer);
				var packetSize = header.Size;
				if (header.Opcode != 221)
				{
					Log.PrintNet(LogType.Debug, LogNetDir.S2P, $"Decoded header: size={packetSize}, opcode={header.Opcode} (0x{header.Opcode:X4}), crypt={((_worldCrypt != null) ? "ON" : "OFF")}", "WorldClient.cs");
				}
				if (packetSize != 0)
				{
					var buffer = new byte[packetSize];
					buffer[0] = headerBuffer[2];
					buffer[1] = headerBuffer[3];
					if (!(await ReceiveBufferFully(new ArraySegment<byte>(buffer, 2, buffer.Length - 2))))
					{
						break;
					}
					var packet = new WorldPacket(buffer);
					packet.SetReceiveTime(Environment.TickCount);
					HandlePacket(packet);
				}
			}
			Log.PrintNet(LogType.Error, LogNetDir.S2P, "Socket Closed By GameWorldServer (payload)", "WorldClient.cs");
			if (!_isSuccessful.HasValue)
			{
				_isSuccessful = false;
			}
			else if (GetSession().WorldClient == this)
			{
				GetSession().OnDisconnect();
			}
		}
		catch (Exception ex)
		{
			var e = ex;
			Log.PrintNet(LogType.Error, LogNetDir.S2P, "Packet Read Error: " + e.Message + Environment.NewLine + e.StackTrace, "WorldClient.cs");
			if (!_isSuccessful.HasValue)
			{
				_isSuccessful = false;
				return;
			}
			Disconnect();
			GetSession().OnDisconnect();
		}
	}

	private void SendPacket(WorldPacket packet)
	{
		_sendMutex.WaitOne();
		try
		{
			var buffer = new ByteBuffer();
			var header = new LegacyClientPacketHeader
			{
				Size = (ushort)(packet.GetSize() + 4),
				Opcode = packet.GetOpcode()
			};
			header.Write(buffer);
			Log.PrintNet(LogType.Debug, LogNetDir.P2S, $"Sending opcode {LegacyVersion.GetUniversalOpcode(header.Opcode)} ({header.Opcode}) with size {header.Size}.", "WorldClient.cs");
			var headerArray = buffer.GetData();
			Log.PrintNet(LogType.Debug, LogNetDir.P2S, $"Raw header ({headerArray.Length} bytes): {BitConverter.ToString(headerArray, 0, Math.Min(headerArray.Length, 6))}", "WorldClient.cs");
			if (_worldCrypt != null)
			{
				_worldCrypt.Encrypt(headerArray, 6);
			}
			buffer.Clear();
			buffer.WriteBytes(headerArray);
			buffer.WriteBytes(packet.GetData(), packet.GetSize());
			var finalData = buffer.GetData();
			Log.PrintNet(LogType.Debug, LogNetDir.P2S, $"Total bytes on wire: {finalData.Length}, first 16: {BitConverter.ToString(finalData, 0, Math.Min(finalData.Length, 16))}", "WorldClient.cs");
			_clientSocket.Send(finalData, SocketFlags.None);
		}
		catch (Exception ex)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2S, "Packet Write Error: " + ex.Message, "WorldClient.cs");
			if (!_isSuccessful.HasValue)
			{
				_isSuccessful = false;
			}
		}
		_sendMutex.ReleaseMutex();
	}

	public void SendPacketToClient(ServerPacket packet, Opcode delayUntilOpcode = Opcode.MSG_NULL_ACTION)
	{
		var opcode = packet.GetUniversalOpcode();
		if (delayUntilOpcode != Opcode.MSG_NULL_ACTION)
		{
			if (_delayedPacketsToClient.ContainsKey(delayUntilOpcode))
			{
				_delayedPacketsToClient[delayUntilOpcode].Add(packet);
				return;
			}
			var packets = new List<ServerPacket>();
			packets.Add(packet);
			_delayedPacketsToClient.Add(delayUntilOpcode, packets);
		}
		else
		{
			SendPacketToClientDirect(packet);
			SendDelayedPacketsToClientOnOpcode(opcode);
		}
	}

	private void SendPacketToClientDirect(ServerPacket packet)
	{
		if (GetSession()?.GameState == null)
		{
			Log.PrintNet(LogType.Warn, LogNetDir.P2C, $"Dropping {packet.GetUniversalOpcode()} - session/gamestate not ready", "WorldClient.cs");
			return;
		}
		var pendingPackets = GetSession().GameState.PendingUninstancedPackets;
		if (packet.GetConnection() == ConnectionType.Realm)
		{
			if (GetSession().RealmSocket == null)
			{
				Log.PrintNet(LogType.Warn, LogNetDir.P2C, $"Queuing {packet.GetUniversalOpcode()} - RealmSocket not ready yet", "WorldClient.cs");
				lock (pendingPackets)
				{
					pendingPackets.Enqueue(packet);
					return;
				}
			}
			var realmSocket = GetSession().RealmSocket;
			if (pendingPackets.Count > 0)
			{
				lock (pendingPackets)
				{
					ServerPacket oldPacket;
					while (pendingPackets.TryDequeue(out oldPacket))
					{
						realmSocket.SendPacket(oldPacket);
					}
				}
			}
			realmSocket.SendPacket(packet);
			return;
		}
		if (GetSession().InstanceSocket == null && !GetSession().GameState.IsConnectedToInstance)
		{
			lock (pendingPackets)
			{
				if (GetSession().InstanceSocket == null && !GetSession().GameState.IsConnectedToInstance)
				{
					pendingPackets.Enqueue(packet);
					Log.PrintNet(LogType.Warn, LogNetDir.P2C, $"Can't send opcode {packet.GetUniversalOpcode()} ({packet.GetOpcode()}) before entering world! Queue (Initial Check)", "WorldClient.cs");
					return;
				}
			}
		}
		while (GetSession().InstanceSocket == null && GetSession().GameState.IsConnectedToInstance)
		{
			Log.PrintNet(LogType.Network, LogNetDir.P2C, $"Waiting to send {packet.GetUniversalOpcode()} ({packet.GetOpcode()}).", "WorldClient.cs");
			Thread.Sleep(200);
			if (GetSession()?.GameState == null) return;
		}
		if (GetSession().InstanceSocket == null)
		{
			lock (pendingPackets)
			{
				pendingPackets.Enqueue(packet);
				Log.PrintNet(LogType.Warn, LogNetDir.P2C, $"Can't send opcode {packet.GetUniversalOpcode()} ({packet.GetOpcode()}) before entering world! Queue (State: {GetSession().GameState.IsConnectedToInstance})", "WorldClient.cs");
				return;
			}
		}
		var instanceSocket = GetSession().InstanceSocket;
		if (pendingPackets.Count > 0)
		{
			lock (pendingPackets)
			{
				ServerPacket oldPacket;
				while (pendingPackets.TryDequeue(out oldPacket))
				{
					instanceSocket.SendPacket(oldPacket);
				}
			}
		}
		instanceSocket.SendPacket(packet);
	}

	public void SendPacketToServer(WorldPacket packet, Opcode delayUntilOpcode = Opcode.MSG_NULL_ACTION)
	{
		var opcode = packet.GetUniversalOpcode(isModern: false);
		if (delayUntilOpcode != Opcode.MSG_NULL_ACTION)
		{
			if (_delayedPacketsToServer.ContainsKey(delayUntilOpcode))
			{
				_delayedPacketsToServer[delayUntilOpcode].Add(packet);
				return;
			}
			var packets = new List<WorldPacket>();
			packets.Add(packet);
			_delayedPacketsToServer.Add(delayUntilOpcode, packets);
		}
		else
		{
			SendPacket(packet);
			SendDelayedPacketsToServerOnOpcode(opcode);
		}
	}

	private void SendDelayedPacketsToServerOnOpcode(Opcode opcode)
	{
		if (_delayedPacketsToServer.ContainsKey(opcode))
		{
			var packets = _delayedPacketsToServer[opcode];
			for (var i = packets.Count - 1; i >= 0; i--)
			{
				SendPacket(packets[i]);
				packets.RemoveAt(i);
			}
		}
	}

	private void SendDelayedPacketsToClientOnOpcode(Opcode opcode)
	{
		if (_delayedPacketsToClient.ContainsKey(opcode))
		{
			var packets = _delayedPacketsToClient[opcode];
			for (var i = packets.Count - 1; i >= 0; i--)
			{
				SendPacketToClientDirect(packets[i]);
				packets.RemoveAt(i);
			}
		}
	}

	public void FlushPendingPackets()
	{
		if (GetSession()?.GameState == null)
		{
			return;
		}
		var pendingPackets = GetSession().GameState.PendingUninstancedPackets;
		if (pendingPackets.Count == 0)
		{
			return;
		}
		lock (pendingPackets)
		{
			ServerPacket next;
			while (pendingPackets.TryPeek(out next))
			{
				var socket = (next.GetConnection() == ConnectionType.Realm) ? GetSession().RealmSocket : GetSession().InstanceSocket;
				if (socket != null)
				{
					pendingPackets.TryDequeue(out next);
					socket.SendPacket(next);
					continue;
				}
				break;
			}
		}
	}

	private static readonly HashSet<Opcode> _suppressedLogOpcodes = new HashSet<Opcode>
	{
		Opcode.SMSG_ON_MONSTER_MOVE,
		Opcode.MSG_MOVE_HEARTBEAT,
		Opcode.MSG_MOVE_START_FORWARD,
		Opcode.MSG_MOVE_STOP,
		Opcode.MSG_MOVE_SET_FACING,
		Opcode.SMSG_MOVE_SET_COLLISION_HGT,
	};

	private void HandlePacket(WorldPacket packet)
	{
		var universalOpcode = packet.GetUniversalOpcode(isModern: false);
		if (!_suppressedLogOpcodes.Contains(universalOpcode))
			Log.PrintNet(LogType.Debug, LogNetDir.S2P, $"Received opcode {universalOpcode} ({packet.GetOpcode()}).", "WorldClient.cs");
		switch (universalOpcode)
		{
		case Opcode.SMSG_AUTH_CHALLENGE:
			HandleAuthChallenge(packet);
			break;
		case Opcode.SMSG_AUTH_RESPONSE:
			HandleAuthResponse(packet);
			break;
		default:
			if (_packetHandlers.ContainsKey(universalOpcode))
			{
				try
				{
					_packetHandlers[universalOpcode](packet);
				}
				catch (OutOfMemoryException ex)
				{
					Log.Print(LogType.Error, $"OOM handling {universalOpcode}: {ex.Message}");
				}
				catch (Exception ex)
				{
					Log.Print(LogType.Error, $"Exception handling {universalOpcode}: {ex.Message}");
					Log.Print(LogType.Error, ex.StackTrace ?? string.Empty);
				}
				break;
			}
			Log.PrintNet(LogType.Warn, LogNetDir.S2P, $"No handler for opcode {universalOpcode} ({packet.GetOpcode()}) (Got unknown packet from WorldServer)", "WorldClient.cs");
			MissingOpcodeTracker.LogUnhandledLegacySMSG(universalOpcode, packet.GetOpcode());
			if (!_isSuccessful.HasValue)
			{
				_isSuccessful = false;
			}
			break;
		case Opcode.SMSG_ADDON_INFO:
			break;
		}
		SendDelayedPacketsToServerOnOpcode(universalOpcode);
	}

	private void HandleAuthChallenge(WorldPacket packet)
	{
		if (Settings.ServerBuild >= ClientVersionBuild.V3_3_5a_12340)
		{
			var one = packet.ReadUInt32();
		}
		var seed = packet.ReadUInt32();
		if (Settings.ServerBuild >= ClientVersionBuild.V3_3_5a_12340)
		{
			var seed2 = packet.ReadBytes(16u).ToBigInteger();
			var seed3 = packet.ReadBytes(16u).ToBigInteger();
		}
		var rand = RandomNumberGenerator.Create();
		var bytes = new byte[4];
		rand.GetBytes(bytes);
		var ourSeed = bytes.ToBigInteger();
		SendAuthResponse((uint)ourSeed, seed);
	}

	public void SendAuthResponse(uint clientSeed, uint serverSeed)
	{
		var zero = 0u;
		var authResponse = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(_username.ToUpper()), BitConverter.GetBytes(zero), BitConverter.GetBytes(clientSeed), BitConverter.GetBytes(serverSeed), GetSession().AuthClient.GetSessionKey());
		var packet = new WorldPacket(Opcode.CMSG_AUTH_SESSION);
		packet.WriteUInt32((uint)Settings.ServerBuild);
		packet.WriteUInt32(_realm.Id.Index);
		packet.WriteBytes(_username.ToUpper().ToCString());
		if (Settings.ServerBuild >= ClientVersionBuild.V3_0_2_9056)
		{
			packet.WriteUInt32(zero);
		}
		packet.WriteUInt32(clientSeed);
		if (Settings.ServerBuild >= ClientVersionBuild.V3_3_5a_12340)
		{
			packet.WriteUInt32(_realm.Id.Region);
			packet.WriteUInt32(_realm.Id.Site);
			packet.WriteUInt32(_realm.Id.Index);
		}
		if (Settings.ServerBuild >= ClientVersionBuild.V3_2_0_10192)
		{
			packet.WriteUInt64(zero);
		}
		packet.WriteBytes(authResponse);
		var addonBytes = new byte[178]
		{
			208, 1, 0, 0, 120, 156, 117, 207, 61, 14,
			194, 48, 12, 5, 224, 114, 14, 184, 12, 97,
			64, 149, 154, 133, 150, 25, 153, 196, 173, 172,
			38, 78, 21, 82, 126, 58, 113, 66, 206, 68,
			81, 133, 24, 98, 188, 126, 126, 79, 182, 114,
			52, 77, 16, 237, 105, 59, 154, 68, 129, 143,
			101, 177, 242, 183, 77, 85, 204, 163, 190, 166,
			32, 37, 135, 45, 161, 179, 154, 152, 60, 12,
			210, 18, 177, 37, 238, 230, 130, 87, 102, 187,
			224, 207, 144, 170, 208, 9, 185, 197, 26, 188,
			39, 9, 35, 180, 73, 188, 105, 175, 235, 49,
			94, 241, 33, 227, 72, 206, 42, 224, 94, 212,
			146, 47, 3, 154, 79, 237, 58, 183, 132, 190,
			14, 166, 199, 180, 252, 146, 167, 53, 152, 24,
			102, 121, 102, 114, 0, 178, 51, 196, 12, 26,
			112, 200, 242, 27, 77, 4, 139, 117, 79, 206,
			253, 99, 98, 140, 178, 145, 71, 13, 12, 29,
			198, 159, 190, 1, 43, 0, 141, 195
		};
		packet.WriteBytes(addonBytes);
		SendPacket(packet);
		InitializeEncryption(GetSession().AuthClient.GetSessionKey());
	}

	private void HandleInitialSpells(WorldPacket packet)
	{
		// Legacy SMSG_INITIAL_SPELLS (298): uint8 unknown + uint16 count + count * (uint32 spellid + uint16 unknown)
		packet.ReadUInt8();
		var count = packet.ReadUInt16();
		var modern = new ModernInitialSpells();
		for (var i = 0; i < count; i++)
		{
			var spellId = packet.ReadUInt32();
			packet.ReadUInt16(); // unknown
			modern.Spells.Add(spellId);
		}
		SendPacketToClient(modern);
	}
	private void HandleAuthResponse(WorldPacket packet)
	{
		var result = (AuthResult)packet.ReadUInt8();
		if (!_isSuccessful.HasValue)
		{
			var billingTimeRemaining = packet.ReadUInt32();
			var billingFlags = packet.ReadUInt8();
			var billingTimeRested = packet.ReadUInt32();
			if (Settings.ServerBuild >= ClientVersionBuild.V2_0_1_6180)
			{
				var expansion = packet.ReadUInt8();
			}
		}
		switch (result)
		{
		case AuthResult.AUTH_OK:
			Log.Print(LogType.Network, "Authentication succeeded!", "WorldClient.cs");
			if (_queuePosition != 0 && GetSession().RealmSocket != null)
			{
				_queuePosition = 0u;
				GetSession().RealmSocket.SendAuthWaitQue(_queuePosition);
			}
			// Proactively query all transport entries so cache is populated before CreateObjects
			foreach (var transportEntry in GameData.TransportPeriods.Keys)
			{
				var goQuery = new WorldPacket(Opcode.CMSG_QUERY_GAME_OBJECT);
				goQuery.WriteUInt32(transportEntry);
				goQuery.WriteUInt64(0); // empty guid
				SendPacket(goQuery);
			}
			Log.Print(LogType.Network, $"Pre-queried {GameData.TransportPeriods.Count} transport entries");
			_isSuccessful = true;
			break;
		case AuthResult.AUTH_WAIT_QUEUE:
			_queuePosition = packet.ReadUInt32();
			Log.Print(LogType.Network, $"Position in queue is {_queuePosition}.", "WorldClient.cs");
			if (_isSuccessful.HasValue && GetSession().RealmSocket != null)
			{
				GetSession().RealmSocket.SendAuthWaitQue(_queuePosition);
			}
			_isSuccessful = true;
			break;
		default:
			Log.Print(LogType.Network, "Authentication failed!", "WorldClient.cs");
			_isSuccessful = false;
			break;
		}
	}

	public void SendPing(uint ping, uint latency)
	{
		if (IsConnected() && _isSuccessful != false)
		{
			var packet = new WorldPacket(Opcode.CMSG_PING);
			packet.WriteUInt32(ping);
			packet.WriteUInt32(latency);
			SendPacket(packet);
		}
	}

	public void InitializePacketHandlers()
	{
		_packetHandlers = new Dictionary<Opcode, Action<WorldPacket>>();
		var methods = typeof(WorldClient).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (var methodInfo in methods)
		{
			foreach (var msgAttr in methodInfo.GetCustomAttributes<PacketHandlerAttribute>())
			{
				if (msgAttr == null || msgAttr.Opcode == Opcode.MSG_NULL_ACTION)
				{
					continue;
				}
				if (_packetHandlers.ContainsKey(msgAttr.Opcode))
				{
					Log.Print(LogType.Error, $"Tried to override OpcodeHandler of {_packetHandlers[msgAttr.Opcode]} with {methodInfo.Name} (Opcode {msgAttr.Opcode})", "WorldClient.cs");
				}
				else
				{
					var parameters = methodInfo.GetParameters();
					if (parameters.Length == 0)
					{
						Log.Print(LogType.Error, "Method: " + methodInfo.Name + " Has no parameters", "WorldClient.cs");
						continue;
					}
					if (parameters[0].ParameterType != typeof(WorldPacket))
					{
						Log.Print(LogType.Error, "Method: " + methodInfo.Name + " has wrong BaseType", "WorldClient.cs");
						continue;
					}
					var del = (Action<WorldPacket>)Delegate.CreateDelegate(typeof(Action<WorldPacket>), this, methodInfo);
					_packetHandlers[msgAttr.Opcode] = del;
				}
			}
		}
	}
}
