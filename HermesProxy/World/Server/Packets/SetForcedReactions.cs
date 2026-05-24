using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SetForcedReactions : ServerPacket
{
	public List<ForcedReaction> Reactions = new List<ForcedReaction>();

	public SetForcedReactions()
		: base(Opcode.SMSG_SET_FORCED_REACTIONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.Reactions.Count);
		foreach (ForcedReaction reaction2 in this.Reactions)
		{
			reaction2.Write(base._worldPacket);
		}
	}
}
