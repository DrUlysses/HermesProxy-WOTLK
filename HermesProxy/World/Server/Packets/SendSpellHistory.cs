using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendSpellHistory : ServerPacket
{
	public List<SpellHistoryEntry> Entries = new List<SpellHistoryEntry>();

	public SendSpellHistory()
		: base(Opcode.SMSG_SEND_SPELL_HISTORY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Entries.Count);
		this.Entries.ForEach(delegate(SpellHistoryEntry p)
		{
			p.Write(base._worldPacket);
		});
	}
}
