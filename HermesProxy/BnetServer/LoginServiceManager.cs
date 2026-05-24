using System.Net;
using Framework;
using Framework.Logging;
using Framework.Web;

namespace BNetServer;

public class LoginServiceManager : Singleton<LoginServiceManager>
{
	private FormInputs formInputs;

	private IPEndPoint externalAddress;

	private IPEndPoint localAddress;

	private LoginServiceManager()
	{
		formInputs = new FormInputs();
	}

	public void Initialize()
	{
		var port = Settings.RestPort;
		if (port is < 0 or > 65535)
		{
			Log.Print(LogType.Error, $"Specified login service port ({port}) out of allowed range (1-65535), defaulting to 8081", "LoginServiceManager.cs");
			port = 8081;
		}
		var configuredAddress = Settings.ExternalAddress;
		if (!IPAddress.TryParse(configuredAddress, out var address))
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
		var input = new FormInput
		{
			Id = "account_name",
			Type = "text",
			Label = "E-mail",
			MaxLength = 320
		};
		formInputs.Inputs.Add(input);
		input = new FormInput
		{
			Id = "password",
			Type = "password",
			Label = "Password",
			MaxLength = 16
		};
		formInputs.Inputs.Add(input);
		input = new FormInput
		{
			Id = "log_in_submit",
			Type = "submit",
			Label = "Log In"
		};
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
