namespace HermesProxy.World.Server.Packets;

internal class SocketGems : ClientPacket
{
	public WowGuid128 ItemGuid;

	public readonly WowGuid128[] Gems = new WowGuid128[3];

	public SocketGems(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		ItemGuid = _worldPacket.ReadPackedGuid128();
		for (var i = 0; i < 3; i++)
		{
			Gems[i] = _worldPacket.ReadPackedGuid128();
		}
	}
}
