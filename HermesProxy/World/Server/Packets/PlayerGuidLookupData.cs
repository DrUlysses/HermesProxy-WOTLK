using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PlayerGuidLookupData
{
	public bool IsDeleted;

	public WowGuid128 AccountID;

	public WowGuid128 BnetAccountID;

	public WowGuid128 GuidActual;

	public string Name = "";

	public ulong GuildClubMemberID;

	public uint VirtualRealmAddress;

	public Race RaceID = Race.None;

	public Gender Sex = Gender.None;

	public Class ClassID = Class.None;

	public byte Level;

	public byte Unused915;

	public DeclinedName DeclinedNames = new DeclinedName();

	public void Write(WorldPacket data)
	{
		data.WriteBit(IsDeleted);
		data.WriteBits(Name.GetByteCount(), 6);
		for (byte i = 0; i < 5; i++)
		{
			data.WriteBits(DeclinedNames.name[i].GetByteCount(), 7);
		}
		data.FlushBits();
		for (byte i2 = 0; i2 < 5; i2++)
		{
			data.WriteString(DeclinedNames.name[i2]);
		}
		data.WritePackedGuid128(AccountID);
		data.WritePackedGuid128(BnetAccountID);
		data.WritePackedGuid128(GuidActual);
		data.WriteUInt64(GuildClubMemberID);
		data.WriteUInt32(VirtualRealmAddress);
		data.WriteUInt8((byte)RaceID);
		data.WriteUInt8((byte)Sex);
		data.WriteUInt8((byte)ClassID);
		data.WriteUInt8(Level);
		data.WriteUInt8(Unused915);
		data.WriteString(Name);
	}
}
