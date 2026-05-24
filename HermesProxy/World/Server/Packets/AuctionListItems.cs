using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

internal class AuctionListItems : ClientPacket
{
	public uint Offset;

	public WowGuid128 Auctioneer;

	public byte MinLevel;

	public byte MaxLevel;

	public int Quality;

	public byte MaxPetLevel;

	public List<byte> KnownPets = new List<byte>();

	public string Name;

	public bool OnlyUsable;

	public bool ExactMatch;

	public List<ClassFilter> ClassFilters = new List<ClassFilter>();

	public List<AuctionSort> Sorts = new List<AuctionSort>();

	public AuctionListItems(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Auctioneer = _worldPacket.ReadPackedGuid128();
		Offset = _worldPacket.ReadUInt32();
		MinLevel = _worldPacket.ReadUInt8();
		MaxLevel = _worldPacket.ReadUInt8();
		Quality = _worldPacket.ReadInt32();
		byte sortCount = _worldPacket.ReadUInt8();
		uint knownPetsCount = _worldPacket.ReadUInt32();
		MaxPetLevel = _worldPacket.ReadUInt8();
		for (int i = 0; i < knownPetsCount; i++)
		{
			KnownPets.Add(_worldPacket.ReadUInt8());
		}

		// 3.4.3: TaintedBy bit is present when an addon initiates the search.
		// Detect by peeking the first bit byte's MSB — if set, TaintedBy is present
		// (search names are always < 128 chars so nameLength MSB would be 0 in old format).
		byte peekByte = _worldPacket.PeekByte();
		bool hasTaintedBy = (peekByte & 0x80) != 0;

		if (hasTaintedBy)
			_worldPacket.HasBit(); // consume TaintedBy bit

		uint nameLength = _worldPacket.ReadBits<uint>(8);
		uint classFiltersCount = _worldPacket.ReadBits<uint>(3);
		OnlyUsable = _worldPacket.HasBit();
		ExactMatch = _worldPacket.HasBit();
		_worldPacket.ResetBitPos();

		Name = _worldPacket.ReadString(nameLength);

		for (int j = 0; j < classFiltersCount; j++)
		{
			ClassFilter classFilter = new ClassFilter();
			classFilter.ItemClass = _worldPacket.ReadInt32();
			uint subClassFiltersCount = _worldPacket.ReadBits<uint>(5);
			for (uint j2 = 0u; j2 < subClassFiltersCount; j2++)
			{
				SubClassFilter filter = new SubClassFilter
				{
					InvTypeMask = (uint)_worldPacket.ReadUInt64(), // 3.4.3 uses uint64
					ItemSubclass = _worldPacket.ReadInt32()
				};
				classFilter.SubClassFilters.Add(filter);
			}
			ClassFilters.Add(classFilter);
		}

		// Skip TaintedBy (AddOnInfo) if present
		if (hasTaintedBy)
		{
			_worldPacket.ResetBitPos();
			uint taintNameLen = _worldPacket.ReadBits<uint>(10);
			uint taintVersionLen = _worldPacket.ReadBits<uint>(10);
			_worldPacket.HasBit(); // Loaded
			_worldPacket.HasBit(); // Disabled
			if (taintNameLen > 0)
				_worldPacket.ReadBytes(taintNameLen);
			if (taintVersionLen > 0)
				_worldPacket.ReadBytes(taintVersionLen);
		}

		uint size = _worldPacket.ReadUInt32();
		byte[] data = _worldPacket.ReadBytes(size);
		WorldPacket sorts = new WorldPacket(_worldPacket.GetOpcode(), data);
		for (int k = 0; k < sortCount; k++)
		{
			AuctionSort sort = new AuctionSort
			{
				Type = sorts.ReadUInt8(),
				Direction = sorts.ReadUInt8()
			};
			Sorts.Add(sort);
		}
	}
}
