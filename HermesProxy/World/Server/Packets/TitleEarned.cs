using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TitleEarned : ServerPacket
{
	public uint Index;

	public TitleEarned(Opcode opcode)
		: base(opcode, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Index);
	}
}
