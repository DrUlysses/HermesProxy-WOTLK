namespace HermesProxy.World.Server.Packets;

public class AddFriend : ClientPacket
{
	public string Note;

	public string Name;

	public AddFriend(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var nameLength = _worldPacket.ReadBits<uint>(9);
		var noteslength = _worldPacket.ReadBits<uint>(10);
		Name = _worldPacket.ReadString(nameLength);
		Note = _worldPacket.ReadString(noteslength);
	}
}
