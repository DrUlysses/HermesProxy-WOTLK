using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestGiverQuestListMessage : ServerPacket
{
	public WowGuid128 QuestGiverGUID;

	public uint GreetEmoteDelay;

	public uint GreetEmoteType;

	public List<ClientGossipQuest> QuestOptions = new List<ClientGossipQuest>();

	public string Greeting = "";

	public QuestGiverQuestListMessage()
		: base(Opcode.SMSG_QUEST_GIVER_QUEST_LIST_MESSAGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.QuestGiverGUID);
		base._worldPacket.WriteUInt32(this.GreetEmoteDelay);
		base._worldPacket.WriteUInt32(this.GreetEmoteType);
		base._worldPacket.WriteInt32(this.QuestOptions.Count);
		base._worldPacket.WriteBits(this.Greeting.GetByteCount(), 11);
		base._worldPacket.FlushBits();
		foreach (ClientGossipQuest quest in this.QuestOptions)
		{
			quest.Write(base._worldPacket);
		}
		base._worldPacket.WriteString(this.Greeting);
	}
}
