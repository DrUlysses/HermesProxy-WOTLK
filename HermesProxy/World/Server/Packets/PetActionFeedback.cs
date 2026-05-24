using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PetActionFeedback : ServerPacket
{
	public int SpellID;
	public byte Response;

	public PetActionFeedback()
		: base(Opcode.SMSG_PET_ACTION_FEEDBACK, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(SpellID);
		_worldPacket.WriteUInt8(Response);
	}
}
