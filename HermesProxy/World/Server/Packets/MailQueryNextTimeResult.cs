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

	public List<MailNextTimeEntry> Mails = new();

	public MailQueryNextTimeResult()
		: base(Opcode.SMSG_MAIL_QUERY_NEXT_TIME_RESULT)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteFloat(NextMailTime);
		_worldPacket.WriteInt32(Mails.Count);
		foreach (var entry in Mails)
		{
			_worldPacket.WritePackedGuid128(entry.SenderGuid);
			_worldPacket.WriteFloat(entry.TimeLeft);
			_worldPacket.WriteInt32(entry.AltSenderID);
			_worldPacket.WriteInt8(entry.AltSenderType);
			_worldPacket.WriteInt32(entry.StationeryID);
		}
	}
}
