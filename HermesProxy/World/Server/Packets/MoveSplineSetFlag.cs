using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MoveSplineSetFlag : ServerPacket
{
	public WowGuid128 MoverGUID;

	public MoveSplineSetFlag(Opcode opcode)
		: base(opcode, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.MoverGUID);
	}
}
