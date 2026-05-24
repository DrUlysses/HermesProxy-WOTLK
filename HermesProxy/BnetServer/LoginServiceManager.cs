using System.Net;
using System.Security.Cryptography.X509Certificates;
using Framework;
using Framework.Logging;
using Framework.Web;

namespace BNetServer;

public class LoginServiceManager : Singleton<LoginServiceManager>
{
	private FormInputs formInputs;

	private IPEndPoint externalAddress;

	private IPEndPoint localAddress;

	private X509Certificate2 certificate;

	private LoginServiceManager()
	{
		formInputs = new FormInputs();
	}

	public void Initialize()
	{
		int port = Settings.RestPort;
		if (port < 0 || port > 65535)
		{
			Log.Print(LogType.Error, $"Specified login service port ({port}) out of allowed range (1-65535), defaulting to 8081", "LoginServiceManager.cs");
			port = 8081;
		}
		string configuredAddress = Settings.ExternalAddress;
		if (!IPAddress.TryParse(configuredAddress, out IPAddress address))
		{
			Log.Print(LogType.Error, "Could not resolve LoginREST.ExternalAddress " + configuredAddress, "LoginServiceManager.cs");
			return;
		}
		externalAddress = new IPEndPoint(address, port);
		configuredAddress = "127.0.0.1";
		if (!IPAddress.TryParse(configuredAddress, out address))
		{
			Log.Print(LogType.Error, "Could not resolve local address.", "LoginServiceManager.cs");
			return;
		}
		localAddress = new IPEndPoint(address, port);
		formInputs.Type = "LOGIN_FORM";
		FormInput input = new FormInput();
		input.Id = "account_name";
		input.Type = "text";
		input.Label = "E-mail";
		input.MaxLength = 320;
		formInputs.Inputs.Add(input);
		input = new FormInput();
		input.Id = "password";
		input.Type = "password";
		input.Label = "Password";
		input.MaxLength = 16;
		formInputs.Inputs.Add(input);
		input = new FormInput();
		input.Id = "log_in_submit";
		input.Type = "submit";
		input.Label = "Log In";
		formInputs.Inputs.Add(input);
	}

	public IPEndPoint GetAddressForClient(IPAddress address)
	{
		if (IPAddress.IsLoopback(address))
		{
			return localAddress;
		}
		return externalAddress;
	}

	public FormInputs GetFormInput()
	{
		return formInputs;
	}
}
