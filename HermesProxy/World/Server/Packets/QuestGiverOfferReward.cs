using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverOfferReward
{
	public WowGuid128 QuestGiverGUID;

	public uint QuestGiverCreatureID = 0u;

	public uint QuestID = 0u;

	public bool AutoLaunched = false;

	public uint SuggestedPartyMembers = 0u;

	public QuestRewards Rewards = new QuestRewards();

	public List<QuestDescEmote> Emotes = new List<QuestDescEmote>();

	public uint[] QuestFlags = new uint[3];

	public void Write(WorldPacket data)
	{
		data.WritePackedGuid128(QuestGiverGUID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			data.WriteInt32((int)QuestGiverCreatureID);
		}
		data.WriteUInt32(QuestID);
		data.WriteUInt32(QuestFlags[0]);
		data.WriteUInt32(QuestFlags[1]);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			data.WriteUInt32((QuestFlags.Length > 2) ? QuestFlags[2] : 0u);
		}
		data.WriteUInt32(SuggestedPartyMembers);
		data.WriteInt32(Emotes.Count);
		foreach (QuestDescEmote emote in Emotes)
		{
			data.WriteInt32((int)emote.Type);
			data.WriteUInt32(emote.Delay);
		}
		data.WriteBit(AutoLaunched);
		data.WriteBit(bit: false);
		data.FlushBits();
		Rewards.Write(data);
	}
}
