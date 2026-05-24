using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BNetServer;
using BNetServer.Services;
using Framework;
using Framework.Constants;
using Framework.Cryptography;
using Framework.IO;
using Framework.Logging;
using Framework.Networking;
using Framework.Realm;
using Google.Protobuf;
using HermesProxy.Enums;
using HermesProxy.World.Client;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;
using HermesProxy.World.Server.Packets;
using JetBrains.Annotations;
using AuthResult = HermesProxy.Auth.AuthResult;

namespace HermesProxy.World.Server;

public class WorldSocket : SocketBase, BnetServices.INetwork
{
	public struct ConnectToKey
	{
		public uint AccountId;

		public ConnectionType connectionType;

		public ulong Key;

		public ulong Raw
		{
			get
			{
				return (ulong)(AccountId | ((long)connectionType << 32)) | (Key << 33);
			}
			set
			{
				AccountId = (uint)(value & 0xFFFFFFFFu);
				connectionType = (ConnectionType)((value >> 32) & 1);
				Key = value >> 33;
			}
		}
	}

	public class CharacterLoginFailed : ServerPacket
	{
		private LoginFailureReason Code;

		public CharacterLoginFailed(LoginFailureReason code)
			: base(Opcode.SMSG_CHARACTER_LOGIN_FAILED)
		{
			Code = code;
		}

		protected override void Write()
		{
			_worldPacket.WriteUInt8((byte)Code);
		}
	}

	public class PacketHandler
	{
		private Action<WorldSocket, ClientPacket> methodCaller;

		private Type packetType;

		public PacketHandler(MethodInfo info, Type type)
		{
			methodCaller = (Action<WorldSocket, ClientPacket>)GetType().GetMethod("CreateDelegate", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type)
				.Invoke(null, new object[1] { info });
			packetType = type;
		}

		public void Invoke(WorldSocket session, WorldPacket packet)
		{
			if (packetType == null)
			{
				return;
			}
			using var clientPacket = (ClientPacket)Activator.CreateInstance(packetType, packet);
			clientPacket.LogPacket(ref session.GetSession().ModernSniff);
			clientPacket.Read();
			methodCaller(session, clientPacket);
		}

		private static Action<WorldSocket, ClientPacket> CreateDelegate<P1>(MethodInfo method) where P1 : ClientPacket
		{
			var d = (Action<WorldSocket, P1>)method.CreateDelegate(typeof(Action<WorldSocket, P1>));
			return delegate(WorldSocket target, ClientPacket p)
			{
				d(target, (P1)p);
			};
		}
	}

	private static readonly string ClientConnectionInitialize = "WORLD OF WARCRAFT CONNECTION - CLIENT TO SERVER - V2";

	private static readonly string ServerConnectionInitialize = "WORLD OF WARCRAFT CONNECTION - SERVER TO CLIENT - V2";

	private static readonly byte[] AuthCheckSeed = new byte[16]
	{
		197, 198, 152, 149, 118, 63, 29, 205, 182, 161,
		55, 40, 179, 18, 255, 138
	};

	private static readonly byte[] SessionKeySeed = new byte[16]
	{
		88, 203, 207, 64, 254, 46, 206, 166, 90, 144,
		184, 1, 104, 108, 40, 11
	};

	private static readonly byte[] ContinuedSessionSeed = new byte[16]
	{
		22, 173, 12, 212, 70, 249, 79, 178, 239, 125,
		234, 42, 23, 102, 77, 47
	};

	private static readonly byte[] EncryptionKeySeed = new byte[16]
	{
		233, 117, 60, 80, 144, 147, 97, 218, 59, 7,
		238, 250, 255, 157, 65, 184
	};

	private static readonly int HeaderSize = 16;

	private SocketBuffer _headerBuffer;

	private SocketBuffer _packetBuffer;

	private ConnectionType _connectType;

	private ulong _key;

	private byte[] _serverChallenge;

	private WorldCrypt _worldCrypt;

	private byte[] _sessionKey;

	private byte[] _encryptKey;

	private ConnectToKey _instanceConnectKey;

	private RealmId _realmId;

	private ZLib.z_stream _compressionStream;

	private ConcurrentDictionary<Opcode, PacketHandler> _clientPacketTable = new();

	private GlobalSessionData _globalSession;

	private Mutex _sendMutex = new();

	private BnetServices.ServiceManager _bnetRpc;

	public GlobalSessionData Session => _globalSession;

	[PacketHandler(Opcode.CMSG_ARENA_TEAM_ROSTER)]
	private void HandleArenaTeamRoster(ArenaTeamRosterRequest arena)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180) || GetSession().GameState.CurrentArenaTeamIds[arena.TeamIndex] == 0)
		{
			var response = new ArenaTeamRosterResponse
			{
				TeamSize = ModernVersion.GetArenaTeamSizeFromIndex(arena.TeamIndex)
			};
			SendPacket(response);
			return;
		}
		var packet = new WorldPacket(Opcode.CMSG_ARENA_TEAM_QUERY);
		packet.WriteUInt32(GetSession().GameState.CurrentArenaTeamIds[arena.TeamIndex]);
		SendPacketToServer(packet);
		var packet2 = new WorldPacket(Opcode.CMSG_ARENA_TEAM_ROSTER);
		packet2.WriteUInt32(GetSession().GameState.CurrentArenaTeamIds[arena.TeamIndex]);
		SendPacketToServer(packet2);
	}

	[PacketHandler(Opcode.CMSG_ARENA_TEAM_QUERY)]
	private void HandleArenaTeamQuery(ArenaTeamQuery arena)
	{
		if (GetSession().GameState.ArenaTeams.TryGetValue(arena.TeamId, out var team))
		{
			var response = new ArenaTeamQueryResponse
			{
				TeamId = arena.TeamId,
				Emblem = new ArenaTeamEmblem
				{
					TeamId = arena.TeamId,
					TeamSize = team.TeamSize,
					BackgroundColor = team.BackgroundColor,
					EmblemStyle = team.EmblemStyle,
					EmblemColor = team.EmblemColor,
					BorderStyle = team.BorderStyle,
					BorderColor = team.BorderColor,
					TeamName = team.Name
				}
			};
			SendPacket(response);
		}
	}

	[PacketHandler(Opcode.CMSG_BATTLEMASTER_JOIN_ARENA)]
	private void HandleBattlematerJoinArena(BattlemasterJoinArena join)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEMASTER_JOIN_ARENA);
		packet.WriteGuid(join.Guid.To64());
		packet.WriteUInt8(join.TeamIndex);
		packet.WriteBool(data: true);
		packet.WriteBool(data: true);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BATTLEMASTER_JOIN_SKIRMISH)]
	private void HandleBattlematerJoinSkirmish(BattlemasterJoinSkirmish join)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEMASTER_JOIN_ARENA);
		packet.WriteGuid(join.Guid.To64());
		packet.WriteUInt8(join.TeamSize);
		packet.WriteBool(join.AsGroup);
		packet.WriteBool(data: false);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ARENA_TEAM_REMOVE)]
	[PacketHandler(Opcode.CMSG_ARENA_TEAM_LEADER)]
	private void HandleArenaUnimplemented(ArenaTeamRemove arena)
	{
		var packet = new WorldPacket(arena.GetUniversalOpcode());
		packet.WriteUInt32(arena.TeamId);
		packet.WriteCString(GetSession().GameState.GetPlayerName(arena.PlayerGuid));
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ARENA_TEAM_DISBAND)]
	[PacketHandler(Opcode.CMSG_ARENA_TEAM_LEAVE)]
	private void HandleArenaTeamLeave(ArenaTeamLeave arena)
	{
		var packet = new WorldPacket(arena.GetUniversalOpcode());
		packet.WriteUInt32(arena.TeamId);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ARENA_TEAM_ACCEPT)]
	[PacketHandler(Opcode.CMSG_ARENA_TEAM_DECLINE)]
	private void HandleArenaTeamInviteResponse(ArenaTeamAccept arena)
	{
		var packet = new WorldPacket(arena.GetUniversalOpcode());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUCTION_HELLO_REQUEST)]
	private void HandleAuctionHelloRequest(InteractWithNPC interact)
	{
		var packet = new WorldPacket(Opcode.MSG_AUCTION_HELLO);
		packet.WriteGuid(interact.CreatureGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUCTION_LIST_BIDDED_ITEMS)]
	private void HandleAuctionListBidderItems(AuctionListBidderItems auction)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUCTION_LIST_BIDDED_ITEMS);
		packet.WriteGuid(auction.Auctioneer.To64());
		packet.WriteUInt32(auction.Offset);
		packet.WriteInt32(auction.AuctionItemIDs.Count);
		foreach (var itemId in auction.AuctionItemIDs)
		{
			packet.WriteUInt32(itemId);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUCTION_LIST_OWNED_ITEMS)]
	private void HandleAuctionListOwnerItems(AuctionListOwnerItems auction)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUCTION_LIST_OWNED_ITEMS);
		packet.WriteGuid(auction.Auctioneer.To64());
		packet.WriteUInt32(auction.Offset);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUCTION_LIST_ITEMS)]
	private void HandleAuctionListItems(AuctionListItems auction)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUCTION_LIST_ITEMS);
		packet.WriteGuid(auction.Auctioneer.To64());
		packet.WriteUInt32(auction.Offset);
		packet.WriteCString(auction.Name);
		packet.WriteUInt8(auction.MinLevel);
		packet.WriteUInt8(auction.MaxLevel);
		if (auction.ClassFilters.Count > 0)
		{
			if (auction.ClassFilters[0].SubClassFilters.Count == 1)
			{
				packet.WriteInt32(ModernToLegacyInventorySlotType(auction.ClassFilters[0].SubClassFilters[0].InvTypeMask));
				packet.WriteInt32(auction.ClassFilters[0].ItemClass);
				packet.WriteInt32(auction.ClassFilters[0].SubClassFilters[0].ItemSubclass);
			}
			else
			{
				packet.WriteInt32(-1);
				packet.WriteInt32(auction.ClassFilters[0].ItemClass);
				packet.WriteInt32(-1);
			}
		}
		else
		{
			packet.WriteInt32(-1);
			packet.WriteInt32(-1);
			packet.WriteInt32(-1);
		}
		packet.WriteInt32(auction.Quality);
		packet.WriteBool(auction.OnlyUsable);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteBool(auction.ExactMatch);
			packet.WriteUInt8((byte)auction.Sorts.Count);
			foreach (var sort in auction.Sorts)
			{
				packet.WriteUInt8(sort.Type);
				packet.WriteUInt8(sort.Direction);
			}
		}
		SendPacketToServer(packet);
		static int ModernToLegacyInventorySlotType(uint modernInventoryFlag)
		{
			if (modernInventoryFlag == uint.MaxValue)
			{
				return -1;
			}
			for (var i = 0; i < 32; i++)
			{
				if ((modernInventoryFlag & (1 << i)) > 0)
				{
					return i;
				}
			}
			return -1;
		}
	}

	private int ModernToLegacyInventorySlotType(uint modernInventoryFlag)
	{
		if (modernInventoryFlag == uint.MaxValue)
		{
			return -1;
		}
		for (byte i = 0; i < 32; i++)
		{
			if ((modernInventoryFlag & (uint)(1 << i)) != 0)
			{
				return i;
			}
		}
		return -1;
	}

	[PacketHandler(Opcode.CMSG_AUCTION_SELL_ITEM)]
	private void HandleAuctionSellItem(AuctionSellItem auction)
	{
		var expireTime = auction.ExpireTime;
		if (LegacyVersion.ExpansionVersion <= 1 && ModernVersion.ExpansionVersion > 1)
		{
			switch (expireTime)
			{
			case 720u:
				expireTime = 120u;
				break;
			case 1440u:
				expireTime = 480u;
				break;
			case 2880u:
				expireTime = 1440u;
				break;
			}
		}
		else if (LegacyVersion.ExpansionVersion > 1 && ModernVersion.ExpansionVersion <= 1)
		{
			switch (expireTime)
			{
			case 120u:
				expireTime = 720u;
				break;
			case 480u:
				expireTime = 1440u;
				break;
			case 1440u:
				expireTime = 2880u;
				break;
			}
		}
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_2_2a_10505))
		{
			foreach (var item in auction.Items)
			{
				var packet = new WorldPacket(Opcode.CMSG_AUCTION_SELL_ITEM);
				packet.WriteGuid(auction.Auctioneer.To64());
				packet.WriteGuid(item.Guid.To64());
				packet.WriteUInt32((uint)auction.MinBid);
				packet.WriteUInt32((uint)auction.BuyoutPrice);
				packet.WriteUInt32(expireTime);
				SendPacketToServer(packet);
			}
			return;
		}
		var packet2 = new WorldPacket(Opcode.CMSG_AUCTION_SELL_ITEM);
		packet2.WriteGuid(auction.Auctioneer.To64());
		packet2.WriteInt32(auction.Items.Count);
		foreach (var item2 in auction.Items)
		{
			packet2.WriteGuid(item2.Guid.To64());
			packet2.WriteUInt32(item2.UseCount);
		}
		packet2.WriteUInt32((uint)auction.MinBid);
		packet2.WriteUInt32((uint)auction.BuyoutPrice);
		packet2.WriteUInt32(expireTime);
		SendPacketToServer(packet2);
	}

	[PacketHandler(Opcode.CMSG_AUCTION_REMOVE_ITEM)]
	private void HandleAuctionRemoveItem(AuctionRemoveItem auction)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUCTION_REMOVE_ITEM);
		packet.WriteGuid(auction.Auctioneer.To64());
		packet.WriteUInt32(auction.AuctionID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUCTION_PLACE_BID)]
	private void HandleAuctionPlaceBId(AuctionPlaceBid auction)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUCTION_PLACE_BID);
		packet.WriteGuid(auction.Auctioneer.To64());
		packet.WriteUInt32(auction.AuctionID);
		packet.WriteUInt32((uint)auction.BidAmount);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BATTLEMASTER_JOIN)]
	private void HandleBattlefieldJoin(BattlemasterJoin join)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEMASTER_JOIN);
		packet.WriteGuid(join.BattlemasterGuid.To64());
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(GameData.GetMapIdFromBattlegroundId(join.BattlefieldListId));
		}
		else
		{
			packet.WriteUInt32(join.BattlefieldListId);
		}
		packet.WriteInt32(join.BattlefieldInstanceID);
		packet.WriteBool(join.JoinAsGroup);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BATTLEFIELD_PORT)]
	private void HandleBattlefieldPort(BattlefieldPort port)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_PORT);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt8(2);
			packet.WriteUInt8(0);
			packet.WriteUInt32(GetSession().GameState.GetBattleFieldQueueType(port.Ticket.Id));
			packet.WriteUInt16(8080);
			packet.WriteBool(port.AcceptedInvite);
		}
		else
		{
			packet.WriteUInt32(GetSession().GameState.GetBattleFieldQueueType(port.Ticket.Id));
			packet.WriteBool(port.AcceptedInvite);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_BATTLEFIELD_STATUS)]
	private void HandleRequestBattlefieldStatus(RequestBattlefieldStatus log)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_STATUS);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PVP_LOG_DATA)]
	private void HandlePvPLogData(PVPLogDataRequest log)
	{
		var packet = new WorldPacket(Opcode.MSG_PVP_LOG_DATA);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BATTLEFIELD_LIST)]
	private void HandleBattlefieldList(BattlefieldListRequest request)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_LIST);
		packet.WriteUInt32((uint)request.ListID);
		packet.WriteUInt8(0); // fromWhere: 0=battlemaster, 1=UI
		packet.WriteUInt8(1); // canGainXP
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BATTLEFIELD_LEAVE)]
	private void HandleBattlefieldLeave(BattlefieldLeave leave)
	{
		var packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_LEAVE);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt8(2);
			packet.WriteUInt8(0);
			packet.WriteUInt32(GetSession().GameState.GetBattleFieldQueueType(1u));
			packet.WriteUInt16(8080);
		}
		else
		{
			packet.WriteUInt32(GetSession().GameState.CurrentMapId.Value);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ENUM_CHARACTERS)]
	private void HandleEnumCharacters(EnumCharacters charEnum)
	{
		var packet = new WorldPacket(Opcode.CMSG_ENUM_CHARACTERS);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GET_ACCOUNT_CHARACTER_LIST)]
	private void HandleGetAccountCharacterList(GetAccountCharacterListRequest request)
	{
		var response = new GetAccountCharacterListResult
		{
			Token = request.Token
		};
		foreach (var ownCharacter in GetSession().GameState.OwnCharacters)
		{
			response.CharacterList.Add(new AccountCharacterListEntry
			{
				AccountId = WowGuid128.Create(HighGuidType703.WowAccount, GetSession().GameAccountInfo.Id),
				CharacterGuid = ownCharacter.CharacterGuid,
				RealmVirtualAddress = GetSession().RealmId.GetAddress(),
				RealmName = "",
				LastLoginUnixSec = ownCharacter.LastLoginUnixSec,
				Name = ownCharacter.Name,
				Race = ownCharacter.RaceId,
				Class = ownCharacter.ClassId,
				Sex = ownCharacter.SexId,
				Level = ownCharacter.Level
			});
		}
		SendPacket(response);
	}

	[PacketHandler(Opcode.CMSG_GENERATE_RANDOM_CHARACTER_NAME)]
	private void HandleGenerateRandomCharacterNameRequest(GenerateRandomCharacterNameRequest randomCharacterName)
	{
		var result = new GenerateRandomCharacterNameResult
		{
			Success = false
		};
		SendPacket(result);
	}

	[PacketHandler(Opcode.CMSG_ALTER_APPEARANCE)]
	private void HandleAlterAppearance(AlterAppearance alter)
	{
		CharacterCustomizations.ConvertModernCustomizationsToLegacy(alter.Customizations, out var skin, out var face, out var hairStyle, out var hairColor, out var facialhair);
		var packet = new WorldPacket(Opcode.CMSG_ALTER_APPEARANCE);
		packet.WriteUInt32(hairStyle);
		packet.WriteUInt32(hairColor);
		packet.WriteUInt32(facialhair);
		packet.WriteUInt32(skin);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_UPDATE_MISSILE_TRAJECTORY)]
	private void HandleUpdateMissileTrajectory(UpdateMissileTrajectory missile)
	{
		var packet = new WorldPacket(Opcode.CMSG_UPDATE_MISSILE_TRAJECTORY);
		packet.WriteGuid(missile.Guid.To64());
		packet.WriteUInt32((uint)missile.SpellID);
		packet.WriteFloat(missile.Pitch);
		packet.WriteFloat(missile.Speed);
		packet.WriteFloat(missile.FirePosX);
		packet.WriteFloat(missile.FirePosY);
		packet.WriteFloat(missile.FirePosZ);
		packet.WriteFloat(missile.ImpactPosX);
		packet.WriteFloat(missile.ImpactPosY);
		packet.WriteFloat(missile.ImpactPosZ);
		packet.WriteUInt8(0); // moveStop
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CREATE_CHARACTER)]
	private void HandleCreateCharacter(CreateCharacter charCreate)
	{
		var packet = new WorldPacket(Opcode.CMSG_CREATE_CHARACTER);
		packet.WriteCString(charCreate.CreateInfo.Name);
		packet.WriteUInt8((byte)charCreate.CreateInfo.RaceId);
		packet.WriteUInt8((byte)charCreate.CreateInfo.ClassId);
		packet.WriteUInt8((byte)charCreate.CreateInfo.Sex);
		CharacterCustomizations.ConvertModernCustomizationsToLegacy(charCreate.CreateInfo.Customizations, out var skin, out var face, out var hairStyle, out var hairColor, out var facialhair);
		packet.WriteUInt8(skin);
		packet.WriteUInt8(face);
		packet.WriteUInt8(hairStyle);
		packet.WriteUInt8(hairColor);
		packet.WriteUInt8(facialhair);
		packet.WriteUInt8(0);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHAR_DELETE)]
	private void HandleCharDelete(CharDelete charDelete)
	{
		var packet = new WorldPacket(Opcode.CMSG_CHAR_DELETE);
		packet.WriteGuid(charDelete.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_LOADING_SCREEN_NOTIFY)]
	private void HandleLoadScreen(LoadingScreenNotify loadingScreenNotify)
	{
		// Only update CurrentMapId when the loading screen is appearing (Showing=true).
		// When it dismisses (Showing=false) the client may send MapID=0, which would
		// incorrectly reset CurrentMapId and cause all subsequent UpdateObject packets
		// to advertise MapID=0 instead of the player's actual map.
		if (loadingScreenNotify.Showing && loadingScreenNotify.MapID > 0)
		{
			GetSession().GameState.CurrentMapId = loadingScreenNotify.MapID;
		}
	}

	[PacketHandler(Opcode.CMSG_QUERY_PLAYER_NAME)]
	private void HandleNameQueryRequest(QueryPlayerName queryPlayerName)
	{
		var packet = new WorldPacket(Opcode.CMSG_NAME_QUERY);
		packet.WriteGuid(queryPlayerName.Player.To64());
		SendPacketToServer(packet, !GetSession().GameState.IsInWorld ? Opcode.SMSG_LOGIN_VERIFY_WORLD : Opcode.MSG_NULL_ACTION);
	}

	[PacketHandler(Opcode.CMSG_QUERY_PLAYER_NAMES)]
	private void HandleNamesQueryRequest(QueryPlayerNames queryPlayerNames)
	{
		foreach (var guid in queryPlayerNames.Players)
		{
			var packet = new WorldPacket(Opcode.CMSG_NAME_QUERY);
			packet.WriteGuid(guid.To64());
			SendPacketToServer(packet, !GetSession().GameState.IsInWorld ? Opcode.SMSG_LOGIN_VERIFY_WORLD : Opcode.MSG_NULL_ACTION);
		}
	}

	[PacketHandler(Opcode.CMSG_PLAYER_LOGIN)]
	private void HandlePlayerLogin(PlayerLogin playerLogin)
	{
		if (!GetSession().GameState.CachedPlayers.TryGetValue(playerLogin.Guid, out var selectedChar))
		{
			Log.Print(LogType.Error, $"Player tried to log in with unknown char id: {playerLogin.Guid}", "CharacterHandler.cs");
			return;
		}
		var realm = GetSession().RealmManager.GetRealm(GetSession().RealmId);
		if (realm == null)
		{
			Log.Print(LogType.Error, $"Player tried to log in to unknown realm id: {GetSession().RealmId}", "CharacterHandler.cs");
			return;
		}
		GetSession().AccountMetaDataMgr.SaveLastSelectedCharacter(realm.Name, selectedChar.Name, playerLogin.Guid.Low, Time.UnixTime);
		if (GetSession().AuthClient != null)
		{
			GetSession().AuthClient.Disconnect();
		}
		SendConnectToInstance(ConnectToSerial.WorldAttempt1);
		GetSession().GameState.IsFirstEnterWorld = true;
		GetSession().GameState.CurrentPlayerGuid = playerLogin.Guid;
		GetSession().GameState.CurrentPlayerInfo = GetSession().GameState.OwnCharacters.Single(x => x.CharacterGuid == playerLogin.Guid);
		GetSession().GameState.CurrentPlayerStorage.LoadCurrentPlayer();
		var packet = new WorldPacket(Opcode.CMSG_PLAYER_LOGIN);
		packet.WriteGuid(playerLogin.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_LOGOUT_REQUEST)]
	private void HandleLogoutRequest(LogoutRequest logoutRequest)
	{
		var packet = new WorldPacket(Opcode.CMSG_LOGOUT_REQUEST);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_LOGOUT_CANCEL)]
	private void HandleLogoutCancel(LogoutCancel logoutCancel)
	{
		var packet = new WorldPacket(Opcode.CMSG_LOGOUT_CANCEL);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_PLAYED_TIME)]
	private void HandleRequestPlayedTime(RequestPlayedTime played)
	{
		var packet = new WorldPacket(Opcode.CMSG_REQUEST_PLAYED_TIME);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteBool(played.TriggerScriptEvent);
		}
		SendPacketToServer(packet);
		GetSession().GameState.ShowPlayedTime = played.TriggerScriptEvent;
	}

	[PacketHandler(Opcode.CMSG_SET_TITLE)]
	private void HandleTogglePvP(SetTitle title)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_TITLE);
		packet.WriteInt32(title.TitleID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_TOGGLE_PVP)]
	private void HandleTogglePvP(TogglePvP pvp)
	{
		var packet = new WorldPacket(Opcode.CMSG_TOGGLE_PVP);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_PVP)]
	private void HandleTogglePvP(SetPvP pvp)
	{
		var packet = new WorldPacket(Opcode.CMSG_TOGGLE_PVP);
		packet.WriteBool(pvp.Enable);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_ACTION_BUTTON)]
	private void HandleSetActionButton(SetActionButton button)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_ACTION_BUTTON);
		packet.WriteUInt8(button.Index);
		packet.WriteUInt16(button.Action);
		packet.WriteUInt16(button.Type);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_ACTION_BAR_TOGGLES)]
	private void HandleSetActionBarToggles(SetActionBarToggles bars)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_ACTION_BAR_TOGGLES);
		packet.WriteUInt8(bars.Mask);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_UNLEARN_SKILL)]
	private void HandleUnlearnSkill(UnlearnSkill skill)
	{
		var packet = new WorldPacket(Opcode.CMSG_UNLEARN_SKILL);
		packet.WriteUInt32(skill.SkillLine);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PLAYER_SHOWING_CLOAK)]
	[PacketHandler(Opcode.CMSG_PLAYER_SHOWING_HELM)]
	private void HandleShowHelmOrCloak(PlayerShowingHelmOrCloak show)
	{
		var packet = new WorldPacket(show.GetUniversalOpcode());
		packet.WriteBool(show.Showing);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_INSPECT)]
	private void HandleInspect(Inspect inspect)
	{
		var packet = new WorldPacket(Opcode.CMSG_INSPECT);
		packet.WriteGuid(inspect.Target.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_INSPECT_HONOR_STATS)]
	private void HandleInspectHonorStats(Inspect inspect)
	{
		var packet = new WorldPacket(Opcode.MSG_INSPECT_HONOR_STATS);
		packet.WriteGuid(inspect.Target.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_INSPECT_PVP)]
	private void HandleInspectArenaTeams(Inspect inspect)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.MSG_INSPECT_ARENA_TEAMS);
			packet.WriteGuid(inspect.Target.To64());
			SendPacketToServer(packet);
			return;
		}
		var pvp = new InspectPvP
		{
			PlayerGUID = inspect.Target
		};
		pvp.ArenaTeams.Add(new ArenaTeamInspectData());
		pvp.ArenaTeams.Add(new ArenaTeamInspectData());
		pvp.ArenaTeams.Add(new ArenaTeamInspectData());
		SendPacket(pvp);
	}

	[PacketHandler(Opcode.CMSG_CHARACTER_RENAME_REQUEST)]
	private void HandleCharacterRenameRequest(CharacterRenameRequest rename)
	{
		var packet = new WorldPacket(Opcode.CMSG_CHARACTER_RENAME_REQUEST);
		packet.WriteGuid(rename.Guid.To64());
		packet.WriteCString(rename.NewName);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHAT_JOIN_CHANNEL)]
	private void HandleChatJoinChannel(JoinChannel join)
	{
		if (GetSession().WorldClient != null)
		{
			GetSession().WorldClient.SendChatJoinChannel(join.ChatChannelId, join.ChannelName, join.Password);
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_LEAVE_CHANNEL)]
	private void HandleChatLeaveChannel(LeaveChannel leave)
	{
		if (GetSession().WorldClient != null)
		{
			GetSession().GameState.LeftChannelName = leave.ChannelName;
			GetSession().WorldClient.SendChatLeaveChannel(leave.ZoneChannelID, leave.ChannelName);
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_OWNER)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_ANNOUNCEMENTS)]
	private void HandleChatChannelCommand(ChannelCommand command)
	{
		var packet = new WorldPacket(command.GetUniversalOpcode());
		packet.WriteCString(command.ChannelName);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_LIST)]
	private void HandleChatChannelList(ChannelCommand command)
	{
		var packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_LIST);
		packet.WriteCString(command.ChannelName);
		SendPacketToServer(packet);
		GetSession().GameState.ChannelDisplayList = false;
	}

	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_DISPLAY_LIST)]
	private void HandleChatChannelDisplayList(ChannelCommand command)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_LIST);
			packet.WriteCString(command.ChannelName);
			SendPacketToServer(packet);
		}
		else
		{
			var packet2 = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_DISPLAY_LIST);
			packet2.WriteCString(command.ChannelName);
			SendPacketToServer(packet2);
		}
		GetSession().GameState.ChannelDisplayList = true;
	}

	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_DECLINE_INVITE)]
	private void HandleChatChannelDeclineInvite(ChannelCommand command)
	{
		if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_DECLINE_INVITE);
			packet.WriteCString(command.ChannelName);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_BAN)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_INVITE)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_KICK)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_MODERATOR)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_SET_OWNER)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_SILENCE_ALL)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_UNBAN)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_UNMODERATOR)]
	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_UNSILENCE_ALL)]
	private void HandleChatChannelPlayerCommand(ChannelPlayerCommand command)
	{
		var packet = new WorldPacket(command.GetUniversalOpcode());
		packet.WriteCString(command.ChannelName);
		packet.WriteCString(command.Name);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHAT_CHANNEL_PASSWORD)]
	private void HandleChatChannelPassword(ChannelPassword command)
	{
		var packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_PASSWORD);
		packet.WriteCString(command.ChannelName);
		packet.WriteCString(command.Password);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_AFK)]
	private void HandleChatMessageAFK(ChatMessageAFK afk)
	{
		var toBeSentTextParts = ConvertTextMessageIntoMaxLengthParts(afk.Text);
		if (toBeSentTextParts.Count >= 1)
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Afk, 0u, toBeSentTextParts[0], "", "");
			}
			else
			{
				GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Afk, 0u, toBeSentTextParts[0], "", "");
			}
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_DND)]
	private void HandleChatMessageDND(ChatMessageDND dnd)
	{
		var toBeSentTextParts = ConvertTextMessageIntoMaxLengthParts(dnd.Text);
		if (toBeSentTextParts.Count >= 1)
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Dnd, 0u, toBeSentTextParts[0], "", "");
			}
			else
			{
				GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Dnd, 0u, toBeSentTextParts[0], "", "");
			}
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_CHANNEL)]
	private void HandleChatMessageChannel(ChatMessageChannel channel)
	{
		var toBeSentTextParts = ConvertTextMessageIntoMaxLengthParts(channel.Text);
		foreach (var text in toBeSentTextParts)
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Channel, channel.Language, text, channel.Target, "");
			}
			else
			{
				GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Channel, channel.Language, text, channel.Target, "");
			}
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_WHISPER)]
	private void HandleChatMessageWhisper(ChatMessageWhisper whisper)
	{
		var toBeSentTextParts = ConvertTextMessageIntoMaxLengthParts(whisper.Text);
		foreach (var text in toBeSentTextParts)
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Whisper, whisper.Language, text, "", whisper.Target);
			}
			else
			{
				GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Whisper, whisper.Language, text, "", whisper.Target);
			}
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_EMOTE)]
	private void HandleChatMessageEmote(ChatMessageEmote emote)
	{
		var toBeSentTextParts = ConvertTextMessageIntoMaxLengthParts(emote.Text);
		if (toBeSentTextParts.Count >= 1)
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Emote, 0u, toBeSentTextParts[0], "", "");
			}
			else
			{
				GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Emote, 0u, toBeSentTextParts[0], "", "");
			}
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_GUILD)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_OFFICER)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_PARTY)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_RAID)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_SAY)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_YELL)]
	[PacketHandler(Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT)]
	private void HandleChatMessage(ChatMessage packet)
	{
		ChatMessageTypeModern type;
		switch (packet.GetUniversalOpcode())
		{
		case Opcode.CMSG_CHAT_MESSAGE_SAY:
			type = ChatMessageTypeModern.Say;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_YELL:
			type = ChatMessageTypeModern.Yell;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_GUILD:
			type = ChatMessageTypeModern.Guild;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
			type = ChatMessageTypeModern.Officer;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_PARTY:
			type = ChatMessageTypeModern.Party;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_RAID:
			type = ChatMessageTypeModern.Raid;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
			type = ChatMessageTypeModern.RaidWarning;
			break;
		case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
			type = !GetSession().GameState.IsInBattleground() ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground;
			break;
		default:
			Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "ChatHandler.cs");
			return;
		}
		var toBeSentTextParts = ConvertTextMessageIntoMaxLengthParts(packet.Text);
		foreach (var text in toBeSentTextParts)
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				var chatMsg = (ChatMessageTypeWotLK)Enum.Parse(typeof(ChatMessageTypeWotLK), type.ToString());
				GetSession().WorldClient.SendMessageChatWotLK(chatMsg, packet.Language, text, "", "");
			}
			else
			{
				var chatMsg2 = (ChatMessageTypeVanilla)Enum.Parse(typeof(ChatMessageTypeVanilla), type.ToString());
				GetSession().WorldClient.SendMessageChatVanilla(chatMsg2, packet.Language, text, "", "");
			}
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_ADDON_MESSAGE)]
	private void HandleAddonMessage(ChatAddonMessage packet)
	{
		var language = uint.MaxValue;
		var text = packet.Params.Prefix + "\t" + packet.Params.Text;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var chatMsg = (ChatMessageTypeWotLK)Enum.Parse(typeof(ChatMessageTypeWotLK), packet.Params.Type.ToString());
			GetSession().WorldClient.SendMessageChatWotLK(chatMsg, language, text, "", "");
		}
		else
		{
			var chatMsg2 = (ChatMessageTypeVanilla)Enum.Parse(typeof(ChatMessageTypeVanilla), packet.Params.Type.ToString());
			GetSession().WorldClient.SendMessageChatVanilla(chatMsg2, language, text, "", "");
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_ADDON_MESSAGE_TARGETED)]
	private void HandleAddonMessageTargeted(ChatAddonMessageTargeted packet)
	{
		var language = uint.MaxValue;
		var text = packet.Params.Prefix + "\t" + packet.Params.Text;
		var channelName = packet.ChannelGuid.IsEmpty() ? "" : GetSession().GameState.GetChannelName((int)packet.ChannelGuid.GetCounter());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var chatMsg = (ChatMessageTypeWotLK)Enum.Parse(typeof(ChatMessageTypeWotLK), packet.Params.Type.ToString());
			GetSession().WorldClient.SendMessageChatWotLK(chatMsg, language, text, channelName, packet.Target);
		}
		else
		{
			var chatMsg2 = (ChatMessageTypeVanilla)Enum.Parse(typeof(ChatMessageTypeVanilla), packet.Params.Type.ToString());
			GetSession().WorldClient.SendMessageChatVanilla(chatMsg2, language, text, channelName, packet.Target);
		}
	}

	[PacketHandler(Opcode.CMSG_SEND_TEXT_EMOTE)]
	private void HandleSendTextEmote(CTextEmote emote)
	{
		var packet = new WorldPacket(Opcode.CMSG_SEND_TEXT_EMOTE);
		packet.WriteInt32(emote.EmoteID);
		packet.WriteInt32(emote.SoundIndex);
		packet.WriteGuid(emote.Target.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHAT_REGISTER_ADDON_PREFIXES)]
	private void HandleChatRegisterAddonPrefixes(ChatRegisterAddonPrefixes addons)
	{
		foreach (var prefix in addons.Prefixes)
		{
			GetSession().GameState.AddonPrefixes.Add(prefix);
		}
	}

	[PacketHandler(Opcode.CMSG_CHAT_UNREGISTER_ALL_ADDON_PREFIXES)]
	private void HandleChatUnregisterAllAddonPrefixes(EmptyClientPacket addons)
	{
		GetSession().GameState.AddonPrefixes.Clear();
	}

	private static List<string> ConvertTextMessageIntoMaxLengthParts(string originalTextMessage)
	{
		var toBeSendTextParts = new List<string>();
		if (originalTextMessage.Length <= 255)
		{
			toBeSendTextParts.Add(originalTextMessage);
		}
		else
		{
			var linkBegin = "(?=\\|c[a-f0-9]{8}\\|H)";
			var linkEnd = "(?<=\\|h\\|r)";
			var splitted = Regex.Split(originalTextMessage, linkBegin + "|" + linkEnd);
			var splittedAndSlicedToMaxLength = splitted.SelectMany(x => x.Chunk(255));
			var strBuilder = new StringBuilder();
			foreach (var part in splittedAndSlicedToMaxLength)
			{
				if (strBuilder.Length + part.Length > 255)
				{
					toBeSendTextParts.Add(strBuilder.ToString());
					strBuilder.Clear();
				}
				strBuilder.Append(part);
			}
			toBeSendTextParts.Add(strBuilder.ToString());
		}
		return toBeSendTextParts;
	}

	[PacketHandler(Opcode.CMSG_UPDATE_ACCOUNT_DATA)]
	private void HandleUpdateAccountData(UserClientUpdateAccountData data)
	{
		GetSession().AccountDataMgr.SaveData(data.PlayerGuid, data.Time, data.DataType, data.Size, data.CompressedData);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_ACCOUNT_DATA)]
	private void HandleRequestAccountData(RequestAccountData data)
	{
		if (GetSession().AccountDataMgr.Data[data.DataType] == null)
		{
			Log.Print(LogType.Error, $"Client requested missing account data {data.DataType}.", "ClientConfigHandler.cs");
			GetSession().AccountDataMgr.Data[data.DataType] = new AccountData();
			GetSession().AccountDataMgr.Data[data.DataType].Type = data.DataType;
			GetSession().AccountDataMgr.Data[data.DataType].Timestamp = Time.UnixTime;
			GetSession().AccountDataMgr.Data[data.DataType].UncompressedSize = 0u;
			GetSession().AccountDataMgr.Data[data.DataType].CompressedData = new byte[0];
		}
		GetSession().AccountDataMgr.Data[data.DataType].Guid = data.PlayerGuid;
		var update = new UpdateAccountData(GetSession().AccountDataMgr.Data[data.DataType]);
		SendPacket(update);
	}

	[PacketHandler(Opcode.CMSG_SAVE_CUF_PROFILES)]
	private void HandleUpdateAccountData(SaveCUFProfiles cuf)
	{
		GetSession().AccountDataMgr.SaveCUFProfiles(cuf.Data);
	}

	[PacketHandler(Opcode.CMSG_ATTACK_SWING)]
	private void HandleAttackSwing(AttackSwing attack)
	{
		var packet = new WorldPacket(Opcode.CMSG_ATTACK_SWING);
		packet.WriteGuid(attack.Victim.To64());
		SendPacketToServer(packet);

		// Modern client doesn't send CMSG_CAST_SPELL for Auto Shot like the old client did.
		// If player has a ranged weapon equipped, auto-cast Auto Shot (spell 75) for them.
		if (GetSession().GameState.HasRangedWeapon())
		{
			var castPacket = new WorldPacket(Opcode.CMSG_CAST_SPELL);
			castPacket.WriteUInt8(0); // cast count
			castPacket.WriteUInt32(75); // Auto Shot spell ID
			castPacket.WriteUInt8(0); // cast flags
			// Target flags: unit target
			castPacket.WriteUInt32(2); // TARGET_FLAG_UNIT
			castPacket.WritePackedGuid(attack.Victim.To64());
			SendPacketToServer(castPacket);
		}
	}

	[PacketHandler(Opcode.CMSG_ATTACK_STOP)]
	private void HandleAttackSwing(AttackStop attack)
	{
		var packet = new WorldPacket(Opcode.CMSG_ATTACK_STOP);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_SHEATHED)]
	private void HandleSetSheathed(SetSheathed sheath)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_SHEATHED);
		packet.WriteInt32(sheath.SheathState);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CAN_DUEL)]
	private void HandleCanDuel(CanDuel request)
	{
		var result = new CanDuelResult
		{
			TargetGUID = request.TargetGUID,
			Result = true
		};
		SendPacket(result);
	}

	[PacketHandler(Opcode.CMSG_DUEL_RESPONSE)]
	private void HandleDuelResponse(DuelResponse response)
	{
		if (response.Accepted)
		{
			var packet = new WorldPacket(Opcode.CMSG_DUEL_ACCEPTED);
			packet.WriteGuid(response.ArbiterGUID.To64());
			SendPacketToServer(packet);
		}
		else
		{
			var packet2 = new WorldPacket(Opcode.CMSG_DUEL_CANCELLED);
			packet2.WriteGuid(response.ArbiterGUID.To64());
			SendPacketToServer(packet2);
		}
	}

	[PacketHandler(Opcode.CMSG_GAME_OBJ_USE)]
	private void HandleGameObjUse(GameObjUse use)
	{
		var guid64 = use.Guid.To64();
		Log.Print(LogType.Debug, $"[GameObjUse] Modern GUID={use.Guid} -> Legacy GUID={guid64} raw=0x{guid64.GetLowValue():X16}");
		var packet = new WorldPacket(Opcode.CMSG_GAME_OBJ_USE);
		packet.WriteGuid(guid64);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GAME_OBJ_REPORT_USE)]
	private void HandleGameObjUse(GameObjReportUse use)
	{
		GetSession().GameState.CurrentInteractedWithGO = use.Guid;
		var guid64 = use.Guid.To64();
		Log.Print(LogType.Debug, $"[GameObjReportUse] Modern={use.Guid} Legacy={guid64} Entry={guid64.GetEntry()} Low={guid64.GetLowValue():X16}", "HandleGameObjReportUse", "");
		// Send GAME_OBJ_USE to trigger the interaction on the server
		var usePacket = new WorldPacket(Opcode.CMSG_GAME_OBJ_USE);
		usePacket.WriteGuid(guid64);
		SendPacketToServer(usePacket);
		// Also send GAME_OBJ_REPORT_USE for tracking
		var reportPacket = new WorldPacket(Opcode.CMSG_GAME_OBJ_REPORT_USE);
		reportPacket.WriteGuid(guid64);
		SendPacketToServer(reportPacket);
	}

	[PacketHandler(Opcode.CMSG_PARTY_INVITE)]
	private void HandleUpdateRaidTarget(PartyInviteClient invite)
	{
		var packet = new WorldPacket(Opcode.CMSG_PARTY_INVITE);
		packet.WriteCString(invite.TargetName);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(0u);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PARTY_INVITE_RESPONSE)]
	private void HandlePartyInviteResponse(PartyInviteResponse invite)
	{
		if (invite.Accept)
		{
			var packet = new WorldPacket(Opcode.CMSG_GROUP_ACCEPT);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				packet.WriteUInt32(0u);
			}
			SendPacketToServer(packet);
		}
		else
		{
			var packet2 = new WorldPacket(Opcode.CMSG_GROUP_DECLINE);
			SendPacketToServer(packet2);
		}
	}

	[PacketHandler(Opcode.CMSG_LEAVE_GROUP)]
	private void HandleLeaveGroup(LeaveGroup leave)
	{
		GetSession().GameState.WeWantToLeaveGroup = true;
		var packet = new WorldPacket(Opcode.CMSG_GROUP_DISBAND);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PARTY_UNINVITE)]
	private void HandlePartyUninvite(PartyUninvite kick)
	{
		var packet = new WorldPacket(Opcode.CMSG_GROUP_UNINVITE_GUID);
		packet.WriteGuid(kick.TargetGUID.To64());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteCString(kick.Reason);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_ASSISTANT_LEADER)]
	private void HandleSetAssistantLeader(SetAssistantLeader assist)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_ASSISTANT_LEADER);
		packet.WriteGuid(assist.TargetGUID.To64());
		packet.WriteBool(assist.Apply);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_EVERYONE_IS_ASSISTANT)]
	private void HandleSetAssistantLeader(SetEveryoneIsAssistant assist)
	{
		var groupMembers = GetSession().GameState.GetCurrentGroup().PlayerList;
		foreach (var member in groupMembers)
		{
			if (!(member.GUID == GetSession().GameState.CurrentPlayerGuid))
			{
				var packet = new WorldPacket(Opcode.CMSG_SET_ASSISTANT_LEADER);
				packet.WriteGuid(member.GUID.To64());
				packet.WriteBool(assist.Apply);
				SendPacketToServer(packet);
			}
		}
	}

	[PacketHandler(Opcode.CMSG_SET_PARTY_LEADER)]
	private void HandleSetPartyLeader(SetPartyLeader leader)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_PARTY_LEADER);
		packet.WriteGuid(leader.TargetGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CONVERT_RAID)]
	private void HandleConvertRaid(ConvertRaid raid)
	{
		var packet = new WorldPacket(Opcode.CMSG_CONVERT_RAID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_DO_READY_CHECK)]
	private void HandlReadyCheck(DoReadyCheck raid)
	{
		var packet = new WorldPacket(Opcode.MSG_RAID_READY_CHECK);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_READY_CHECK_RESPONSE)]
	private void HandlReadyCheckResponse(ReadyCheckResponseClient raid)
	{
		var packet = new WorldPacket(Opcode.MSG_RAID_READY_CHECK);
		packet.WriteBool(raid.IsReady);
		SendPacketToServer(packet);
		var ready = new ReadyCheckResponse
		{
			Player = GetSession().GameState.CurrentPlayerGuid,
			IsReady = raid.IsReady,
			PartyGUID = WowGuid128.Create(HighGuidType703.Party, 1000uL)
		};
		SendPacket(ready);
	}

	[PacketHandler(Opcode.CMSG_UPDATE_RAID_TARGET)]
	private void HandleUpdateRaidTarget(UpdateRaidTarget update)
	{
		var packet = new WorldPacket(Opcode.MSG_RAID_TARGET_UPDATE);
		packet.WriteInt8(update.Symbol);
		packet.WriteGuid(update.Target.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SUMMON_RESPONSE)]
	private void HandleSummonResponse(SummonResponse update)
	{
		if (update.Accept || LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_SUMMON_RESPONSE);
			packet.WriteGuid(update.SummonerGUID.To64());
			packet.WriteBool(update.Accept);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_MINIMAP_PING)]
	private void HandleMinimapPing(MinimapPingClient ping)
	{
		var packet = new WorldPacket(Opcode.MSG_MINIMAP_PING);
		packet.WriteVector2(ping.Position);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_RANDOM_ROLL)]
	private void HandleMinimapPing(RandomRollClient roll)
	{
		var packet = new WorldPacket(Opcode.MSG_RANDOM_ROLL);
		packet.WriteInt32(roll.Min);
		packet.WriteInt32(roll.Max);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_PARTY_MEMBER_STATS)]
	private void HandleRequestPartyMemberStats(RequestPartyMemberStats request)
	{
		var packet = new WorldPacket(Opcode.CMSG_REQUEST_PARTY_MEMBER_STATS);
		packet.WriteGuid(request.TargetGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GROUP_CHANGE_SUB_GROUP)]
	private void HandleGroupChangeSubGroup(ChangeSubGroup group)
	{
		var packet = new WorldPacket(Opcode.CMSG_GROUP_CHANGE_SUB_GROUP);
		packet.WriteCString(GetSession().GameState.GetPlayerName(group.TargetGUID));
		packet.WriteUInt8(group.NewSubGroup);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GROUP_SWAP_SUB_GROUP)]
	private void HandleGroupSwapSubGroup(SwapSubGroups group)
	{
		var packet = new WorldPacket(Opcode.CMSG_GROUP_SWAP_SUB_GROUP);
		packet.WriteCString(GetSession().GameState.GetPlayerName(group.FirstTarget));
		packet.WriteCString(GetSession().GameState.GetPlayerName(group.SecondTarget));
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_GUILD_INFO)]
	private void HandleQueryGuildInfo(QueryGuildInfo query)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_GUILD_INFO);
		packet.WriteUInt32((uint)query.GuildGuid.GetCounter());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_PERMISSIONS_QUERY)]
	private void HandleGuildPermissionsQuery(GuildPermissionsQuery query)
	{
		if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.MSG_GUILD_PERMISSIONS);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY_QUERY)]
	private void HandleGuildBankRemainingWithdrawnMoneyQuery(GuildBankRemainingWithdrawMoneyQuery query)
	{
		if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.MSG_GUILD_BANK_MONEY_WITHDRAWN);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_GUILD_GET_ROSTER)]
	private void HandleGuildGetRoster(GuildGetRoster query)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_INFO);
		SendPacketToServer(packet);
		var packet2 = new WorldPacket(Opcode.CMSG_GUILD_GET_ROSTER);
		SendPacketToServer(packet2);
	}

	[PacketHandler(Opcode.CMSG_GUILD_UPDATE_MOTD_TEXT)]
	private void HandleGuildUpdateMotdText(GuildUpdateMotdText text)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_UPDATE_MOTD_TEXT);
		packet.WriteCString(text.MotdText);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_UPDATE_INFO_TEXT)]
	private void HandleGuildUpdateInfoText(GuildUpdateInfoText text)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_UPDATE_INFO_TEXT);
		packet.WriteCString(text.InfoText);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_SET_MEMBER_NOTE)]
	private void HandleGuildSetMemberNote(GuildSetMemberNote note)
	{
		var packet = new WorldPacket(note.IsPublic ? Opcode.CMSG_GUILD_SET_PUBLIC_NOTE : Opcode.CMSG_GUILD_SET_OFFICER_NOTE);
		packet.WriteCString(GetSession().GameState.GetPlayerName(note.NoteeGUID));
		packet.WriteCString(note.Note);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_PROMOTE_MEMBER)]
	private void HandleGuildPromoteMember(GuildPromoteMember promote)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_PROMOTE_MEMBER);
		packet.WriteCString(GetSession().GameState.GetPlayerName(promote.Promotee));
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_DEMOTE_MEMBER)]
	private void HandleGuildDemoteMember(GuildDemoteMember demote)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_DEMOTE_MEMBER);
		packet.WriteCString(GetSession().GameState.GetPlayerName(demote.Demotee));
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_OFFICER_REMOVE_MEMBER)]
	private void HandleGuildOfficerRemoveMember(GuildOfficerRemoveMember remove)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_OFFICER_REMOVE_MEMBER);
		packet.WriteCString(GetSession().GameState.GetPlayerName(remove.Removee));
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_INVITE_BY_NAME)]
	private void HandleGuildInviteByName(GuildInviteByName invite)
	{
		if (invite.ArenaTeamId == 0)
		{
			var packet = new WorldPacket(Opcode.CMSG_GUILD_INVITE_BY_NAME);
			packet.WriteCString(invite.Name);
			SendPacketToServer(packet);
		}
		else
		{
			var packet2 = new WorldPacket(Opcode.CMSG_ARENA_TEAM_INVITE);
			packet2.WriteUInt32(invite.ArenaTeamId);
			packet2.WriteCString(invite.Name);
			SendPacketToServer(packet2);
		}
	}

	[PacketHandler(Opcode.CMSG_GUILD_SET_RANK_PERMISSIONS)]
	private void HandleGuildSetRankPermissions(GuildSetRankPermissions rank)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_SET_RANK_PERMISSIONS);
		packet.WriteUInt32(rank.RankID);
		packet.WriteUInt32(rank.Flags);
		packet.WriteCString(rank.RankName);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteInt32(rank.WithdrawGoldLimit);
			for (var i = 0; i < 6; i++)
			{
				packet.WriteUInt32(rank.TabFlags[i]);
				packet.WriteUInt32(rank.TabWithdrawItemLimit[i]);
			}
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_ADD_RANK)]
	private void HandleGuildAddRank(GuildAddRank rank)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_ADD_RANK);
		packet.WriteCString(rank.Name);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_DELETE_RANK)]
	private void HandleGuildDeleteRank(GuildDeleteRank rank)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_DELETE_RANK);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_SET_GUILD_MASTER)]
	private void HandleGuildSetGuildMaster(GuildSetGuildMaster master)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_SET_GUILD_MASTER);
		packet.WriteCString(master.NewMasterName);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_LEAVE)]
	private void HandleGuildLeave(GuildLeave leave)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_LEAVE);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ACCEPT_GUILD_INVITE)]
	private void HandleGuildAccept(AcceptGuildInvite accept)
	{
		var packet = new WorldPacket(Opcode.CMSG_ACCEPT_GUILD_INVITE);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_DECLINE_INVITATION)]
	private void HandleGuildDecline(DeclineGuildInvite decline)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_DECLINE_INVITATION);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_DELETE)]
	private void HandleGuildDelete(GuildDelete delete)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_DELETE);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SAVE_GUILD_EMBLEM)]
	private void HandleSaveGuildEmblem(SaveGuildEmblem emblem)
	{
		var packet = new WorldPacket(Opcode.MSG_SAVE_GUILD_EMBLEM);
		packet.WriteGuid(emblem.DesignerGUID.To64());
		packet.WriteUInt32(emblem.EmblemStyle);
		packet.WriteUInt32(emblem.EmblemColor);
		packet.WriteUInt32(emblem.BorderStyle);
		packet.WriteUInt32(emblem.BorderColor);
		packet.WriteUInt32(emblem.BackgroundColor);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_DECLINE_GUILD_INVITES)]
	private void HandleDeclineGuildInvites(SetAutoDeclineGuildInvites packet)
	{
		GetSession().GameState.CurrentPlayerStorage.Settings.SetAutoBlockGuildInvites(packet.GuildInvitesShouldGetBlocked);
		var updateData = new ObjectUpdate(GetSession().GameState.CurrentPlayerGuid, UpdateTypeModern.Values, GetSession());
		var flags = GetSession().GameState.CurrentPlayerStorage.Settings.CreateNewFlags();
		updateData.PlayerData.PlayerFlags = (uint)flags;
		var updatePacket = new UpdateObject(GetSession().GameState);
		updatePacket.ObjectUpdates.Add(updateData);
		GetSession().WorldClient.SendPacketToClient(updatePacket);
	}

	[PacketHandler(Opcode.CMSG_GUILD_AUTO_DECLINE_INVITATION)]
	private void HandleGuildAutoDeclineInvitation(AutoDeclineGuildInvite autoDecline)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_DECLINE_INVITATION);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_ACTIVATE)]
	private void HandleGuildBankActivate(GuildBankAtivate activate)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_ACTIVATE);
		packet.WriteGuid(activate.BankGuid.To64());
		packet.WriteBool(activate.FullUpdate);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_QUERY_TAB)]
	private void HandleGuildBankQueryTab(GuildBankQueryTab query)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_QUERY_TAB);
		packet.WriteGuid(query.BankGuid.To64());
		packet.WriteUInt8(query.Tab);
		packet.WriteBool(query.FullUpdate);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_DEPOSIT_MONEY)]
	private void HandleGuildBankDepositMoney(GuildBankDepositMoney deposit)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_DEPOSIT_MONEY);
		packet.WriteGuid(deposit.BankGuid.To64());
		packet.WriteUInt32((uint)deposit.Money);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_TEXT_QUERY)]
	private void HandleGuildBankTextQuery(GuildBankTextQuery query)
	{
		var packet = new WorldPacket(Opcode.MSG_QUERY_GUILD_BANK_TEXT);
		packet.WriteUInt8((byte)query.Tab);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_UPDATE_TAB)]
	private void HandleGuildBankUpdateTab(GuildBankUpdateTab update)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_UPDATE_TAB);
		packet.WriteGuid(update.BankGuid.To64());
		packet.WriteUInt8(update.BankTab);
		packet.WriteCString(update.Name);
		packet.WriteCString(update.Icon);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_LOG_QUERY)]
	private void HandleGuildBankLogQuery(GuildBankLogQuery query)
	{
		var packet = new WorldPacket(Opcode.MSG_GUILD_BANK_LOG_QUERY);
		packet.WriteUInt8((byte)query.Tab);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_SET_TAB_TEXT)]
	private void HandleGuildBankSetTabText(GuildBankSetTabText query)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SET_TAB_TEXT);
		packet.WriteUInt8((byte)query.Tab);
		packet.WriteCString(query.TabText);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_BUY_TAB)]
	private void HandleGuildBankBuyTab(GuildBankBuyTab buy)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_BUY_TAB);
		packet.WriteGuid(buy.BankGuid.To64());
		packet.WriteUInt8(buy.BankTab);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GUILD_BANK_WITHDRAW_MONEY)]
	private void HandleGuildBankBuyTab(GuildBankWithdrawMoney withdraw)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_WITHDRAW_MONEY);
		packet.WriteGuid(withdraw.BankGuid.To64());
		packet.WriteUInt32((uint)withdraw.Money);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUTO_GUILD_BANK_ITEM)]
	private void HandleGuildBankItem(AutoGuildBankItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: false);
		packet.WriteUInt8(item.BankTab);
		packet.WriteUInt8(item.BankSlot);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: false);
		if (item.ContainerSlot.HasValue)
		{
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
			packet.WriteUInt8(item.ContainerItemSlot);
		}
		else
		{
			packet.WriteUInt8(byte.MaxValue);
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
		}
		packet.WriteBool(data: false);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(0u);
		}
		else
		{
			packet.WriteUInt8(0);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SPLIT_ITEM_TO_GUILD_BANK)]
	[PacketHandler(Opcode.CMSG_MERGE_ITEM_WITH_GUILD_BANK_ITEM)]
	private void HandleSplitItemToGuildBank(SplitItemToGuildBank item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: false);
		packet.WriteUInt8(item.BankTab);
		packet.WriteUInt8(item.BankSlot);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: false);
		if (item.ContainerSlot.HasValue)
		{
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
			packet.WriteUInt8(item.ContainerItemSlot);
		}
		else
		{
			packet.WriteUInt8(byte.MaxValue);
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
		}
		packet.WriteBool(data: false);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(item.StackCount);
		}
		else
		{
			packet.WriteUInt8((byte)item.StackCount);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUTO_STORE_GUILD_BANK_ITEM)]
	private void HandleAutoStoreGuildBankItem(AutoStoreGuildBankItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: false);
		packet.WriteUInt8(item.BankTab);
		packet.WriteUInt8(item.BankSlot);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: true);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(0u);
		}
		else
		{
			packet.WriteUInt8(0);
		}
		packet.WriteBool(data: true);
		packet.WriteUInt8(0);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_STORE_GUILD_BANK_ITEM)]
	private void HandleStoreGuildBankItem(AutoGuildBankItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: false);
		packet.WriteUInt8(item.BankTab);
		packet.WriteUInt8(item.BankSlot);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: false);
		if (item.ContainerSlot.HasValue)
		{
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
			packet.WriteUInt8(item.ContainerItemSlot);
		}
		else
		{
			packet.WriteUInt8(byte.MaxValue);
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
		}
		packet.WriteBool(data: true);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(0u);
		}
		else
		{
			packet.WriteUInt8(0);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MERGE_GUILD_BANK_ITEM_WITH_ITEM)]
	[PacketHandler(Opcode.CMSG_SPLIT_GUILD_BANK_ITEM_TO_INVENTORY)]
	private void HandleMergeGuildBankItemWithItem(SplitItemToGuildBank item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: false);
		packet.WriteUInt8(item.BankTab);
		packet.WriteUInt8(item.BankSlot);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: false);
		if (item.ContainerSlot.HasValue)
		{
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
			packet.WriteUInt8(item.ContainerItemSlot);
		}
		else
		{
			packet.WriteUInt8(byte.MaxValue);
			packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
		}
		packet.WriteBool(data: true);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(item.StackCount);
		}
		else
		{
			packet.WriteUInt8((byte)item.StackCount);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_GUILD_BANK_ITEM)]
	private void HandleMoveGuildBankItem(MoveGuildBankItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: true);
		packet.WriteUInt8(item.BankTab2);
		packet.WriteUInt8(item.BankSlot2);
		packet.WriteUInt32(0u);
		packet.WriteUInt8(item.BankTab1);
		packet.WriteUInt8(item.BankSlot1);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: false);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(0u);
		}
		else
		{
			packet.WriteUInt8(0);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SPLIT_GUILD_BANK_ITEM)]
	[PacketHandler(Opcode.CMSG_MERGE_GUILD_BANK_ITEM_WITH_GUILD_BANK_ITEM)]
	private void HandleMoveGuildBankItem(SplitGuildBankItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
		packet.WriteGuid(item.BankGuid.To64());
		packet.WriteBool(data: true);
		packet.WriteUInt8(item.BankTab2);
		packet.WriteUInt8(item.BankSlot2);
		packet.WriteUInt32(0u);
		packet.WriteUInt8(item.BankTab1);
		packet.WriteUInt8(item.BankSlot1);
		packet.WriteUInt32(0u);
		packet.WriteBool(data: false);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(item.StackCount);
		}
		else
		{
			packet.WriteUInt8((byte)item.StackCount);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_DB_QUERY_BULK)]
	private void HandleDbQueryBulk(DBQueryBulk query)
	{
		foreach (var id in query.Queries)
		{
			var reply = new DBReply
			{
				RecordID = id,
				TableHash = query.TableHash,
				Status = HotfixStatus.Invalid,
				Timestamp = (uint)Time.UnixTime
			};
			Log.PrintNet(LogType.Debug, LogNetDir.C2P, $"DB_QUERY_BULK requested ({query.TableHash}) #{id}", "HotfixHandler.cs");
			if (query.TableHash == DB2Hash.BroadcastText)
			{
				var bct = GameData.GetBroadcastText(id);
				if (bct == null)
				{
					bct = new BroadcastText
					{
						Entry = id,
						MaleText = "Clear your cache!",
						FemaleText = "Clear your cache!"
					};
				}
				reply.Status = HotfixStatus.Valid;
				reply.Data.WriteCString(bct.MaleText);
				reply.Data.WriteCString(bct.FemaleText);
				reply.Data.WriteUInt32(bct.Entry);
				reply.Data.WriteUInt32(bct.Language);
				reply.Data.WriteUInt32(0u);
				reply.Data.WriteUInt16(0);
				reply.Data.WriteUInt8(0);
				reply.Data.WriteUInt32(0u);
				if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
				{
					reply.Data.WriteUInt32(0u);
				}
				for (var i = 0; i < 2; i++)
				{
					reply.Data.WriteUInt32(0u);
				}
				for (var j = 0; j < 3; j++)
				{
					reply.Data.WriteUInt16(bct.Emotes[j]);
				}
				for (var k = 0; k < 3; k++)
				{
					reply.Data.WriteUInt16(bct.EmoteDelays[k]);
				}
			}
			else if (query.TableHash == DB2Hash.Item)
			{
				var item = GameData.GetItemTemplate(id);
				if (item != null)
				{
					reply.Status = HotfixStatus.Valid;
					GameData.WriteItemHotfix(item, reply.Data);
				}
				else if (GetSession().WorldClient != null && GetSession().WorldClient.IsConnected())
				{
					if (!GetSession().GameState.RequestedItemHotfixes.Contains(id))
					{
						GetSession().GameState.RequestedItemHotfixes.Add(id);
						var packet2 = new WorldPacket(Opcode.CMSG_ITEM_QUERY_SINGLE);
						packet2.WriteUInt32(id);
						if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
						{
							packet2.WriteGuid(WowGuid64.Empty);
						}
						SendPacketToServer(packet2);
					}
					continue;
				}
			}
			else if (query.TableHash == DB2Hash.ItemSparse)
			{
				var item2 = GameData.GetItemTemplate(id);
				if (item2 != null)
				{
					reply.Status = HotfixStatus.Valid;
					GameData.WriteItemSparseHotfix(item2, reply.Data);
				}
				else if (GetSession().WorldClient != null && GetSession().WorldClient.IsConnected())
				{
					if (!GetSession().GameState.RequestedItemSparseHotfixes.Contains(id))
					{
						GetSession().GameState.RequestedItemSparseHotfixes.Add(id);
						var packet3 = new WorldPacket(Opcode.CMSG_ITEM_QUERY_SINGLE);
						packet3.WriteUInt32(id);
						if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
						{
							packet3.WriteGuid(WowGuid64.Empty);
						}
						SendPacketToServer(packet3);
					}
					continue;
				}
			}
			SendPacket(reply);
		}
	}

	[PacketHandler(Opcode.CMSG_HOTFIX_REQUEST)]
	private void HandleHotfixRequest(HotfixRequest request)
	{
		var connect = new HotfixConnect();
		foreach (var id in request.Hotfixes)
		{
			if (GameData.Hotfixes.TryGetValue(id, out var record))
			{
				Log.Print(LogType.Debug, $"Hotfix record {record.RecordId} from {record.TableHash}.", "HotfixHandler.cs");
				connect.Hotfixes.Add(record);
			}
		}
		SendPacket(connect);
	}

	[PacketHandler(Opcode.CMSG_RESET_INSTANCES)]
	private void HandleResetInstances(EmptyClientPacket reset)
	{
		var packet = new WorldPacket(Opcode.CMSG_RESET_INSTANCES);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_RAID_INFO)]
	private void HandleRequestRaidInfo(EmptyClientPacket reset)
	{
		var packet = new WorldPacket(Opcode.CMSG_REQUEST_RAID_INFO);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BUY_ITEM)]
	private void HandleBuyItem(BuyItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_BUY_ITEM);
		packet.WriteGuid(item.VendorGUID.To64());
		packet.WriteUInt32(item.Item.ItemID);
		var quantity = item.Quantity / GetSession().GameState.GetItemBuyCount(item.Item.ItemID);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
		{
			packet.WriteUInt32(ModernVersion.ExpansionVersion >= 3 ? item.Muid : item.Slot);
			packet.WriteUInt32(quantity);
		}
		else
		{
			packet.WriteUInt8((byte)quantity);
		}
		packet.WriteUInt8((byte)item.BagSlot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SELL_ITEM)]
	private void HandleSellItem(SellItem item)
	{
		var vendorGuid64 = item.VendorGUID.To64();
		var itemGuid64 = item.ItemGUID.To64();
		Log.Print(LogType.Debug, $"[SellItem] Item128={item.ItemGUID} → Item64={itemGuid64} Vendor128={item.VendorGUID} → Vendor64={vendorGuid64}", "");
		var packet = new WorldPacket(Opcode.CMSG_SELL_ITEM);
		packet.WriteGuid(vendorGuid64);
		packet.WriteGuid(itemGuid64);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WriteUInt32(item.Amount);
		}
		else
		{
			packet.WriteUInt8((byte)item.Amount);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SPLIT_ITEM)]
	private void HandleSplitItem(SplitItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_SPLIT_ITEM);
		var containerSlot1 = item.FromPackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.FromPackSlot) : item.FromPackSlot;
		var slot1 = item.FromPackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.FromSlot) : item.FromSlot;
		var containerSlot2 = item.ToPackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ToPackSlot) : item.ToPackSlot;
		var slot2 = item.ToPackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ToSlot) : item.ToSlot;
		packet.WriteUInt8(containerSlot1);
		packet.WriteUInt8(slot1);
		packet.WriteUInt8(containerSlot2);
		packet.WriteUInt8(slot2);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WriteInt32(item.Quantity);
		}
		else
		{
			packet.WriteUInt8((byte)item.Quantity);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SWAP_INV_ITEM)]
	private void HandleSwapInvItem(SwapInvItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_SWAP_INV_ITEM);
		var slot1 = ModernVersion.AdjustInventorySlot(item.Slot1);
		var slot2 = ModernVersion.AdjustInventorySlot(item.Slot2);
		// Modern client: Slot2=source, Slot1=destination (reversed from field names)
		// Legacy server expects: srcSlot first, dstSlot second
		packet.WriteUInt8(slot2);
		packet.WriteUInt8(slot1);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SWAP_ITEM)]
	private void HandleSwapItem(SwapItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_SWAP_ITEM);
		var containerSlotB = item.ContainerSlotB != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ContainerSlotB) : item.ContainerSlotB;
		var slotB = item.ContainerSlotB == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.SlotB) : item.SlotB;
		var containerSlotA = item.ContainerSlotA != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ContainerSlotA) : item.ContainerSlotA;
		var slotA = item.ContainerSlotA == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.SlotA) : item.SlotA;
		packet.WriteUInt8(containerSlotB);
		packet.WriteUInt8(slotB);
		packet.WriteUInt8(containerSlotA);
		packet.WriteUInt8(slotA);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_DESTROY_ITEM)]
	private void HandleDestroyItem(DestroyItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_DESTROY_ITEM);
		var containerSlot = item.ContainerId != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ContainerId) : item.ContainerId;
		var slot = item.ContainerId == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.SlotNum) : item.SlotNum;
		packet.WriteUInt8(containerSlot);
		packet.WriteUInt8(slot);
		packet.WriteUInt32(item.Count);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SPELL_CLICK)]
	private void HandleSpellClick(SpellClick click)
	{
		var packet = new WorldPacket(Opcode.CMSG_SPELL_CLICK);
		packet.WriteGuid(click.SpellClickUnitGuid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUTO_STORE_BAG_ITEM)]
	private void HandleAutoStoreBagItem(AutoStoreBagItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUTO_STORE_BAG_ITEM);
		var srcBag = item.ContainerSlotA != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ContainerSlotA) : item.ContainerSlotA;
		packet.WriteUInt8(srcBag);
		packet.WriteUInt8(item.SlotA);
		var dstBag = item.ContainerSlotB != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ContainerSlotB) : item.ContainerSlotB;
		packet.WriteUInt8(dstBag);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUTO_EQUIP_ITEM)]
	[PacketHandler(Opcode.CMSG_AUTOSTORE_BANK_ITEM)]
	[PacketHandler(Opcode.CMSG_AUTOBANK_ITEM)]
	private void HandleAutoEquipItem(AutoEquipItem item)
	{
		var packet = new WorldPacket(item.GetUniversalOpcode());
		var containerSlot = item.PackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot;
		var slot = item.PackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot;
		packet.WriteUInt8(containerSlot);
		packet.WriteUInt8(slot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_AUTO_EQUIP_ITEM_SLOT)]
	private void HandleAutoEquipItemSlot(AutoEquipItemSlot item)
	{
		var packet = new WorldPacket(Opcode.CMSG_AUTO_EQUIP_ITEM_SLOT);
		packet.WriteGuid(item.Item.To64());
		var slot = ModernVersion.AdjustInventorySlot(item.ItemDstSlot);
		packet.WriteUInt8(slot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_READ_ITEM)]
	private void HandleReadItem(ReadItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_READ_ITEM);
		var containerSlot = item.PackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot;
		var slot = item.PackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot;
		packet.WriteUInt8(containerSlot);
		packet.WriteUInt8(slot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REMOVE_GLYPH)]
	private void HandleRemoveGlyph(RemoveGlyph glyph)
	{
		var packet = new WorldPacket(Opcode.CMSG_REMOVE_GLYPH);
		packet.WriteUInt32(glyph.GlyphSlot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_INSTANCE_LOCK_RESPONSE)]
	private void HandleInstanceLockResponse(InstanceLockResponse lockResponse)
	{
		var packet = new WorldPacket(Opcode.CMSG_INSTANCE_LOCK_RESPONSE);
		packet.WriteUInt8(lockResponse.AcceptLock ? (byte)1 : (byte)0);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_POI_QUERY)]
	private void HandleQuestPOIQuery(QuestPOIQuery query)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_POI_QUERY);
		var count = Math.Min(query.MissingQuestCount, 25);
		packet.WriteUInt32((uint)count);
		for (var i = 0; i < count; i++)
		{
			packet.WriteUInt32((uint)query.MissingQuestPOIs[i]);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BUY_BACK_ITEM)]
	private void HandleBuyBackItem(BuyBackItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_BUY_BACK_ITEM);
		packet.WriteGuid(item.VendorGUID.To64());
		var slot = ModernVersion.AdjustInventorySlot((byte)item.Slot);
		packet.WriteUInt32(slot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REPAIR_ITEM)]
	private void HandleRepairItem(RepairItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_REPAIR_ITEM);
		packet.WriteGuid(item.VendorGUID.To64());
		packet.WriteGuid(item.ItemGUID.To64());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteBool(item.UseGuildBank);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SOCKET_GEMS)]
	private void HandleSocketGems(SocketGems gems)
	{
		var packet = new WorldPacket(Opcode.CMSG_SOCKET_GEMS);
		packet.WriteGuid(gems.ItemGuid.To64());
		for (var i = 0; i < 3; i++)
		{
			packet.WriteGuid(gems.Gems[i].To64());
		}
		SendPacketToServer(packet);
		var success = new SocketGemsSuccess
		{
			ItemGuid = gems.ItemGuid
		};
		SendPacket(success);
	}

	[PacketHandler(Opcode.CMSG_OPEN_ITEM)]
	private void HandleOpenItem(OpenItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_OPEN_ITEM);
		var containerSlot = item.PackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot;
		var slot = item.PackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot;
		packet.WriteUInt8(containerSlot);
		packet.WriteUInt8(slot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_AMMO)]
	private void HandleSetAmmo(SetAmmo ammo)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_AMMO);
		packet.WriteUInt32(ammo.ItemId);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CANCEL_TEMP_ENCHANTMENT)]
	private void HandleCancelTempEnchantment(CancelTempEnchantment cancel)
	{
		if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_CANCEL_TEMP_ENCHANTMENT);
			packet.WriteUInt32(cancel.EnchantmentSlot);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_WRAP_ITEM)]
	private void HandleWrapItem(WrapItem item)
	{
		var packet = new WorldPacket(Opcode.CMSG_WRAP_ITEM);
		var giftBag = item.GiftBag != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.GiftBag) : item.GiftBag;
		var giftSlot = item.GiftBag == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.GiftSlot) : item.GiftSlot;
		var itemBag = item.ItemBag != byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ItemBag) : item.ItemBag;
		var itemSlot = item.ItemBag == byte.MaxValue ? ModernVersion.AdjustInventorySlot(item.ItemSlot) : item.ItemSlot;
		packet.WriteUInt8(giftBag);
		packet.WriteUInt8(giftSlot);
		packet.WriteUInt8(itemBag);
		packet.WriteUInt8(itemSlot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_LOOT_RELEASE)]
	private void HandleLootRelease(LootRelease loot)
	{
		var packet = new WorldPacket(Opcode.CMSG_LOOT_RELEASE);
		packet.WriteGuid(loot.Owner.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_LOOT_ITEM)]
	[PacketHandler(Opcode.CMSG_AUTOSTORE_LOOT_ITEM)]
	private void HandleLootItem(LootItemPkt loot)
	{
		foreach (var item in loot.Loot)
		{
			var packet = new WorldPacket(Opcode.CMSG_AUTOSTORE_LOOT_ITEM);
			packet.WriteUInt8(item.LootListID);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_LOOT_UNIT)]
	private void HandleLootUnit(LootUnit loot)
	{
		var packet = new WorldPacket(Opcode.CMSG_LOOT_UNIT);
		packet.WriteGuid(loot.Unit.To64());
		SendPacketToServer(packet);
		GetSession().GameState.LastLootTargetGuid = loot.Unit.To64();
	}

	[PacketHandler(Opcode.CMSG_LOOT_MONEY)]
	private void HandleLootMoney(LootMoney loot)
	{
		var packet = new WorldPacket(Opcode.CMSG_LOOT_MONEY);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_LOOT_METHOD)]
	private void HandleSetLootMethod(SetLootMethod loot)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_LOOT_METHOD);
		packet.WriteUInt32((uint)loot.LootMethod);
		packet.WriteGuid(loot.LootMasterGUID.To64());
		packet.WriteUInt32(loot.LootThreshold);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_OPT_OUT_OF_LOOT)]
	private void HandleOptOutOfLoot(OptOutOfLoot loot)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_OPT_OUT_OF_LOOT);
			packet.WriteInt32(loot.PassOnLoot ? 1 : 0);
			SendPacketToServer(packet);
		}
		else
		{
			GetSession().GameState.IsPassingOnLoot = loot.PassOnLoot;
		}
	}

	[PacketHandler(Opcode.CMSG_LOOT_ROLL)]
	private void HandleLootRoll(LootRoll loot)
	{
		var packet = new WorldPacket(Opcode.CMSG_LOOT_ROLL);
		packet.WriteGuid(loot.LootObj.To64());
		packet.WriteUInt32(loot.LootListID);
		packet.WriteUInt8((byte)loot.RollType);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_LOOT_MASTER_GIVE)]
	private void HandleLootMasterGive(LootMasterGive loot)
	{
		foreach (var item in loot.Loot)
		{
			var packet = new WorldPacket(Opcode.CMSG_LOOT_MASTER_GIVE);
			packet.WriteGuid(item.LootObj.To64());
			packet.WriteUInt8(item.LootListID);
			packet.WriteGuid(loot.TargetGUID.To64());
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_QUERY_NEXT_MAIL_TIME)]
	private void HandleMailGetList(EmptyClientPacket mail)
	{
		var packet = new WorldPacket(Opcode.MSG_QUERY_NEXT_MAIL_TIME);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_GET_LIST)]
	private void HandleMailGetList(MailGetList mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_GET_LIST);
		packet.WriteGuid(mail.Mailbox.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_CREATE_TEXT_ITEM)]
	private void HandleMailCreateTextItem(MailCreateTextItem mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_CREATE_TEXT_ITEM);
		packet.WriteGuid(mail.Mailbox.To64());
		packet.WriteUInt32(mail.MailID);
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(0u);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_DELETE)]
	private void HandleMailDelete(MailDelete mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_DELETE);
		packet.WriteGuid(GetSession().GameState.CurrentInteractedWithGO.To64());
		packet.WriteUInt32(mail.MailID);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(0u);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_MARK_AS_READ)]
	private void HandleMailMarkAsRead(MailMarkAsRead mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_MARK_AS_READ);
		packet.WriteGuid(mail.Mailbox.To64());
		packet.WriteUInt32(mail.MailID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_RETURN_TO_SENDER)]
	private void HandleMailReturnToSender(MailReturnToSender mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_RETURN_TO_SENDER);
		packet.WriteGuid(GetSession().GameState.CurrentInteractedWithGO.To64());
		packet.WriteUInt32(mail.MailID);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteGuid(mail.SenderGUID.To64());
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_TAKE_ITEM)]
	private void HandleMailTakeItem(MailTakeItem mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_TAKE_ITEM);
		packet.WriteGuid(mail.Mailbox.To64());
		packet.WriteUInt32(mail.MailID);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(mail.AttachID);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MAIL_TAKE_MONEY)]
	private void HandleMailTakeMoney(MailTakeMoney mail)
	{
		var packet = new WorldPacket(Opcode.CMSG_MAIL_TAKE_MONEY);
		packet.WriteGuid(mail.Mailbox.To64());
		packet.WriteUInt32(mail.MailID);
		SendPacketToServer(packet);
	}

	private void BuildSendMail(SendMail mail, List<SendMail.MailAttachment> attachments)
	{
		var packet = new WorldPacket(Opcode.CMSG_SEND_MAIL);
		packet.WriteGuid(mail.Mailbox.To64());
		packet.WriteCString(mail.Target);
		packet.WriteCString(mail.Subject);
		packet.WriteCString(mail.Body);
		packet.WriteInt32(mail.StationeryID);
		packet.WriteUInt32(0u);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt8((byte)attachments.Count);
			foreach (var item in attachments)
			{
				packet.WriteUInt8(item.AttachPosition);
				packet.WriteGuid(item.ItemGUID.To64());
			}
		}
		else if (attachments.Count > 0)
		{
			packet.WriteGuid(attachments[0].ItemGUID.To64());
		}
		else
		{
			packet.WriteGuid(WowGuid64.Empty);
		}
		packet.WriteUInt32((uint)mail.SendMoney);
		packet.WriteUInt32((uint)mail.Cod);
		packet.WriteUInt64(0uL);
		packet.WriteUInt8(0);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SEND_MAIL)]
	private void HandleSendMail(SendMail mail)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) || mail.Attachments.Count <= 1)
		{
			BuildSendMail(mail, mail.Attachments);
			return;
		}
		mail.SendMoney /= mail.Attachments.Count;
		mail.Cod /= mail.Attachments.Count;
		foreach (var item in mail.Attachments)
		{
			var attachments = new List<SendMail.MailAttachment>();
			attachments.Add(item);
			BuildSendMail(mail, attachments);
			Thread.Sleep(500);
		}
	}

	[PacketHandler(Opcode.CMSG_TIME_SYNC_RESPONSE)]
	private void HandleTimeSyncResponse(TimeSyncResponse response)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_TIME_SYNC_RESPONSE);
			packet.WriteUInt32(response.SequenceIndex);
			packet.WriteUInt32(response.ClientTime);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_AREA_TRIGGER)]
	private void HandleAreaTrigger(AreaTriggerPkt at)
	{
		if (at.Entered)
		{
			GetSession().GameState.LastEnteredAreaTrigger = at.AreaTriggerID;
			var packet = new WorldPacket(Opcode.CMSG_AREA_TRIGGER);
			packet.WriteUInt32(at.AreaTriggerID);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_SET_SELECTION)]
	private void HandleSetSelection(SetSelection selection)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_SELECTION);
		packet.WriteGuid(selection.TargetGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REPOP_REQUEST)]
	private void HandleRepopRequest(RepopRequest repop)
	{
		var packet = new WorldPacket(Opcode.CMSG_REPOP_REQUEST);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteBool(repop.CheckInstance);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_CORPSE_LOCATION_FROM_CLIENT)]
	private void HandleQueryCorpseLocationFromClient(QueryCorpseLocationFromClient query)
	{
		var packet = new WorldPacket(Opcode.MSG_CORPSE_QUERY);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_RECLAIM_CORPSE)]
	private void HandleReclaimCorpse(ReclaimCorpse corpse)
	{
		var packet = new WorldPacket(Opcode.CMSG_RECLAIM_CORPSE);
		packet.WriteGuid(corpse.CorpseGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_STAND_STATE_CHANGE)]
	private void HandleStandStateChange(StandStateChange state)
	{
		var packet = new WorldPacket(Opcode.CMSG_STAND_STATE_CHANGE);
		packet.WriteUInt32(state.StandState);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_OPENING_CINEMATIC)]
	[PacketHandler(Opcode.CMSG_NEXT_CINEMATIC_CAMERA)]
	[PacketHandler(Opcode.CMSG_COMPLETE_CINEMATIC)]
	private void HandleCinematicPacket(ClientCinematicPkt cinematic)
	{
		var packet = new WorldPacket(cinematic.GetUniversalOpcode());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_FAR_SIGHT)]
	private void HandleFarSight(FarSight sight)
	{
		var packet = new WorldPacket(Opcode.CMSG_FAR_SIGHT);
		packet.WriteBool(sight.Enable);
		SendPacketToServer(packet);
		GetSession().GameState.IsInFarSight = sight.Enable;
	}

	[PacketHandler(Opcode.CMSG_MOUNT_SPECIAL_ANIM)]
	private void HandleMountSpecialAnim(MountSpecial mount)
	{
		var packet = new WorldPacket(Opcode.CMSG_MOUNT_SPECIAL_ANIM);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_TUTORIAL_FLAG)]
	private void HandleTutorialFlag(TutorialSetFlag tutorial)
	{
		switch (tutorial.Action)
		{
		case TutorialAction.Clear:
		{
			var packet3 = new WorldPacket(Opcode.CMSG_TUTORIAL_CLEAR);
			SendPacketToServer(packet3);
			break;
		}
		case TutorialAction.Reset:
		{
			var packet2 = new WorldPacket(Opcode.CMSG_TUTORIAL_RESET);
			SendPacketToServer(packet2);
			break;
		}
		case TutorialAction.Update:
		{
			var packet = new WorldPacket(Opcode.CMSG_TUTORIAL_FLAG);
			packet.WriteUInt32(tutorial.TutorialBit);
			SendPacketToServer(packet);
			break;
		}
		}
	}

	[PacketHandler(Opcode.CMSG_REQUEST_LFG_LIST_BLACKLIST)]
	private void HandleRequestLFGListBlacklist(EmptyClientPacket request)
	{
		var blacklist = new LFGListUpdateBlacklist();
		if (ModernVersion.ExpansionVersion > 1)
		{
			blacklist.AddBlacklist(796, 3);
			blacklist.AddBlacklist(797, 3);
			blacklist.AddBlacklist(798, 3);
			blacklist.AddBlacklist(799, 3);
			blacklist.AddBlacklist(800, 3);
			blacklist.AddBlacklist(801, 3);
			blacklist.AddBlacklist(802, 3);
			blacklist.AddBlacklist(803, 3);
			blacklist.AddBlacklist(804, 3);
			blacklist.AddBlacklist(805, 3);
			blacklist.AddBlacklist(806, 3);
			blacklist.AddBlacklist(807, 3);
			blacklist.AddBlacklist(808, 3);
			blacklist.AddBlacklist(809, 3);
			blacklist.AddBlacklist(810, 3);
			blacklist.AddBlacklist(811, 3);
			blacklist.AddBlacklist(812, 3);
			blacklist.AddBlacklist(813, 3);
			blacklist.AddBlacklist(814, 3);
			blacklist.AddBlacklist(815, 3);
			blacklist.AddBlacklist(816, 3);
			blacklist.AddBlacklist(817, 3);
			blacklist.AddBlacklist(818, 3);
			blacklist.AddBlacklist(820, 3);
			blacklist.AddBlacklist(827, 3);
			blacklist.AddBlacklist(828, 3);
			blacklist.AddBlacklist(829, 3);
			blacklist.AddBlacklist(835, 1031);
			blacklist.AddBlacklist(837, 3);
			blacklist.AddBlacklist(849, 1031);
			blacklist.AddBlacklist(850, 1031);
			blacklist.AddBlacklist(851, 1031);
			blacklist.AddBlacklist(852, 1031);
			blacklist.AddBlacklist(853, 3);
			blacklist.AddBlacklist(854, 3);
			blacklist.AddBlacklist(855, 3);
			blacklist.AddBlacklist(856, 3);
			blacklist.AddBlacklist(857, 3);
			blacklist.AddBlacklist(858, 3);
			blacklist.AddBlacklist(859, 3);
			blacklist.AddBlacklist(860, 3);
			blacklist.AddBlacklist(861, 3);
			blacklist.AddBlacklist(862, 3);
			blacklist.AddBlacklist(863, 3);
			blacklist.AddBlacklist(864, 3);
			blacklist.AddBlacklist(865, 3);
			blacklist.AddBlacklist(866, 3);
			blacklist.AddBlacklist(867, 3);
			blacklist.AddBlacklist(868, 3);
			blacklist.AddBlacklist(869, 3);
			blacklist.AddBlacklist(870, 3);
			blacklist.AddBlacklist(871, 3);
			blacklist.AddBlacklist(872, 3);
			blacklist.AddBlacklist(873, 3);
			blacklist.AddBlacklist(874, 3);
			blacklist.AddBlacklist(875, 3);
			blacklist.AddBlacklist(876, 3);
			blacklist.AddBlacklist(877, 3);
			blacklist.AddBlacklist(878, 3);
			blacklist.AddBlacklist(879, 3);
			blacklist.AddBlacklist(880, 3);
			blacklist.AddBlacklist(881, 3);
			blacklist.AddBlacklist(882, 3);
			blacklist.AddBlacklist(883, 3);
			blacklist.AddBlacklist(884, 3);
			blacklist.AddBlacklist(885, 3);
			blacklist.AddBlacklist(886, 3);
			blacklist.AddBlacklist(887, 3);
			blacklist.AddBlacklist(888, 3);
			blacklist.AddBlacklist(889, 3);
			blacklist.AddBlacklist(890, 3);
			blacklist.AddBlacklist(891, 3);
			blacklist.AddBlacklist(892, 3);
			blacklist.AddBlacklist(893, 3);
			blacklist.AddBlacklist(898, 3);
			blacklist.AddBlacklist(899, 3);
			blacklist.AddBlacklist(900, 3);
			blacklist.AddBlacklist(901, 3);
			blacklist.AddBlacklist(902, 1031);
			blacklist.AddBlacklist(917, 1031);
			blacklist.AddBlacklist(919, 3);
			blacklist.AddBlacklist(920, 3);
			blacklist.AddBlacklist(921, 3);
			blacklist.AddBlacklist(922, 3);
			blacklist.AddBlacklist(923, 3);
			blacklist.AddBlacklist(924, 3);
			blacklist.AddBlacklist(926, 3);
			blacklist.AddBlacklist(927, 3);
			blacklist.AddBlacklist(928, 3);
			blacklist.AddBlacklist(929, 3);
			blacklist.AddBlacklist(930, 3);
			blacklist.AddBlacklist(932, 3);
			blacklist.AddBlacklist(934, 3);
		}
		SendPacket(blacklist);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_CONQUEST_FORMULA_CONSTANTS)]
	private void HandleRequestConquestFormulaConstants(EmptyClientPacket request)
	{
		var response = new ConquestFormulaConstants
		{
			PvpMinCPPerWeek = 1500,
			PvpMaxCPPerWeek = 3000,
			PvpCPBaseCoefficient = 1511.26f,
			PvpCPExpCoefficient = 1639.28f,
			PvpCPNumerator = 0.00412f
		};
		SendPacket(response);
	}

	[PacketHandler(Opcode.CMSG_OBJECT_UPDATE_FAILED)]
	private void HandleObjectUpdateFailed(ObjectUpdateFailed fail)
	{
		Log.Print(LogType.Error, $"Object update failed for {fail.ObjectGuid}.", "MiscHandler.cs");
	}

	[PacketHandler(Opcode.CMSG_SET_DUNGEON_DIFFICULTY)]
	private void HandleSetDungeonDifficulty(SetDungeonDifficulty difficulty)
	{
		var packet = new WorldPacket(Opcode.MSG_SET_DUNGEON_DIFFICULTY);
		uint dificultyId = (byte)Enum.Parse(typeof(DifficultyLegacy), ((DifficultyModern)difficulty.DifficultyID/*cast due to .constrained prefix*/).ToString());
		packet.WriteUInt32(dificultyId);
		SendPacketToServer(packet);
		var difficultySet = new DungeonDifficultySet
		{
			DifficultyID = (int)difficulty.DifficultyID
		};
		SendPacket(difficultySet);
	}

	[PacketHandler(Opcode.CMSG_VIOLENCE_LEVEL)]
	private void HandleViolenceLevel(ViolenceLevelPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_REQUEST_PVP_REWARDS)]
	private void HandleRequestPvpRewards(RequestPvpRewardsPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_GET_ITEM_PURCHASE_DATA)]
	private void HandleGetItemPurchaseData(EmptyClientPacket packet)
	{
		var legacyPacket = new WorldPacket(Opcode.CMSG_GET_ITEM_PURCHASE_DATA);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_ITEM_PURCHASE_REFUND)]
	private void HandleItemPurchaseRefund(EmptyClientPacket packet)
	{
		var legacyPacket = new WorldPacket(Opcode.CMSG_ITEM_PURCHASE_REFUND);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_RATED_PVP_INFO)]
	private void HandleRequestRatedPvpInfo(RequestRatedPvpInfoPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_OVERRIDE_SCREEN_FLASH)]
	private void HandleOverrideScreenFlash(OverrideScreenFlashPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_QUEUED_MESSAGES_END)]
	private void HandleQueuedMessagesEnd(QueuedMessagesEndPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_BATTLE_PAY_GET_PRODUCT_LIST)]
	private void HandleBattlePayGetProductList(BattlePayGetProductListPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_BATTLE_PAY_GET_PURCHASE_LIST)]
	private void HandleBattlePayGetPurchaseList(BattlePayGetPurchaseListPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_GET_UNDELETE_CHARACTER_COOLDOWN_STATUS)]
	private void HandleGetUndeleteCharacterCooldownStatus(GetUndeleteCharCooldownPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_UPDATE_VAS_PURCHASE_STATES)]
	private void HandleUpdateVasPurchaseStates(UpdateVasPurchaseStatesPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_DF_GET_SYSTEM_INFO)]
	private void HandleDfGetSystemInfo(DfGetSystemInfoPkt packet)
	{
		if (packet.Player)
		{
			var legacyPacket = new WorldPacket(Opcode.CMSG_LFG_PLAYER_LOCK_INFO_REQUEST);
			SendPacketToServer(legacyPacket);
		}
		else
		{
			var legacyPacket = new WorldPacket(Opcode.CMSG_LFG_PARTY_LOCK_INFO_REQUEST);
			SendPacketToServer(legacyPacket);
		}
	}

	[PacketHandler(Opcode.CMSG_DF_JOIN)]
	private void HandleDfJoin(DfJoinPkt packet)
	{
		// Legacy 3.3.5a format: uint32 Roles, bool NoPartialClear, bool Achievements, uint8 slotCount, uint32[] Slots, uint8 needsCount(3), uint8[3] Needs, string Comment
		var legacyPacket = new WorldPacket(Opcode.CMSG_LFG_JOIN);
		legacyPacket.WriteUInt32(packet.Roles);
		legacyPacket.WriteUInt8(0); // NoPartialClear
		legacyPacket.WriteUInt8(0); // Achievements
		legacyPacket.WriteUInt8((byte)packet.Slots.Length);
		for (var i = 0; i < packet.Slots.Length; i++)
			legacyPacket.WriteUInt32(packet.Slots[i]);
		legacyPacket.WriteUInt8(3); // Needs count
		legacyPacket.WriteUInt8(0); // Need 1
		legacyPacket.WriteUInt8(0); // Need 2
		legacyPacket.WriteUInt8(0); // Need 3
		legacyPacket.WriteCString(""); // Comment
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_DF_LEAVE)]
	private void HandleDfLeave(DfLeavePkt packet)
	{
		var legacyPacket = new WorldPacket(Opcode.CMSG_LFG_LEAVE);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_DF_GET_JOIN_STATUS)]
	private void HandleDfGetJoinStatus(DfGetJoinStatusPkt packet)
	{
		var legacyPacket = new WorldPacket(Opcode.CMSG_DF_GET_JOIN_STATUS);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_CALENDAR_GET_NUM_PENDING)]
	private void HandleCalendarGetNumPending(CalendarGetNumPendingPkt packet)
	{
		var legacyPacket = new WorldPacket(Opcode.CMSG_CALENDAR_GET_NUM_PENDING);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_GUILD_SET_ACHIEVEMENT_TRACKING)]
	private void HandleGuildSetAchievementTracking(GuildSetAchievementTrackingPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_QUERY_COUNTDOWN_TIMER)]
	private void HandleQueryCountdownTimer(QueryCountdownTimerPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_REQUEST_FORCED_REACTIONS)]
	private void HandleRequestForcedReactions(RequestForcedReactionsPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_LFG_LIST_GET_STATUS)]
	private void HandleLfgListGetStatus(LfgListGetStatusPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_BATTLE_PET_REQUEST_JOURNAL)]
	private void HandleBattlePetRequestJournal(BattlePetRequestJournalPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_REQUEST_CEMETERY_LIST)]
	private void HandleRequestCemeteryList(RequestCemeteryListPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_CLOSE_INTERACTION)]
	private void HandleCloseInteraction(CloseInteractionPkt packet)
	{
	}

	[PacketHandler(Opcode.CMSG_REPORT_CLIENT_VARIABLES)]
	private void HandleReportClientVariables(GenericNoOpPkt pkt)
	{
	}

	[PacketHandler(Opcode.CMSG_REPORT_ENABLED_ADDONS)]
	private void HandleReportEnabledAddons(GenericNoOpPkt pkt)
	{
	}

	[PacketHandler(Opcode.CMSG_REPORT_KEYBINDING_EXECUTION_COUNTS)]
	private void HandleReportKeybindingCounts(GenericNoOpPkt pkt)
	{
	}

	[PacketHandler(Opcode.CMSG_DISCARDED_TIME_SYNC_ACKS)]
	private void HandleDiscardedTimeSyncAcks(GenericNoOpPkt pkt)
	{
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_CLOSE_QUEST)]
	private void HandleQuestGiverCloseQuest(GenericNoOpPkt pkt)
	{
	}

	[PacketHandler(Opcode.CMSG_MOVE_CHANGE_TRANSPORT)]
	[PacketHandler(Opcode.CMSG_MOVE_DISMISS_VEHICLE)]
	[PacketHandler(Opcode.CMSG_MOVE_FALL_LAND)]
	[PacketHandler(Opcode.CMSG_MOVE_FALL_RESET)]
	[PacketHandler(Opcode.CMSG_MOVE_HEARTBEAT)]
	[PacketHandler(Opcode.CMSG_MOVE_JUMP)]
	[PacketHandler(Opcode.CMSG_MOVE_REMOVE_MOVEMENT_FORCES)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_FACING)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_FACING_HEARTBEAT)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_FLY)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_PITCH)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_RUN_MODE)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_WALK_MODE)]
	[PacketHandler(Opcode.CMSG_MOVE_START_ASCEND)]
	[PacketHandler(Opcode.CMSG_MOVE_START_BACKWARD)]
	[PacketHandler(Opcode.CMSG_MOVE_START_DESCEND)]
	[PacketHandler(Opcode.CMSG_MOVE_START_FORWARD)]
	[PacketHandler(Opcode.CMSG_MOVE_START_PITCH_DOWN)]
	[PacketHandler(Opcode.CMSG_MOVE_START_PITCH_UP)]
	[PacketHandler(Opcode.CMSG_MOVE_START_SWIM)]
	[PacketHandler(Opcode.CMSG_MOVE_START_TURN_LEFT)]
	[PacketHandler(Opcode.CMSG_MOVE_START_TURN_RIGHT)]
	[PacketHandler(Opcode.CMSG_MOVE_START_STRAFE_LEFT)]
	[PacketHandler(Opcode.CMSG_MOVE_START_STRAFE_RIGHT)]
	[PacketHandler(Opcode.CMSG_MOVE_STOP)]
	[PacketHandler(Opcode.CMSG_MOVE_STOP_ASCEND)]
	[PacketHandler(Opcode.CMSG_MOVE_STOP_PITCH)]
	[PacketHandler(Opcode.CMSG_MOVE_STOP_STRAFE)]
	[PacketHandler(Opcode.CMSG_MOVE_STOP_SWIM)]
	[PacketHandler(Opcode.CMSG_MOVE_STOP_TURN)]
	[PacketHandler(Opcode.CMSG_MOVE_DOUBLE_JUMP)]
	private void HandlePlayerMove(ClientPlayerMovement movement)
	{
		var opcodeName = movement.GetUniversalOpcode().ToString();
		opcodeName = opcodeName.Replace("CMSG", "MSG");
		var opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
		if (opcode == 0)
		{
			opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
		}
		var packet = new WorldPacket(opcode);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WritePackedGuid(movement.Guid.To64());
		}
		movement.MoveInfo.WriteMovementInfoLegacy(packet);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_TELEPORT_ACK)]
	private void HandleMoveTeleportAck(MoveTeleportAck teleport)
	{
		var packet = new WorldPacket(Opcode.MSG_MOVE_TELEPORT_ACK);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WritePackedGuid(teleport.MoverGUID.To64());
		}
		else
		{
			packet.WriteGuid(teleport.MoverGUID.To64());
		}
		packet.WriteUInt32(teleport.MoveCounter);
		packet.WriteUInt32(teleport.MoveTime);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_WORLD_PORT_RESPONSE)]
	private void HandleWorldPortResponse(WorldPortResponse teleport)
	{
		GetSession().GameState.IsWaitingForWorldPortAck = false;
		var packet = new WorldPacket(Opcode.MSG_MOVE_WORLDPORT_ACK);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_FORCE_FLIGHT_BACK_SPEED_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_FLIGHT_SPEED_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_PITCH_RATE_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_RUN_BACK_SPEED_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_RUN_SPEED_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_SWIM_BACK_SPEED_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_SWIM_SPEED_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_TURN_RATE_CHANGE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_WALK_SPEED_CHANGE_ACK)]
	private void HandleMoveForceSpeedChangeAck(MovementSpeedAck speed)
	{
		var opcode = speed.GetUniversalOpcode();
		var flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
		var flag2 = flag;
		if (flag2)
		{
			var flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
			flag2 = flag3;
		}
		if (!flag2)
		{
			var packet = new WorldPacket(opcode);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
			{
				packet.WritePackedGuid(speed.MoverGUID.To64());
			}
			else
			{
				packet.WriteGuid(speed.MoverGUID.To64());
			}
			packet.WriteUInt32(speed.Ack.MoveCounter);
			speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
			packet.WriteFloat(speed.Speed);
			SendPacketToServer(packet);
		}
	}

	private MovementFlagModern GetFlagForAckOpcode(Opcode opcode)
	{
		return opcode switch
		{
			Opcode.CMSG_MOVE_FEATHER_FALL_ACK => MovementFlagModern.CanSafeFall, 
			Opcode.CMSG_MOVE_HOVER_ACK => MovementFlagModern.Hover, 
			Opcode.CMSG_MOVE_SET_CAN_FLY_ACK => MovementFlagModern.CanFly, 
			Opcode.CMSG_MOVE_WATER_WALK_ACK => MovementFlagModern.Waterwalking, 
			_ => MovementFlagModern.None, 
		};
	}

	[PacketHandler(Opcode.CMSG_MOVE_FEATHER_FALL_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_HOVER_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_SET_CAN_FLY_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_WATER_WALK_ACK)]
	private void HandleMoveForceAck1(MovementAckMessage movementAck)
	{
		var packet = new WorldPacket(movementAck.GetUniversalOpcode());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WritePackedGuid(movementAck.MoverGUID.To64());
		}
		else
		{
			packet.WriteGuid(movementAck.MoverGUID.To64());
		}
		packet.WriteUInt32(movementAck.Ack.MoveCounter);
		movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
		packet.WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(GetFlagForAckOpcode(movementAck.GetUniversalOpcode())) ? 1 : 0);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_FORCE_ROOT_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_FORCE_UNROOT_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_KNOCK_BACK_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_GRAVITY_DISABLE_ACK)]
	[PacketHandler(Opcode.CMSG_MOVE_GRAVITY_ENABLE_ACK)]
	private void HandleMoveForceAck2(MovementAckMessage movementAck)
	{
		var packet = new WorldPacket(movementAck.GetUniversalOpcode());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WritePackedGuid(movementAck.MoverGUID.To64());
		}
		else
		{
			packet.WriteGuid(movementAck.MoverGUID.To64());
		}
		packet.WriteUInt32(movementAck.Ack.MoveCounter);
		movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_ACTIVE_MOVER)]
	private void HandleMoveSetActiveMover(SetActiveMover move)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_ACTIVE_MOVER);
		packet.WriteGuid(move.MoverGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_INIT_ACTIVE_MOVER_COMPLETE)]
	private void HandleMoveInitActiveMoverComplete(InitActiveMoverComplete move)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_ACTIVE_MOVER);
		packet.WriteGuid(GetSession().GameState.CurrentPlayerGuid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_SPLINE_DONE)]
	private void HandleMoveSplineDone(MoveSplineDone movement)
	{
		var packet = new WorldPacket(Opcode.CMSG_MOVE_SPLINE_DONE);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WritePackedGuid(movement.Guid.To64());
		}
		movement.MoveInfo.WriteMovementInfoLegacy(packet);
		packet.WriteInt32(movement.SplineID);
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteFloat(0f);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_MOVE_TIME_SKIPPED)]
	private void HandleMoveSplineDone(MoveTimeSkipped movement)
	{
		var packet = new WorldPacket(Opcode.CMSG_MOVE_TIME_SKIPPED);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
		{
			packet.WritePackedGuid(movement.MoverGUID.To64());
		}
		else
		{
			packet.WriteGuid(movement.MoverGUID.To64());
		}
		packet.WriteUInt32(movement.TimeSkipped);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_VEHICLE_EXIT)]
	[PacketHandler(Opcode.CMSG_REQUEST_VEHICLE_PREV_SEAT)]
	[PacketHandler(Opcode.CMSG_REQUEST_VEHICLE_NEXT_SEAT)]
	private void HandleRequestVehicleAction(EmptyClientPacket packet)
	{
		var legacyPacket = new WorldPacket(packet.GetUniversalOpcode());
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_VEHICLE_SWITCH_SEAT)]
	private void HandleRequestVehicleSwitchSeat(RequestVehicleSwitchSeat switchSeat)
	{
		var packet = new WorldPacket(Opcode.CMSG_REQUEST_VEHICLE_SWITCH_SEAT);
		packet.WritePackedGuid(switchSeat.Vehicle.To64());
		packet.WriteUInt8(switchSeat.SeatIndex);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GM_TICKET_GET_CASE_STATUS)]
	private void HandleGMTicketGetCaseStatus(EmptyClientPacket packet)
	{
		var response = new GMTicketCaseStatus();
		SendPacket(response);
	}

	[PacketHandler(Opcode.CMSG_CANCEL_GROWTH_AURA)]
	[PacketHandler(Opcode.CMSG_HEARTH_AND_RESURRECT)]
	[PacketHandler(Opcode.CMSG_STABLE_REVIVE_PET)]
	[PacketHandler(Opcode.CMSG_QUERY_QUESTS_COMPLETED)]
	[PacketHandler(Opcode.CMSG_GM_TICKET_DELETE_TICKET)]
	[PacketHandler(Opcode.CMSG_GM_TICKET_GET_TICKET)]
	[PacketHandler(Opcode.CMSG_GM_TICKET_GET_SYSTEM_STATUS)]
	private void HandleSimpleEmptyPacket(EmptyClientPacket packet)
	{
		var legacyPacket = new WorldPacket(packet.GetUniversalOpcode());
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_ZONEUPDATE)]
	private void HandleZoneUpdate(ZoneUpdatePkt packet)
	{
		var zoneId = packet.ZoneId;
		var legacyPacket = new WorldPacket(Opcode.CMSG_ZONEUPDATE);
		legacyPacket.WriteUInt32(zoneId);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_GM_TICKET_UPDATE_TEXT)]
	private void HandleGMTicketUpdateText(GMTicketUpdateTextPkt packet)
	{
		var message = packet.Message;
		var legacyPacket = new WorldPacket(Opcode.CMSG_GM_TICKET_UPDATE_TEXT);
		legacyPacket.WriteCString(message);
		SendPacketToServer(legacyPacket);
	}

	[PacketHandler(Opcode.CMSG_BANKER_ACTIVATE)]
	[PacketHandler(Opcode.CMSG_BINDER_ACTIVATE)]
	[PacketHandler(Opcode.CMSG_LIST_INVENTORY)]
	[PacketHandler(Opcode.CMSG_SPIRIT_HEALER_ACTIVATE)]
	[PacketHandler(Opcode.CMSG_TALK_TO_GOSSIP)]
	[PacketHandler(Opcode.CMSG_TRAINER_LIST)]
	[PacketHandler(Opcode.CMSG_BATTLEMASTER_HELLO)]
	[PacketHandler(Opcode.CMSG_AREA_SPIRIT_HEALER_QUERY)]
	[PacketHandler(Opcode.CMSG_AREA_SPIRIT_HEALER_QUEUE)]
	private void HandleInteractWithNPC(InteractWithNPC interact)
	{
		var packet = new WorldPacket(interact.GetUniversalOpcode());
		packet.WriteGuid(interact.CreatureGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_GOSSIP_SELECT_OPTION)]
	private void HandleGossipSelectOption(GossipSelectOption gossip)
	{
		var packet = new WorldPacket(Opcode.CMSG_GOSSIP_SELECT_OPTION);
		packet.WriteGuid(gossip.GossipUnit.To64());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(gossip.GossipID);
		}
		packet.WriteUInt32(gossip.GossipIndex);
		if (!string.IsNullOrEmpty(gossip.PromotionCode))
		{
			packet.WriteCString(gossip.PromotionCode);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BUY_BANK_SLOT)]
	private void HandleBuyBankSlot(BuyBankSlot bank)
	{
		var packet = new WorldPacket(Opcode.CMSG_BUY_BANK_SLOT);
		packet.WriteGuid(bank.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_TRAINER_BUY_SPELL)]
	private void HandleTrainerBuySpell(TrainerBuySpell buy)
	{
		var packet = new WorldPacket(Opcode.CMSG_TRAINER_BUY_SPELL);
		packet.WriteGuid(buy.TrainerGUID.To64());
		if (ModernVersion.ExpansionVersion > 1 && LegacyVersion.ExpansionVersion <= 1)
		{
			buy.SpellID = GetSession().GameState.GetLearnSpellFromRealSpell(buy.SpellID);
		}
		packet.WriteUInt32(buy.SpellID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CONFIRM_RESPEC_WIPE)]
	private void HandleConfirmRespecWipe(ConfirmRespecWipe respec)
	{
		switch (respec.RespecType)
		{
		case SpecResetType.Talents:
		{
			var packet2 = new WorldPacket(Opcode.MSG_TALENT_WIPE_CONFIRM);
			packet2.WriteGuid(respec.TrainerGUID.To64());
			SendPacketToServer(packet2);
			break;
		}
		case SpecResetType.PetTalents:
		{
			var packet = new WorldPacket(Opcode.CMSG_PET_UNLEARN);
			packet.WriteGuid(respec.TrainerGUID.To64());
			SendPacketToServer(packet);
			break;
		}
		default:
			Log.Print(LogType.Error, $"Unhandled respec type {respec.RespecType}.", "NPCHandler.cs");
			break;
		}
	}

	[PacketHandler(Opcode.CMSG_PET_ACTION)]
	private void HandlePetAction(PetAction act)
	{
		var packet = new WorldPacket(Opcode.CMSG_PET_ACTION);
		packet.WriteGuid(act.PetGUID.To64());
		packet.WriteUInt32(act.Action);
		packet.WriteGuid(act.TargetGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PET_STOP_ATTACK)]
	private void HandlePetStopAttack(PetStopAttack stop)
	{
		var packet = new WorldPacket(Opcode.CMSG_PET_STOP_ATTACK);
		packet.WriteGuid(stop.PetGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PET_SET_ACTION)]
	private void HandlePetStopAttack(PetSetAction action)
	{
		var packet = new WorldPacket(Opcode.CMSG_PET_SET_ACTION);
		packet.WriteGuid(action.PetGUID.To64());
		packet.WriteUInt32(action.Index);
		packet.WriteUInt32(action.Action);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PET_RENAME)]
	private void HandlePetRename(PetRename pet)
	{
		var packet = new WorldPacket(Opcode.CMSG_PET_RENAME);
		packet.WriteGuid(pet.RenameData.PetGUID.To64());
		packet.WriteCString(pet.RenameData.NewName);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteBool(pet.RenameData.HasDeclinedNames);
			if (pet.RenameData.HasDeclinedNames)
			{
				for (var i = 0; i < 5; i++)
				{
					packet.WriteCString(pet.RenameData.DeclinedNames.name[i]);
				}
			}
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_STABLED_PETS)]
	private void HandleRequestStabledPets(RequestStabledPets stable)
	{
		var packet = new WorldPacket(Opcode.MSG_LIST_STABLED_PETS);
		packet.WriteGuid(stable.StableMaster.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BUY_STABLE_SLOT)]
	private void HandleBuyStableSlot(BuyStableSlot stable)
	{
		var packet = new WorldPacket(Opcode.CMSG_BUY_STABLE_SLOT);
		packet.WriteGuid(stable.StableMaster.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PET_ABANDON)]
	private void HandlePetAbandon(PetAbandon pet)
	{
		var packet = new WorldPacket(Opcode.CMSG_PET_ABANDON);
		packet.WriteGuid(pet.PetGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_STABLE_PET)]
	private void HandleStablePet(StablePet pet)
	{
		var packet = new WorldPacket(Opcode.CMSG_STABLE_PET);
		packet.WriteGuid(pet.StableMaster.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_UNSTABLE_PET)]
	private void HandleUnstablePet(UnstablePet pet)
	{
		var packet = new WorldPacket(Opcode.CMSG_UNSTABLE_PET);
		packet.WriteGuid(pet.StableMaster.To64());
		packet.WriteUInt32(pet.PetNumber);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_STABLE_SWAP_PET)]
	private void HandleStableSwapPet(StableSwapPet pet)
	{
		var packet = new WorldPacket(Opcode.CMSG_STABLE_SWAP_PET);
		packet.WriteGuid(pet.StableMaster.To64());
		packet.WriteUInt32(pet.PetNumber);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PET_CANCEL_AURA)]
	private void HandlePetCancelAura(PetCancelAura cancel)
	{
		var packet = new WorldPacket(Opcode.CMSG_PET_CANCEL_AURA);
		packet.WriteGuid(cancel.PetGUID.To64());
		packet.WriteUInt32(cancel.SpellID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_REQUEST_PET_INFO)]
	private void HandleRequestPetInfo(PetInfoRequest r)
	{
		var packet = new WorldPacket(Opcode.CMSG_REQUEST_PET_INFO);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PETITION_BUY)]
	private void HandlePetitionBuy(PetitionBuy petition)
	{
		var packet = new WorldPacket(Opcode.CMSG_PETITION_BUY);
		packet.WriteGuid(petition.Unit.To64());
		packet.WriteUInt32(0u);
		packet.WriteUInt64(0uL);
		packet.WriteCString(petition.Title);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteCString("");
		}
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt16(0);
		}
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		packet.WriteUInt32(0u);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			for (var i = 0; i < 10; i++)
			{
				packet.WriteCString("");
			}
		}
		else
		{
			packet.WriteUInt16(0);
			packet.WriteUInt8(0);
		}
		packet.WriteUInt32(petition.Index);
		packet.WriteUInt32(0u);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PETITION_SHOW_SIGNATURES)]
	private void HandlePetitionShowSignatures(PetitionShowSignatures petition)
	{
		var packet = new WorldPacket(Opcode.CMSG_PETITION_SHOW_SIGNATURES);
		packet.WriteGuid(petition.Item.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_PETITION)]
	private void HandleQueryPetition(QueryPetition petition)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_PETITION);
		packet.WriteUInt32(petition.PetitionID);
		packet.WriteGuid(petition.ItemGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PETITION_RENAME_GUILD)]
	private void HandlePetitionRenameGuild(PetitionRenameGuild petition)
	{
		var packet = new WorldPacket(Opcode.MSG_PETITION_RENAME);
		packet.WriteGuid(petition.PetitionGuid.To64());
		packet.WriteCString(petition.NewGuildName);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_OFFER_PETITION)]
	private void HandleOfferPetition(OfferPetition petition)
	{
		var packet = new WorldPacket(Opcode.CMSG_OFFER_PETITION);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(petition.UnkInt);
		}
		packet.WriteGuid(petition.ItemGUID.To64());
		packet.WriteGuid(petition.TargetPlayer.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_DECLINE_PETITION)]
	private void HandleDeclinePetition(DeclinePetition petition)
	{
		var packet = new WorldPacket(Opcode.MSG_PETITION_DECLINE);
		packet.WriteGuid(petition.PetitionGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SIGN_PETITION)]
	private void HandleSignPetition(SignPetition petition)
	{
		var packet = new WorldPacket(Opcode.CMSG_SIGN_PETITION);
		packet.WriteGuid(petition.PetitionGUID.To64());
		packet.WriteUInt8(petition.Choice);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_TURN_IN_PETITION)]
	private void HandleTurnInPetition(TurnInPetition petition)
	{
		var packet = new WorldPacket(Opcode.CMSG_TURN_IN_PETITION);
		packet.WriteGuid(petition.Item.To64());
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(petition.BackgroundColor);
			packet.WriteUInt32(petition.EmblemStyle);
			packet.WriteUInt32(petition.EmblemColor);
			packet.WriteUInt32(petition.BorderStyle);
			packet.WriteUInt32(petition.BorderColor);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_TIME)]
	private void HandleQueryTime(EmptyClientPacket queryTime)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_TIME);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_QUEST_INFO)]
	private void HandleQueryQuestInfo(QueryQuestInfo queryQuest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_QUEST_INFO);
		packet.WriteUInt32(queryQuest.QuestID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_CREATURE)]
	private void HandleQueryCreature(QueryCreature queryCreature)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_CREATURE);
		packet.WriteUInt32(queryCreature.CreatureID);
		packet.WriteGuid(queryCreature.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_GAME_OBJECT)]
	private void HandleQueryGameObject(QueryGameObject queryGo)
	{
		// Respond from cache immediately if available (avoids round-trip for transports)
		if (GetSession().GameState.GameObjectQueryCache.TryGetValue(queryGo.GameObjectID, out var cached))
		{
			var response = new QueryGameObjectResponse
			{
				GameObjectID = cached.GameObjectID,
				Guid = WowGuid128.Empty,
				Allow = cached.Allow,
				Stats = cached.Stats
			};
			SendPacket(response);
			return;
		}
		var packet = new WorldPacket(Opcode.CMSG_QUERY_GAME_OBJECT);
		packet.WriteUInt32(queryGo.GameObjectID);
		packet.WriteGuid(queryGo.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_PAGE_TEXT)]
	private void HandleQueryPageText(QueryPageText queryText)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_PAGE_TEXT);
		packet.WriteUInt32(queryText.PageTextID);
		packet.WriteGuid(queryText.ItemGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_NPC_TEXT)]
	private void HandleQueryNpcText(QueryNPCText queryText)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_NPC_TEXT);
		packet.WriteUInt32(queryText.TextID);
		packet.WriteGuid(queryText.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUERY_PET_NAME)]
	private void HandleQueryPetName(QueryPetName queryName)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUERY_PET_NAME);
		packet.WriteUInt32(queryName.UnitGUID.GetEntry());
		packet.WriteGuid(queryName.UnitGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_WHO)]
	private void HandleWhoRequest(WhoRequestPkt who)
	{
		var packet = new WorldPacket(Opcode.CMSG_WHO);
		packet.WriteInt32(who.Request.MinLevel);
		packet.WriteInt32(who.Request.MaxLevel);
		packet.WriteCString(who.Request.Name);
		packet.WriteCString(who.Request.Guild);
		packet.WriteInt32((int)who.Request.RaceFilter);
		packet.WriteInt32(who.Request.ClassFilter);
		packet.WriteInt32(who.Areas.Count);
		foreach (var area in who.Areas)
		{
			packet.WriteInt32(area);
		}
		packet.WriteInt32(who.Request.Words.Count);
		foreach (var word in who.Request.Words)
		{
			packet.WriteCString(word);
		}
		SendPacketToServer(packet);
		GetSession().GameState.LastWhoRequestId = who.RequestID;
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_QUERY_QUEST)]
	private void HandleQuestGiverQueryQuest(QuestGiverQueryQuest quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_QUERY_QUEST);
		packet.WriteGuid(quest.QuestGiverGUID.To64());
		packet.WriteUInt32(quest.QuestID);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteBool(quest.RespondToGiver);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_ACCEPT_QUEST)]
	private void HandleQuestGiverAcceptQuest(QuestGiverAcceptQuest quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_ACCEPT_QUEST);
		packet.WriteGuid(quest.QuestGiverGUID.To64());
		packet.WriteUInt32(quest.QuestID);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901))
		{
			packet.WriteInt32(quest.StartCheat ? 1 : 0);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_LOG_REMOVE_QUEST)]
	private void HandleQuestLogRemoveQuest(QuestLogRemoveQuest quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_LOG_REMOVE_QUEST);
		packet.WriteUInt8(quest.Slot);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_STATUS_QUERY)]
	private void HandleQuestGiverStatusQuery(QuestGiverStatusQuery query)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_STATUS_QUERY);
		packet.WriteGuid(query.QuestGiverGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_STATUS_MULTIPLE_QUERY)]
	private void HandleQuestGiverStatusMultipleQuery(QuestGiverStatusMultipleQuery query)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_STATUS_MULTIPLE_QUERY);
			SendPacketToServer(packet);
			return;
		}
		var UNIT_NPC_FLAGS = ModernVersion.GetUpdateField(UnitField.UNIT_NPC_FLAGS);
		if (UNIT_NPC_FLAGS < 0)
		{
			return;
		}
		var npcGuids = new List<WowGuid128>();
		GetSession().GameState.ObjectCacheMutex.WaitOne();
		foreach (var obj in GetSession().GameState.ObjectCacheModern)
		{
			if (obj.Key.GetObjectType() == ObjectType.Unit && obj.Value.GetUpdateField<uint>(UNIT_NPC_FLAGS).HasAnyFlag(NPCFlags.QuestGiver))
			{
				npcGuids.Add(obj.Key);
			}
		}
		GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
		foreach (var guid in npcGuids)
		{
			var packet2 = new WorldPacket(Opcode.CMSG_QUEST_GIVER_STATUS_QUERY);
			packet2.WriteGuid(guid.To64());
			SendPacketToServer(packet2);
		}
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_HELLO)]
	private void HandleQuestGiverHello(QuestGiverHello hello)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_HELLO);
		packet.WriteGuid(hello.QuestGiverGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_REQUEST_REWARD)]
	private void HandleQuestGiverRequestReward(QuestGiverRequestReward quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_REQUEST_REWARD);
		packet.WriteGuid(quest.QuestGiverGUID.To64());
		packet.WriteUInt32(quest.QuestID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_CHOOSE_REWARD)]
	private void HandleQuestGiverChooseReward(QuestGiverChooseReward quest)
	{
		var choiceIndex = 0;
		if (quest.Choice.Item.ItemID != 0)
		{
			var questTemplate = GameData.GetQuestTemplate(quest.QuestID);
			if (questTemplate == null)
			{
				Log.Print(LogType.Error, "Unable to select quest reward because quest template is missing. Try again.", "QuestHandler.cs");
				var packet2 = new WorldPacket(Opcode.CMSG_QUERY_QUEST_INFO);
				packet2.WriteUInt32(quest.QuestID);
				SendPacketToServer(packet2);
				var fail = new QuestGiverQuestFailed
				{
					QuestID = quest.QuestID,
					Reason = InventoryResult.ItemNotFound
				};
				SendPacket(fail);
				return;
			}
			for (var i = 0; i < questTemplate.UnfilteredChoiceItems.Length; i++)
			{
				if (questTemplate.UnfilteredChoiceItems[i].ItemID == quest.Choice.Item.ItemID)
				{
					choiceIndex = i;
					break;
				}
			}
		}
		var packet3 = new WorldPacket(Opcode.CMSG_QUEST_GIVER_CHOOSE_REWARD);
		packet3.WriteGuid(quest.QuestGiverGUID.To64());
		packet3.WriteUInt32(quest.QuestID);
		packet3.WriteInt32(choiceIndex);
		SendPacketToServer(packet3);
	}

	[PacketHandler(Opcode.CMSG_QUEST_GIVER_COMPLETE_QUEST)]
	private void HandleQuestGiverCompleteQuest(QuestGiverCompleteQuest quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_COMPLETE_QUEST);
		packet.WriteGuid(quest.QuestGiverGUID.To64());
		packet.WriteUInt32(quest.QuestID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_CONFIRM_ACCEPT)]
	private void HandleQuestConfirmAcceptResponse(QuestConfirmAcceptResponse quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_QUEST_CONFIRM_ACCEPT);
		packet.WriteUInt32(quest.QuestID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PUSH_QUEST_TO_PARTY)]
	private void HandlePushQuestToParty(PushQuestToParty quest)
	{
		var packet = new WorldPacket(Opcode.CMSG_PUSH_QUEST_TO_PARTY);
		packet.WriteUInt32(quest.QuestID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_QUEST_PUSH_RESULT)]
	private void HandleQuestPushResult(QuestPushResultResponse quest)
	{
		var packet = new WorldPacket(Opcode.MSG_QUEST_PUSH_RESULT);
		packet.WriteGuid(quest.SenderGUID.To64());
		packet.WriteUInt8((byte)quest.Result);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_FACTION_AT_WAR)]
	private void HandleSetFactionAtWar(SetFactionAtWar faction)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_FACTION_AT_WAR);
		packet.WriteUInt32(faction.FactionIndex);
		packet.WriteBool(data: true);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_FACTION_NOT_AT_WAR)]
	private void HandleSetFactionNotAtWar(SetFactionNotAtWar faction)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_FACTION_AT_WAR);
		packet.WriteUInt32(faction.FactionIndex);
		packet.WriteBool(data: false);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_FACTION_INACTIVE)]
	private void HandleSetFactionInactive(SetFactionInactive faction)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_FACTION_INACTIVE);
		packet.WriteUInt32(faction.FactionIndex);
		packet.WriteBool(faction.State);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_WATCHED_FACTION)]
	private void HandleSetFactionInactive(SetWatchedFaction faction)
	{
		var packet = new WorldPacket(Opcode.CMSG_SET_WATCHED_FACTION);
		packet.WriteUInt32(faction.FactionIndex);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CHANGE_REALM_TICKET)]
	private void HandleChangeRealmTicket(ChangeRealmTicket request)
	{
		var response = new ChangeRealmTicketResponse
		{
			Token = request.Token
		};
		if (!GetSession().AuthClient.IsConnected() && GetSession().AuthClient.Reconnect() != AuthResult.SUCCESS)
		{
			Log.Print(LogType.Error, "Failed to reconnect to auth server.", "SessionHandler.cs");
			response.Allow = false;
			SendPacket(response);
		}
		else
		{
			_bnetRpc.SetClientSecret(request.Secret);
			response.Allow = true;
			response.Ticket = new ByteBuffer(new byte[1]);
			SendPacket(response);
		}
	}

	[PacketHandler(Opcode.CMSG_BATTLENET_REQUEST)]
	private void HandleBattlenetRequest(BattlenetRequest request)
	{
		if (_bnetRpc == null)
		{
			Log.Print(LogType.Error, $"Client tried {108} without authentication", "SessionHandler.cs");
		}
		else
		{
			_bnetRpc.Invoke(0u, (OriginalHash)request.Method.GetServiceHash(), request.Method.GetMethodId(), request.Method.Token, new CodedInputStream(request.Data));
		}
	}

	[PacketHandler(Opcode.CMSG_CONTACT_LIST)]
	private void HandleContactList(ContactListRequest contacts)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_FRIEND_LIST);
			SendPacketToServer(packet);
		}
		else
		{
			var packet2 = new WorldPacket(Opcode.CMSG_CONTACT_LIST);
			packet2.WriteUInt32((uint)contacts.Flags);
			SendPacketToServer(packet2);
		}
	}

	[PacketHandler(Opcode.CMSG_ADD_FRIEND)]
	private void HandleAddFriend(AddFriend friend)
	{
		var packet = new WorldPacket(Opcode.CMSG_ADD_FRIEND);
		packet.WriteCString(friend.Name);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteCString(friend.Note);
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ADD_IGNORE)]
	private void HandleAddIgnore(AddIgnore ignore)
	{
		var packet = new WorldPacket(Opcode.CMSG_ADD_IGNORE);
		packet.WriteCString(ignore.Name);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_DEL_FRIEND)]
	[PacketHandler(Opcode.CMSG_DEL_IGNORE)]
	private void HandleDelFriend(DelFriend friend)
	{
		var packet = new WorldPacket(friend.GetUniversalOpcode());
		packet.WriteGuid(friend.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_CONTACT_NOTES)]
	private void HandleSetContactNotes(SetContactNotes friend)
	{
		if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_SET_CONTACT_NOTES);
			packet.WriteGuid(friend.Guid.To64());
			packet.WriteCString(friend.Notes);
			SendPacketToServer(packet);
		}
	}

	private SpellCastTargetFlags ConvertSpellTargetFlags(SpellTargetData target)
	{
		var targetFlags = SpellCastTargetFlags.None;
		if (target.Unit != null && !target.Unit.IsEmpty())
		{
			if (target.Flags.HasFlag(SpellCastTargetFlags.Unit))
			{
				targetFlags |= SpellCastTargetFlags.Unit;
			}
			if (target.Flags.HasFlag(SpellCastTargetFlags.CorpseEnemy))
			{
				targetFlags |= SpellCastTargetFlags.CorpseEnemy;
			}
			if (target.Flags.HasFlag(SpellCastTargetFlags.GameObject))
			{
				targetFlags |= SpellCastTargetFlags.GameObject;
			}
			if (target.Flags.HasFlag(SpellCastTargetFlags.CorpseAlly))
			{
				targetFlags |= SpellCastTargetFlags.CorpseAlly;
			}
			if (target.Flags.HasFlag(SpellCastTargetFlags.UnitMinipet))
			{
				targetFlags |= SpellCastTargetFlags.UnitMinipet;
			}
		}
		if ((target.Item != null) & !target.Item.IsEmpty())
		{
			if (target.Flags.HasFlag(SpellCastTargetFlags.Item))
			{
				targetFlags |= SpellCastTargetFlags.Item;
			}
			if (target.Flags.HasFlag(SpellCastTargetFlags.TradeItem))
			{
				targetFlags |= SpellCastTargetFlags.TradeItem;
			}
		}
		if (target.SrcLocation != null)
		{
			targetFlags |= SpellCastTargetFlags.SourceLocation;
		}
		if (target.DstLocation != null)
		{
			targetFlags |= SpellCastTargetFlags.DestLocation;
		}
		if (!string.IsNullOrEmpty(target.Name))
		{
			targetFlags |= SpellCastTargetFlags.String;
		}
		return targetFlags;
	}

	private void WriteSpellTargets(SpellTargetData target, SpellCastTargetFlags targetFlags, WorldPacket packet)
	{
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt16((ushort)targetFlags);
		}
		else
		{
			packet.WriteUInt32((uint)targetFlags);
		}
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.CorpseMask | SpellCastTargetFlags.Unit | SpellCastTargetFlags.GameObject | SpellCastTargetFlags.UnitMinipet))
		{
			packet.WritePackedGuid(target.Unit.To64());
		}
		if (targetFlags.HasFlag(SpellCastTargetFlags.TradeItem) && target.Item == WowGuid128.Create(HighGuidType703.Uniq, 10uL))
		{
			packet.WritePackedGuid(new WowGuid64(6uL));
		}
		else if (targetFlags.HasFlag(SpellCastTargetFlags.Item))
		{
			packet.WritePackedGuid(target.Item.To64());
		}
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.SourceLocation))
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
			{
				packet.WritePackedGuid(target.SrcLocation.Transport.To64());
			}
			packet.WriteVector3(target.SrcLocation.Location);
		}
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.DestLocation))
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
			{
				packet.WritePackedGuid(target.DstLocation.Transport.To64());
			}
			packet.WriteVector3(target.DstLocation.Location);
		}
		if (targetFlags.HasAnyFlag(SpellCastTargetFlags.String))
		{
			packet.WriteCString(target.Name);
		}
	}

	public void SendCastRequestFailed(ClientCastRequest castRequest, bool isPet)
	{
		if (!castRequest.HasStarted)
		{
			var prepare2 = new SpellPrepare
			{
				ClientCastID = castRequest.ClientGUID,
				ServerCastID = castRequest.ServerGUID
			};
			SendPacket(prepare2);
		}
		if (isPet)
		{
			var failed = new PetCastFailed
			{
				SpellID = castRequest.SpellId,
				Reason = 123u,
				CastID = castRequest.ServerGUID
			};
			SendPacket(failed);
		}
		else
		{
			var failed2 = new CastFailed
			{
				SpellID = castRequest.SpellId,
				SpellXSpellVisualID = castRequest.SpellXSpellVisualId,
				Reason = 123u,
				CastID = castRequest.ServerGUID
			};
			SendPacket(failed2);
		}
	}

	// Mining proficiency spells → Opening spell (6478) — mining nodes are opened like chests
	private static readonly HashSet<uint> _miningProficiencySpells = new()
	{
		2575, 2576, 3564, 10248, 29354, 50310
	};
	// Herbalism proficiency spells → Opening spell (6478) — herb nodes are opened like chests
	private static readonly HashSet<uint> _herbalismProficiencySpells = new()
	{
		2366, 2368, 3570, 11993, 28695, 50300
	};

	[PacketHandler(Opcode.CMSG_CAST_SPELL)]
	private void HandleCastSpell(CastSpell cast)
	{
		// Modern client sends gathering proficiency spells when clicking nodes.
		// Translate to actual gathering spell. GO target will be injected from
		// CurrentInteractedWithGO (set by previous CMSG_GAME_OBJ_REPORT_USE).
		// Query fishing bobber template so the client knows it's a FISHINGNODE
		if (cast.Cast.SpellID == 7620 || cast.Cast.SpellID == 7731 || cast.Cast.SpellID == 7732 ||
			cast.Cast.SpellID == 18248 || cast.Cast.SpellID == 33095 || cast.Cast.SpellID == 51294)
		{
			if (!GetSession().GameState.GameObjectQueryCache.ContainsKey(35591))
			{
				var goQuery = new WorldPacket(Opcode.CMSG_QUERY_GAME_OBJECT);
				goQuery.WriteUInt32(35591);
				goQuery.WriteUInt64(0);
				SendPacketToServer(goQuery);
			}
		}
		// Modern client sends gathering proficiency spells targeting nodes.
		// Don't translate — these spells have SPELL_EFFECT_OPEN_LOCK and work as-is.
		// Just inject the GO target if not already set.
		if (_miningProficiencySpells.Contains(cast.Cast.SpellID) ||
			_herbalismProficiencySpells.Contains(cast.Cast.SpellID))
		{
			Log.Print(LogType.Debug, $"[CastSpell] Gathering spell {cast.Cast.SpellID} — injecting GO target", "");
			if ((cast.Cast.Target.Unit == null || cast.Cast.Target.Unit.IsEmpty()) && GetSession().GameState.CurrentInteractedWithGO != null && !GetSession().GameState.CurrentInteractedWithGO.IsEmpty())
			{
				cast.Cast.Target.Unit = GetSession().GameState.CurrentInteractedWithGO;
				cast.Cast.Target.Flags |= SpellCastTargetFlags.GameObject;
			}
		}
		if (Settings.ServerSpellDelay > 0)
		{
			Thread.Sleep(Settings.ServerSpellDelay);
		}
		if (GameData.NextMeleeSpells.Contains(cast.Cast.SpellID) || GameData.AutoRepeatSpells.Contains(cast.Cast.SpellID))
		{
			var castRequest = new ClientCastRequest
			{
				Timestamp = Environment.TickCount,
				SpellId = cast.Cast.SpellID,
				SpellXSpellVisualId = cast.Cast.SpellXSpellVisualID,
				ClientGUID = cast.Cast.CastID
			};
			if (GetSession().GameState.CurrentClientSpecialCast != null)
			{
				castRequest.ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, 10000 + cast.Cast.CastID.GetCounter());
				SendCastRequestFailed(castRequest, isPet: false);
				return;
			}
			castRequest.ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, cast.Cast.SpellID + GetSession().GameState.CurrentPlayerGuid.GetCounter());
			var prepare = new SpellPrepare
			{
				ClientCastID = cast.Cast.CastID,
				ServerCastID = castRequest.ServerGUID
			};
			SendPacket(prepare);
			GetSession().GameState.CurrentClientSpecialCast = castRequest;
		}
		else
		{
			var castRequest2 = new ClientCastRequest
			{
				Timestamp = Environment.TickCount,
				SpellId = cast.Cast.SpellID,
				SpellXSpellVisualId = cast.Cast.SpellXSpellVisualID,
				ClientGUID = cast.Cast.CastID,
				ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, 10000 + cast.Cast.CastID.GetCounter())
			};
			if (GetSession().GameState.CurrentClientNormalCast != null)
			{
				if (GetSession().GameState.CurrentClientNormalCast.HasStarted)
				{
					SendCastRequestFailed(castRequest2, isPet: false);
				}
				else if (GetSession().GameState.CurrentClientNormalCast.Timestamp + 10000 < castRequest2.Timestamp)
				{
					Log.Print(LogType.Warn, $"Clearing CurrentClientNormalCast because of 10 sec timeout! (oldSpell:{GetSession().GameState.CurrentClientNormalCast.SpellId} newSpell:{castRequest2.SpellId})", "SpellHandler.cs");
					Log.Print(LogType.Warn, "Are you playing on a server with another patch?", "SpellHandler.cs");
					SendCastRequestFailed(GetSession().GameState.CurrentClientNormalCast, isPet: false);
					GetSession().GameState.CurrentClientNormalCast = null;
					foreach (var pending in GetSession().GameState.PendingClientCasts)
					{
						SendCastRequestFailed(pending, isPet: false);
					}
					GetSession().GameState.PendingClientCasts.Clear();
					SendCastRequestFailed(castRequest2, isPet: false);
				}
				else
				{
					GetSession().GameState.PendingClientCasts.Add(castRequest2);
				}
				return;
			}
			GetSession().GameState.CurrentClientNormalCast = castRequest2;
		}
		// If casting Opening spell (6478) with no target, inject the game object
		// from CMSG_GAME_OBJ_REPORT_USE — modern client sends the spell without a target
		if (cast.Cast.SpellID == 6478 && (cast.Cast.Target.Unit == null || cast.Cast.Target.Unit.IsEmpty()) && GetSession().GameState.CurrentInteractedWithGO != null && !GetSession().GameState.CurrentInteractedWithGO.IsEmpty())
		{
			cast.Cast.Target.Unit = GetSession().GameState.CurrentInteractedWithGO;
			cast.Cast.Target.Flags |= SpellCastTargetFlags.GameObject;
		}
		var targetFlags = ConvertSpellTargetFlags(cast.Cast.Target);
		Log.Print(LogType.Debug, $"[CastSpell] SpellID={cast.Cast.SpellID} TargetFlags=0x{(uint)targetFlags:X} ModernFlags=0x{(uint)cast.Cast.Target.Flags:X} Unit={cast.Cast.Target.Unit} Item={cast.Cast.Target.Item}", "");
		var packet = new WorldPacket(Opcode.CMSG_CAST_SPELL);
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt32(cast.Cast.SpellID);
		}
		else if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt32(cast.Cast.SpellID);
			packet.WriteUInt8(0);
		}
		else
		{
			packet.WriteUInt8(0);
			packet.WriteUInt32(cast.Cast.SpellID);
			packet.WriteUInt8((byte)cast.Cast.SendCastFlags);
		}
		WriteSpellTargets(cast.Cast.Target, targetFlags, packet);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_PET_CAST_SPELL)]
	private void HandlePetCastSpell(PetCastSpell cast)
	{
		if (Settings.ServerSpellDelay > 0)
		{
			Thread.Sleep(Settings.ServerSpellDelay);
		}
		var castRequest = new ClientCastRequest
		{
			Timestamp = Environment.TickCount,
			SpellId = cast.Cast.SpellID,
			SpellXSpellVisualId = cast.Cast.SpellXSpellVisualID,
			ClientGUID = cast.Cast.CastID,
			ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, 10000 + cast.Cast.CastID.GetCounter())
		};
		if (GetSession().GameState.CurrentClientPetCast != null)
		{
			if (GetSession().GameState.CurrentClientPetCast.HasStarted)
			{
				SendCastRequestFailed(castRequest, isPet: true);
			}
			else if (GetSession().GameState.CurrentClientPetCast.Timestamp + 10000 < castRequest.Timestamp)
			{
				Log.Print(LogType.Warn, $"Clearing CurrentClientPetCast because of 10 sec timeout! (oldSpell:{GetSession().GameState.CurrentClientPetCast.SpellId} newSpell:{castRequest.SpellId})", "SpellHandler.cs");
				SendCastRequestFailed(GetSession().GameState.CurrentClientPetCast, isPet: true);
				GetSession().GameState.CurrentClientPetCast = null;
				foreach (var pending in GetSession().GameState.PendingClientPetCasts)
				{
					SendCastRequestFailed(pending, isPet: true);
				}
				GetSession().GameState.PendingClientPetCasts.Clear();
				SendCastRequestFailed(castRequest, isPet: true);
			}
			else
			{
				GetSession().GameState.PendingClientPetCasts.Add(castRequest);
			}
		}
		else
		{
			GetSession().GameState.CurrentClientPetCast = castRequest;
			var targetFlags = ConvertSpellTargetFlags(cast.Cast.Target);
			var packet = new WorldPacket(Opcode.CMSG_PET_CAST_SPELL);
			packet.WriteGuid(cast.PetGUID.To64());
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				packet.WriteUInt8(0);
			}
			packet.WriteUInt32(cast.Cast.SpellID);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				packet.WriteUInt8((byte)cast.Cast.SendCastFlags);
			}
			WriteSpellTargets(cast.Cast.Target, targetFlags, packet);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_USE_ITEM)]
	private void HandleUseItem(UseItem use)
	{
		if (Settings.ServerSpellDelay > 0)
		{
			Thread.Sleep(Settings.ServerSpellDelay);
		}
		var castRequest = new ClientCastRequest
		{
			Timestamp = Environment.TickCount,
			SpellId = use.Cast.SpellID,
			SpellXSpellVisualId = use.Cast.SpellXSpellVisualID,
			ClientGUID = use.Cast.CastID,
			ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, GetSession().GameState.CurrentMapId.Value, use.Cast.SpellID, 10000 + use.Cast.CastID.GetCounter()),
			ItemGUID = use.CastItem
		};
		Log.Print(LogType.Debug, $"[UseItem] SpellID={use.Cast.SpellID} PackSlot={use.PackSlot} Slot={use.Slot} ItemGUID={use.CastItem} PendingCast={GetSession().GameState.CurrentClientNormalCast != null}", "");
		if (GetSession().GameState.CurrentClientNormalCast != null)
		{
			if (GetSession().GameState.CurrentClientNormalCast.HasStarted)
			{
				SendCastRequestFailed(castRequest, isPet: false);
			}
			else if (GetSession().GameState.CurrentClientNormalCast.Timestamp + 10000 < castRequest.Timestamp)
			{
				Log.Print(LogType.Warn, $"Clearing CurrentClientNormalCast because of 10 sec timeout! (oldSpell:{GetSession().GameState.CurrentClientNormalCast.SpellId} newSpell:{castRequest.SpellId})", "SpellHandler.cs");
				SendCastRequestFailed(GetSession().GameState.CurrentClientNormalCast, isPet: false);
				GetSession().GameState.CurrentClientNormalCast = null;
				foreach (var pending in GetSession().GameState.PendingClientCasts)
				{
					SendCastRequestFailed(pending, isPet: false);
				}
				GetSession().GameState.PendingClientCasts.Clear();
				SendCastRequestFailed(castRequest, isPet: false);
			}
			else
			{
				GetSession().GameState.PendingClientCasts.Add(castRequest);
			}
		}
		else
		{
			GetSession().GameState.CurrentClientNormalCast = castRequest;
			var packet = new WorldPacket(Opcode.CMSG_USE_ITEM);
			var containerSlot = use.PackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(use.PackSlot) : use.PackSlot;
			var slot = use.PackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(use.Slot) : use.Slot;
			packet.WriteUInt8(containerSlot); // bagIndex
			packet.WriteUInt8(slot); // slot
			packet.WriteUInt8(GetSession().GameState.GetItemSpellSlot(use.CastItem, use.Cast.SpellID)); // castCount
			packet.WriteUInt32(use.Cast.SpellID); // spellId
			packet.WriteGuid(use.CastItem.To64()); // itemGUID
			packet.WriteUInt32(0u); // glyphIndex
			packet.WriteUInt8(0); // castFlags
			var targetFlags = ConvertSpellTargetFlags(use.Cast.Target);
			WriteSpellTargets(use.Cast.Target, targetFlags, packet);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_CANCEL_CAST)]
	private void HandleCancelCast(CancelCast cast)
	{
		if (Settings.ServerSpellDelay > 0)
		{
			Thread.Sleep(Settings.ServerSpellDelay);
		}
		var packet = new WorldPacket(Opcode.CMSG_CANCEL_CAST);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			packet.WriteUInt8(0);
		}
		packet.WriteUInt32(cast.SpellID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CANCEL_CHANNELLING)]
	private void HandleCancelChannelling(CancelChannelling cast)
	{
		// Modern 3.4.3 client sends CMSG_CANCEL_CHANNELLING for every ESC press.
		// Use the stored channeled spell ID from CHANNEL_START when available.
		var spellId = GetSession().GameState.CurrentChanneledSpellId;
		if (spellId == 0)
			return;
		Log.Print(LogType.Debug, $"[CancelChannel] Cancelling channeled spell {spellId}", "");
		GetSession().GameState.CurrentChanneledSpellId = 0;
		if (Settings.ServerSpellDelay > 0)
		{
			Thread.Sleep(Settings.ServerSpellDelay);
		}
		var packet = new WorldPacket(Opcode.CMSG_CANCEL_CHANNELLING);
		packet.WriteInt32((int)spellId);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CANCEL_AUTO_REPEAT_SPELL)]
	private void HandleCancelAutoRepeatSpell(CancelAutoRepeatSpell spell)
	{
		if (Settings.ServerSpellDelay > 0)
		{
			Thread.Sleep(Settings.ServerSpellDelay);
		}
		var packet = new WorldPacket(Opcode.CMSG_CANCEL_AUTO_REPEAT_SPELL);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CANCEL_AURA)]
	private void HandleCancelAura(CancelAura aura)
	{
		var packet = new WorldPacket(Opcode.CMSG_CANCEL_AURA);
		packet.WriteUInt32(aura.SpellID);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CANCEL_MOUNT_AURA)]
	private void HandleCancelMountAura(EmptyClientPacket cancel)
	{
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_CANCEL_MOUNT_AURA);
			SendPacketToServer(packet);
			return;
		}
		var guid = GetSession().GameState.CurrentPlayerGuid;
		var updateFields = GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
		if (updateFields == null)
		{
			return;
		}
		for (byte i = 0; i < 32; i++)
		{
			var aura = GetSession().WorldClient.ReadAuraSlot(i, guid, updateFields);
			if (aura != null && GameData.MountAuras.Contains(aura.SpellID))
			{
				var packet2 = new WorldPacket(Opcode.CMSG_CANCEL_AURA);
				packet2.WriteUInt32(aura.SpellID);
				SendPacketToServer(packet2);
			}
		}
	}

	[PacketHandler(Opcode.CMSG_LEARN_TALENT)]
	private void HandleLearnTalent(LearnTalent talent)
	{
		var packet = new WorldPacket(Opcode.CMSG_LEARN_TALENT);
		packet.WriteUInt32(talent.TalentID);
		packet.WriteUInt32(talent.Rank);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_RESURRECT_RESPONSE)]
	private void HandleResurrectResponse(ResurrectResponse revive)
	{
		var packet = new WorldPacket(Opcode.CMSG_RESURRECT_RESPONSE);
		packet.WriteGuid(revive.CasterGUID.To64());
		packet.WriteUInt8(revive.Response == 0 ? (byte)1 : (byte)0);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SELF_RES)]
	private void HandleSelfRes(SelfRes revive)
	{
		var packet = new WorldPacket(Opcode.CMSG_SELF_RES);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_TOTEM_DESTROYED)]
	private void HandleTotemDestroyed(TotemDestroyed totem)
	{
		if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			var packet = new WorldPacket(Opcode.CMSG_TOTEM_DESTROYED);
			packet.WriteUInt8(totem.Slot);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_SUPPORT_TICKET_SUBMIT_COMPLAINT)]
	private void HandleSupportTicketSubmitComplaint(SupportTicketSubmitComplaint complaint)
	{
		var targetPlayerName = Session.GameState.GetPlayerName(complaint.TargetCharacterGuid);
		if (string.IsNullOrWhiteSpace(targetPlayerName))
		{
			Session.SendHermesTextMessage("Unable to report player because CharacterName was not resolved (can be fixed by restarting the client)", isError: true);
			return;
		}
		var ticketText = "[REPORTED VIA QUICKMENU]\r\nI would like to report player '" + targetPlayerName + "'";
		if (!WowGuid128.IsUnknownPlayerGuid(complaint.TargetCharacterGuid))
		{
			ticketText += $"  (id: {complaint.TargetCharacterGuid.GetCounter()})";
		}
		if (complaint.ComplaintType != GmTicketComplaintType.Unknown)
		{
			ticketText += $" for {complaint.ComplaintType}";
		}
		if (complaint.SelectedMailInfo != null)
		{
			ticketText = ticketText + "\r\n" + $"Mail in question (id: {complaint.SelectedMailInfo.MailId}) with subject '{complaint.SelectedMailInfo.MailSubject}'";
		}
		if (!complaint.TextNote.IsEmpty())
		{
			ticketText += "\r\n-------------";
			ticketText = ticketText + "\r\n" + complaint.TextNote;
		}
		var packet = new WorldPacket(Opcode.CMSG_GM_TICKET_CREATE);
		if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			packet.WriteUInt8(2);
			packet.WriteUInt32(complaint.Header.SelfPlayerMapId);
			packet.WriteVector3(complaint.Header.SelfPlayerPos);
			packet.WriteCString(ticketText);
			packet.WriteCString("");
		}
		else
		{
			packet.WriteUInt32(complaint.Header.SelfPlayerMapId);
			packet.WriteVector3(complaint.Header.SelfPlayerPos);
			packet.WriteCString(ticketText);
			packet.WriteUInt32(0u);
			packet.WriteUInt32(0u);
			packet.WriteUInt32(0u);
			packet.WriteBytes(Array.Empty<byte>());
		}
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_TAXI_NODE_STATUS_QUERY)]
	[PacketHandler(Opcode.CMSG_TAXI_QUERY_AVAILABLE_NODES)]
	private void HandleTaxiNodesQuery(InteractWithNPC interact)
	{
		var packet = new WorldPacket(interact.GetUniversalOpcode());
		packet.WriteGuid(interact.CreatureGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ENABLE_TAXI_NODE)]
	private void HandleEnableTaxiNode(InteractWithNPC interact)
	{
		var packet = new WorldPacket(Opcode.CMSG_TALK_TO_GOSSIP);
		packet.WriteGuid(interact.CreatureGUID.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_ACTIVATE_TAXI)]
	private void HandleActivateTaxi(ActivateTaxi taxi)
	{
		if (TaxiPathExist(GetSession().GameState.CurrentTaxiNode, taxi.Node))
		{
			var packet = new WorldPacket(Opcode.CMSG_ACTIVATE_TAXI);
			packet.WriteGuid(taxi.FlightMaster.To64());
			packet.WriteUInt32(GetSession().GameState.CurrentTaxiNode);
			packet.WriteUInt32(taxi.Node);
			SendPacketToServer(packet);
		}
		else
		{
			var path = GetTaxiPath(GetSession().GameState.CurrentTaxiNode, taxi.Node, GetSession().GameState.UsableTaxiNodes);
			if (path.Count <= 1)
			{
				return;
			}
			var packet2 = new WorldPacket(Opcode.CMSG_ACTIVATE_TAXI_EXPRESS);
			packet2.WriteGuid(taxi.FlightMaster.To64());
			packet2.WriteUInt32(0u);
			packet2.WriteUInt32((uint)path.Count);
			foreach (var itr in path)
			{
				packet2.WriteUInt32(itr);
			}
			SendPacketToServer(packet2);
		}
		GetSession().GameState.IsWaitingForTaxiStart = true;
	}

	private bool TaxiPathExist(uint from, uint to)
	{
		foreach (var itr in GameData.TaxiPaths)
		{
			if ((itr.Value.From == from && itr.Value.To == to) || (itr.Value.From == to && itr.Value.To == from))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsTaxiNodeKnown(uint node, List<byte> usableNodes)
	{
		var field = (byte)((node - 1) / 8);
		var submask = (uint)(1 << (byte)((node - 1) % 8));
		return (usableNodes[field] & submask) == submask;
	}

	private HashSet<uint> GetTaxiPath(uint from, uint to, List<byte> usableNodes)
	{
		var nodes = new HashSet<uint> { from };
		var graphCopy = new int[GameData.TaxiNodesGraph.GetLength(0), GameData.TaxiNodesGraph.GetLength(1)];
		Buffer.BlockCopy(GameData.TaxiNodesGraph, 0, graphCopy, 0, GameData.TaxiNodesGraph.Length * 4);
		for (var i = 1u; i < graphCopy.GetLength(0); i++)
		{
			if (!IsTaxiNodeKnown(i, usableNodes))
			{
				for (var itr = 0u; itr < graphCopy.GetLength(1); itr++)
				{
					graphCopy[i, itr] = 0;
				}
				for (var itr2 = 0u; itr2 < graphCopy.GetLength(0); itr2++)
				{
					graphCopy[itr2, i] = 0;
				}
			}
		}
		var minDist = Dijkstra(graphCopy, (int)from, (int)to, graphCopy.GetLength(0), nodes);
		return nodes;
	}

	private int MinDistance(int[] dist, bool[] sptSet, int vCnt)
	{
		var min = int.MaxValue;
		var min_index = -1;
		for (var v = 0; v < vCnt; v++)
		{
			if (!sptSet[v] && dist[v] <= min)
			{
				min = dist[v];
				min_index = v;
			}
		}
		return min_index;
	}

	private void SavePath(int[] parent, int j, HashSet<uint> nodes)
	{
		if (parent[j] != -1)
		{
			SavePath(parent, parent[j], nodes);
			nodes.Add((uint)j);
		}
	}

	private int Dijkstra(int[,] graph, int src, int dest, int vCnt, HashSet<uint> nodes)
	{
		var dist = new int[vCnt];
		var parent = new int[vCnt];
		var sptSet = new bool[vCnt];
		for (var i = 0; i < vCnt; i++)
		{
			dist[i] = int.MaxValue;
			sptSet[i] = false;
			parent[i] = -1;
		}
		dist[src] = 0;
		for (var count = 0; count < vCnt - 1; count++)
		{
			var u = MinDistance(dist, sptSet, vCnt);
			sptSet[u] = true;
			for (var v = 0; v < vCnt; v++)
			{
				if (!sptSet[v] && graph[u, v] != 0 && dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v])
				{
					parent[v] = u;
					dist[v] = dist[u] + graph[u, v];
				}
			}
		}
		SavePath(parent, dest, nodes);
		return dist[dest];
	}

	[PacketHandler(Opcode.CMSG_INITIATE_TRADE)]
	private void HandleInitiateTrade(InitiateTrade trade)
	{
		var packet = new WorldPacket(Opcode.CMSG_INITIATE_TRADE);
		packet.WriteGuid(trade.Guid.To64());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_SET_TRADE_GOLD)]
	private void HandleSetTradeGold(SetTradeGold trade)
	{
		var tradeSession = GetSession().GameState.CurrentTrade;
		if (tradeSession == null)
		{
			Log.Print(LogType.Error, $"Got {trade.GetUniversalOpcode()} without trade session", "TradeHandler.cs");
		}
		else
		{
			tradeSession.ClientStateIndex++;
			var packet = new WorldPacket(Opcode.CMSG_SET_TRADE_GOLD);
			packet.WriteInt32((int)trade.Coinage);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_ACCEPT_TRADE)]
	private void HandleAcceptTrade(AcceptTrade trade)
	{
		var packet = new WorldPacket(Opcode.CMSG_ACCEPT_TRADE);
		packet.WriteUInt32(trade.StateIndex);
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_BEGIN_TRADE)]
	[PacketHandler(Opcode.CMSG_BUSY_TRADE)]
	[PacketHandler(Opcode.CMSG_CANCEL_TRADE)]
	[PacketHandler(Opcode.CMSG_UNACCEPT_TRADE)]
	[PacketHandler(Opcode.CMSG_IGNORE_TRADE)]
	private void HandleEmptyTradePacket(EmptyClientPacket trade)
	{
		// Only forward if a trade session is active — modern client sends CANCEL_TRADE
		// on NPC interaction as a safety measure, which spams server errors
		if (trade.GetUniversalOpcode() == Opcode.CMSG_CANCEL_TRADE && GetSession().GameState.CurrentTrade == null)
			return;
		var packet = new WorldPacket(trade.GetUniversalOpcode());
		SendPacketToServer(packet);
	}

	[PacketHandler(Opcode.CMSG_CLEAR_TRADE_ITEM)]
	private void HandleClearTradeItem(ClearTradeItem trade)
	{
		var tradeSession = GetSession().GameState.CurrentTrade;
		if (tradeSession == null)
		{
			Log.Print(LogType.Error, $"Got {trade.GetUniversalOpcode()} without trade session", "TradeHandler.cs");
		}
		else
		{
			tradeSession.ClientStateIndex++;
			var packet = new WorldPacket(Opcode.CMSG_CLEAR_TRADE_ITEM);
			packet.WriteUInt8(trade.TradeSlot);
			SendPacketToServer(packet);
		}
	}

	[PacketHandler(Opcode.CMSG_SET_TRADE_ITEM)]
	private void HandleSetTradeItem(SetTradeItem trade)
	{
		var tradeSession = GetSession().GameState.CurrentTrade;
		if (tradeSession == null)
		{
			Log.Print(LogType.Error, $"Got {trade.GetUniversalOpcode()} without trade session", "TradeHandler.cs");
			return;
		}
		tradeSession.ClientStateIndex++;
		var packet = new WorldPacket(Opcode.CMSG_SET_TRADE_ITEM);
		packet.WriteUInt8(trade.TradeSlot);
		var containerSlot = trade.PackSlot != byte.MaxValue ? ModernVersion.AdjustInventorySlot(trade.PackSlot) : trade.PackSlot;
		var slot = trade.PackSlot == byte.MaxValue ? ModernVersion.AdjustInventorySlot(trade.ItemSlotInPack) : trade.ItemSlotInPack;
		packet.WriteUInt8(containerSlot);
		packet.WriteUInt8(slot);
		SendPacketToServer(packet);
	}

	public WorldSocket(Socket socket)
		: base(socket)
	{
		_connectType = ConnectionType.Realm;
		_serverChallenge = Array.Empty<byte>().GenerateRandomKey(16);
		_worldCrypt = new WorldCrypt();
		_encryptKey = new byte[16];
		_headerBuffer = new SocketBuffer(HeaderSize);
		_packetBuffer = new SocketBuffer();
		InitializePacketHandlers();
	}

	public override void Dispose()
	{
		_serverChallenge = null;
		_sessionKey = null;
		_compressionStream = null;
		base.Dispose();
	}

	public GlobalSessionData GetSession()
	{
		return _globalSession;
	}

	public override void Accept()
	{
		var ip_address = GetRemoteIpAddress().ToString();
		_packetBuffer.Resize(ClientConnectionInitialize.Length + 1);
		AsyncReadWithCallback(InitializeHandler);
		var packet = new ByteBuffer();
		packet.WriteString(ServerConnectionInitialize);
		packet.WriteString("\n");
		AsyncWrite(packet.GetData());
	}

	private void InitializeHandler(SocketAsyncEventArgs args)
	{
		if (args.SocketError != SocketError.Success)
		{
			CloseSocket();
		}
		else
		{
			if (args.BytesTransferred <= 0 || _packetBuffer.GetRemainingSpace() <= 0)
			{
				return;
			}
			var readHeaderSize = Math.Min(args.BytesTransferred, _packetBuffer.GetRemainingSpace());
			_packetBuffer.Write(args.Buffer, 0, readHeaderSize);
			if (_packetBuffer.GetRemainingSpace() > 0)
			{
				AsyncReadWithCallback(InitializeHandler);
				return;
			}
			var buffer = new ByteBuffer(_packetBuffer.GetData());
			var initializer = buffer.ReadString((uint)ClientConnectionInitialize.Length);
			if (initializer != ClientConnectionInitialize)
			{
				CloseSocket();
				return;
			}
			var terminator = buffer.ReadUInt8();
			if (terminator != 10)
			{
				CloseSocket();
				return;
			}
			_compressionStream = new ZLib.z_stream();
			var z_res1 = ZLib.deflateInit2(_compressionStream, 1, 8, -15, 8, 0);
			if (z_res1 != 0)
			{
				CloseSocket();
				Log.Print(LogType.Error, $"Can't initialize packet compression (zlib: deflateInit2_) Error code: {z_res1}", "WorldSocket.cs");
			}
			else
			{
				_packetBuffer.Resize(0);
				_packetBuffer.Reset();
				HandleSendAuthSession();
				AsyncRead();
			}
		}
	}

	public override void ReadHandler(SocketAsyncEventArgs args)
	{
		if (!IsOpen())
		{
			return;
		}
		var currentReadIndex = 0;
		while (currentReadIndex < args.BytesTransferred)
		{
			if (_headerBuffer.GetRemainingSpace() > 0)
			{
				var readHeaderSize = Math.Min(args.BytesTransferred - currentReadIndex, _headerBuffer.GetRemainingSpace());
				_headerBuffer.Write(args.Buffer, currentReadIndex, readHeaderSize);
				currentReadIndex += readHeaderSize;
				if (_headerBuffer.GetRemainingSpace() > 0)
				{
					break;
				}
				if (!ReadHeader())
				{
					CloseSocket();
					return;
				}
			}
			if (_packetBuffer.GetRemainingSpace() > 0)
			{
				var readDataSize = Math.Min(args.BytesTransferred - currentReadIndex, _packetBuffer.GetRemainingSpace());
				_packetBuffer.Write(args.Buffer, currentReadIndex, readDataSize);
				currentReadIndex += readDataSize;
				if (_packetBuffer.GetRemainingSpace() > 0)
				{
					break;
				}
			}
			var result = ReadData();
			_headerBuffer.Reset();
			switch (result)
			{
			case ReadDataHandlerResult.WaitingForQuery:
				return;
			case ReadDataHandlerResult.Ok:
				continue;
			}
			CloseSocket();
			return;
		}
		AsyncRead();
	}

	private bool ReadHeader()
	{
		var header = new PacketHeader();
		header.Read(_headerBuffer.GetData());
		_packetBuffer.Resize(header.Size);
		return true;
	}

	private static readonly HashSet<Opcode> _suppressedLogOpcodes = new()
	{
		Opcode.CMSG_HOTFIX_REQUEST,
		Opcode.UNKNOWN_SMSG,
	};

	private ReadDataHandlerResult ReadData()
	{
		var header = new PacketHeader();
		header.Read(_headerBuffer.GetData());
		if (!_worldCrypt.Decrypt(_packetBuffer.GetData(), header.Tag))
		{
			Log.Print(LogType.Error, $"WorldSocket.ReadData(): client {GetRemoteIpAddress()} failed to decrypt packet (size: {header.Size})", "WorldSocket.cs");
			return ReadDataHandlerResult.Error;
		}
		var packet = new WorldPacket(_packetBuffer.GetData());
		_packetBuffer.Reset();
		var opcode = packet.GetUniversalOpcode(isModern: true);
		Log.PrintNet(LogType.Debug, LogNetDir.C2P, $"Received opcode {opcode.ToString()} ({packet.GetOpcode()}).", "WorldSocket.cs");
		if (!_suppressedLogOpcodes.Contains(opcode) && !header.IsValidSize())
		{
			Log.Print(LogType.Error, $"WorldSocket.ReadHeaderHandler(): client {GetRemoteIpAddress()} sent malformed packet (size: {header.Size})", "WorldSocket.cs");
			return ReadDataHandlerResult.Error;
		}
		switch (opcode)
		{
		case Opcode.CMSG_PING:
		{
			var ping = new Ping(packet);
			ping.Read();
			if (_connectType == ConnectionType.Realm && GetSession().WorldClient != null && GetSession().WorldClient.IsConnected() && GetSession().WorldClient.IsAuthenticated())
			{
				GetSession().WorldClient.SendPing(ping.Serial, ping.Latency);
			}
			else
			{
				HandlePing(ping);
			}
			break;
		}
		case Opcode.CMSG_AUTH_SESSION:
		{
			var authSession = new AuthSession(packet);
			authSession.Read();
			HandleAuthSession(authSession);
			return ReadDataHandlerResult.WaitingForQuery;
		}
		case Opcode.CMSG_AUTH_CONTINUED_SESSION:
		{
			var authContinuedSession = new AuthContinuedSession(packet);
			authContinuedSession.Read();
			HandleAuthContinuedSession(authContinuedSession);
			return ReadDataHandlerResult.WaitingForQuery;
		}
		case Opcode.CMSG_LOG_DISCONNECT:
		{
			var reason = packet.ReadUInt32();
			Log.Print(LogType.Server, $"Client disconnected with reason {reason}.", "WorldSocket.cs");
			if (_connectType == ConnectionType.Realm)
			{
				if (GetSession().AuthClient != null)
				{
					GetSession().AuthClient.Disconnect();
				}
				if (GetSession().WorldClient != null)
				{
					GetSession().WorldClient.Disconnect();
				}
			}
			if (GetSession().ModernSniff != null)
			{
				GetSession().ModernSniff.CloseFile();
				GetSession().ModernSniff = null;
			}
			break;
		}
		case Opcode.CMSG_ENABLE_NAGLE:
			SetNoDelay(enable: false);
			break;
		case Opcode.CMSG_CONNECT_TO_FAILED:
		{
			var connectToFailed = new ConnectToFailed(packet);
			connectToFailed.Read();
			HandleConnectToFailed(connectToFailed);
			break;
		}
		case Opcode.CMSG_ENTER_ENCRYPTED_MODE_ACK:
			HandleEnterEncryptedModeAck();
			break;
		case Opcode.CMSG_SERVER_TIME_OFFSET_REQUEST:
			SendServerTimeOffset();
			break;
		case Opcode.CMSG_SOCIAL_CONTRACT_REQUEST:
			SendSocialContractRequestResponse();
			break;
		default:
			HandlePacket(packet);
			break;
		case Opcode.CMSG_KEEP_ALIVE:
			break;
		}
		return ReadDataHandlerResult.Ok;
	}

	public void HandlePacket(WorldPacket packet)
	{
		var universalOpcode = packet.GetUniversalOpcode(isModern: true);
		var handler = GetHandler(universalOpcode);
		if (handler != null)
		{
			handler.Invoke(this, packet);
			return;
		}
		Log.PrintNet(LogType.Warn, LogNetDir.C2P, $"No handler for opcode {universalOpcode} ({packet.GetOpcode()}) (Got unknown packet from ModernClient)", "WorldSocket.cs");
		MissingOpcodeTracker.LogUnhandledCMSG(universalOpcode, packet.GetOpcode());
	}

	private void SendPacketToServer(WorldPacket packet, Opcode delayUntilOpcode = Opcode.MSG_NULL_ACTION)
	{
		if (GetSession().WorldClient != null)
		{
			GetSession().WorldClient.SendPacketToServer(packet, delayUntilOpcode);
			return;
		}
		Log.Print(LogType.Error, $"Attempt to send opcode {packet.GetUniversalOpcode(isModern: false)} ({packet.GetOpcode()}) while WorldClient is disconnected!", "WorldSocket.cs");
	}

	public PacketHandler GetHandler(Opcode opcode)
	{
		return _clientPacketTable.LookupByKey(opcode);
	}

	public void SendPacket(ServerPacket packet)
	{
		if (!IsOpen())
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2C, $"Can't send {packet.GetUniversalOpcode()}, socket is closed!", "WorldSocket.cs");
			if (GetSession() != null)
			{
				if (GetSession().RealmSocket == this)
				{
					GetSession().RealmSocket = null;
				}
				else if (GetSession().InstanceSocket == this)
				{
					GetSession().InstanceSocket = null;
				}
				GetSession().OnDisconnect();
			}
			return;
		}
		packet.WritePacketData();
		if (packet.SkipSend)
		{
			return;
		}
		if (GetSession() != null)
		{
			packet.LogPacket(ref GetSession().ModernSniff);
		}
		_sendMutex.WaitOne();
		var data = packet.GetData();
		var universalOpcode = packet.GetUniversalOpcode();
		var opcode = (ushort)packet.GetOpcode();
		if (opcode == 0 && universalOpcode != Opcode.MSG_NULL_ACTION)
		{
			Log.PrintNet(LogType.Warn, LogNetDir.P2C, $"Dropping packet {universalOpcode} - missing modern opcode mapping! (size={data.Length})", "WorldSocket.cs");
			MissingOpcodeTracker.LogDroppedSMSG(universalOpcode, data.Length);
			_sendMutex.ReleaseMutex();
			return;
		}
		if (universalOpcode != Opcode.SMSG_ON_MONSTER_MOVE)
			Log.PrintNet(LogType.Debug, LogNetDir.P2C, $"Sending opcode {universalOpcode} ({opcode}), size={data.Length}.", "WorldSocket.cs");
		var buffer = new ByteBuffer();
		var packetSize = data.Length;
		if (packetSize > 1024 && _worldCrypt.IsInitialized && ModernVersion.ExpansionVersion < 3)
		{
			buffer.WriteInt32(packetSize + 2);
			buffer.WriteUInt32(ZLib.adler32(ZLib.adler32(2552748273u, BitConverter.GetBytes(opcode), 2u), data, (uint)packetSize));
			byte[] compressedData;
			var compressedSize = CompressPacket(data, opcode, out compressedData);
			buffer.WriteUInt32(ZLib.adler32(2552748273u, compressedData, compressedSize));
			buffer.WriteBytes(compressedData, compressedSize);
			packetSize = (int)(compressedSize + 12);
			opcode = (ushort)ModernVersion.GetCurrentOpcode(Opcode.SMSG_COMPRESSED_PACKET);
			data = buffer.GetData();
		}
		buffer = new ByteBuffer();
		buffer.WriteUInt16(opcode);
		buffer.WriteBytes(data);
		packetSize += 2;
		data = buffer.GetData();
		var header = new PacketHeader
		{
			Size = packetSize
		};
		_worldCrypt.Encrypt(ref data, ref header.Tag);
		var byteBuffer = new ByteBuffer();
		header.Write(byteBuffer);
		byteBuffer.WriteBytes(data);
		AsyncWrite(byteBuffer.GetData());
		_sendMutex.ReleaseMutex();
	}

	public uint CompressPacket(byte[] data, ushort opcode, out byte[] outData)
	{
		var uncompressedData = BitConverter.GetBytes(opcode).Combine(data);
		var bufferSize = ZLib.deflateBound(_compressionStream, (uint)data.Length);
		outData = new byte[bufferSize];
		_compressionStream.next_out = 0;
		_compressionStream.avail_out = bufferSize;
		_compressionStream.out_buf = outData;
		_compressionStream.next_in = 0u;
		_compressionStream.avail_in = (uint)uncompressedData.Length;
		_compressionStream.in_buf = uncompressedData;
		var z_res = ZLib.deflate(_compressionStream, 2);
		if (z_res != 0)
		{
			Log.PrintNet(LogType.Error, LogNetDir.P2C, $"Can't compress packet data (zlib: deflate) Error code: {z_res} msg: {_compressionStream.msg}", "WorldSocket.cs");
			return 0u;
		}
		return bufferSize - _compressionStream.avail_out;
	}

	public override bool Update()
	{
		if (!base.Update())
		{
			return false;
		}
		return true;
	}

	private void HandleSendAuthSession()
	{
		var challenge = new AuthChallenge
		{
			Challenge = _serverChallenge,
			DosChallenge = new byte[32].GenerateRandomKey(32),
			DosZeroBits = 1
		};
		SendPacket(challenge);
	}

	private void HandleAuthSession(AuthSession authSession)
	{
		_globalSession = BnetSessionTicketStorage.SessionsByName[authSession.RealmJoinTicket];
		_bnetRpc = new BnetServices.ServiceManager("WorldSocket", this, _globalSession);
		HandleAuthSessionCallback(authSession);
	}

	private void HandleAuthSessionCallback(AuthSession authSession)
	{
		var buildInfo = GetSession().RealmManager.GetBuildInfo(GetSession().Build);
		if (buildInfo == null)
		{
			SendAuthResponseError(BattlenetRpcErrorCode.BadVersion);
			Log.Print(LogType.Error, $"WorldSocket.HandleAuthSessionCallback: Missing auth seed for realm build {GetSession().Build} ({GetRemoteIpAddress()}).", "WorldSocket.cs");
			CloseSocket();
			GetSession().OnDisconnect();
			return;
		}
		var address = GetRemoteIpAddress();
		if (GetSession().OS != "Wn64" && GetSession().OS != "Mc64" && GetSession().OS != "MacA")
		{
			Log.Print(LogType.Error, $"WorldSocket.HandleAuthSession: Unknown OS for account: {GetSession().GameAccountInfo.Id} ('{authSession.RealmJoinTicket}') address: {address}", "WorldSocket.cs");
			CloseSocket();
			GetSession().OnDisconnect();
			return;
		}
		var platformSeed = buildInfo.BuildSeeds.GetValueOrDefault(GetSession().OS);
		if (platformSeed == null || !TrySeed(platformSeed))
		{
			Log.Print(LogType.Debug, "WorldSocket.HandleAuthSession: Fallback to static seed", "WorldSocket.cs");
			if (!TrySeed(buildInfo.FallbackStaticSeed))
			{
				Log.Print(LogType.Warn, $"WorldSocket.HandleAuthSession: Seed mismatch for account: {GetSession().GameAccountInfo.Id} ('{authSession.RealmJoinTicket}') - BYPASSING for testing", "WorldSocket.cs");
			}
		}
		var keyData = new Sha256();
		keyData.Finish(GetSession().SessionKey);
		var sessionKeyHmac = new HmacSha256(keyData.Digest);
		sessionKeyHmac.Process(_serverChallenge, 16);
		sessionKeyHmac.Process(authSession.LocalChallenge, authSession.LocalChallenge.Count);
		sessionKeyHmac.Finish(SessionKeySeed, 16);
		_sessionKey = new byte[40];
		var sessionKeyGenerator = new SessionKeyGenerator(sessionKeyHmac.Digest, 32);
		sessionKeyGenerator.Generate(_sessionKey, 40u);
		var encryptKeyGen = new HmacSha256(_sessionKey);
		encryptKeyGen.Process(authSession.LocalChallenge, authSession.LocalChallenge.Count);
		encryptKeyGen.Process(_serverChallenge, 16);
		encryptKeyGen.Finish(EncryptionKeySeed, 16);
		Buffer.BlockCopy(encryptKeyGen.Digest, 0, _encryptKey, 0, 16);
		GetSession().SessionKey = _sessionKey;
		Log.Print(LogType.Server, $"WorldSocket:HandleAuthSession: Client '{authSession.RealmJoinTicket}' authenticated successfully from {address}.", "WorldSocket.cs");
		_realmId = new RealmId((byte)authSession.RegionID, (byte)authSession.BattlegroupID, authSession.RealmID);
		GetSession().WorldClient = new WorldClient();
		if (!GetSession().WorldClient.ConnectToWorldServer(GetSession().RealmManager.GetRealm(_realmId), GetSession()))
		{
			SendAuthResponseError(BattlenetRpcErrorCode.BadServer);
			Log.Print(LogType.Error, "The WorldClient failed to connect to the selected world server!", "WorldSocket.cs");
			Session.AccountMetaDataMgr.InvalidateLastSelectedCharacter();
			CloseSocket();
			GetSession().OnDisconnect();
		}
		else
		{
			SendPacket(new EnterEncryptedMode(_encryptKey, enabled: true));
			AsyncRead();
		}
		bool TrySeed(byte[] seed)
		{
			var digestKeyHash = new Sha256();
			digestKeyHash.Process(GetSession().SessionKey, GetSession().SessionKey.Length);
			digestKeyHash.Finish(seed);
			var hmac = new HmacSha256(digestKeyHash.Digest);
			hmac.Process(authSession.LocalChallenge, authSession.LocalChallenge.Count);
			hmac.Process(_serverChallenge, 16);
			hmac.Finish(AuthCheckSeed, 16);
			return hmac.Digest.Compare(authSession.Digest);
		}
	}

	private void HandleAuthContinuedSession(AuthContinuedSession authSession)
	{
		var key = default(ConnectToKey);
		var key2 = key.Raw = authSession.Key;
		_key = key2;
		_connectType = key.connectionType;
		if (_connectType != ConnectionType.Instance)
		{
			SendAuthResponseError(BattlenetRpcErrorCode.Denied);
			CloseSocket();
		}
		else
		{
			HandleAuthContinuedSessionCallback(authSession);
		}
	}

	private void HandleAuthContinuedSessionCallback(AuthContinuedSession authSession)
	{
		var key = default(ConnectToKey);
		var key2 = key.Raw = authSession.Key;
		_key = key2;
		_globalSession = BnetSessionTicketStorage.SessionsByKey[_key];
		var accountId = key.AccountId;
		var login = GetSession().AccountInfo.Login;
		_sessionKey = GetSession().SessionKey;
		var hmac = new HmacSha256(_sessionKey);
		hmac.Process(BitConverter.GetBytes(authSession.Key), 8);
		hmac.Process(authSession.LocalChallenge, authSession.LocalChallenge.Length);
		hmac.Process(_serverChallenge, 16);
		hmac.Finish(ContinuedSessionSeed, 16);
		if (!hmac.Digest.Compare(authSession.Digest))
		{
			Log.Print(LogType.Error, $"WorldSocket.HandleAuthContinuedSession: Authentication failed for account: {accountId} ('{login}') address: {GetRemoteIpAddress()}", "WorldSocket.cs");
			CloseSocket();
		}
		else
		{
			var encryptKeyGen = new HmacSha256(_sessionKey);
			encryptKeyGen.Process(authSession.LocalChallenge, authSession.LocalChallenge.Length);
			encryptKeyGen.Process(_serverChallenge, 16);
			encryptKeyGen.Finish(EncryptionKeySeed, 16);
			Buffer.BlockCopy(encryptKeyGen.Digest, 0, _encryptKey, 0, 16);
			SendPacket(new EnterEncryptedMode(_encryptKey, enabled: true));
			AsyncRead();
		}
	}

	public void SendConnectToInstance(ConnectToSerial serial)
	{
		var externalIp = IPAddress.Parse(Settings.ExternalAddress);
		if (IPAddress.IsLoopback(GetRemoteIpAddress().Address))
		{
			externalIp = IPAddress.Loopback;
		}
		else if (externalIp.Equals(IPAddress.Loopback))
		{
			externalIp = GetLocalIpAddress().Address;
		}
		var instanceAddress = new IPEndPoint(externalIp, Settings.InstancePort);
		_instanceConnectKey.AccountId = GetSession().AccountInfo.Id;
		_instanceConnectKey.connectionType = ConnectionType.Instance;
		_instanceConnectKey.Key = RandomHelper.URand(0, int.MaxValue);
		BnetSessionTicketStorage.AddNewSessionByKey(_instanceConnectKey.Raw, GetSession());
		var connectTo = new ConnectTo
		{
			Key = _instanceConnectKey.Raw,
			Serial = serial,
			Payload =
			{
				Port = (ushort)Settings.InstancePort
			},
			Con = 1
		};
		if (instanceAddress.AddressFamily == AddressFamily.InterNetwork)
		{
			connectTo.Payload.Where.IPv4 = instanceAddress.Address.GetAddressBytes();
			connectTo.Payload.Where.Type = ConnectTo.AddressType.IPv4;
		}
		else
		{
			connectTo.Payload.Where.IPv6 = instanceAddress.Address.GetAddressBytes();
			connectTo.Payload.Where.Type = ConnectTo.AddressType.IPv6;
		}
		SendPacket(connectTo);
	}

	public void AbortLogin(LoginFailureReason reason)
	{
		SendPacket(new CharacterLoginFailed(reason));
	}

	private void HandleConnectToFailed(ConnectToFailed connectToFailed)
	{
		switch (connectToFailed.Serial)
		{
		case ConnectToSerial.WorldAttempt1:
			SendConnectToInstance(ConnectToSerial.WorldAttempt2);
			break;
		case ConnectToSerial.WorldAttempt2:
			SendConnectToInstance(ConnectToSerial.WorldAttempt3);
			break;
		case ConnectToSerial.WorldAttempt3:
			SendConnectToInstance(ConnectToSerial.WorldAttempt4);
			break;
		case ConnectToSerial.WorldAttempt4:
			SendConnectToInstance(ConnectToSerial.WorldAttempt5);
			break;
		case ConnectToSerial.WorldAttempt5:
			Log.Print(LogType.Error, "Failed to connect 5 times to world socket, aborting login", "WorldSocket.cs");
			AbortLogin(LoginFailureReason.NoWorld);
			break;
		}
	}

	[PacketHandler(Opcode.CMSG_READY_FOR_ACCOUNT_DATA_TIMES)]
	private void HandleReadyForAccountDataTimes(ReadyForAccountDataTimesPkt packet)
	{
		// 3.4.3 client sends this after entering world. 
		// We should respond with SMSG_ACCOUNT_DATA_TIMES if we want to support settings sync.
		SendMotd();
		SendAccountDataTimes();
	}

	private void HandleEnterEncryptedModeAck()
	{
		_worldCrypt.Initialize(_encryptKey);
		if (_connectType == ConnectionType.Realm)
		{
			SendAuthResponse(BattlenetRpcErrorCode.Ok, GetSession().WorldClient.GetQueuePosition());
			SendSetTimeZoneInformation();
			SendFeatureSystemStatusGlueScreen();
			SendClientCacheVersion(0u);
			SendAvailableHotfixes();
			SendBnetConnectionState(1);
			GetSession().AccountDataMgr = new AccountDataManager(GetSession().Username, GetSession().RealmManager.GetRealm(_realmId).Name);
			GetSession().RealmSocket = this;
			GetSession().WorldClient.FlushPendingPackets();
		}
		else
		{
			Log.Print(LogType.Server, "Client has connected to the instance server.", "WorldSocket.cs");
			SendPacket(new ResumeComms(ConnectionType.Instance));
			GetSession().GameState.IsConnectedToInstance = true;
			GetSession().InstanceSocket = this;
			GetSession().WorldClient.FlushPendingPackets();
		}
	}

	public void SendAuthResponseError(BattlenetRpcErrorCode code)
	{
		var response = new AuthResponse
		{
			SuccessInfo = null,
			WaitInfo = null,
			Result = code
		};
		SendPacket(response);
	}

	public void SendAuthResponse(BattlenetRpcErrorCode code, uint queuePos = 0u)
	{
		var response = new AuthResponse
		{
			Result = code
		};
		if (code == BattlenetRpcErrorCode.Ok)
		{
			response.SuccessInfo = new AuthResponse.AuthSuccessInfo
			{
				ActiveExpansionLevel = LegacyVersion.ExpansionVersion,
				AccountExpansionLevel = LegacyVersion.ExpansionVersion,
				VirtualRealmAddress = _realmId.GetAddress(),
				Time = (uint)Time.UnixTime
			};
			var realm = GetSession().RealmManager.GetRealm(_realmId);
			response.SuccessInfo.VirtualRealms.Add(new VirtualRealmInfo(realm.Id.GetAddress(), isHomeRealm: true, isInternalRealm: false, realm.Name, realm.NormalizedName));
			var availableRaces = new List<AuthResponse.RaceClassAvailability>();
			var race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 1
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(2, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(8, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(9, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 2
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(7, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(9, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 3
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(2, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 4
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(11, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 5
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(8, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(9, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 6
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(7, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(11, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 7
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(8, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(9, 0, 0));
			availableRaces.Add(race);
			race = new AuthResponse.RaceClassAvailability
			{
				RaceID = 8
			};
			race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(7, 0, 0));
			race.Classes.Add(new AuthResponse.ClassAvailability(8, 0, 0));
			availableRaces.Add(race);
			if (ModernVersion.ExpansionVersion >= 2 && LegacyVersion.ExpansionVersion >= 2)
			{
				race = new AuthResponse.RaceClassAvailability
				{
					RaceID = 10
				};
				race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(4, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(8, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(9, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(2, 0, 0));
				availableRaces.Add(race);
				race = new AuthResponse.RaceClassAvailability
				{
					RaceID = 11
				};
				race.Classes.Add(new AuthResponse.ClassAvailability(1, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(2, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(3, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(5, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(8, 0, 0));
				race.Classes.Add(new AuthResponse.ClassAvailability(7, 0, 0));
				availableRaces.Add(race);
			}
			response.SuccessInfo.AvailableClasses = availableRaces;
		}
		if (queuePos != 0)
		{
			response.WaitInfo = new AuthWaitInfo
			{
				WaitCount = queuePos
			};
		}
		SendPacket(response);
	}

	public void SendAuthWaitQue(uint position)
	{
		if (position != 0)
		{
			var waitQueueUpdate = new WaitQueueUpdate
			{
				WaitInfo =
				{
					WaitCount = position,
					WaitTime = 0u,
					HasFCM = false
				}
			};
			SendPacket(waitQueueUpdate);
		}
		else
		{
			SendPacket(new WaitQueueFinish());
		}
	}

	public void SendSetTimeZoneInformation()
	{
		var packet = new SetTimeZoneInformation
		{
			ServerTimeTZ = "Europe/Paris",
			GameTimeTZ = "Europe/Paris"
		};
		SendPacket(packet);
	}

	public void SendFeatureSystemStatusGlueScreen()
	{
		var features = new FeatureSystemStatusGlueScreen
		{
			BpayStoreAvailable = false,
			BpayStoreDisabledByParentalControls = false,
			CharUndeleteEnabled = false,
			BpayStoreEnabled = false,
			MaxCharactersPerRealm = 10,
			MinimumExpansionLevel = 0,
			MaximumExpansionLevel = LegacyVersion.ExpansionVersion,
			ActiveSeason = 2,
			Unk14 = true
		};
		var europaTicketConfig = new EuropaTicketConfig();
		europaTicketConfig.ThrottleState.MaxTries = 10u;
		europaTicketConfig.ThrottleState.PerMilliseconds = 60000u;
		europaTicketConfig.ThrottleState.TryCount = 1u;
		europaTicketConfig.ThrottleState.LastResetTimeBeforeNow = 111111u;
		europaTicketConfig.TicketsEnabled = true;
		europaTicketConfig.BugsEnabled = true;
		europaTicketConfig.ComplaintsEnabled = true;
		europaTicketConfig.SuggestionsEnabled = true;
		features.EuropaTicketSystemStatus = europaTicketConfig;
		SendPacket(features);
	}

	public void SendFeatureSystemStatus()
	{
		var features = new FeatureSystemStatus
		{
			ComplaintStatus = 2,
			ScrollOfResurrectionRequestsRemaining = 1u,
			ScrollOfResurrectionMaxRequestsPerDay = 1u,
			CfgRealmID = 1u,
			CfgRealmRecID = 1,
			TwitterPostThrottleLimit = 60u,
			TwitterPostThrottleCooldown = 20u,
			TokenPollTimeSeconds = 300u,
			KioskSessionMinutes = 30u,
			BpayStoreProductDeliveryDelay = 180u,
			HiddenUIClubsPresenceUpdateTimer = 60000u,
			VoiceEnabled = false,
			BrowserEnabled = false,
			EuropaTicketSystemStatus = new EuropaTicketConfig()
		};
		features.EuropaTicketSystemStatus.ThrottleState.MaxTries = 10u;
		features.EuropaTicketSystemStatus.ThrottleState.PerMilliseconds = 60000u;
		features.EuropaTicketSystemStatus.ThrottleState.TryCount = 1u;
		features.EuropaTicketSystemStatus.ThrottleState.LastResetTimeBeforeNow = 111111u;
		features.TutorialsEnabled = true;
		features.Unk67 = true;
		features.QuestSessionEnabled = true;
		features.BattlegroundsEnabled = true;
		features.QuickJoinConfig.ToastDuration = 7f;
		features.QuickJoinConfig.DelayDuration = 10f;
		features.QuickJoinConfig.QueueMultiplier = 1f;
		features.QuickJoinConfig.PlayerMultiplier = 1f;
		features.QuickJoinConfig.PlayerFriendValue = 5f;
		features.QuickJoinConfig.PlayerGuildValue = 1f;
		features.QuickJoinConfig.ThrottleDecayTime = 60f;
		features.QuickJoinConfig.ThrottlePrioritySpike = 20f;
		features.QuickJoinConfig.ThrottlePvPPriorityNormal = 50f;
		features.QuickJoinConfig.ThrottlePvPPriorityLow = 1f;
		features.QuickJoinConfig.ThrottlePvPHonorThreshold = 10f;
		features.QuickJoinConfig.ThrottleLfgListPriorityDefault = 50f;
		features.QuickJoinConfig.ThrottleLfgListPriorityAbove = 100f;
		features.QuickJoinConfig.ThrottleLfgListPriorityBelow = 50f;
		features.QuickJoinConfig.ThrottleLfgListIlvlScalingAbove = 1f;
		features.QuickJoinConfig.ThrottleLfgListIlvlScalingBelow = 1f;
		features.QuickJoinConfig.ThrottleRfPriorityAbove = 100f;
		features.QuickJoinConfig.ThrottleRfIlvlScalingAbove = 1f;
		features.QuickJoinConfig.ThrottleDfMaxItemLevel = 850f;
		features.QuickJoinConfig.ThrottleDfBestPriority = 80f;
		features.Squelch.IsSquelched = false;
		features.Squelch.BnetAccountGuid = WowGuid128.Create(HighGuidType703.BNetAccount, GetSession().AccountInfo.Id);
		features.Squelch.GuildGuid = WowGuid128.Empty;
		features.EuropaTicketSystemStatus.TicketsEnabled = true;
		features.EuropaTicketSystemStatus.BugsEnabled = true;
		features.EuropaTicketSystemStatus.ComplaintsEnabled = true;
		features.EuropaTicketSystemStatus.SuggestionsEnabled = true;
		features.EuropaTicketSystemStatus.ThrottleState.MaxTries = 10u;
		features.EuropaTicketSystemStatus.ThrottleState.PerMilliseconds = 60000u;
		features.EuropaTicketSystemStatus.ThrottleState.TryCount = 1u;
		features.EuropaTicketSystemStatus.ThrottleState.LastResetTimeBeforeNow = 10627480u;
		SendPacket(features);
	}

	public void SendSeasonInfo()
	{
		var seasonInfo = new SeasonInfo();
		if (LegacyVersion.ExpansionVersion > 1 && ModernVersion.ExpansionVersion > 1)
		{
			seasonInfo.CurrentSeason = 2;
			seasonInfo.PreviousSeason = 1;
		}
		var resolved = ModernVersion.GetCurrentOpcode(Opcode.SMSG_SEASON_INFO);
		Log.Print(LogType.Debug, $"SeasonInfo opcode resolved to: {resolved} (0x{resolved:X4})", "WorldSocket.cs");
		SendPacket(seasonInfo);
	}

	public void SendMotd()
	{
		var motd = new MOTD();
		SendPacket(motd);
	}

	public void SendClientCacheVersion(uint version)
	{
		var cache = new ClientCacheVersion
		{
			CacheVersion = version
		};
		SendPacket(cache);
	}

	public void SendAvailableHotfixes()
	{
		var hotfixes = new AvailableHotfixes
		{
			VirtualRealmAddress = GetSession().RealmId.GetAddress()
		};
		SendPacket(hotfixes);
	}

	public void SendBnetConnectionState(byte state)
	{
		var bnetConnected = new ConnectionStatus
		{
			State = state
		};
		SendPacket(bnetConnected);
	}

	public void SendServerTimeOffset()
	{
		var response = new ServerTimeOffset
		{
			Time = Time.UnixTime
		};
		SendPacket(response);
	}

	public void SendSocialContractRequestResponse()
	{
		var response = new SocialContractRequestResponse();
		SendPacket(response);
	}

	private void HandlePing(Ping ping)
	{
		SendPacket(new Pong(ping.Serial));
	}

	public void SendAccountDataTimes()
	{
		var guid = GetSession().GameState.CurrentPlayerGuid;
		GetSession().AccountDataMgr.LoadAllData(guid);
		var accountData = new AccountDataTimes
		{
			PlayerGuid = guid,
			ServerTime = Time.UnixTime
		};
		var count = ModernVersion.GetAccountDataCount();
		accountData.AccountTimes = new long[count];
		for (var i = 0; i < count; i++)
		{
			accountData.AccountTimes[i] = GetSession().AccountDataMgr.Data[i] != null ? GetSession().AccountDataMgr.Data[i].Timestamp : 0;
		}
		SendPacket(accountData);
	}

	public void SendRpcMessage(uint serviceId, OriginalHash service, uint methodId, uint token, BattlenetRpcErrorCode status, IMessage? message)
	{
		var methodInfo = default(MethodCall);
		methodInfo.SetServiceHash((uint)service);
		methodInfo.SetMethodId(methodId);
		methodInfo.Token = token;
		methodInfo.ObjectId = serviceId;
		var bytes = message == null ? Array.Empty<byte>() : message.ToByteArray();
		var response = new BattlenetResponse
		{
			Method = methodInfo,
			Status = status,
			Data = new ByteBuffer(bytes)
		};
		SendPacket(response);
	}

	public IPEndPoint GetRemoteIpEndPoint()
	{
		return GetRemoteIpAddress();
	}

	public void InitializePacketHandlers()
	{
		var methods = typeof(WorldSocket).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (var methodInfo in methods)
		{
			foreach (var msgAttr in methodInfo.GetCustomAttributes<PacketHandlerAttribute>())
			{
				if (msgAttr == null || msgAttr.Opcode == Opcode.MSG_NULL_ACTION)
				{
					continue;
				}
				if (_clientPacketTable.ContainsKey(msgAttr.Opcode))
				{
					Log.Print(LogType.Error, $"Tried to override OpcodeHandler of {_clientPacketTable[msgAttr.Opcode]} with {methodInfo.Name} (Opcode {msgAttr.Opcode})", "WorldSocket.cs");
				}
				else
				{
					var parameters = methodInfo.GetParameters();
					if (parameters.Length == 0)
					{
						Log.Print(LogType.Error, "Method: " + methodInfo.Name + " Has no paramters", "WorldSocket.cs");
					}
					else if (!typeof(ClientPacket).IsAssignableFrom(parameters[0].ParameterType))
					{
						Log.Print(LogType.Error, "Method: " + methodInfo.Name + " has wrong BaseType", "WorldSocket.cs");
					}
					else
					{
						_clientPacketTable[msgAttr.Opcode] = new PacketHandler(methodInfo, parameters[0].ParameterType);
					}
				}
			}
		}
	}
}
