using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MailQueryNextTimeResult : ServerPacket
{
	public class MailNextTimeEntry
	{
		public WowGuid128 SenderGuid;

		public float TimeLeft;

		public int AltSenderID;

		public sbyte AltSenderType;

		public int StationeryID;
	}

	public float NextMailTime;

	public List<MailNextTimeEntry> Mails = new List<MailNextTimeEntry>();

	public MailQueryNextTimeResult()
		: base(Opcode.SMSG_MAIL_QUERY_NEXT_TIME_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteFloat(this.NextMailTime);
		base._worldPacket.WriteInt32(this.Mails.Count);
		foreach (MailNextTimeEntry entry in this.Mails)
		{
			base._worldPacket.WritePackedGuid128(entry.SenderGuid);
			base._worldPacket.WriteFloat(entry.TimeLeft);
			base._worldPacket.WriteInt32(entry.AltSenderID);
			base._worldPacket.WriteInt8(entry.AltSenderType);
			base._worldPacket.WriteInt32(entry.StationeryID);
		}
	}
}
