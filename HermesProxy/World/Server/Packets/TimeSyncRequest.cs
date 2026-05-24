using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TimeSyncRequest : ServerPacket
{
	public uint SequenceIndex;

	public TimeSyncRequest()
		: base(Opcode.SMSG_TIME_SYNC_REQUEST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.SequenceIndex);
	}
}
