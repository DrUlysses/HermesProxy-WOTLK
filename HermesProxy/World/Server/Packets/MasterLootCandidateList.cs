using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class MasterLootCandidateList : ServerPacket
{
	public WowGuid128 LootObj;

	public List<WowGuid128> Players = new List<WowGuid128>();

	public MasterLootCandidateList()
		: base(Opcode.SMSG_LOOT_MASTER_LIST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WriteInt32(this.Players.Count);
		foreach (WowGuid128 guid in this.Players)
		{
			base._worldPacket.WritePackedGuid128(guid);
		}
	}
}
