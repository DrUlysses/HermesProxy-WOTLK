using System;
using Framework;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public abstract class ClientPacket : IDisposable
{
	protected readonly WorldPacket _worldPacket;

	protected ClientPacket(WorldPacket worldPacket)
	{
		_worldPacket = worldPacket;
	}

	public abstract void Read();

	public void Dispose()
	{
		_worldPacket.Dispose();
	}

	public uint GetOpcode()
	{
		return _worldPacket.GetOpcode();
	}

	public Opcode GetUniversalOpcode()
	{
		return ModernVersion.GetUniversalOpcode(GetOpcode());
	}

	public void LogPacket(ref SniffFile? sniffFile)
	{
		if (!Settings.PacketsLog || sniffFile == null)
			return;
		sniffFile.WritePacket(GetOpcode(), isFromClient: true, _worldPacket.GetData());
	}
}
