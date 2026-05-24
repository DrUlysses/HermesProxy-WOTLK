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
		base._worldPacket.WriteInt32(this.Data.Count);
		foreach (Record data in this.Data)
		{
			base._worldPacket.WriteUInt32(data.Type);
			base._worldPacket.WriteUInt32(data.Quantity);
			base._worldPacket.WriteBit(data.WeeklyQuantity.HasValue);
			base._worldPacket.WriteBit(data.MaxWeeklyQuantity.HasValue);
			base._worldPacket.WriteBit(data.TrackedQuantity.HasValue);
			base._worldPacket.WriteBit(data.MaxQuantity.HasValue);
			base._worldPacket.WriteBit(data.Unused901.HasValue);
			base._worldPacket.WriteBits(data.Flags, 5);
			base._worldPacket.FlushBits();
			if (data.WeeklyQuantity.HasValue)
			{
				base._worldPacket.WriteUInt32(data.WeeklyQuantity.Value);
			}
			if (data.MaxWeeklyQuantity.HasValue)
			{
				base._worldPacket.WriteUInt32(data.MaxWeeklyQuantity.Value);
			}
			if (data.TrackedQuantity.HasValue)
			{
				base._worldPacket.WriteUInt32(data.TrackedQuantity.Value);
			}
			if (data.MaxQuantity.HasValue)
			{
				base._worldPacket.WriteInt32(data.MaxQuantity.Value);
			}
			if (data.Unused901.HasValue)
			{
				base._worldPacket.WriteInt32(data.Unused901.Value);
			}
		}
	}
}
