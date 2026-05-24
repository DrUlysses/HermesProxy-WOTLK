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
		var targetLen = _worldPacket.ReadBits<uint>(9);
		var textLen = _worldPacket.ReadBits<uint>(9);
		Target = _worldPacket.ReadString(targetLen);
		Text = _worldPacket.ReadString(textLen);
	}
}
