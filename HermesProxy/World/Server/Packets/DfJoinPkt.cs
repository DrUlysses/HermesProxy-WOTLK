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
		QueueAsGroup = _worldPacket.HasBit();
		var hasPartyIndex = _worldPacket.HasBit();
		_worldPacket.HasBit(); // Unknown
		Roles = _worldPacket.ReadUInt8();
		var slotCount = _worldPacket.ReadUInt32();
		if (hasPartyIndex)
			_worldPacket.ReadUInt8();
		Slots = new uint[slotCount];
		for (var i = 0; i < slotCount; i++)
			Slots[i] = _worldPacket.ReadUInt32();
	}
}
