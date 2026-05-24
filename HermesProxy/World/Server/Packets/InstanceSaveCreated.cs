using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class InstanceSaveCreated : ServerPacket
{
	public bool Gm;

	public InstanceSaveCreated()
		: base(Opcode.SMSG_INSTANCE_SAVE_CREATED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Gm);
		base._worldPacket.FlushBits();
	}
}
