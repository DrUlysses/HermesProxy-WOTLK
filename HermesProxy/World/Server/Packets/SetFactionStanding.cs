using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SetFactionStanding : ServerPacket
{
	public float ReferAFriendBonus;

	public float BonusFromAchievementSystem;

	public readonly List<FactionStandingData> Factions = new();

	public bool ShowVisual;

	public SetFactionStanding()
		: base(Opcode.SMSG_SET_FACTION_STANDING, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteFloat(ReferAFriendBonus);
		_worldPacket.WriteFloat(BonusFromAchievementSystem);
		_worldPacket.WriteInt32(Factions.Count);
		foreach (var faction in Factions)
		{
			faction.Write(_worldPacket);
		}
		_worldPacket.WriteBit(ShowVisual);
		_worldPacket.FlushBits();
	}
}
