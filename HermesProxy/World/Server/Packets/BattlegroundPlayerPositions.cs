using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class BattlegroundPlayerPositions : ServerPacket
{
	public List<BattlegroundPlayerPosition> FlagCarriers = new List<BattlegroundPlayerPosition>();

	public BattlegroundPlayerPositions()
		: base(Opcode.SMSG_BATTLEGROUND_PLAYER_POSITIONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.FlagCarriers.Count);
		foreach (BattlegroundPlayerPosition flagCarrier in this.FlagCarriers)
		{
			flagCarrier.Write(base._worldPacket);
		}
	}
}
