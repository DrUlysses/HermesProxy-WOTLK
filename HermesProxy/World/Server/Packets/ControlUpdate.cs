using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ControlUpdate : ServerPacket
{
	public WowGuid128 Guid;

	public bool HasControl;

	public ControlUpdate()
		: base(Opcode.SMSG_CONTROL_UPDATE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteBit(this.HasControl);
		base._worldPacket.FlushBits();
	}
}
