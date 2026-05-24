using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GroupDestroyed : ServerPacket
{
	public GroupDestroyed()
		: base(Opcode.SMSG_GROUP_DESTROYED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
	}
}
