using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryNPCTextResponse : ServerPacket
{
	public uint TextID;

	public bool Allow;

	public float[] Probabilities = new float[8];

	public uint[] BroadcastTextID = new uint[8];

	public QueryNPCTextResponse()
		: base(Opcode.SMSG_QUERY_NPC_TEXT_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(TextID);
		_worldPacket.WriteBit(Allow);
		_worldPacket.FlushBits();
		_worldPacket.WriteInt32(Allow ? 64 : 0);
		if (Allow)
		{
			for (uint i = 0u; i < 8; i++)
			{
				_worldPacket.WriteFloat(Probabilities[i]);
			}
			for (uint i2 = 0u; i2 < 8; i2++)
			{
				_worldPacket.WriteUInt32(BroadcastTextID[i2]);
			}
		}
	}
}
