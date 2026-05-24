using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MailListEntry
{
	public int MailID;

	public MailType SenderType;

	public WowGuid128 SenderCharacter;

	public uint? AltSenderID;

	public ulong Cod;

	public int StationeryID;

	public ulong SentMoney;

	public uint Flags;

	public float DaysLeft;

	public int MailTemplateID;

	public string Subject = "";

	public string Body = "";

	public uint ItemTextId;

	public readonly List<MailAttachedItem> Attachments = new();

	/// <summary>
	/// TC343 MailListEntry write format:
	///   uint64 MailID, uint32 SenderType, uint64 Cod, int32 StationeryID,
	///   uint64 SentMoney, int32 Flags, float DaysLeft, int32 MailTemplateID,
	///   uint32 AttachmentCount,
	///   THEN based on SenderType: PackedGuid128 OR int32 AltSenderID,
	///   WriteBits Subject(8), WriteBits Body(13), FlushBits,
	///   Attachments[], Subject string, Body string
	/// </summary>
	public void Write(WorldPacket data)
	{
		data.WriteUInt64((ulong)MailID);
		data.WriteUInt32((uint)SenderType);
		data.WriteUInt64(Cod);
		data.WriteInt32(StationeryID);
		data.WriteUInt64(SentMoney);
		data.WriteInt32((int)Flags);
		data.WriteFloat(DaysLeft);
		data.WriteInt32(MailTemplateID);
		data.WriteInt32(Attachments.Count);

		// TC343: sender written unconditionally based on type (not optional bits)
		switch (SenderType)
		{
		case MailType.Normal:
			data.WritePackedGuid128(SenderCharacter ?? WowGuid128.Empty);
			break;
		case MailType.Auction:
		case MailType.Item:
		case MailType.Creature:
		case MailType.GameObject:
			data.WriteInt32((int)AltSenderID.GetValueOrDefault());
			break;
		}

		data.WriteBits(Subject.GetByteCount(), 8);
		data.WriteBits(Body.GetByteCount(), 13);
		data.FlushBits();
		Attachments.ForEach(delegate(MailAttachedItem p)
		{
			p.Write(data);
		});
		data.WriteString(Subject);
		data.WriteString(Body);
	}
}
