using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootRollBroadcast : ServerPacket
{
	public WowGuid128 LootObj;

	public WowGuid128 Player;

	public int Roll;

	public RollType RollType;

	public LootItemData Item = new LootItemData();

	public bool Autopassed = false;

	public LootRollBroadcast()
		: base(Opcode.SMSG_LOOT_ROLL)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WritePackedGuid128(this.Player);
		base._worldPacket.WriteInt32(this.Roll);
		base._worldPacket.WriteUInt8((byte)this.RollType);
		this.Item.Write(base._worldPacket);
		base._worldPacket.WriteBit(this.Autopassed);
		base._worldPacket.FlushBits();
	}
}
