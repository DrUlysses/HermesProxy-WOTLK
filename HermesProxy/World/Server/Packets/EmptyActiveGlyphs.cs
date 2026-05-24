using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyActiveGlyphs : ServerPacket
{
	public EmptyActiveGlyphs()
		: base(Opcode.SMSG_ACTIVE_GLYPHS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.FlushBits();
	}
}
