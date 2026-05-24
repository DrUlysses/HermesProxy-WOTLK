using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class EnchantmentLog : ServerPacket
{
	public WowGuid128 Owner;

	public WowGuid128 Caster;

	public WowGuid128 ItemGUID;

	public int ItemID;

	public int Enchantment;

	public int EnchantSlot = 1;

	public EnchantmentLog()
		: base(Opcode.SMSG_ENCHANTMENT_LOG)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Owner);
		base._worldPacket.WritePackedGuid128(this.Caster);
		base._worldPacket.WritePackedGuid128(this.ItemGUID);
		base._worldPacket.WriteInt32(this.ItemID);
		base._worldPacket.WriteInt32(this.Enchantment);
		base._worldPacket.WriteInt32(this.EnchantSlot);
	}
}
