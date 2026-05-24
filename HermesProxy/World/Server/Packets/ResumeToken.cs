using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ResumeToken : ServerPacket
{
	public uint SequenceIndex = 1u;

	public uint Reason = 1u;

	public ResumeToken()
		: base(Opcode.SMSG_RESUME_TOKEN, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.SequenceIndex);
		base._worldPacket.WriteBits(this.Reason, 2);
		base._worldPacket.FlushBits();
	}
}
