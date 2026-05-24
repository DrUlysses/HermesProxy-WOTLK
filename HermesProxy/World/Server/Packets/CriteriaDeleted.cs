using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CriteriaDeleted : ServerPacket
{
	public uint CriteriaID;

	public CriteriaDeleted()
		: base(Opcode.SMSG_CRITERIA_DELETED)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(CriteriaID);
	}
}
