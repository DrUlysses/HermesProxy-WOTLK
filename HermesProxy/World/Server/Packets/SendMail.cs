using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class SendMail : ClientPacket
{
	public struct MailAttachment
	{
		public byte AttachPosition;

		public WowGuid128 ItemGUID;
	}

	public WowGuid128 Mailbox;

	public int StationeryID;

	public long SendMoney;

	public long Cod;

	public string Target;

	public string Subject;

	public string Body;

	public List<MailAttachment> Attachments = new List<MailAttachment>();

	public SendMail(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Mailbox = _worldPacket.ReadPackedGuid128();
		StationeryID = _worldPacket.ReadInt32();
		SendMoney = _worldPacket.ReadInt64();
		Cod = _worldPacket.ReadInt64();
		var targetLength = _worldPacket.ReadBits<uint>(9);
		var subjectLength = _worldPacket.ReadBits<uint>(9);
		var bodyLength = _worldPacket.ReadBits<uint>(11);
		var count = _worldPacket.ReadBits<uint>(5);
		Target = _worldPacket.ReadString(targetLength);
		Subject = _worldPacket.ReadString(subjectLength);
		Body = _worldPacket.ReadString(bodyLength);
		for (var i = 0; i < count; i++)
		{
			var att = new MailAttachment
			{
				AttachPosition = _worldPacket.ReadUInt8(),
				ItemGUID = _worldPacket.ReadPackedGuid128()
			};
			Attachments.Add(att);
		}
	}
}
