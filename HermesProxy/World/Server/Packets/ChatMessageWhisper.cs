namespace HermesProxy.World.Server.Packets;

public class ChatMessageWhisper : ClientPacket
{
	public uint Language;

	public string Text;

	public string Target;

	public ChatMessageWhisper(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Language = _worldPacket.ReadUInt32();
		uint targetLen = _worldPacket.ReadBits<uint>(9);
		uint textLen = _worldPacket.ReadBits<uint>(9);
		Target = _worldPacket.ReadString(targetLen);
		Text = _worldPacket.ReadString(textLen);
	}
}
