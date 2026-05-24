using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendUnlearnSpells : ServerPacket
{
	public readonly List<uint> Spells = new();

	public SendUnlearnSpells()
		: base(Opcode.SMSG_SEND_UNLEARN_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Spells.Count);
		foreach (var spell in Spells)
		{
			_worldPacket.WriteUInt32(spell);
		}
	}
}
