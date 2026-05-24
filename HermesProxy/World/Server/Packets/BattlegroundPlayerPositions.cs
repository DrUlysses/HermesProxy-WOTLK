using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class BattlegroundPlayerPositions : ServerPacket
{
	public readonly List<BattlegroundPlayerPosition> FlagCarriers = new();

	public BattlegroundPlayerPositions()
		: base(Opcode.SMSG_BATTLEGROUND_PLAYER_POSITIONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(FlagCarriers.Count);
		foreach (var flagCarrier in FlagCarriers)
		{
			flagCarrier.Write(_worldPacket);
		}
	}
}
