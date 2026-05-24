namespace HermesProxy.World.Server.Packets;

public class InstanceLockResponse : ClientPacket
{
	public bool AcceptLock;

	public InstanceLockResponse(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		AcceptLock = _worldPacket.ReadBit();
	}
}
