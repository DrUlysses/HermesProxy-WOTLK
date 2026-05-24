using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class InstanceResetFailed : ServerPacket
{
	public uint MapID;

	public ResetFailedReason ResetFailedReason;

	public InstanceResetFailed()
		: base(Opcode.SMSG_INSTANCE_RESET_FAILED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteBits(this.ResetFailedReason, 2);
		base._worldPacket.FlushBits();
	}
}
