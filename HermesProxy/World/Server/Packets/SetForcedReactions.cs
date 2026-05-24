using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SetForcedReactions : ServerPacket
{
	public readonly List<ForcedReaction> Reactions = new();

	public SetForcedReactions()
		: base(Opcode.SMSG_SET_FORCED_REACTIONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Reactions.Count);
		foreach (var reaction2 in Reactions)
		{
			reaction2.Write(_worldPacket);
		}
	}
}
