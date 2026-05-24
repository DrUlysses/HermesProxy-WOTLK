namespace HermesProxy.World.Server.Packets;

internal class ChannelPassword : ClientPacket
{
	public string ChannelName;
	public string Password;

	public ChannelPassword(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint channelNameLength = _worldPacket.ReadBits<uint>(7);
		uint passwordLength = _worldPacket.ReadBits<uint>(7);
		ChannelName = _worldPacket.ReadString(channelNameLength);
		Password = _worldPacket.ReadString(passwordLength);
	}
}
