using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendSpellCharges : ServerPacket
{
	public List<SpellChargeEntry> Entries = new List<SpellChargeEntry>();

	public SendSpellCharges()
		: base(Opcode.SMSG_SEND_SPELL_CHARGES, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Entries.Count);
		this.Entries.ForEach(delegate(SpellChargeEntry p)
		{
			p.Write(base._worldPacket);
		});
	}
}
