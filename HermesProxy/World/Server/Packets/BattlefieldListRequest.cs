namespace HermesProxy.World.Server.Packets;

public class BattlefieldListRequest : ClientPacket
{
	public int ListID;

	public BattlefieldListRequest(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		this.ListID = base._worldPacket.ReadInt32();
	}
}
