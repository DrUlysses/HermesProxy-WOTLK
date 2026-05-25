using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BNetServer;
using BNetServer.Networking;
using Framework;
using Framework.Logging;
using Framework.Networking;
using HermesProxy.BnetServer.Networking;
using HermesProxy.Configuration;
using HermesProxy.World;
using HermesProxy.World.Server;

namespace HermesProxy;

internal class Server
{
	private static readonly string? _buildTag;

	public static void ServerMain(CommandLineArguments args)
	{
		Log.Print(LogType.Server, "Starting Hermes Proxy...", "Server.cs");
		Log.Print(LogType.Server, "Version " + GetVersionInformation(), "Server.cs");
		var workingDirectory = args.WorkingDirectory ?? Path.GetDirectoryName(AppContext.BaseDirectory) ?? ".";

		if (Environment.CurrentDirectory != workingDirectory)
		{
			Log.Print(LogType.Storage, "Switching working directory", "Server.cs");
			Log.Print(LogType.Storage, "Old: " + Environment.CurrentDirectory, "Server.cs");
			Directory.SetCurrentDirectory(workingDirectory);
			Environment.CurrentDirectory = workingDirectory;
			Log.Print(LogType.Storage, "New: " + Environment.CurrentDirectory, "Server.cs");
			Thread.Sleep(TimeSpan.FromSeconds(1.0));
		}
		Log.Start();
		
		ConfigurationParser config;
		try
		{
			config = ConfigurationParser.ParseFromFile(args.ConfigFileLocation, args.OverwrittenConfigValues);
		}
		catch (FileNotFoundException)
		{
			Log.Print(LogType.Error, "Config loading failed", "Server.cs");
			return;
		}
		if (!Settings.LoadAndVerifyFrom(config))
		{
			Log.Print(LogType.Error, "The verification of the config failed", "Server.cs");
			return;
		}
		Log.DebugLogEnabled = Settings.DebugOutput;
		Log.Print(LogType.Debug, "Debug logging enabled", "Server.cs");
		if (!AesGcm.IsSupported)
		{
			Log.Print(LogType.Error, "AesGcm is not supported on your platform", "Server.cs");
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return;
			Log.Print(LogType.Error, "Since you are on MacOS, you can install openssl@3 via homebrew", "Server.cs");
			Log.Print(LogType.Error, "Run this:      brew install openssl@3", "Server.cs");
			Log.Print(LogType.Error, "Start Hermes:  DYLD_LIBRARY_PATH=/opt/homebrew/opt/openssl@3/lib ./HermesProxy", "Server.cs");
			return;
		}
		Log.Print(LogType.Server, $"Modern (Client) Build: {Settings.ClientBuild}", "Server.cs");
		Log.Print(LogType.Server, $"Legacy (Server) Build: {Settings.ServerBuild}", "Server.cs");
		GameData.LoadEverything();
		var bindIp = NetworkUtils.ResolveOrDirectIPv64(Settings.ExternalAddress);
		if (!IPAddress.IsLoopback(bindIp))
		{
			bindIp = IPAddress.Any;
		}
		Log.Print(LogType.Network, "External IP: " + Settings.ExternalAddress, "Server.cs");
		Singleton<LoginServiceManager>.Instance.Initialize();
		var bnetSocketServer = StartServer<BnetTcpSession>(new IPEndPoint(bindIp, Settings.BNetPort));
		var restSocketServer = StartServer<BnetRestApiSession>(new IPEndPoint(bindIp, Settings.RestPort));
		var realmSocketServer = StartServer<RealmSocket>(new IPEndPoint(bindIp, Settings.RealmPort));
		var worldSocketServer = StartServer<WorldSocket>(new IPEndPoint(bindIp, Settings.InstancePort));
		while (restSocketServer.IsListening || bnetSocketServer.IsListening || realmSocketServer.IsListening || worldSocketServer.IsListening)
		{
			Thread.Sleep(TimeSpan.FromSeconds(10.0));
		}
		Console.WriteLine($"(restSocketServer.IsListening: {restSocketServer.IsListening}");
		Console.WriteLine($"(bnetSocketServer.IsListening: {bnetSocketServer.IsListening}");
		Console.WriteLine($"(realmSocketServer.IsListening: {realmSocketServer.IsListening}");
		Console.WriteLine($"(worldSocketServer.IsListening: {worldSocketServer.IsListening}");
	}

	private static SocketManager<TSocketType> StartServer<TSocketType>(IPEndPoint bindIp) where TSocketType : ISocket
	{
		var socketManager = new SocketManager<TSocketType>();
		Log.Print(LogType.Server, $"Starting {typeof(TSocketType).Name} service on {bindIp}...", "Server.cs");
		if (!socketManager.StartNetwork(bindIp.Address.ToString(), bindIp.Port))
		{
			throw new Exception("Failed to start " + typeof(TSocketType).Name + " service");
		}
		Thread.Sleep(50);
		return socketManager;
	}

	private static async Task CheckForUpdate()
	{
		try
		{
			if (GitVersionInformation.CommitsSinceVersionSource != "0" || GitVersionInformation.UncommittedChanges != "0")
			{
				return;
			}
			using var client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(5.0);
			client.DefaultRequestHeaders.Add("User-Agent", "curl/7.0.0");
			var response = await client.GetAsync("https://api.github.com/repos/advocaite/HermesProxy-WOTLK/releases/latest");
			response.EnsureSuccessStatusCode();
			var parsedJson = JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
			var commitDateStr = parsedJson["created_at"].ToString();
			var commitDate = DateTime.Parse(commitDateStr, CultureInfo.InvariantCulture).ToUniversalTime();
			var myCommitDateStr = GitVersionInformation.CommitDate;
			var myCommitDate = DateTime.Parse(myCommitDateStr, CultureInfo.InvariantCulture).ToUniversalTime();
			if (commitDate > myCommitDate)
			{
				Console.WriteLine("------------------------");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"HermesProxy update available v{GitVersionInformation.Major}.{GitVersionInformation.Minor} => {parsedJson["tag_name"]} ({commitDate:yyyy-MM-dd})");
				Console.WriteLine("Please download new version from https://github.com/advocaite/HermesProxy-WOTLK/releases/latest");
				Console.ResetColor();
				Console.WriteLine("------------------------");
				Console.WriteLine();
				Thread.Sleep(10000);
			}
		}
		catch
		{
		}
	}

	private static string GetVersionInformation()
	{
		var commitDate = DateTime.Parse(GitVersionInformation.CommitDate, CultureInfo.InvariantCulture).ToUniversalTime();
		var version = $"{commitDate:yyyy-MM-dd} {_buildTag}{GitVersionInformation.MajorMinorPatch}";
		if (GitVersionInformation.CommitsSinceVersionSource != "0")
		{
			version += $"+{GitVersionInformation.CommitsSinceVersionSource}({GitVersionInformation.ShortSha})";
		}
		if (GitVersionInformation.UncommittedChanges != "0")
		{
			version += " dirty";
		}
		return version;
	}
}
