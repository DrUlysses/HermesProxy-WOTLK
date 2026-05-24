using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootRollsComplete : ServerPacket
{
	public WowGuid128 LootObj;

	public byte LootListID;

	public LootRollsComplete()
		: base(Opcode.SMSG_LOOT_ROLLS_COMPLETE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WriteUInt8(this.LootListID);
	}
}
