using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class StartLootRoll : ServerPacket
{
	public WowGuid128 LootObj;

	public uint MapID;

	public uint RollTime;

	public LootMethod Method = LootMethod.GroupLoot;

	public RollMask ValidRolls;

	public LootItemData Item = new LootItemData();

	public StartLootRoll()
		: base(Opcode.SMSG_LOOT_START_ROLL)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteUInt32(this.RollTime);
		base._worldPacket.WriteUInt8((byte)this.ValidRolls);
		base._worldPacket.WriteUInt8((byte)this.Method);
		this.Item.Write(base._worldPacket);
	}
}
