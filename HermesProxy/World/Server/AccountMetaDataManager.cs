using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Framework.Logging;

namespace HermesProxy.World.Server;

public class AccountMetaDataManager
{
	private const string LAST_CHARACTER_FILE = "last_character.txt";

	private const string COMPLETED_QUESTS_FILE = "completed_quests.csv";

	private const string SETTINGS_FILE = "settings.json";

	private readonly string _accountName;

	private string GetAccountMetaDataDirectory()
	{
		var path = Path.GetFullPath(Path.Combine("AccountData", _accountName));
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
	}

	private string GetAccountCharacterMetaDataDirectory(string realm, string characterName)
	{
		var path = Path.GetFullPath(Path.Combine("AccountData", _accountName, realm, characterName));
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
	}

	public AccountMetaDataManager(string accountName)
	{
		_accountName = accountName;
	}

	public (string realmName, string charName, ulong charLowerGuid, long lastLoginUnixSec)? GetLastSelectedCharacter()
	{
		var path = Path.Combine(GetAccountMetaDataDirectory(), "last_character.txt");
		if (!File.Exists(path))
		{
			return null;
		}
		var rawContent = File.ReadAllText(path, Encoding.UTF8);
		var content = rawContent.Split(',');
		if (content.Length != 4)
		{
			Log.Print(LogType.Error, "Invalid split size in 'GetLastSelectedCharacter' for account '" + _accountName + "'", "AccountDataManager.cs");
			return null;
		}
		return (content[0], content[1], ulong.Parse(content[3]), long.Parse(content[2]));
	}

	public void SaveLastSelectedCharacter(string realmName, string charName, ulong charLowerGuid, long lastLoginUnixSec)
	{
		var dir = GetAccountMetaDataDirectory();
		var path = Path.Combine(dir, "last_character.txt");
		File.WriteAllText(path, $"{realmName},{charName},{charLowerGuid},{lastLoginUnixSec}", Encoding.UTF8);
		Log.Print(LogType.Debug, "Saved last selected char in '" + path + "'", "AccountDataManager.cs");
	}

	public void InvalidateLastSelectedCharacter()
	{
		var dir = GetAccountMetaDataDirectory();
		var path = Path.Combine(dir, "last_character.txt");
		if (File.Exists(path))
		{
			File.WriteAllText(path, "");
			Log.Print(LogType.Debug, "Invalidated last selected character entry in '" + path + "'", "AccountDataManager.cs");
		}
	}

	public List<uint> GetAllCompletedQuests(string realmName, string charName)
	{
		var dir = GetAccountCharacterMetaDataDirectory(realmName, charName);
		var path = Path.Combine(dir, "completed_quests.csv");
		if (!File.Exists(path))
		{
			return new List<uint>();
		}
		var lines = File.ReadAllLines(path).ToList();
		return lines.Select(x => uint.Parse(x.Split(',').FirstOrDefault() ?? "0")).ToList();
	}

	public void MarkQuestAsCompleted(string realmName, string charName, uint questId)
	{
		var dir = GetAccountCharacterMetaDataDirectory(realmName, charName);
		var path = Path.Combine(dir, "completed_quests.csv");
		var when = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		File.AppendAllLines(path, new string[1] { $"{questId},{when}" }, Encoding.UTF8);
	}

	public void MarkQuestAsNotCompleted(string realmName, string charName, uint questId)
	{
		var dir = GetAccountCharacterMetaDataDirectory(realmName, charName);
		var path = Path.Combine(dir, "completed_quests.csv");
		var needle = questId.ToString();
		var lines = File.ReadAllLines(path).ToList();
		lines.RemoveAll(l => l.Split(',').FirstOrDefault()?.Equals(needle) ?? false);
		File.WriteAllLines(path, lines);
	}

	public void SaveCharacterSettingsStorage(string realmName, string charName, PlayerSettings.InternalStorage settings)
	{
		var dir = GetAccountCharacterMetaDataDirectory(realmName, charName);
		var path = Path.Combine(dir, "settings.json");
		var options = new JsonSerializerOptions
		{
			WriteIndented = true
		};
		var jsonString = JsonSerializer.Serialize(settings, options);
		File.WriteAllText(path, jsonString, Encoding.UTF8);
	}

	public PlayerSettings.InternalStorage LoadCharacterSettingsStorage(string realmName, string charName)
	{
		var dir = GetAccountCharacterMetaDataDirectory(realmName, charName);
		var path = Path.Combine(dir, "settings.json");
		if (!File.Exists(path))
		{
			var fallback = new PlayerSettings.InternalStorage();
			SaveCharacterSettingsStorage(realmName, charName, fallback);
			return fallback;
		}
		var jsonString = File.ReadAllText(path, Encoding.UTF8);
		return JsonSerializer.Deserialize<PlayerSettings.InternalStorage>(jsonString);
	}
}
