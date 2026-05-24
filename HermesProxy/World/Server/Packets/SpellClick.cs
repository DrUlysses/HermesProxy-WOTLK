namespace HermesProxy.World.Server.Packets;

public class SpellClick : ClientPacket
{
	public WowGuid128 SpellClickUnitGuid;
	public bool TryAutoDismount;

	public SpellClick(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		SpellClickUnitGuid = _worldPacket.ReadPackedGuid128();
		TryAutoDismount = _worldPacket.ReadBit();
	}
}
