namespace HermesProxy.World.Server.Packets;

public class ChatAddonMessage : ClientPacket
{
	public readonly ChatAddonMessageParams Params = new();

	public ChatAddonMessage(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Params.Read(_worldPacket);
	}
}
