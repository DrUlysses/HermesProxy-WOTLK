namespace HermesProxy.World.Server.Packets;

public class QuestRewards
{
	public uint ChoiceItemCount;

	public uint ItemCount;

	public uint Money;

	public uint XP;

	public uint ArtifactXP;

	public uint ArtifactCategoryID;

	public uint Honor;

	public uint Title;

	public uint FactionFlags;

	public readonly int[] SpellCompletionDisplayID = new int[3];

	public uint SpellCompletionID;

	public uint SkillLineID;

	public uint NumSkillUps;

	public uint TreasurePickerID;

	public readonly QuestChoiceItem[] ChoiceItems = new QuestChoiceItem[6];

	public readonly uint[] ItemID = new uint[4];

	public readonly uint[] ItemQty = new uint[4];

	public readonly uint[] FactionID = new uint[5];

	public readonly int[] FactionValue = new int[5];

	public readonly int[] FactionOverride = new int[5];

	public readonly int[] FactionCapIn = new int[5];

	public readonly uint[] CurrencyID = new uint[4];

	public readonly uint[] CurrencyQty = new uint[4];

	public bool IsBoostSpell;

	public QuestRewards()
	{
		for (var i = 0; i < 6; i++)
		{
			ChoiceItems[i] = new QuestChoiceItem();
		}
	}

	public void Write(WorldPacket data)
	{
		data.WriteUInt32(ChoiceItemCount);
		data.WriteUInt32(ItemCount);
		for (var i = 0; i < 4; i++)
		{
			data.WriteUInt32(ItemID[i]);
			data.WriteUInt32(ItemQty[i]);
		}
		data.WriteUInt32(Money);
		data.WriteUInt32(XP);
		data.WriteUInt64(ArtifactXP);
		data.WriteUInt32(ArtifactCategoryID);
		data.WriteUInt32(Honor);
		data.WriteUInt32(Title);
		data.WriteUInt32(FactionFlags);
		for (var j = 0; j < 5; j++)
		{
			data.WriteUInt32(FactionID[j]);
			data.WriteInt32(FactionValue[j]);
			data.WriteInt32(FactionOverride[j]);
			data.WriteInt32(FactionCapIn[j]);
		}
		var spellCompletionDisplayID = SpellCompletionDisplayID;
		foreach (var id in spellCompletionDisplayID)
		{
			data.WriteInt32(id);
		}
		data.WriteUInt32(SpellCompletionID);
		for (var l = 0; l < 4; l++)
		{
			data.WriteUInt32(CurrencyID[l]);
			data.WriteUInt32(CurrencyQty[l]);
		}
		data.WriteUInt32(SkillLineID);
		data.WriteUInt32(NumSkillUps);
		data.WriteUInt32(TreasurePickerID);
		var choiceItems = ChoiceItems;
		foreach (var choice in choiceItems)
		{
			choice.Write(data);
		}
		data.WriteBit(IsBoostSpell);
		data.FlushBits();
	}
}
