namespace HermesProxy.World.Server.Packets;

public class ChatMessageAFK : ClientPacket
{
	public string Text;

	public ChatMessageAFK(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var len = _worldPacket.ReadBits<uint>(9);
		Text = _worldPacket.ReadString(len);
	}
}
