using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ServerPetitionShowList : ServerPacket
{
	public WowGuid128 Unit;

	public List<PetitionEntry> Petitions = new List<PetitionEntry>();

	public ServerPetitionShowList()
		: base(Opcode.SMSG_PETITION_SHOW_LIST)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(Unit);
		_worldPacket.WriteInt32(Petitions.Count);
		foreach (var petition2 in Petitions)
		{
			petition2.Write(_worldPacket);
		}
	}
}
