using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SocialContractRequestResponse : ServerPacket
{
	public SocialContractRequestResponse()
		: base(Opcode.SMSG_SOCIAL_CONTRACT_REQUEST_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBool(data: false);
	}
}
