using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BNetServer;
using Framework;
using Framework.Networking;
using Framework.Serialization;
using Framework.Web;
using HermesProxy.Auth;
using HermesProxy.Enums;
using HermesProxy.World.Server;

namespace HermesProxy.BnetServer.Networking;

public class BnetRestApiSession : SSLSocket
{
	private const string BNET_SERVER_BASE_PATH = "/bnetserver/";

	private const string TICKET_PREFIX = "HP-";

	public BnetRestApiSession(Socket socket)
		: base(socket)
	{
	}

	public override void Accept()
	{
		AsyncHandshake(BnetServerCertificate.Certificate).Wait();
	}

	public override async Task ReadHandler(byte[] data, int receivedLength)
	{
		var httpRequest = HttpHelper.ParseRequest(data, receivedLength);
		if (httpRequest == null || !RequestRouter(httpRequest))
		{
			CloseSocket();
		}
		else
		{
			await AsyncRead();
		}
	}

	private bool RequestRouter(HttpHeader httpRequest)
	{
		if (!httpRequest.Path.StartsWith("/bnetserver/"))
		{
			SendEmptyResponse(HttpCode.NotFound).Wait();
			return false;
		}
		var path = httpRequest.Path["/bnetserver/".Length..];
		var pathElements = path.Split('/');
		(string, string) tuple = (pathElements[0], httpRequest.Method);
		var tuple2 = tuple;
		var (text, _) = tuple2;
		if (text == "login")
		{
			var item = tuple2.Item2;
			switch (item)
			{
				case "GET":
					SendResponse(
						HttpCode.Ok,
						Singleton<LoginServiceManager>.Instance.GetFormInput()
					).Wait();
					return true;
				case "POST":
					HandleLoginRequest(pathElements, httpRequest);
					return true;
			}
		}
		SendEmptyResponse(HttpCode.NotFound).Wait();
		return false;
	}

	private void HandleLoginRequest(string[] pathElements, HttpHeader request)
	{
		var loginForm = Json.CreateObjectOrNull<LogonData>(request.Content);
		if (loginForm == null)
		{
			SendEmptyResponse(HttpCode.InternalServerError).Wait();
			return;
		}
		var globalSession = new GlobalSessionData
		{
			OS = pathElements[1],
			Build = uint.Parse(pathElements[2]),
			Locale = pathElements[3]
		};
		if (Settings.ClientBuild != (ClientVersionBuild)globalSession.Build)
		{
			SendAuthError(AuthResult.FAIL_WRONG_MODERN_VER).Wait();
			return;
		}
		var login = "";
		var password = "";
		foreach (var field in loginForm.Inputs)
		{
			var id = field.Id;
			var text = id;
			if (!(text == "account_name"))
			{
				if (text == "password")
				{
					password = field.Value;
				}
			}
			else
			{
				login = field.Value.Trim().ToUpperInvariant();
			}
		}
		globalSession.AuthClient = new AuthClient(globalSession);
		var response = globalSession.AuthClient.ConnectToAuthServer(login, password, globalSession.Locale);
		if (response != AuthResult.SUCCESS)
		{
			SendAuthError(response).Wait();
			return;
		}
		globalSession.AuthClient.SendRealmListUpdateRequest();
		var loginResult = new LogonResult();
		var ticket = Array.Empty<byte>().GenerateRandomKey(20);
		var loginTicket = globalSession.LoginTicket = "HP-" + ticket.ToHexString();
		globalSession.Username = login;
		globalSession.AccountMetaDataMgr = new AccountMetaDataManager(login);
		BnetSessionTicketStorage.AddNewSessionByName(login, globalSession);
		BnetSessionTicketStorage.AddNewSessionByTicket(loginTicket, globalSession);
		loginResult.LoginTicket = loginTicket;
		loginResult.AuthenticationState = "DONE";
		SendResponse(HttpCode.Ok, loginResult).Wait();
	}

	private async Task SendResponse<T>(HttpCode code, T response)
	{
		await AsyncWrite(HttpHelper.CreateResponse(code, Json.CreateString(response)));
	}

	private async Task SendAuthError(AuthResult response)
	{
		var loginResult = new LogonResult();
		var logonResult = loginResult;
		var logonResult2 = loginResult;
		var logonResult3 = loginResult;
		var tuple = response switch
		{
			AuthResult.FAIL_UNKNOWN_ACCOUNT => ("LOGIN", "UNABLE_TO_DECODE", "Invalid username or password."), 
			AuthResult.FAIL_INCORRECT_PASSWORD => ("LOGIN", "UNABLE_TO_DECODE", "Invalid password."), 
			AuthResult.FAIL_BANNED => ("LOGIN", "UNABLE_TO_DECODE", "This account has been closed and is no longer available for use."), 
			AuthResult.FAIL_SUSPENDED => ("LOGIN", "UNABLE_TO_DECODE", "This account has been temporarily suspended."), 
			AuthResult.FAIL_VERSION_INVALID => ("LOGIN", "UNABLE_TO_DECODE", "Your version is not supported by this server.\nMake sure you are using the latest HermesProxy version from GitHub.\n(Maybe HermesProxy is blocked on the server)\n"), 
			AuthResult.FAIL_INTERNAL_ERROR => ("LOGON", "UNABLE_TO_DECODE", "There was an internal error. Please try again later."), 
			_ => ("LOGON", "UNABLE_TO_DECODE", $"Error: {response}"), 
		};
		(logonResult.AuthenticationState, logonResult2.ErrorCode, logonResult3.ErrorMessage) = tuple;
		await SendResponse(HttpCode.BadRequest, loginResult);
	}

	private async Task SendEmptyResponse(HttpCode code)
	{
		await SendResponse<object>(code, new { });
	}
}
