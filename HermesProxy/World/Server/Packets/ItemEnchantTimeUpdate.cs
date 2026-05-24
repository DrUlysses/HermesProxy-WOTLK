using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ItemEnchantTimeUpdate : ServerPacket
{
	public WowGuid128 ItemGuid;

	public uint DurationLeft;

	public uint Slot;

	public WowGuid128 OwnerGuid;

	public ItemEnchantTimeUpdate()
		: base(Opcode.SMSG_ITEM_ENCHANT_TIME_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ItemGuid);
		base._worldPacket.WriteUInt32(this.DurationLeft);
		base._worldPacket.WriteUInt32(this.Slot);
		base._worldPacket.WritePackedGuid128(this.OwnerGuid);
	}
}
