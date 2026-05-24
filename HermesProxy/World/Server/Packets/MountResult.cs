using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MountResult : ServerPacket
{
	public int Result;

	public MountResult()
		: base(Opcode.SMSG_MOUNT_RESULT, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Result);
	}
}
