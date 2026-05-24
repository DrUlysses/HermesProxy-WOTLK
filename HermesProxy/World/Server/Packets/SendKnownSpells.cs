using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SendKnownSpells : ServerPacket
{
	public bool InitialLogin;

	public List<uint> KnownSpells = new List<uint>();

	public List<uint> FavoriteSpells = new List<uint>();

	public SendKnownSpells()
		: base(Opcode.SMSG_SEND_KNOWN_SPELLS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(InitialLogin);
		_worldPacket.WriteInt32(KnownSpells.Count);
		_worldPacket.WriteInt32(FavoriteSpells.Count);
		foreach (var spellId in KnownSpells)
		{
			_worldPacket.WriteUInt32(spellId);
		}
		foreach (var spellId2 in FavoriteSpells)
		{
			_worldPacket.WriteUInt32(spellId2);
		}
	}
}
