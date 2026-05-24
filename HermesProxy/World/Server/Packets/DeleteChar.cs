using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DeleteChar : ServerPacket
{
	public byte Code;

	public DeleteChar()
		: base(Opcode.SMSG_DELETE_CHAR)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.Code);
	}
}
