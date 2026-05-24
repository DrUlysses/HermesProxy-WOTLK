using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using Bgs.Protocol;
using Bgs.Protocol.Account.V1;
using Bgs.Protocol.Authentication.V1;
using Bgs.Protocol.Challenge.V1;
using Bgs.Protocol.Connection.V1;
using Bgs.Protocol.GameUtilities.V1;
using BNetServer.Networking;
using Framework.Constants;
using Framework.Logging;
using Framework.Serialization;
using Framework.Util;
using Framework.Web;
using Google.Protobuf;
using HermesProxy;
using Attribute = Bgs.Protocol.Attribute;
using LogonResult = Bgs.Protocol.Authentication.V1.LogonResult;

namespace BNetServer.Services;

public class BnetServices
{
	public class BnetServiceHandlerInfo
	{
		public readonly ServiceRequirement Requirement;

		public readonly Delegate MethodCaller;

		public readonly Type RequestType;

		public readonly Type ResponseType;

		public BnetServiceHandlerInfo(ServiceRequirement requirement, MethodInfo info, ParameterInfo[] parameters)
		{
			Requirement = requirement;
			RequestType = parameters[0].ParameterType;
			if (parameters.Length > 1)
			{
				ResponseType = parameters[1].ParameterType;
			}
			MethodCaller = info.CreateDelegate(ResponseType != null ? Expression.GetDelegateType(typeof(BnetServices), RequestType, ResponseType, info.ReturnType) : Expression.GetDelegateType(typeof(BnetServices), RequestType, info.ReturnType));
		}
	}

	public interface INetwork
	{
		void SendRpcMessage(uint serviceId, OriginalHash service, uint methodId, uint token, BattlenetRpcErrorCode status, IMessage? message);

		void CloseSocket();

		IPEndPoint GetRemoteIpEndPoint();
	}

	public class ServiceManager
	{
		private static readonly ConcurrentDictionary<(OriginalHash Service, uint MethodId), BnetServiceHandlerInfo> _serviceHandlers;

		private readonly BnetServices _serviceHolder;

		static ServiceManager()
		{
			_serviceHandlers = new ConcurrentDictionary<(OriginalHash, uint), BnetServiceHandlerInfo>();
			var currentAsm = Assembly.GetExecutingAssembly();
			var types = currentAsm.GetTypes();
			foreach (var type in types)
			{
				var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
				foreach (var methodInfo in methods)
				{
					foreach (var serviceAttr in methodInfo.GetCustomAttributes<ServiceAttribute>())
					{
						if (serviceAttr == null)
						{
							continue;
						}
						(OriginalHash, uint) key = (serviceAttr.ServiceHash, serviceAttr.MethodId);
						if (_serviceHandlers.ContainsKey(key))
						{
							Log.Print(LogType.Error, $"Tried to override ServiceHandler: {_serviceHandlers[key]} with {methodInfo.Name} (ServiceHash: {serviceAttr.ServiceHash} MethodId: {serviceAttr.MethodId})", ".cctor", "BnetServices.ServiceManager.cs");
						}
						else
						{
							var parameters = methodInfo.GetParameters();
							if (parameters.Length == 0)
							{
								Log.Print(LogType.Error, "Method: " + methodInfo.Name + " needs atleast one parameter", ".cctor", "BnetServices.ServiceManager.cs");
							}
							else
							{
								_serviceHandlers[key] = new BnetServiceHandlerInfo(serviceAttr.Requirement, methodInfo, parameters);
							}
						}
					}
				}
			}
		}

		public ServiceManager(string connectionPath, INetwork net, GlobalSessionData? initialSession)
		{
			_serviceHolder = new BnetServices(connectionPath, net, initialSession);
		}

		public void SetClientSecret(byte[] key)
		{
			for (var i = 0; i < Math.Min(_serviceHolder._clientSecret.Length, key.Length); i++)
			{
				_serviceHolder._clientSecret[i] = key[i];
			}
		}

		public void Invoke(uint serviceId, OriginalHash serviceHash, uint methodId, uint requestToken, CodedInputStream stream)
		{
			if (!_serviceHandlers.TryGetValue((serviceHash, methodId), out var handler))
			{
				_serviceHolder.ServiceLog(LogType.Warn, $"Client requested service {serviceHash}/m:{methodId} but this service is not implemented - sending OK stub");
				SendResponse(null);
				return;
			}
			if (handler.Requirement != ServiceRequirement.Always && handler.Requirement != _serviceHolder.CurrentMatchingRequirement())
			{
				_serviceHolder.ServiceLog(LogType.Warn, $"Client requested service {serviceHash}/m:{methodId} but with invalid state, required: {handler.Requirement} but only has {_serviceHolder.CurrentMatchingRequirement()}!");
				SendErrorResponse(BattlenetRpcErrorCode.Denied);
				return;
			}
			_serviceHolder.ServiceLog(LogType.Debug, $"Client requested service {serviceHash}/m:{methodId}");
			var request = (IMessage)Activator.CreateInstance(handler.RequestType);
			request.MergeFrom(stream);
			if (handler.ResponseType != null)
			{
				var response = (IMessage)Activator.CreateInstance(handler.ResponseType);
				var status = (BattlenetRpcErrorCode)handler.MethodCaller.DynamicInvoke(_serviceHolder, request, response);
				if (status == BattlenetRpcErrorCode.Ok)
				{
					SendResponse(response);
				}
				else
				{
					SendErrorResponse(status);
				}
			}
			else
			{
				var status = (BattlenetRpcErrorCode)handler.MethodCaller.DynamicInvoke(_serviceHolder, request);
				if (status != BattlenetRpcErrorCode.Ok)
				{
					SendErrorResponse(status);
				}
			}
			void SendErrorResponse(BattlenetRpcErrorCode errorCode)
			{
				SendRpcMessage(errorCode, null);
			}
			void SendResponse(IMessage message)
			{
				SendRpcMessage(BattlenetRpcErrorCode.Ok, message);
			}
			void SendRpcMessage(BattlenetRpcErrorCode status2, IMessage? message)
			{
				if (_serviceHolder._connectionPath == "WorldSocket")
				{
					_serviceHolder._net.SendRpcMessage(serviceId, serviceHash, methodId, requestToken, status2, message);
				}
				else
				{
					_serviceHolder._net.SendRpcMessage(254u, serviceHash, methodId, requestToken, status2, message);
				}
			}
		}
	}

	private static uint _serverInvokedRequestToken;

	private Dictionary<uint, Action<CodedInputStream>> _callbackHandlers = new();

	private GlobalSessionData _globalSession;

	private readonly byte[] _clientSecret = new byte[32];

	private readonly string _connectionPath;

	private readonly INetwork _net;

	public GlobalSessionData Session => _globalSession;

	private BnetServices()
	{
	}

	private BnetServices(string connectionPath, INetwork net, GlobalSessionData? initialSession)
	{
		_connectionPath = connectionPath;
		_net = net;
		_globalSession = initialSession;
	}

	public GlobalSessionData GetSession()
	{
		return _globalSession;
	}

	private void SendRequest(OriginalHash service, uint methodId, IMessage? data)
	{
		_serverInvokedRequestToken++;
		_net.SendRpcMessage(0u, service, methodId, _serverInvokedRequestToken, BattlenetRpcErrorCode.Ok, data);
	}

	private void CloseSocket()
	{
		_net.CloseSocket();
	}

	private IPEndPoint GetRemoteIpEndPoint()
	{
		return _net.GetRemoteIpEndPoint();
	}

	private void ServiceLog(LogType type, string message)
	{
		var prefix = new StringBuilder();
		var stringBuilder = prefix;
		var stringBuilder2 = stringBuilder;
		var handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
		handler.AppendLiteral("[");
		handler.AppendFormatted(_connectionPath);
		handler.AppendLiteral("]");
		stringBuilder2.Append(ref handler);
		stringBuilder = prefix;
		var stringBuilder3 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder);
		handler.AppendLiteral("[");
		handler.AppendFormatted(GetRemoteIpEndPoint());
		stringBuilder3.Append(ref handler);
		if (GetSession() != null)
		{
			if (GetSession().AccountInfo != null && !GetSession().AccountInfo.Login.IsEmpty())
			{
				prefix.Append(", Account: " + GetSession().AccountInfo.Login);
			}
			if (GetSession().GameAccountInfo != null)
			{
				prefix.Append(", Game account: " + GetSession().GameAccountInfo.Name);
			}
		}
		prefix.Append(']');
		Log.Print(type, $"{prefix} {message}", "BnetServices.cs");
	}

	public ServiceRequirement CurrentMatchingRequirement()
	{
		return _globalSession != null ? ServiceRequirement.LoggedIn : ServiceRequirement.Unauthorized;
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.AccountService, 30u)]
	private BattlenetRpcErrorCode HandleGetAccountState(GetAccountStateRequest request, GetAccountStateResponse response)
	{
		if (request.Options.FieldPrivacyInfo)
		{
			response.State = new AccountState
			{
				PrivacyInfo = new PrivacyInfo
				{
					IsUsingRid = false,
					IsVisibleForViewFriends = false,
					IsHiddenFromFriendFinder = true
				}
			};
			response.Tags = new AccountFieldTags
			{
				PrivacyInfoTag = 3620373325u
			};
		}
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.AccountService, 31u)]
	private BattlenetRpcErrorCode HandleGetGameAccountState(GetGameAccountStateRequest request, GetGameAccountStateResponse response)
	{
		if (request.Options.FieldGameLevelInfo)
		{
			var gameAccountInfo = GetSession().AccountInfo.GameAccounts.LookupByKey(request.GameAccountId.Low);
			if (gameAccountInfo != null)
			{
				response.State = new GameAccountState
				{
					GameLevelInfo = new GameLevelInfo
					{
						Name = gameAccountInfo.DisplayName,
						Program = 5730135u
					}
				};
			}
			response.Tags = new GameAccountFieldTags
			{
				GameLevelInfoTag = 1548145795u
			};
		}
		if (request.Options.FieldGameStatus)
		{
			if (response.State == null)
			{
				response.State = new GameAccountState();
			}
			response.State.GameStatus = new GameStatus();
			var gameAccountInfo2 = GetSession().AccountInfo.GameAccounts.LookupByKey(request.GameAccountId.Low);
			if (gameAccountInfo2 != null)
			{
				response.State.GameStatus.IsSuspended = gameAccountInfo2.IsBanned;
				response.State.GameStatus.IsBanned = gameAccountInfo2.IsPermanenetlyBanned;
				response.State.GameStatus.SuspensionExpires = gameAccountInfo2.UnbanDate * 1000000;
			}
			response.State.GameStatus.Program = 5730135u;
			response.Tags.GameStatusTag = 2562154393u;
		}
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.Unauthorized, OriginalHash.AuthenticationService, 1u)]
	private BattlenetRpcErrorCode HandleLogon(LogonRequest logonRequest, NoData response)
	{
		if (logonRequest.Program != "WoW")
		{
			ServiceLog(LogType.Error, "Battlenet.LogonRequest: Attempted to log in with game other than WoW (using " + logonRequest.Program + ")!");
			return BattlenetRpcErrorCode.BadProgram;
		}
		if (logonRequest.ApplicationVersion != ModernVersion.BuildInt)
		{
			ServiceLog(LogType.Error, $"Battlenet.LogonRequest: Attempted to log in with wrong game version (using {logonRequest.ApplicationVersion})!");
			return BattlenetRpcErrorCode.BadVersion;
		}
		if (logonRequest.Platform != "Win" && logonRequest.Platform != "Wn64" && logonRequest.Platform != "Mc64" && logonRequest.Platform != "MacA")
		{
			ServiceLog(LogType.Error, "Battlenet.LogonRequest: Attempted to log in from an unsupported platform (using " + logonRequest.Platform + ")!");
			return BattlenetRpcErrorCode.BadPlatform;
		}
		if (!LocaleChecker.IsValidLocale(logonRequest.Locale.ToEnum<Locale>()))
		{
			ServiceLog(LogType.Error, "Battlenet.LogonRequest: Attempted to log in with unsupported locale (using " + logonRequest.Locale + ")!");
			return BattlenetRpcErrorCode.BadLocale;
		}
		var endpoint = Singleton<LoginServiceManager>.Instance.GetAddressForClient(GetRemoteIpEndPoint().Address);
		var externalChallenge = new ChallengeExternalRequest
		{
			PayloadType = "web_auth_url",
			Payload = ByteString.CopyFromUtf8($"https://{endpoint.Address}:{endpoint.Port}/bnetserver/login/{logonRequest.Platform}/{logonRequest.ApplicationVersion}/{logonRequest.Locale}/")
		};
		SendRequest(OriginalHash.ChallengeListener, 3u, externalChallenge);
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.Unauthorized, OriginalHash.AuthenticationService, 7u)]
	private BattlenetRpcErrorCode HandleVerifyWebCredentials(VerifyWebCredentialsRequest verifyWebCredentialsRequest)
	{
		if (!BnetSessionTicketStorage.SessionsByTicket.TryGetValue(verifyWebCredentialsRequest.WebCredentials.ToStringUtf8(), out var tmpSession))
		{
			return BattlenetRpcErrorCode.Denied;
		}
		tmpSession.AccountInfo = new AccountInfo(tmpSession.Username);
		if (tmpSession.AccountInfo.LoginTicketExpiry < Time.UnixTime)
		{
			return BattlenetRpcErrorCode.TimedOut;
		}
		if (tmpSession.AccountInfo.IsBanned)
		{
			if (tmpSession.AccountInfo.IsPermanenetlyBanned)
			{
				ServiceLog(LogType.Debug, "Session.HandleVerifyWebCredentials: Banned account " + tmpSession.AccountInfo.Login + " tried to login!");
				return BattlenetRpcErrorCode.GameAccountBanned;
			}
			ServiceLog(LogType.Debug, "Session.HandleVerifyWebCredentials: Temporarily banned account " + tmpSession.AccountInfo.Login + " tried to login!");
			return BattlenetRpcErrorCode.GameAccountSuspended;
		}
		var logonResult = new LogonResult
		{
			ErrorCode = 0u,
			AccountId = new EntityId
			{
				Low = tmpSession.AccountInfo.Id,
				High = 72057594037927936uL
			}
		};
		foreach (var gameAccount in tmpSession.AccountInfo.GameAccounts.Values)
		{
			var gameAccountId = new EntityId
			{
				Low = gameAccount.Id,
				High = 144115196671520593uL
			};
			logonResult.GameAccountId.Add(gameAccountId);
		}
		tmpSession.SessionKey = new byte[64].GenerateRandomKey(64);
		logonResult.SessionKey = ByteString.CopyFrom(tmpSession.SessionKey);
		_globalSession = tmpSession;
		SendRequest(OriginalHash.AuthenticationListener, 5u, logonResult);
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.Unauthorized, OriginalHash.ConnectionService, 1u)]
	private BattlenetRpcErrorCode HandleConnect(ConnectRequest request, ConnectResponse response)
	{
		if (request.ClientId != null)
		{
			response.ClientId.MergeFrom(request.ClientId);
		}
		response.ServerId = new ProcessId
		{
			Label = (uint)Environment.ProcessId,
			Epoch = (uint)Time.UnixTime
		};
		response.ServerTime = (ulong)Time.UnixTimeMilliseconds;
		response.UseBindlessRpc = request.UseBindlessRpc;
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.Always, OriginalHash.ConnectionService, 5u)]
	private BattlenetRpcErrorCode HandleKeepAlive(NoData request)
	{
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.Always, OriginalHash.ConnectionService, 7u)]
	private BattlenetRpcErrorCode HandleRequestDisconnect(DisconnectRequest request)
	{
		if (GetSession() != null && GetSession().AuthClient != null)
		{
			GetSession().AuthClient.Disconnect();
		}
		var disconnectNotification = new DisconnectNotification
		{
			ErrorCode = request.ErrorCode
		};
		SendRequest(OriginalHash.ConnectionService, 4u, disconnectNotification);
		CloseSocket();
		return BattlenetRpcErrorCode.Ok;
	}

	private string GetCommandEndingForVersion()
	{
		if (ModernVersion.ExpansionVersion == 1)
		{
			return "c1";
		}
		if (ModernVersion.ExpansionVersion == 2)
		{
			return "bcc1";
		}
		if (ModernVersion.ExpansionVersion == 3)
		{
			return "wotlk1";
		}
		return "b9";
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.GameUtilitiesService, 1u)]
	private BattlenetRpcErrorCode HandleProcessClientRequest(ClientRequest request, ClientResponse response)
	{
		Attribute command = null;
		var Params = new Dictionary<string, Variant>();
		for (var i = 0; i < request.Attribute.Count; i++)
		{
			var attr = request.Attribute[i];
			Params[attr.Name] = attr.Value;
			if (attr.Name.Contains("Command_"))
			{
				command = attr;
			}
		}
		if (command == null)
		{
			ServiceLog(LogType.Error, "Sent ClientRequest with no command.");
			return BattlenetRpcErrorCode.RpcMalformedRequest;
		}
		ServiceLog(LogType.Debug, "GameUtilitiesService method: " + command.Name);
		if (command.Name == "Command_RealmListTicketRequest_v1_" + GetCommandEndingForVersion())
		{
			return GetRealmListTicket(Params, response);
		}
		if (command.Name == "Command_LastCharPlayedRequest_v1_" + GetCommandEndingForVersion())
		{
			return GetLastCharPlayed(Params, response);
		}
		if (command.Name == "Command_RealmListRequest_v1_" + GetCommandEndingForVersion())
		{
			return GetRealmList(Params, response);
		}
		if (command.Name == "Command_RealmJoinRequest_v1_" + GetCommandEndingForVersion())
		{
			return JoinRealm(Params, response);
		}
		ServiceLog(LogType.Warn, "Sent unhandled command '" + command.Name + "'.");
		return BattlenetRpcErrorCode.RpcNotImplemented;
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.GameUtilitiesService, 10u)]
	private BattlenetRpcErrorCode HandleGetAllValuesForAttribute(GetAllValuesForAttributeRequest request, GetAllValuesForAttributeResponse response)
	{
		if (request.AttributeKey == "Command_RealmListRequest_v1_" + GetCommandEndingForVersion())
		{
			GetSession().AuthClient.WaitOrRequestRealmList();
			GetSession().RealmManager.WriteSubRegions(response);
			return BattlenetRpcErrorCode.Ok;
		}
		return BattlenetRpcErrorCode.RpcNotImplemented;
	}

	private BattlenetRpcErrorCode GetRealmListTicket(Dictionary<string, Variant> Params, ClientResponse response)
	{
		var identity = Params.LookupByKey("Param_Identity");
		if (identity != null)
		{
			var realmListTicketIdentity = Json.CreateObject<RealmListTicketIdentity>(identity.BlobValue.ToStringUtf8(), split: true);
			var gameAccount = GetSession().AccountInfo.GameAccounts.LookupByKey(realmListTicketIdentity.GameAccountId);
			if (gameAccount != null)
			{
				GetSession().GameAccountInfo = gameAccount;
			}
		}
		if (GetSession().GameAccountInfo == null)
		{
			return BattlenetRpcErrorCode.UtilServerInvalidIdentityArgs;
		}
		if (GetSession().GameAccountInfo.IsPermanenetlyBanned)
		{
			return BattlenetRpcErrorCode.GameAccountBanned;
		}
		if (GetSession().GameAccountInfo.IsBanned)
		{
			return BattlenetRpcErrorCode.GameAccountSuspended;
		}
		var clientInfoOk = false;
		var clientInfo = Params.LookupByKey("Param_ClientInfo");
		if (clientInfo != null)
		{
			var realmListTicketClientInformation = Json.CreateObject<RealmListTicketClientInformation>(clientInfo.BlobValue.ToStringUtf8(), split: true);
			clientInfoOk = true;
			for (var i = 0; i < Math.Min(_clientSecret.Length, realmListTicketClientInformation.Info.Secret.Count); i++)
			{
				_clientSecret[i] = (byte)realmListTicketClientInformation.Info.Secret[i];
			}
		}
		if (!clientInfoOk)
		{
			return BattlenetRpcErrorCode.WowServicesDeniedRealmListTicket;
		}
		response.Attribute.AddBlob("Param_RealmListTicket", ByteString.CopyFrom("AuthRealmListTicket", Encoding.UTF8));
		return BattlenetRpcErrorCode.Ok;
	}

	private BattlenetRpcErrorCode GetLastCharPlayed(Dictionary<string, Variant> Params, ClientResponse response)
	{
		var subRegion = Params.LookupByKey("Command_LastCharPlayedRequest_v1_" + GetCommandEndingForVersion());
		if (subRegion == null)
		{
			return BattlenetRpcErrorCode.UtilServerUnknownRealm;
		}
		(string, string, ulong, long)? rawLastPlayedChar = GetSession().AccountMetaDataMgr.GetLastSelectedCharacter();
		if (!rawLastPlayedChar.HasValue)
		{
			return BattlenetRpcErrorCode.Ok;
		}
		(string realmName, string charName, ulong charLowerGuid, long lastLoginUnixSec) lastPlayedChar = rawLastPlayedChar.Value;
		GetSession().AuthClient.WaitOrRequestRealmList();
		var realm = GetSession().RealmManager.GetRealms().FirstOrDefault(r => r.Name == lastPlayedChar.realmName && !r.Flags.HasFlag(RealmFlags.Offline));
		if (realm == null)
		{
			return BattlenetRpcErrorCode.UtilServerFailedToSerializeResponse;
		}
		var compressedRealmEntry = GetSession().RealmManager.GetCompressdRealmEntryJSON(realm, GetSession().Build);
		if (compressedRealmEntry.Length == 0)
		{
			return BattlenetRpcErrorCode.UtilServerFailedToSerializeResponse;
		}
		response.Attribute.AddBlob("Param_RealmEntry", ByteString.CopyFrom(compressedRealmEntry));
		response.Attribute.AddString("Param_CharacterName", lastPlayedChar.charName);
		response.Attribute.AddBlob("Param_CharacterGUID", ByteString.CopyFrom(BitConverter.GetBytes(lastPlayedChar.charLowerGuid)));
		response.Attribute.AddInt("Param_LastPlayedTime", lastPlayedChar.lastLoginUnixSec);
		return BattlenetRpcErrorCode.Ok;
	}

	private BattlenetRpcErrorCode GetRealmList(Dictionary<string, Variant> Params, ClientResponse response)
	{
		if (GetSession().GameAccountInfo == null)
		{
			return BattlenetRpcErrorCode.UserServerBadWowAccount;
		}
		if (!GetSession().AuthClient.IsConnected())
		{
			return BattlenetRpcErrorCode.UtilServerMissingRealmList;
		}
		var subRegionId = "";
		var subRegion = Params.LookupByKey("Command_RealmListRequest_v1_" + GetCommandEndingForVersion());
		if (subRegion != null)
		{
			subRegionId = subRegion.StringValue;
		}
		var compressedRealmList = GetSession().RealmManager.GetRealmList(GetSession().Build, subRegionId);
		if (compressedRealmList.Length == 0)
		{
			return BattlenetRpcErrorCode.UtilServerFailedToSerializeResponse;
		}
		response.Attribute.AddBlob("Param_RealmList", ByteString.CopyFrom(compressedRealmList));
		var realmCharacterCounts = new RealmCharacterCountList();
		foreach (var realm in GetSession().RealmManager.GetRealms())
		{
			var countEntry = new RealmCharacterCountEntry
			{
				WowRealmAddress = (int)realm.Id.GetAddress(),
				Count = realm.CharacterCount
			};
			realmCharacterCounts.Counts.Add(countEntry);
		}
		var compressedCharCount = Json.Deflate("JSONRealmCharacterCountList", realmCharacterCounts);
		response.Attribute.AddBlob("Param_CharacterCountList", ByteString.CopyFrom(compressedCharCount));
		return BattlenetRpcErrorCode.Ok;
	}

	private BattlenetRpcErrorCode JoinRealm(Dictionary<string, Variant> Params, ClientResponse response)
	{
		var realmAddress = Params.LookupByKey("Param_RealmAddress");
		if (realmAddress == null)
		{
			return BattlenetRpcErrorCode.WowServicesInvalidJoinTicket;
		}
		return GetSession().RealmManager.JoinRealm(GetSession(), (uint)realmAddress.UintValue, GetSession().Build, GetRemoteIpEndPoint().Address, _clientSecret, GetSession().GameAccountInfo.Name, response);
	}
}
