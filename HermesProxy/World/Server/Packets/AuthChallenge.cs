using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuthChallenge : ServerPacket
{
	public byte[] Challenge = new byte[16];

	public byte[] DosChallenge = new byte[32];

	public byte DosZeroBits;

	public AuthChallenge()
		: base(Opcode.SMSG_AUTH_CHALLENGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBytes(this.DosChallenge);
		base._worldPacket.WriteBytes(this.Challenge);
		base._worldPacket.WriteUInt8(this.DosZeroBits);
	}
}
