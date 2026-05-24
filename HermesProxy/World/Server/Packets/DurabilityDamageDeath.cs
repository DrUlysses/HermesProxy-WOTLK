using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class DurabilityDamageDeath : ServerPacket
{
	public uint Percent;

	public DurabilityDamageDeath()
		: base(Opcode.SMSG_DURABILITY_DAMAGE_DEATH)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Percent);
	}
}
