using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Framework;
using Framework.Networking;
using Framework.Serialization;
using Framework.Web;
using HermesProxy;
using HermesProxy.Auth;
using HermesProxy.Enums;
using HermesProxy.World.Server;

namespace BNetServer.Networking;

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
		AsyncHandshake(BnetServerCertificate.Certificate);
	}

	public override async Task ReadHandler(byte[] data, int receivedLength)
	{
		HttpHeader httpRequest = HttpHelper.ParseRequest(data, receivedLength);
		if (httpRequest == null || !RequestRouter(httpRequest))
		{
			CloseSocket();
		}
		else
		{
			await AsyncRead();
		}
	}

	public bool RequestRouter(HttpHeader httpRequest)
	{
		if (!httpRequest.Path.StartsWith("/bnetserver/"))
		{
			SendEmptyResponse(HttpCode.NotFound);
			return false;
		}
		string path = httpRequest.Path.Substring("/bnetserver/".Length);
		string[] pathElements = path.Split('/');
		(string, string) tuple = (pathElements[0], httpRequest.Method);
		(string, string) tuple2 = tuple;
		var (text, _) = tuple2;
		if (text == "login")
		{
			string item = tuple2.Item2;
			if (item == "GET")
			{
				SendResponse(HttpCode.Ok, Singleton<LoginServiceManager>.Instance.GetFormInput());
				return true;
			}
			if (item == "POST")
			{
				HandleLoginRequest(pathElements, httpRequest);
				return true;
			}
		}
		SendEmptyResponse(HttpCode.NotFound);
		return false;
	}

	public Task HandleLoginRequest(string[] pathElements, HttpHeader request)
	{
		LogonData loginForm = Json.CreateObject<LogonData>(request.Content);
		if (loginForm == null)
		{
			return SendEmptyResponse(HttpCode.InternalServerError);
		}
		GlobalSessionData globalSession = new GlobalSessionData();
		globalSession.OS = pathElements[1];
		globalSession.Build = uint.Parse(pathElements[2]);
		globalSession.Locale = pathElements[3];
		if (Settings.ClientBuild != (ClientVersionBuild)globalSession.Build)
		{
			return SendAuthError(AuthResult.FAIL_WRONG_MODERN_VER);
		}
		string login = "";
		string password = "";
		foreach (FormInputValue field in loginForm.Inputs)
		{
			string id = field.Id;
			string text = id;
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
		AuthResult response = globalSession.AuthClient.ConnectToAuthServer(login, password, globalSession.Locale);
		if (response != AuthResult.SUCCESS)
		{
			return SendAuthError(response);
		}
		globalSession.AuthClient.SendRealmListUpdateRequest();
		LogonResult loginResult = new LogonResult();
		byte[] ticket = Array.Empty<byte>().GenerateRandomKey(20);
		string loginTicket = (globalSession.LoginTicket = "HP-" + ticket.ToHexString());
		globalSession.Username = login;
		globalSession.AccountMetaDataMgr = new AccountMetaDataManager(login);
		BnetSessionTicketStorage.AddNewSessionByName(login, globalSession);
		BnetSessionTicketStorage.AddNewSessionByTicket(loginTicket, globalSession);
		loginResult.LoginTicket = loginTicket;
		loginResult.AuthenticationState = "DONE";
		return SendResponse(HttpCode.Ok, loginResult);
	}

	private async Task SendResponse<T>(HttpCode code, T response)
	{
		await AsyncWrite(HttpHelper.CreateResponse(code, Json.CreateString(response)));
	}

	private async Task SendAuthError(AuthResult response)
	{
		LogonResult loginResult = new LogonResult();
		LogonResult logonResult = loginResult;
		LogonResult logonResult2 = loginResult;
		LogonResult logonResult3 = loginResult;
		(string, string, string) tuple = response switch
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
