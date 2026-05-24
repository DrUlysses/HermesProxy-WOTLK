namespace HermesProxy.World.Server.Packets;

public class QuestPOIQuery : ClientPacket
{
	public int MissingQuestCount;
	public int[] MissingQuestPOIs = new int[175];

	public QuestPOIQuery(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		MissingQuestCount = _worldPacket.ReadInt32();
		for (int i = 0; i < MissingQuestCount && i < 175; i++)
		{
			MissingQuestPOIs[i] = _worldPacket.ReadInt32();
		}
	}
}
