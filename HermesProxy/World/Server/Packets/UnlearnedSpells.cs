using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class UnlearnedSpells : ServerPacket
{
	public List<uint> Spells = new List<uint>();

	public bool SuppressMessaging;

	public UnlearnedSpells()
		: base(Opcode.SMSG_UNLEARNED_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Spells.Count);
		foreach (uint spellId in this.Spells)
		{
			base._worldPacket.WriteUInt32(spellId);
		}
		base._worldPacket.WriteBit(this.SuppressMessaging);
		base._worldPacket.FlushBits();
	}
}
