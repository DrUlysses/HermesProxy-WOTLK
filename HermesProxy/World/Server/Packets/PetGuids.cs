using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PetGuids : ServerPacket
{
	public readonly List<WowGuid128> Guids = new();

	public PetGuids()
		: base(Opcode.SMSG_PET_GUIDS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Guids.Count);
		foreach (var guid in Guids)
		{
			_worldPacket.WritePackedGuid128(guid);
		}
	}
}
