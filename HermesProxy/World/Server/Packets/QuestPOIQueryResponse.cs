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
		_worldPacket.WriteInt32(QuestPOIDataStats.Count);
		_worldPacket.WriteInt32(QuestPOIDataStats.Count);

		foreach (QuestPOIData questData in QuestPOIDataStats)
		{
			_worldPacket.WriteInt32(questData.QuestID);
			_worldPacket.WriteInt32(questData.Blobs.Count);

			foreach (QuestPOIBlobData blob in questData.Blobs)
			{
				_worldPacket.WriteInt32(blob.BlobIndex);
				_worldPacket.WriteInt32(blob.ObjectiveIndex);
				_worldPacket.WriteInt32(blob.QuestObjectiveID);
				_worldPacket.WriteInt32(blob.QuestObjectID);
				_worldPacket.WriteInt32(blob.MapID);
				_worldPacket.WriteInt32(blob.UiMapID);
				_worldPacket.WriteInt32(blob.Priority);
				_worldPacket.WriteInt32(blob.Flags);
				_worldPacket.WriteInt32(blob.WorldEffectID);
				_worldPacket.WriteInt32(blob.PlayerConditionID);
				_worldPacket.WriteInt32(blob.NavigationPlayerConditionID);
				_worldPacket.WriteInt32(blob.SpawnTrackingID);
				_worldPacket.WriteInt32(blob.Points.Count);

				foreach (QuestPOIBlobPoint point in blob.Points)
				{
					_worldPacket.WriteInt16(point.X);
					_worldPacket.WriteInt16(point.Y);
					_worldPacket.WriteInt16(point.Z);
				}

				_worldPacket.WriteBit(blob.AlwaysAllowMergingBlobs);
				_worldPacket.FlushBits();
			}
		}
	}
}
