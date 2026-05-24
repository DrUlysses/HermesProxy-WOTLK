namespace HermesProxy.World.Server.Packets;

public class ChatMessage : ClientPacket
{
	public string Text;

	public uint Language;

	public ChatMessage(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Language = _worldPacket.ReadUInt32();
		var len = _worldPacket.ReadBits<uint>(11);
		Text = _worldPacket.ReadString(len);
	}
}
