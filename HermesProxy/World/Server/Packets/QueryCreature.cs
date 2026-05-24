using System.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryCreature : ClientPacket
{
	public uint CreatureID;

	public WowGuid128 Guid;

	public QueryCreature(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		CreatureID = _worldPacket.ReadUInt32();
		Guid = WowGuid128.Create(HighGuidType703.Creature, 0u, CreatureID, 1u);
		if (!_worldPacket.CanRead())
		{
			return;
		}
		try
		{
			var packedGuid = _worldPacket.ReadPackedGuid128();
			if (!packedGuid.IsEmpty())
			{
				Guid = packedGuid;
			}
		}
		catch (EndOfStreamException)
		{
		}
	}
}
