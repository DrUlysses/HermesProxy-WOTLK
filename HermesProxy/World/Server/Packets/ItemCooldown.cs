using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ItemCooldown : ServerPacket
{
	public WowGuid128 ItemGuid;

	public uint SpellID;

	public uint Cooldown;

	public ItemCooldown()
		: base(Opcode.SMSG_ITEM_COOLDOWN)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ItemGuid);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.Cooldown);
	}
}
