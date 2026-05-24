namespace HermesProxy.World.Server.Packets;

public class JoinChannel : ClientPacket
{
	public string Password;

	public string ChannelName;

	public int ChatChannelId;

	public JoinChannel(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		ChatChannelId = _worldPacket.ReadInt32();
		uint channelLength = _worldPacket.ReadBits<uint>(7);
		uint passwordLength = _worldPacket.ReadBits<uint>(7);
		_worldPacket.ResetBitPos();
		ChannelName = _worldPacket.ReadString(channelLength);
		Password = _worldPacket.ReadString(passwordLength);
	}
}
