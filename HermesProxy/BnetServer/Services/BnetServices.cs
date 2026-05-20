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
			this.Requirement = requirement;
			this.RequestType = parameters[0].ParameterType;
			if (parameters.Length > 1)
			{
				this.ResponseType = parameters[1].ParameterType;
			}
			this.MethodCaller = info.CreateDelegate((this.ResponseType != null) ? Expression.GetDelegateType(typeof(BnetServices), this.RequestType, this.ResponseType, info.ReturnType) : Expression.GetDelegateType(typeof(BnetServices), this.RequestType, info.ReturnType));
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
			ServiceManager._serviceHandlers = new ConcurrentDictionary<(OriginalHash, uint), BnetServiceHandlerInfo>();
			Assembly currentAsm = Assembly.GetExecutingAssembly();
			Type[] types = currentAsm.GetTypes();
			foreach (Type type in types)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					foreach (ServiceAttribute serviceAttr in methodInfo.GetCustomAttributes<ServiceAttribute>())
					{
						if (serviceAttr == null)
						{
							continue;
						}
						(OriginalHash, uint) key = (serviceAttr.ServiceHash, serviceAttr.MethodId);
						if (ServiceManager._serviceHandlers.ContainsKey(key))
						{
							Log.Print(LogType.Error, $"Tried to override ServiceHandler: {ServiceManager._serviceHandlers[key]} with {methodInfo.Name} (ServiceHash: {serviceAttr.ServiceHash} MethodId: {serviceAttr.MethodId})", ".cctor", "BnetServices.ServiceManager.cs");
						}
						else
						{
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length == 0)
							{
								Log.Print(LogType.Error, "Method: " + methodInfo.Name + " needs atleast one parameter", ".cctor", "BnetServices.ServiceManager.cs");
							}
							else
							{
								ServiceManager._serviceHandlers[key] = new BnetServiceHandlerInfo(serviceAttr.Requirement, methodInfo, parameters);
							}
						}
					}
				}
			}
		}

		public ServiceManager(string connectionPath, INetwork net, GlobalSessionData? initialSession)
		{
			this._serviceHolder = new BnetServices(connectionPath, net, initialSession);
		}

		public void SetClientSecret(byte[] key)
		{
			for (int i = 0; i < Math.Min(this._serviceHolder._clientSecret.Length, key.Length); i++)
			{
				this._serviceHolder._clientSecret[i] = key[i];
			}
		}

		public void Invoke(uint serviceId, OriginalHash serviceHash, uint methodId, uint requestToken, CodedInputStream stream)
		{
			if (!ServiceManager._serviceHandlers.TryGetValue((serviceHash, methodId), out var handler))
			{
				this._serviceHolder.ServiceLog(LogType.Warn, $"Client requested service {serviceHash}/m:{methodId} but this service is not implemented - sending OK stub");
				SendResponse(null);
				return;
			}
			if (handler.Requirement != ServiceRequirement.Always && handler.Requirement != this._serviceHolder.CurrentMatchingRequirement())
			{
				this._serviceHolder.ServiceLog(LogType.Warn, $"Client requested service {serviceHash}/m:{methodId} but with invalid state, required: {handler.Requirement} but only has {this._serviceHolder.CurrentMatchingRequirement()}!");
				SendErrorResponse(BattlenetRpcErrorCode.Denied);
				return;
			}
			this._serviceHolder.ServiceLog(LogType.Debug, $"Client requested service {serviceHash}/m:{methodId}");
			IMessage request = (IMessage)Activator.CreateInstance(handler.RequestType);
			request.MergeFrom(stream);
			if (handler.ResponseType != null)
			{
				IMessage response = (IMessage)Activator.CreateInstance(handler.ResponseType);
				BattlenetRpcErrorCode status = (BattlenetRpcErrorCode)handler.MethodCaller.DynamicInvoke(this._serviceHolder, request, response);
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
				BattlenetRpcErrorCode status = (BattlenetRpcErrorCode)handler.MethodCaller.DynamicInvoke(this._serviceHolder, request);
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
				if (this._serviceHolder._connectionPath == "WorldSocket")
				{
					this._serviceHolder._net.SendRpcMessage(serviceId, serviceHash, methodId, requestToken, status2, message);
				}
				else
				{
					this._serviceHolder._net.SendRpcMessage(254u, serviceHash, methodId, requestToken, status2, message);
				}
			}
		}
	}

	private static uint _serverInvokedRequestToken;

	private Dictionary<uint, Action<CodedInputStream>> _callbackHandlers = new Dictionary<uint, Action<CodedInputStream>>();

	private GlobalSessionData _globalSession;

	private readonly byte[] _clientSecret = new byte[32];

	private readonly string _connectionPath;

	private readonly INetwork _net;

	public GlobalSessionData Session => this._globalSession;

	private BnetServices()
	{
	}

	private BnetServices(string connectionPath, INetwork net, GlobalSessionData? initialSession)
	{
		this._connectionPath = connectionPath;
		this._net = net;
		this._globalSession = initialSession;
	}

	public GlobalSessionData GetSession()
	{
		return this._globalSession;
	}

	private void SendRequest(OriginalHash service, uint methodId, IMessage? data)
	{
		BnetServices._serverInvokedRequestToken++;
		this._net.SendRpcMessage(0u, service, methodId, BnetServices._serverInvokedRequestToken, BattlenetRpcErrorCode.Ok, data);
	}

	private void CloseSocket()
	{
		this._net.CloseSocket();
	}

	private IPEndPoint GetRemoteIpEndPoint()
	{
		return this._net.GetRemoteIpEndPoint();
	}

	private void ServiceLog(LogType type, string message)
	{
		StringBuilder prefix = new StringBuilder();
		StringBuilder stringBuilder = prefix;
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
		handler.AppendLiteral("[");
		handler.AppendFormatted(this._connectionPath);
		handler.AppendLiteral("]");
		stringBuilder2.Append(ref handler);
		stringBuilder = prefix;
		StringBuilder stringBuilder3 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder);
		handler.AppendLiteral("[");
		handler.AppendFormatted(this.GetRemoteIpEndPoint());
		stringBuilder3.Append(ref handler);
		if (this.GetSession() != null)
		{
			if (this.GetSession().AccountInfo != null && !this.GetSession().AccountInfo.Login.IsEmpty())
			{
				prefix.Append(", Account: " + this.GetSession().AccountInfo.Login);
			}
			if (this.GetSession().GameAccountInfo != null)
			{
				prefix.Append(", Game account: " + this.GetSession().GameAccountInfo.Name);
			}
		}
		prefix.Append(']');
		Log.Print(type, $"{prefix} {message}", "ServiceLog", "BnetServices.cs");
	}

	public ServiceRequirement CurrentMatchingRequirement()
	{
		return (this._globalSession != null) ? ServiceRequirement.LoggedIn : ServiceRequirement.Unauthorized;
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.AccountService, 30u)]
	private BattlenetRpcErrorCode HandleGetAccountState(GetAccountStateRequest request, GetAccountStateResponse response)
	{
		if (request.Options.FieldPrivacyInfo)
		{
			response.State = new AccountState();
			response.State.PrivacyInfo = new PrivacyInfo();
			response.State.PrivacyInfo.IsUsingRid = false;
			response.State.PrivacyInfo.IsVisibleForViewFriends = false;
			response.State.PrivacyInfo.IsHiddenFromFriendFinder = true;
			response.Tags = new AccountFieldTags();
			response.Tags.PrivacyInfoTag = 3620373325u;
		}
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.AccountService, 31u)]
	private BattlenetRpcErrorCode HandleGetGameAccountState(GetGameAccountStateRequest request, GetGameAccountStateResponse response)
	{
		if (request.Options.FieldGameLevelInfo)
		{
			GameAccountInfo gameAccountInfo = this.GetSession().AccountInfo.GameAccounts.LookupByKey(request.GameAccountId.Low);
			if (gameAccountInfo != null)
			{
				response.State = new GameAccountState();
				response.State.GameLevelInfo = new GameLevelInfo();
				response.State.GameLevelInfo.Name = gameAccountInfo.DisplayName;
				response.State.GameLevelInfo.Program = 5730135u;
			}
			response.Tags = new GameAccountFieldTags();
			response.Tags.GameLevelInfoTag = 1548145795u;
		}
		if (request.Options.FieldGameStatus)
		{
			if (response.State == null)
			{
				response.State = new GameAccountState();
			}
			response.State.GameStatus = new GameStatus();
			GameAccountInfo gameAccountInfo2 = this.GetSession().AccountInfo.GameAccounts.LookupByKey(request.GameAccountId.Low);
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
			this.ServiceLog(LogType.Error, "Battlenet.LogonRequest: Attempted to log in with game other than WoW (using " + logonRequest.Program + ")!");
			return BattlenetRpcErrorCode.BadProgram;
		}
		if (logonRequest.ApplicationVersion != ModernVersion.BuildInt)
		{
			this.ServiceLog(LogType.Error, $"Battlenet.LogonRequest: Attempted to log in with wrong game version (using {logonRequest.ApplicationVersion})!");
			return BattlenetRpcErrorCode.BadVersion;
		}
		if (logonRequest.Platform != "Win" && logonRequest.Platform != "Wn64" && logonRequest.Platform != "Mc64" && logonRequest.Platform != "MacA")
		{
			this.ServiceLog(LogType.Error, "Battlenet.LogonRequest: Attempted to log in from an unsupported platform (using " + logonRequest.Platform + ")!");
			return BattlenetRpcErrorCode.BadPlatform;
		}
		if (!LocaleChecker.IsValidLocale(logonRequest.Locale.ToEnum<Locale>()))
		{
			this.ServiceLog(LogType.Error, "Battlenet.LogonRequest: Attempted to log in with unsupported locale (using " + logonRequest.Locale + ")!");
			return BattlenetRpcErrorCode.BadLocale;
		}
		IPEndPoint endpoint = Singleton<LoginServiceManager>.Instance.GetAddressForClient(this.GetRemoteIpEndPoint().Address);
		ChallengeExternalRequest externalChallenge = new ChallengeExternalRequest();
		externalChallenge.PayloadType = "web_auth_url";
		externalChallenge.Payload = ByteString.CopyFromUtf8($"https://{endpoint.Address}:{endpoint.Port}/bnetserver/login/{logonRequest.Platform}/{logonRequest.ApplicationVersion}/{logonRequest.Locale}/");
		this.SendRequest(OriginalHash.ChallengeListener, 3u, externalChallenge);
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
				this.ServiceLog(LogType.Debug, "Session.HandleVerifyWebCredentials: Banned account " + tmpSession.AccountInfo.Login + " tried to login!");
				return BattlenetRpcErrorCode.GameAccountBanned;
			}
			this.ServiceLog(LogType.Debug, "Session.HandleVerifyWebCredentials: Temporarily banned account " + tmpSession.AccountInfo.Login + " tried to login!");
			return BattlenetRpcErrorCode.GameAccountSuspended;
		}
		Bgs.Protocol.Authentication.V1.LogonResult logonResult = new Bgs.Protocol.Authentication.V1.LogonResult();
		logonResult.ErrorCode = 0u;
		logonResult.AccountId = new EntityId();
		logonResult.AccountId.Low = tmpSession.AccountInfo.Id;
		logonResult.AccountId.High = 72057594037927936uL;
		foreach (GameAccountInfo gameAccount in tmpSession.AccountInfo.GameAccounts.Values)
		{
			EntityId gameAccountId = new EntityId();
			gameAccountId.Low = gameAccount.Id;
			gameAccountId.High = 144115196671520593uL;
			logonResult.GameAccountId.Add(gameAccountId);
		}
		tmpSession.SessionKey = new byte[64].GenerateRandomKey(64);
		logonResult.SessionKey = ByteString.CopyFrom(tmpSession.SessionKey);
		this._globalSession = tmpSession;
		this.SendRequest(OriginalHash.AuthenticationListener, 5u, logonResult);
		return BattlenetRpcErrorCode.Ok;
	}

	[Service(ServiceRequirement.Unauthorized, OriginalHash.ConnectionService, 1u)]
	private BattlenetRpcErrorCode HandleConnect(ConnectRequest request, ConnectResponse response)
	{
		if (request.ClientId != null)
		{
			response.ClientId.MergeFrom(request.ClientId);
		}
		response.ServerId = new ProcessId();
		response.ServerId.Label = (uint)Environment.ProcessId;
		response.ServerId.Epoch = (uint)Time.UnixTime;
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
		if (this.GetSession() != null && this.GetSession().AuthClient != null)
		{
			this.GetSession().AuthClient.Disconnect();
		}
		DisconnectNotification disconnectNotification = new DisconnectNotification();
		disconnectNotification.ErrorCode = request.ErrorCode;
		this.SendRequest(OriginalHash.ConnectionService, 4u, disconnectNotification);
		this.CloseSocket();
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
		Bgs.Protocol.Attribute command = null;
		Dictionary<string, Variant> Params = new Dictionary<string, Variant>();
		for (int i = 0; i < request.Attribute.Count; i++)
		{
			Bgs.Protocol.Attribute attr = request.Attribute[i];
			Params[attr.Name] = attr.Value;
			if (attr.Name.Contains("Command_"))
			{
				command = attr;
			}
		}
		if (command == null)
		{
			this.ServiceLog(LogType.Error, "Sent ClientRequest with no command.");
			return BattlenetRpcErrorCode.RpcMalformedRequest;
		}
		this.ServiceLog(LogType.Debug, "GameUtilitiesService method: " + command.Name);
		if (command.Name == "Command_RealmListTicketRequest_v1_" + this.GetCommandEndingForVersion())
		{
			return this.GetRealmListTicket(Params, response);
		}
		if (command.Name == "Command_LastCharPlayedRequest_v1_" + this.GetCommandEndingForVersion())
		{
			return this.GetLastCharPlayed(Params, response);
		}
		if (command.Name == "Command_RealmListRequest_v1_" + this.GetCommandEndingForVersion())
		{
			return this.GetRealmList(Params, response);
		}
		if (command.Name == "Command_RealmJoinRequest_v1_" + this.GetCommandEndingForVersion())
		{
			return this.JoinRealm(Params, response);
		}
		this.ServiceLog(LogType.Warn, "Sent unhandled command '" + command.Name + "'.");
		return BattlenetRpcErrorCode.RpcNotImplemented;
	}

	[Service(ServiceRequirement.LoggedIn, OriginalHash.GameUtilitiesService, 10u)]
	private BattlenetRpcErrorCode HandleGetAllValuesForAttribute(GetAllValuesForAttributeRequest request, GetAllValuesForAttributeResponse response)
	{
		if (request.AttributeKey == "Command_RealmListRequest_v1_" + this.GetCommandEndingForVersion())
		{
			this.GetSession().AuthClient.WaitOrRequestRealmList();
			this.GetSession().RealmManager.WriteSubRegions(response);
			return BattlenetRpcErrorCode.Ok;
		}
		return BattlenetRpcErrorCode.RpcNotImplemented;
	}

	private BattlenetRpcErrorCode GetRealmListTicket(Dictionary<string, Variant> Params, ClientResponse response)
	{
		Variant identity = Params.LookupByKey("Param_Identity");
		if (identity != null)
		{
			RealmListTicketIdentity realmListTicketIdentity = Json.CreateObject<RealmListTicketIdentity>(identity.BlobValue.ToStringUtf8(), split: true);
			GameAccountInfo gameAccount = this.GetSession().AccountInfo.GameAccounts.LookupByKey(realmListTicketIdentity.GameAccountId);
			if (gameAccount != null)
			{
				this.GetSession().GameAccountInfo = gameAccount;
			}
		}
		if (this.GetSession().GameAccountInfo == null)
		{
			return BattlenetRpcErrorCode.UtilServerInvalidIdentityArgs;
		}
		if (this.GetSession().GameAccountInfo.IsPermanenetlyBanned)
		{
			return BattlenetRpcErrorCode.GameAccountBanned;
		}
		if (this.GetSession().GameAccountInfo.IsBanned)
		{
			return BattlenetRpcErrorCode.GameAccountSuspended;
		}
		bool clientInfoOk = false;
		Variant clientInfo = Params.LookupByKey("Param_ClientInfo");
		if (clientInfo != null)
		{
			RealmListTicketClientInformation realmListTicketClientInformation = Json.CreateObject<RealmListTicketClientInformation>(clientInfo.BlobValue.ToStringUtf8(), split: true);
			clientInfoOk = true;
			for (int i = 0; i < Math.Min(this._clientSecret.Length, realmListTicketClientInformation.Info.Secret.Count); i++)
			{
				this._clientSecret[i] = (byte)realmListTicketClientInformation.Info.Secret[i];
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
		Variant subRegion = Params.LookupByKey("Command_LastCharPlayedRequest_v1_" + this.GetCommandEndingForVersion());
		if (subRegion == null)
		{
			return BattlenetRpcErrorCode.UtilServerUnknownRealm;
		}
		(string, string, ulong, long)? rawLastPlayedChar = this.GetSession().AccountMetaDataMgr.GetLastSelectedCharacter();
		if (!rawLastPlayedChar.HasValue)
		{
			return BattlenetRpcErrorCode.Ok;
		}
		(string realmName, string charName, ulong charLowerGuid, long lastLoginUnixSec) lastPlayedChar = rawLastPlayedChar.Value;
		this.GetSession().AuthClient.WaitOrRequestRealmList();
		Realm realm = this.GetSession().RealmManager.GetRealms().FirstOrDefault((Realm r) => r.Name == lastPlayedChar.realmName && !r.Flags.HasFlag(RealmFlags.Offline));
		if (realm == null)
		{
			return BattlenetRpcErrorCode.UtilServerFailedToSerializeResponse;
		}
		byte[] compressedRealmEntry = this.GetSession().RealmManager.GetCompressdRealmEntryJSON(realm, this.GetSession().Build);
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
		if (this.GetSession().GameAccountInfo == null)
		{
			return BattlenetRpcErrorCode.UserServerBadWowAccount;
		}
		if (!this.GetSession().AuthClient.IsConnected())
		{
			return BattlenetRpcErrorCode.UtilServerMissingRealmList;
		}
		string subRegionId = "";
		Variant subRegion = Params.LookupByKey("Command_RealmListRequest_v1_" + this.GetCommandEndingForVersion());
		if (subRegion != null)
		{
			subRegionId = subRegion.StringValue;
		}
		byte[] compressedRealmList = this.GetSession().RealmManager.GetRealmList(this.GetSession().Build, subRegionId);
		if (compressedRealmList.Length == 0)
		{
			return BattlenetRpcErrorCode.UtilServerFailedToSerializeResponse;
		}
		response.Attribute.AddBlob("Param_RealmList", ByteString.CopyFrom(compressedRealmList));
		RealmCharacterCountList realmCharacterCounts = new RealmCharacterCountList();
		foreach (Realm realm in this.GetSession().RealmManager.GetRealms())
		{
			RealmCharacterCountEntry countEntry = new RealmCharacterCountEntry();
			countEntry.WowRealmAddress = (int)realm.Id.GetAddress();
			countEntry.Count = realm.CharacterCount;
			realmCharacterCounts.Counts.Add(countEntry);
		}
		byte[] compressedCharCount = Json.Deflate("JSONRealmCharacterCountList", realmCharacterCounts);
		response.Attribute.AddBlob("Param_CharacterCountList", ByteString.CopyFrom(compressedCharCount));
		return BattlenetRpcErrorCode.Ok;
	}

	private BattlenetRpcErrorCode JoinRealm(Dictionary<string, Variant> Params, ClientResponse response)
	{
		Variant realmAddress = Params.LookupByKey("Param_RealmAddress");
		if (realmAddress == null)
		{
			return BattlenetRpcErrorCode.WowServicesInvalidJoinTicket;
		}
		return this.GetSession().RealmManager.JoinRealm(this.GetSession(), (uint)realmAddress.UintValue, this.GetSession().Build, this.GetRemoteIpEndPoint().Address, this._clientSecret, this.GetSession().GameAccountInfo.Name, response);
	}
}
