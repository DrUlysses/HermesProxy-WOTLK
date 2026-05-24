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
		var sortCount = _worldPacket.ReadUInt8();
		var knownPetsCount = _worldPacket.ReadUInt32();
		MaxPetLevel = _worldPacket.ReadUInt8();
		for (var i = 0; i < knownPetsCount; i++)
		{
			KnownPets.Add(_worldPacket.ReadUInt8());
		}

		// 3.4.3: TaintedBy bit is present when an addon initiates the search.
		// Detect by peeking the first bit byte's MSB — if set, TaintedBy is present
		// (search names are always < 128 chars so nameLength MSB would be 0 in old format).
		var peekByte = _worldPacket.PeekByte();
		var hasTaintedBy = (peekByte & 0x80) != 0;

		if (hasTaintedBy)
			_worldPacket.HasBit(); // consume TaintedBy bit

		var nameLength = _worldPacket.ReadBits<uint>(8);
		var classFiltersCount = _worldPacket.ReadBits<uint>(3);
		OnlyUsable = _worldPacket.HasBit();
		ExactMatch = _worldPacket.HasBit();
		_worldPacket.ResetBitPos();

		Name = _worldPacket.ReadString(nameLength);

		for (var j = 0; j < classFiltersCount; j++)
		{
			var classFilter = new ClassFilter
			{
				ItemClass = _worldPacket.ReadInt32()
			};
			var subClassFiltersCount = _worldPacket.ReadBits<uint>(5);
			for (var j2 = 0u; j2 < subClassFiltersCount; j2++)
			{
				var filter = new SubClassFilter
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
			var taintNameLen = _worldPacket.ReadBits<uint>(10);
			var taintVersionLen = _worldPacket.ReadBits<uint>(10);
			_worldPacket.HasBit(); // Loaded
			_worldPacket.HasBit(); // Disabled
			if (taintNameLen > 0)
				_worldPacket.ReadBytes(taintNameLen);
			if (taintVersionLen > 0)
				_worldPacket.ReadBytes(taintVersionLen);
		}

		var size = _worldPacket.ReadUInt32();
		var data = _worldPacket.ReadBytes(size);
		var sorts = new WorldPacket(_worldPacket.GetOpcode(), data);
		for (var k = 0; k < sortCount; k++)
		{
			var sort = new AuctionSort
			{
				Type = sorts.ReadUInt8(),
				Direction = sorts.ReadUInt8()
			};
			Sorts.Add(sort);
		}
	}
}
