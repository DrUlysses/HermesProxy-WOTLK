using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ClientCacheVersion : ServerPacket
{
	public uint CacheVersion = 0u;

	public ClientCacheVersion()
		: base(Opcode.SMSG_CACHE_VERSION)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.CacheVersion);
	}
}
