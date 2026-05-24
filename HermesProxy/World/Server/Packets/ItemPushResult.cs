using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ItemPushResult : ServerPacket
{
	public enum DisplayType
	{
		Hidden = 0,
		Received = 1,
		EncounterLoot = 2,
		Loot = 3
	}

	public WowGuid128 PlayerGUID;

	public byte Slot;

	public int SlotInBag;

	public ItemInstance Item = new ItemInstance();

	public int QuestLogItemID;

	public uint Quantity;

	public uint QuantityInInventory;

	public int DungeonEncounterID;

	public int BattlePetSpeciesID;

	public int BattlePetBreedID;

	public uint BattlePetBreedQuality;

	public int BattlePetLevel;

	public WowGuid128 ItemGUID;

	public bool Pushed;

	public DisplayType DisplayText;

	public bool Created;

	public bool IsBonusRoll;

	public bool IsEncounterLoot;

	public ItemPushResult()
		: base(Opcode.SMSG_ITEM_PUSH_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PlayerGUID);
		base._worldPacket.WriteUInt8(this.Slot);
		base._worldPacket.WriteInt32(this.SlotInBag);
		base._worldPacket.WriteInt32(this.QuestLogItemID);
		base._worldPacket.WriteUInt32(this.Quantity);
		base._worldPacket.WriteUInt32(this.QuantityInInventory);
		base._worldPacket.WriteInt32(this.DungeonEncounterID);
		base._worldPacket.WriteInt32(this.BattlePetSpeciesID);
		base._worldPacket.WriteInt32(this.BattlePetBreedID);
		base._worldPacket.WriteUInt32(this.BattlePetBreedQuality);
		base._worldPacket.WriteInt32(this.BattlePetLevel);
		base._worldPacket.WritePackedGuid128(this.ItemGUID);
		base._worldPacket.WriteBit(this.Pushed);
		base._worldPacket.WriteBit(this.Created);
		base._worldPacket.WriteBits((uint)this.DisplayText, 3);
		base._worldPacket.WriteBit(this.IsBonusRoll);
		base._worldPacket.WriteBit(this.IsEncounterLoot);
		base._worldPacket.FlushBits();
		this.Item.Write(base._worldPacket);
	}
}
