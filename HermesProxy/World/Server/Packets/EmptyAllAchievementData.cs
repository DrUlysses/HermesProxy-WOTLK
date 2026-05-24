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
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteInt32(0);
	}
}
