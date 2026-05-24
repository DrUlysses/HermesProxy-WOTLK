using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BinderConfirm : ServerPacket
{
	public WowGuid128 Guid;

	public BinderConfirm()
		: base(Opcode.SMSG_BINDER_CONFIRM)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
	}
}
