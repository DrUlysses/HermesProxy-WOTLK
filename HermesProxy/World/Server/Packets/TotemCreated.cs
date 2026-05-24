using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class TotemCreated : ServerPacket
{
	public byte Slot;

	public WowGuid128 Totem;

	public uint Duration;

	public uint SpellId;

	public float TimeMod = 1f;

	public bool CannotDismiss = false;

	public TotemCreated()
		: base(Opcode.SMSG_TOTEM_CREATED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.Slot);
		base._worldPacket.WritePackedGuid128(this.Totem);
		base._worldPacket.WriteUInt32(this.Duration);
		base._worldPacket.WriteUInt32(this.SpellId);
		base._worldPacket.WriteFloat(this.TimeMod);
		base._worldPacket.WriteBit(this.CannotDismiss);
		base._worldPacket.FlushBits();
	}
}
