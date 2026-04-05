namespace HermesProxy.World.Server.Packets;

public struct CriteriaProgressPkt
{
	public uint Id;

	public ulong Quantity;

	public WowGuid128 Player;

	public uint Flags;

	public long Date;

	public long TimeFromStart;

	public long TimeFromCreate;

	public ulong? RafAcceptanceID;

	public void Write(WorldPacket data)
	{
		data.WriteUInt32(this.Id);
		data.WriteUInt64(this.Quantity);
		data.WritePackedGuid128(this.Player);
		data.WriteUInt32(0); // Unused_10_1_5
		data.WriteUInt32(this.Flags);
		data.WritePackedTime(this.Date);
		data.WriteInt64(this.TimeFromStart);
		data.WriteInt64(this.TimeFromCreate);
		data.WriteBit(this.RafAcceptanceID.HasValue);
		data.FlushBits();
		if (this.RafAcceptanceID.HasValue)
		{
			data.WriteUInt64(this.RafAcceptanceID.Value);
		}
	}
}
