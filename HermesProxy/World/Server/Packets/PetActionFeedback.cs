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
		base._worldPacket.WriteInt32(this.SpellID);
		base._worldPacket.WriteUInt8(this.Response);
	}
}
