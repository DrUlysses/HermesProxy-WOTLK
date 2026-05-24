using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyAllAchievementData : ServerPacket
{
	public EmptyAllAchievementData()
		: base(Opcode.SMSG_ALL_ACHIEVEMENT_DATA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(0);
		_worldPacket.WriteInt32(0);
	}
}
