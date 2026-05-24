namespace HermesProxy.World.Server.Packets;

public class ChatMessageDND : ClientPacket
{
	public string Text;

	public ChatMessageDND(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		uint len = _worldPacket.ReadBits<uint>(9);
		Text = _worldPacket.ReadString(len);
	}
}
