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
		base._worldPacket.WriteUInt32(this.GameObjectID);
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.FlushBits();
		ByteBuffer statsData = new ByteBuffer();
		if (this.Allow)
		{
			statsData.WriteUInt32(this.Stats.Type);
			statsData.WriteUInt32(this.Stats.DisplayID);
			for (int i = 0; i < 4; i++)
			{
				statsData.WriteCString(this.Stats.Name[i]);
			}
			statsData.WriteCString(this.Stats.IconName);
			statsData.WriteCString(this.Stats.CastBarCaption);
			statsData.WriteCString(this.Stats.UnkString);
			int dataFieldsCount = (ModernVersion.AddedInClassicVersion(1, 14, 1, 2, 5, 3) ? 35 : 34);
			for (int j = 0; j < dataFieldsCount; j++)
			{
				statsData.WriteInt32(this.Stats.Data[j]);
			}
			statsData.WriteFloat(this.Stats.Size);
			statsData.WriteUInt8((byte)this.Stats.QuestItems.Count);
			foreach (uint questItem in this.Stats.QuestItems)
			{
				statsData.WriteUInt32(questItem);
			}
			statsData.WriteUInt32(this.Stats.ContentTuningId);
			statsData.WriteUInt32(this.Stats.RequiredLevel);
		}
		base._worldPacket.WriteUInt32(statsData.GetSize());
		if (statsData.GetSize() != 0)
		{
			base._worldPacket.WriteBytes(statsData);
		}
	}
}
