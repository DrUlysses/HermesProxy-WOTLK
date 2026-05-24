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
			if (_logPath == null)
			{
				_logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "missing_opcodes.log");
			}
			return _logPath;
		}
	}

	public static void LogDroppedSMSG(Opcode universalOpcode, int size)
	{
		var key = $"DROPPED_SMSG:{universalOpcode}:sz{size}";
		Log(key, $"[DROPPED SMSG] {universalOpcode} (mapped to opcode 0, size={size}) - needs modern opcode value or handler");
	}

	public static void LogUnhandledCMSG(Opcode universalOpcode, uint rawOpcode)
	{
		var key = $"UNHANDLED_CMSG:{universalOpcode}:{rawOpcode}";
		Log(key, $"[UNHANDLED CMSG] {universalOpcode} (raw=0x{rawOpcode:X4}/{rawOpcode}) - needs handler");
	}

	public static void LogUnhandledLegacySMSG(Opcode universalOpcode, uint rawOpcode)
	{
		var key = $"UNHANDLED_LEGACY_SMSG:{universalOpcode}";
		Log(key, $"[UNHANDLED LEGACY SMSG] {universalOpcode} (raw=0x{rawOpcode:X4}/{rawOpcode}) - needs conversion handler");
	}

	private static void Log(string key, string message)
	{
		lock (_lock)
		{
			if (_logged.Contains(key))
			{
				return;
			}
			_logged.Add(key);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
				File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}\n");
			}
			catch
			{
			}
		}
	}

	public static void Reset()
	{
		lock (_lock)
		{
			_logged.Clear();
			try
			{
				File.Delete(LogPath);
			}
			catch
			{
			}
		}
	}
}
