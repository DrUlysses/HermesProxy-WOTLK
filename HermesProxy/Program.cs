using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace HermesProxy;

public class Program
{
	public static class CommandLineArgumentsTemplate
	{
		public static readonly Option<string?> ConfigFileLocation = new Option<string>("--config", delegate(ArgumentResult result)
		{
			if (result.Tokens.Count == 0)
			{
				return "HermesProxy.config";
			}
			var value = result.Tokens.Single().Value;
			if (!File.Exists(value))
			{
				result.ErrorMessage = "Error: config file '" + value + "' does not exist";
				return null;
			}
			return value;
		}, isDefault: true, "The config file that will be used");

		public static readonly Option<string?> WorkingDirectory = new Option<string>("--workDir", delegate(ArgumentResult result)
		{
			if (result.Tokens.Count == 0)
			{
				return ".";
			}
			var value = result.Tokens.Single().Value;
			if (!Directory.Exists(value))
			{
				result.ErrorMessage = "Error: directory '" + value + "' does not exist";
				return null;
			}
			return value;
		}, isDefault: true, "The working directory that will be used");

		public static readonly Option<bool> DisableVersionCheck = new Option<bool>("--no-version-check", "Disables the initial version update check");

		public static readonly Option<string[]> OverwrittenConfigValues = new Option<string[]>("--set", "Overwrites a specific config value. Example: --set ServerAddress=logon.example.com");
	}

	public static int Main(string[] args)
	{
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		var commandTree = new RootCommand("Hermes Proxy: Allows you to play on legacy WoW server with modern client")
		{
			CommandLineArgumentsTemplate.ConfigFileLocation,
			CommandLineArgumentsTemplate.WorkingDirectory,
			CommandLineArgumentsTemplate.DisableVersionCheck,
			CommandLineArgumentsTemplate.OverwrittenConfigValues
		};
		var parser = new CommandLineBuilder(commandTree).UseDefaults().Build();
		commandTree.SetHandler(delegate(InvocationContext ctx)
		{
			var parseResult = ctx.ParseResult;
			var args2 = new CommandLineArguments
			{
				ConfigFileLocation = parseResult.GetValueForOption(CommandLineArgumentsTemplate.ConfigFileLocation),
				WorkingDirectory = parseResult.GetValueForOption(CommandLineArgumentsTemplate.WorkingDirectory),
				DisableVersionCheck = parseResult.GetValueForOption(CommandLineArgumentsTemplate.DisableVersionCheck),
				OverwrittenConfigValues = ParseMultiArgument(parseResult.GetValueForOption(CommandLineArgumentsTemplate.OverwrittenConfigValues))
			};
			Server.ServerMain(args2);
		});
		var exitCode = 1;
		try
		{
			exitCode = parser.Invoke(args);
		}
		catch (Exception value)
		{
			Console.WriteLine($"Error occured: {value}");
		}
		if (OsSpecific.AreWeInOurOwnConsole())
		{
			Thread.Sleep(TimeSpan.FromSeconds(3.0));
			Console.WriteLine("Press enter to close");
			Console.ReadLine();
		}
		return exitCode;
	}

	private static Dictionary<string, string> ParseMultiArgument(string[]? multiArgs)
	{
		if (multiArgs == null)
		{
			return new Dictionary<string, string>();
		}
		var result = new Dictionary<string, string>();
		foreach (var arg in multiArgs)
		{
			var keyValue = arg.Split('=', 2);
			if (keyValue.Length != 2)
			{
				throw new Exception("Invalid argument '" + arg + "'");
			}
			result[keyValue[0]] = keyValue[1];
		}
		return result;
	}
}
