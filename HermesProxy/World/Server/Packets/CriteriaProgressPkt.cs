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
		data.WriteUInt32(Id);
		data.WriteUInt64(Quantity);
		data.WritePackedGuid128(Player);
		data.WriteUInt32(0); // Unused_10_1_5
		data.WriteUInt32(Flags);
		data.WritePackedTime(Date);
		data.WriteInt64(TimeFromStart);
		data.WriteInt64(TimeFromCreate);
		data.WriteBit(RafAcceptanceID.HasValue);
		data.FlushBits();
		if (RafAcceptanceID.HasValue)
		{
			data.WriteUInt64(RafAcceptanceID.Value);
		}
	}
}
