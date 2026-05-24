using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootRollWon : ServerPacket
{
	public WowGuid128 LootObj;

	public WowGuid128 Winner;

	public int Roll;

	public RollType RollType;

	public LootItemData Item = new LootItemData();

	public byte MainSpec;

	public LootRollWon()
		: base(Opcode.SMSG_LOOT_ROLL_WON)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WritePackedGuid128(this.Winner);
		base._worldPacket.WriteInt32(this.Roll);
		base._worldPacket.WriteUInt8((byte)this.RollType);
		this.Item.Write(base._worldPacket);
		base._worldPacket.WriteUInt8(this.MainSpec);
	}
}
