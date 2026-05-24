using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootAllPassed : ServerPacket
{
	public WowGuid128 LootObj;

	public LootItemData Item = new LootItemData();

	public LootAllPassed()
		: base(Opcode.SMSG_LOOT_ALL_PASSED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		this.Item.Write(base._worldPacket);
	}
}
