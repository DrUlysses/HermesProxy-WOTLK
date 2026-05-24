using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SetFactionStanding : ServerPacket
{
	public float ReferAFriendBonus;

	public float BonusFromAchievementSystem;

	public List<FactionStandingData> Factions = new List<FactionStandingData>();

	public bool ShowVisual;

	public SetFactionStanding()
		: base(Opcode.SMSG_SET_FACTION_STANDING, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteFloat(this.ReferAFriendBonus);
		base._worldPacket.WriteFloat(this.BonusFromAchievementSystem);
		base._worldPacket.WriteInt32(this.Factions.Count);
		foreach (FactionStandingData faction in this.Factions)
		{
			faction.Write(base._worldPacket);
		}
		base._worldPacket.WriteBit(this.ShowVisual);
		base._worldPacket.FlushBits();
	}
}
