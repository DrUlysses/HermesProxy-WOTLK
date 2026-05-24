using System.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server;

public class AccountDataManager
{
	public AccountData[] Data;

	private string _accountName;

	private string _realmName;

	public AccountDataManager(string accountName, string realmName)
	{
		_accountName = accountName;
		_realmName = realmName.Trim();
	}

	public static bool IsGlobalDataType(uint type)
	{
		switch ((AccountDataType)type)
		{
		case AccountDataType.GlobalConfigCache:
		case AccountDataType.GlobalBindingsCache:
		case AccountDataType.GlobalMacrosCache:
		case AccountDataType.GlobalTTSCache:
		case AccountDataType.GlobalFlaggedCache:
			return true;
		default:
			return false;
		}
	}

	public string GetAccountDataDirectory()
	{
		var path = Path.GetFullPath(Path.Combine("AccountData", _accountName, _realmName));
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
	}

	public string GetFullFileName(WowGuid128 guid, uint type)
	{
		var file = ((!IsGlobalDataType(type)) ? $"data-{type}-{guid.GetLowValue()}-{guid.GetHighValue()}.bin" : $"data-{type}.bin");
		var path = GetAccountDataDirectory();
		return Path.Combine(path, file);
	}

	public void LoadAllData(WowGuid128 guid)
	{
		Data = new AccountData[ModernVersion.GetAccountDataCount()];
		for (var i = 0u; i < ModernVersion.GetAccountDataCount(); i++)
		{
			Data[i] = LoadData(guid, i);
		}
	}

	public AccountData LoadData(WowGuid128 guid, uint type)
	{
		AccountData data = null;
		var fileName = GetFullFileName(guid, type);
		if (File.Exists(fileName))
		{
			using (File.OpenRead(GetFullFileName(guid, type)))
			{
				using var reader = new BinaryReader(File.OpenRead(GetFullFileName(guid, type)));
				data = new AccountData();
				var guidLow = reader.ReadUInt64();
				var guidHigh = reader.ReadUInt64();
				data.Guid = new WowGuid128(guidHigh, guidLow);
				if (!IsGlobalDataType(type))
				{
				}
				data.Timestamp = reader.ReadInt64();
				data.Type = reader.ReadUInt32();
				data.UncompressedSize = reader.ReadUInt32();
				var compressedSize = reader.ReadInt32();
				data.CompressedData = reader.ReadBytes(compressedSize);
			}
		}
		return data;
	}

	public void SaveData(WowGuid128 guid, long timestamp, uint type, uint uncompressedSize, byte[] compressedData)
	{
		if (compressedData == null)
		{
			return;
		}
		if (Data[type] == null)
		{
			Data[type] = new AccountData();
		}
		Data[type].Guid = guid;
		Data[type].Timestamp = timestamp;
		Data[type].Type = type;
		Data[type].UncompressedSize = uncompressedSize;
		Data[type].CompressedData = compressedData;
		using var writer = new BinaryWriter(File.Open(GetFullFileName(guid, type), FileMode.Create));
		writer.Write(guid.GetLowValue());
		writer.Write(guid.GetHighValue());
		writer.Write(timestamp);
		writer.Write(type);
		writer.Write(uncompressedSize);
		writer.Write(compressedData.Length);
		writer.Write(compressedData);
	}

	public byte[] LoadCUFProfiles()
	{
		var fileName = Path.Combine(GetAccountDataDirectory(), "cuf.bin");
		if (File.Exists(fileName))
		{
			using (var file = File.OpenRead(fileName))
			{
				using (new BinaryReader(file))
				{
					return File.ReadAllBytes(fileName);
				}
			}
		}
		return new byte[4];
	}

	public void SaveCUFProfiles(byte[] data)
	{
		var fileName = Path.Combine(GetAccountDataDirectory(), "cuf.bin");
		using var writer = new BinaryWriter(File.Open(fileName, FileMode.Create));
		writer.Write(data);
	}
}
