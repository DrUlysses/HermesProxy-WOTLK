namespace HermesProxy.World.Server.Packets;

public class GuildInviteByName : ClientPacket
{
	public string Name;

	public uint ArenaTeamId;

	public GuildInviteByName(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint nameLen = _worldPacket.ReadBits<uint>(9);
		bool isArena = _worldPacket.HasBit();
		Name = _worldPacket.ReadString(nameLen);
		if (isArena)
		{
			ArenaTeamId = _worldPacket.ReadUInt32();
		}
	}
}
