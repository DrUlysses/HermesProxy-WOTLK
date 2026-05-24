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
		base._worldPacket.WriteBit(this.InitialLogin);
		base._worldPacket.WriteInt32(this.KnownSpells.Count);
		base._worldPacket.WriteInt32(this.FavoriteSpells.Count);
		foreach (uint spellId in this.KnownSpells)
		{
			base._worldPacket.WriteUInt32(spellId);
		}
		foreach (uint spellId2 in this.FavoriteSpells)
		{
			base._worldPacket.WriteUInt32(spellId2);
		}
	}
}
