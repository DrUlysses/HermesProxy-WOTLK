using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverStatusMultiple : ServerPacket
{
	public readonly List<QuestGiverInfo> QuestGivers = new();

	public QuestGiverStatusMultiple()
		: base(Opcode.SMSG_QUEST_GIVER_STATUS_MULTIPLE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(QuestGivers.Count);
		foreach (var questGiver in QuestGivers)
		{
			_worldPacket.WritePackedGuid128(questGiver.Guid);
			_worldPacket.WriteUInt64((ulong)questGiver.Status);
		}
	}
}
