using Framework.Constants;
using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryGameObjectResponse : ServerPacket
{
	public uint GameObjectID;

	public WowGuid128 Guid;

	public bool Allow;

	public GameObjectStats Stats;

	public QueryGameObjectResponse()
		: base(Opcode.SMSG_QUERY_GAME_OBJECT_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(GameObjectID);
		_worldPacket.WritePackedGuid128(Guid);
		_worldPacket.WriteBit(Allow);
		_worldPacket.FlushBits();
		var statsData = new ByteBuffer();
		if (Allow)
		{
			statsData.WriteUInt32(Stats.Type);
			statsData.WriteUInt32(Stats.DisplayID);
			for (var i = 0; i < 4; i++)
			{
				statsData.WriteCString(Stats.Name[i]);
			}
			statsData.WriteCString(Stats.IconName);
			statsData.WriteCString(Stats.CastBarCaption);
			statsData.WriteCString(Stats.UnkString);
			var dataFieldsCount = ModernVersion.AddedInClassicVersion(1, 14, 1, 2, 5, 3) ? 35 : 34;
			for (var j = 0; j < dataFieldsCount; j++)
			{
				statsData.WriteInt32(Stats.Data[j]);
			}
			statsData.WriteFloat(Stats.Size);
			statsData.WriteUInt8((byte)Stats.QuestItems.Count);
			foreach (var questItem in Stats.QuestItems)
			{
				statsData.WriteUInt32(questItem);
			}
			statsData.WriteUInt32(Stats.ContentTuningId);
			statsData.WriteUInt32(Stats.RequiredLevel);
		}
		_worldPacket.WriteUInt32(statsData.GetSize());
		if (statsData.GetSize() != 0)
		{
			_worldPacket.WriteBytes(statsData);
		}
	}
}
