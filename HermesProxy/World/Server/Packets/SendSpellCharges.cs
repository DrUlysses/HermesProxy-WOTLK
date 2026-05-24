using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendSpellCharges : ServerPacket
{
	public readonly List<SpellChargeEntry> Entries = new();

	public SendSpellCharges()
		: base(Opcode.SMSG_SEND_SPELL_CHARGES, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Entries.Count);
		Entries.ForEach(delegate(SpellChargeEntry p)
		{
			p.Write(_worldPacket);
		});
	}
}
