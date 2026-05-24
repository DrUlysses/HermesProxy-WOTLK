using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendUnlearnSpells : ServerPacket
{
	public List<uint> Spells = new List<uint>();

	public SendUnlearnSpells()
		: base(Opcode.SMSG_SEND_UNLEARN_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Spells.Count);
		foreach (uint spell in this.Spells)
		{
			base._worldPacket.WriteUInt32(spell);
		}
	}
}
