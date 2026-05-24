using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class UnlearnedSpells : ServerPacket
{
	public readonly List<uint> Spells = new();

	public bool SuppressMessaging;

	public UnlearnedSpells()
		: base(Opcode.SMSG_UNLEARNED_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Spells.Count);
		foreach (var spellId in Spells)
		{
			_worldPacket.WriteUInt32(spellId);
		}
		_worldPacket.WriteBit(SuppressMessaging);
		_worldPacket.FlushBits();
	}
}
