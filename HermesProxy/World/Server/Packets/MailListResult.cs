using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MailListResult : ServerPacket
{
	public int TotalNumRecords;

	public readonly List<MailListEntry> Mails = new();

	public MailListResult()
		: base(Opcode.SMSG_MAIL_LIST_RESULT)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32((uint)Mails.Count);
		_worldPacket.WriteInt32(TotalNumRecords);
		Mails.ForEach(delegate(MailListEntry p)
		{
			p.Write(_worldPacket);
		});
	}
}
