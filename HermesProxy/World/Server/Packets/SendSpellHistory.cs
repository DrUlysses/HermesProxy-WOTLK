using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendSpellHistory : ServerPacket
{
	public readonly List<SpellHistoryEntry> Entries = new();

	public SendSpellHistory()
		: base(Opcode.SMSG_SEND_SPELL_HISTORY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Entries.Count);
		Entries.ForEach(delegate(SpellHistoryEntry p)
		{
			p.Write(_worldPacket);
		});
	}
}
