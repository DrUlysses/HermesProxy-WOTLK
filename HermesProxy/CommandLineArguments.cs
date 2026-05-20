using System.Collections.Generic;

namespace HermesProxy;

public class CommandLineArguments
{
	public string? ConfigFileLocation { get; init; }

	public string? WorkingDirectory { get; init; }

	public bool DisableVersionCheck { get; init; }

	public Dictionary<string, string> OverwrittenConfigValues { get; init; }
}
