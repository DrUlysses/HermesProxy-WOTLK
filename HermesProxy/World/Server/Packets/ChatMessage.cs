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
		this.Language = base._worldPacket.ReadUInt32();
		uint len = base._worldPacket.ReadBits<uint>(11);
		this.Text = base._worldPacket.ReadString(len);
	}
}
