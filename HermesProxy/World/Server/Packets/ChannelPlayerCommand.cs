namespace HermesProxy.World.Server.Packets;

internal class ChannelPlayerCommand : ClientPacket
{
	public string ChannelName;
	public string Name;

	public ChannelPlayerCommand(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint channelNameLength = _worldPacket.ReadBits<uint>(7);
		uint nameLength = _worldPacket.ReadBits<uint>(9);
		ChannelName = _worldPacket.ReadString(channelNameLength);
		Name = _worldPacket.ReadString(nameLength);
	}
}
