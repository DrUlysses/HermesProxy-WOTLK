using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SetupCurrency : ServerPacket
{
	public struct Record
	{
		public uint Type;

		public uint Quantity;

		public uint? WeeklyQuantity;

		public uint? MaxWeeklyQuantity;

		public uint? TrackedQuantity;

		public int? MaxQuantity;

		public int? Unused901;

		public byte Flags;
	}

	public List<Record> Data = new List<Record>();

	public SetupCurrency()
		: base(Opcode.SMSG_SETUP_CURRENCY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Data.Count);
		foreach (Record data in Data)
		{
			_worldPacket.WriteUInt32(data.Type);
			_worldPacket.WriteUInt32(data.Quantity);
			_worldPacket.WriteBit(data.WeeklyQuantity.HasValue);
			_worldPacket.WriteBit(data.MaxWeeklyQuantity.HasValue);
			_worldPacket.WriteBit(data.TrackedQuantity.HasValue);
			_worldPacket.WriteBit(data.MaxQuantity.HasValue);
			_worldPacket.WriteBit(data.Unused901.HasValue);
			_worldPacket.WriteBits(data.Flags, 5);
			_worldPacket.FlushBits();
			if (data.WeeklyQuantity.HasValue)
			{
				_worldPacket.WriteUInt32(data.WeeklyQuantity.Value);
			}
			if (data.MaxWeeklyQuantity.HasValue)
			{
				_worldPacket.WriteUInt32(data.MaxWeeklyQuantity.Value);
			}
			if (data.TrackedQuantity.HasValue)
			{
				_worldPacket.WriteUInt32(data.TrackedQuantity.Value);
			}
			if (data.MaxQuantity.HasValue)
			{
				_worldPacket.WriteInt32(data.MaxQuantity.Value);
			}
			if (data.Unused901.HasValue)
			{
				_worldPacket.WriteInt32(data.Unused901.Value);
			}
		}
	}
}
