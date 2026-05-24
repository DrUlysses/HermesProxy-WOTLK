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
		this.CreatureID = base._worldPacket.ReadUInt32();
		this.Guid = WowGuid128.Create(HighGuidType703.Creature, 0u, this.CreatureID, 1u);
		if (!base._worldPacket.CanRead())
		{
			return;
		}
		try
		{
			WowGuid128 packedGuid = base._worldPacket.ReadPackedGuid128();
			if (!packedGuid.IsEmpty())
			{
				this.Guid = packedGuid;
			}
		}
		catch (EndOfStreamException)
		{
		}
	}
}
