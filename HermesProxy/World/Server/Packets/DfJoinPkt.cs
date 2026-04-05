namespace HermesProxy.World.Server.Packets;

public class DfJoinPkt : ClientPacket
{
	public bool QueueAsGroup;
	public byte Roles;
	public uint[] Slots;

	public DfJoinPkt(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		this.QueueAsGroup = base._worldPacket.HasBit();
		bool hasPartyIndex = base._worldPacket.HasBit();
		base._worldPacket.HasBit(); // Unknown
		this.Roles = base._worldPacket.ReadUInt8();
		uint slotCount = base._worldPacket.ReadUInt32();
		if (hasPartyIndex)
			base._worldPacket.ReadUInt8();
		this.Slots = new uint[slotCount];
		for (int i = 0; i < slotCount; i++)
			this.Slots[i] = base._worldPacket.ReadUInt32();
	}
}
