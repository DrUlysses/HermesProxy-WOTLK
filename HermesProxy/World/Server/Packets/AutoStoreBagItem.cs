namespace HermesProxy.World.Server.Packets;

public class AutoStoreBagItem : ClientPacket
{
	public InvUpdate Inv;
	public byte ContainerSlotB;
	public byte ContainerSlotA;
	public byte SlotA;

	public AutoStoreBagItem(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Inv = new InvUpdate(_worldPacket);
		ContainerSlotB = _worldPacket.ReadUInt8();
		ContainerSlotA = _worldPacket.ReadUInt8();
		SlotA = _worldPacket.ReadUInt8();
	}
}
