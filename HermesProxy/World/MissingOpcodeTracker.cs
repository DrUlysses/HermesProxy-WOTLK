using System;
using System.Collections.Generic;
using System.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public static class MissingOpcodeTracker
{
	private static readonly HashSet<string> _logged = new HashSet<string>();

	private static readonly object _lock = new object();

	private static string _logPath;

	private static string LogPath
	{
		get
		{
			if (MissingOpcodeTracker._logPath == null)
			{
				MissingOpcodeTracker._logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "missing_opcodes.log");
			}
			return MissingOpcodeTracker._logPath;
		}
	}

	public static void LogDroppedSMSG(Opcode universalOpcode, int size)
	{
		string key = $"DROPPED_SMSG:{universalOpcode}:sz{size}";
		MissingOpcodeTracker.Log(key, $"[DROPPED SMSG] {universalOpcode} (mapped to opcode 0, size={size}) - needs modern opcode value or handler");
	}

	public static void LogUnhandledCMSG(Opcode universalOpcode, uint rawOpcode)
	{
		string key = $"UNHANDLED_CMSG:{universalOpcode}:{rawOpcode}";
		MissingOpcodeTracker.Log(key, $"[UNHANDLED CMSG] {universalOpcode} (raw=0x{rawOpcode:X4}/{rawOpcode}) - needs handler");
	}

	public static void LogUnhandledLegacySMSG(Opcode universalOpcode, uint rawOpcode)
	{
		string key = $"UNHANDLED_LEGACY_SMSG:{universalOpcode}";
		MissingOpcodeTracker.Log(key, $"[UNHANDLED LEGACY SMSG] {universalOpcode} (raw=0x{rawOpcode:X4}/{rawOpcode}) - needs conversion handler");
	}

	private static void Log(string key, string message)
	{
		lock (MissingOpcodeTracker._lock)
		{
			if (MissingOpcodeTracker._logged.Contains(key))
			{
				return;
			}
			MissingOpcodeTracker._logged.Add(key);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(MissingOpcodeTracker.LogPath));
				File.AppendAllText(MissingOpcodeTracker.LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}\n");
			}
			catch
			{
			}
		}
	}

	public static void Reset()
	{
		lock (MissingOpcodeTracker._lock)
		{
			MissingOpcodeTracker._logged.Clear();
			try
			{
				File.Delete(MissingOpcodeTracker.LogPath);
			}
			catch
			{
			}
		}
	}
}
