using System;

namespace HermesProxy.World.Server.Packets;

public class GuildRankData
{
	public byte RankID;

	public uint RankOrder;

	public uint Flags;

	public int WithdrawGoldLimit;

	public string RankName;

	public readonly uint[] TabFlags = new uint[6];

	public readonly uint[] TabWithdrawItemLimit = new uint[6];

	public void Write(WorldPacket data)
	{
		data.WriteUInt8(RankID);
		data.WriteUInt32(RankOrder);
		data.WriteUInt32(Flags);
		data.WriteInt32(WithdrawGoldLimit);
		for (byte i = 0; i < 6; i++)
		{
			data.WriteUInt32(TabFlags[i]);
			data.WriteUInt32(TabWithdrawItemLimit[i]);
		}
		data.WriteBits(RankName.GetByteCount(), 7);
		data.WriteString(RankName);
	}
}
