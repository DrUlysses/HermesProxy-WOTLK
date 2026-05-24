namespace HermesProxy.World.Server.Packets;

public class ReadyForAccountDataTimesPkt : ClientPacket
{
    public ReadyForAccountDataTimesPkt(WorldPacket packet) : base(packet) { }
    public override void Read() { }
}
