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
		base._worldPacket.WritePackedGuid128(this.Unit);
		base._worldPacket.WriteInt32(this.Petitions.Count);
		foreach (PetitionEntry petition2 in this.Petitions)
		{
			petition2.Write(base._worldPacket);
		}
	}
}
