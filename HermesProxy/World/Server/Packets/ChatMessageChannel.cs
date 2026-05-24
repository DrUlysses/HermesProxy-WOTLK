namespace HermesProxy.World.Server.Packets;

public class ChatMessageChannel : ClientPacket
{
	public uint Language;

	public WowGuid128 ChannelGUID;

	public string Text;

	public string Target;

	public ChatMessageChannel(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Language = _worldPacket.ReadUInt32();
		ChannelGUID = _worldPacket.ReadPackedGuid128();
		uint targetLen = _worldPacket.ReadBits<uint>(9);
		uint textLen = _worldPacket.ReadBits<uint>(9);
		Target = _worldPacket.ReadString(targetLen);
		Text = _worldPacket.ReadString(textLen);
	}
}
