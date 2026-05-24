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
		base._worldPacket.WriteUInt32(this.TextID);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteInt32(this.Allow ? 64 : 0);
		if (this.Allow)
		{
			for (uint i = 0u; i < 8; i++)
			{
				base._worldPacket.WriteFloat(this.Probabilities[i]);
			}
			for (uint i2 = 0u; i2 < 8; i2++)
			{
				base._worldPacket.WriteUInt32(this.BroadcastTextID[i2]);
			}
		}
	}
}
