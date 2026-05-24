using Framework;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public abstract class ServerPacket
{
	private byte[] _buffer;

	private readonly ConnectionType _connectionType;

	private readonly Opcode _universalOpcode;

	protected readonly WorldPacket _worldPacket;

	public bool SkipSend { get; protected set; }

	protected ServerPacket(Opcode universalOpcode)
	{
		_connectionType = ConnectionType.Realm;
		_universalOpcode = universalOpcode;
		var opcode = ModernVersion.GetCurrentOpcode(universalOpcode);
		_worldPacket = new WorldPacket(opcode);
	}

	protected ServerPacket(Opcode universalOpcode, ConnectionType type = ConnectionType.Realm)
	{
		_connectionType = type;
		_universalOpcode = universalOpcode;
		var opcode = ModernVersion.GetCurrentOpcode(universalOpcode);
		_worldPacket = new WorldPacket(opcode);
	}

	public void Clear()
	{
		_worldPacket.Clear();
		_buffer = null;
	}

	public uint GetOpcode()
	{
		return _worldPacket.GetOpcode();
	}

	public Opcode GetUniversalOpcode()
	{
		return _universalOpcode;
	}

	public byte[] GetData()
	{
		return _buffer;
	}

	public void LogPacket(ref SniffFile sniffFile)
	{
		if (!Settings.PacketsLog) return;
		if (sniffFile == null)
		{
			sniffFile = new SniffFile("modern", (ushort)Settings.ClientBuild);
			sniffFile.WriteHeader();
		}
		sniffFile.WritePacket(GetOpcode(), isFromClient: false, GetData());
	}

	protected abstract void Write();

	public void WritePacketData()
	{
		if (_buffer != null) return;
		Write();
		_buffer = _worldPacket.GetData();
		_worldPacket.Dispose();
	}

	public ConnectionType GetConnection()
	{
		return _connectionType;
	}
}
