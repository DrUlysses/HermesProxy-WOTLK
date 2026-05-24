using System;
using System.IO;
using System.Threading;

namespace HermesProxy.World;

public class SniffFile
{
	private BinaryWriter _fileWriter;

	private ushort _gameVersion;

	private Mutex _mutex = new();

	public SniffFile(string fileName, ushort build)
	{
		var dir = "PacketsLog";
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}
		var file = fileName + "_" + build + "_" + Time.UnixTime + ".pkt";
		var path = Path.Combine(dir, file);
		_fileWriter = new BinaryWriter(File.Open(path, FileMode.Create));
		_gameVersion = build;
	}

	public void WriteHeader()
	{
		_fileWriter.Write('P');
		_fileWriter.Write('K');
		_fileWriter.Write('T');
		ushort sniffVersion = 513;
		_fileWriter.Write(sniffVersion);
		_fileWriter.Write(_gameVersion);
		for (var i = 0; i < 40; i++)
		{
			byte zero = 0;
			_fileWriter.Write(zero);
		}
	}

	public void WritePacket(uint opcode, bool isFromClient, byte[] data)
	{
		_mutex.WaitOne();
		var direction = (byte)(!isFromClient ? byte.MaxValue : 0);
		_fileWriter.Write(direction);
		var unixtime = (uint)Time.UnixTime;
		_fileWriter.Write(unixtime);
		_fileWriter.Write(Environment.TickCount);
		if (isFromClient)
		{
			var packetSize = (uint)(data.Length - 2 + 4);
			_fileWriter.Write(packetSize);
			_fileWriter.Write(opcode);
			for (var i = 2; i < data.Length; i++)
			{
				_fileWriter.Write(data[i]);
			}
		}
		else
		{
			var packetSize2 = (uint)(data.Length + 2);
			_fileWriter.Write(packetSize2);
			var opcode2 = (ushort)opcode;
			_fileWriter.Write(opcode2);
			_fileWriter.Write(data);
		}
		_mutex.ReleaseMutex();
	}

	public void CloseFile()
	{
		_fileWriter.Close();
	}
}
