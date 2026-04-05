using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellTargetData
{
	public SpellCastTargetFlags Flags;

	public WowGuid128 Unit;

	public WowGuid128 Item;

	public TargetLocation SrcLocation;

	public TargetLocation DstLocation;

	public float? Orientation;

	public int? MapID;

	public string Name = "";

	public void Read(WorldPacket data)
	{
		this.Flags = (SpellCastTargetFlags)data.ReadBits<uint>(28);
		if (data.HasBit())
		{
			this.SrcLocation = new TargetLocation();
		}
		if (data.HasBit())
		{
			this.DstLocation = new TargetLocation();
		}
		if (data.HasBit())
		{
			this.Orientation = 0f;
		}
		if (data.HasBit())
		{
			this.MapID = 0;
		}
		uint nameLength = data.ReadBits<uint>(7);
		this.Unit = data.ReadPackedGuid128();
		this.Item = data.ReadPackedGuid128();
		if (this.SrcLocation != null)
		{
			this.SrcLocation.Read(data);
		}
		if (this.DstLocation != null)
		{
			this.DstLocation.Read(data);
		}
		if (this.Orientation.HasValue)
		{
			this.Orientation = data.ReadFloat();
		}
		if (this.MapID.HasValue)
		{
			this.MapID = data.ReadInt32();
		}
		this.Name = data.ReadString(nameLength);
	}

	public void Write(WorldPacket data)
	{
		data.WriteBits((uint)this.Flags, 28);
		data.WriteBit(this.SrcLocation != null);
		data.WriteBit(this.DstLocation != null);
		data.WriteBit(this.Orientation.HasValue);
		data.WriteBit(this.MapID.HasValue);
		data.WriteBits(this.Name.GetByteCount(), 7);
		data.FlushBits();
		data.WritePackedGuid128(this.Unit);
		data.WritePackedGuid128(this.Item);
		if (this.SrcLocation != null)
		{
			this.SrcLocation.Write(data);
		}
		if (this.DstLocation != null)
		{
			this.DstLocation.Write(data);
		}
		if (this.Orientation.HasValue)
		{
			data.WriteFloat(this.Orientation.Value);
		}
		if (this.MapID.HasValue)
		{
			data.WriteInt32(this.MapID.Value);
		}
		data.WriteString(this.Name);
	}
}
