using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AllAccountCriteria : ServerPacket
{
	public readonly List<CriteriaProgressPkt> Progress = new();

	public AllAccountCriteria()
		: base(Opcode.SMSG_ALL_ACCOUNT_CRITERIA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Progress.Count);
		foreach (var item in Progress)
		{
			item.Write(_worldPacket);
		}
	}
}
