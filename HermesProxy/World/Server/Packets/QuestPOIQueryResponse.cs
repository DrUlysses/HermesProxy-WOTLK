using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QuestPOIBlobPoint
{
	public short X;
	public short Y;
	public short Z;
}

public class QuestPOIBlobData
{
	public int BlobIndex;
	public int ObjectiveIndex;
	public int QuestObjectiveID;
	public int QuestObjectID;
	public int MapID;
	public int UiMapID;
	public int Priority;
	public int Flags;
	public int WorldEffectID;
	public int PlayerConditionID;
	public int NavigationPlayerConditionID;
	public int SpawnTrackingID;
	public bool AlwaysAllowMergingBlobs;
	public List<QuestPOIBlobPoint> Points = new List<QuestPOIBlobPoint>();
}

public class QuestPOIData
{
	public int QuestID;
	public List<QuestPOIBlobData> Blobs = new List<QuestPOIBlobData>();
}

public class QuestPOIQueryResponse : ServerPacket
{
	public List<QuestPOIData> QuestPOIDataStats = new List<QuestPOIData>();

	public QuestPOIQueryResponse()
		: base(Opcode.SMSG_QUEST_POI_QUERY_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.QuestPOIDataStats.Count);
		base._worldPacket.WriteInt32(this.QuestPOIDataStats.Count);

		foreach (QuestPOIData questData in this.QuestPOIDataStats)
		{
			base._worldPacket.WriteInt32(questData.QuestID);
			base._worldPacket.WriteInt32(questData.Blobs.Count);

			foreach (QuestPOIBlobData blob in questData.Blobs)
			{
				base._worldPacket.WriteInt32(blob.BlobIndex);
				base._worldPacket.WriteInt32(blob.ObjectiveIndex);
				base._worldPacket.WriteInt32(blob.QuestObjectiveID);
				base._worldPacket.WriteInt32(blob.QuestObjectID);
				base._worldPacket.WriteInt32(blob.MapID);
				base._worldPacket.WriteInt32(blob.UiMapID);
				base._worldPacket.WriteInt32(blob.Priority);
				base._worldPacket.WriteInt32(blob.Flags);
				base._worldPacket.WriteInt32(blob.WorldEffectID);
				base._worldPacket.WriteInt32(blob.PlayerConditionID);
				base._worldPacket.WriteInt32(blob.NavigationPlayerConditionID);
				base._worldPacket.WriteInt32(blob.SpawnTrackingID);
				base._worldPacket.WriteInt32(blob.Points.Count);

				foreach (QuestPOIBlobPoint point in blob.Points)
				{
					base._worldPacket.WriteInt16(point.X);
					base._worldPacket.WriteInt16(point.Y);
					base._worldPacket.WriteInt16(point.Z);
				}

				base._worldPacket.WriteBit(blob.AlwaysAllowMergingBlobs);
				base._worldPacket.FlushBits();
			}
		}
	}
}
