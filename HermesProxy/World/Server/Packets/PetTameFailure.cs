using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PetTameFailure : ServerPacket
{
	public byte Result;

	public PetTameFailure()
		: base(Opcode.SMSG_PET_TAME_FAILURE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.Result);
	}
}
