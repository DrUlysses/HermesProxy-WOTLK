using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AchievementDeleted : ServerPacket
{
	public uint AchievementID;
	public uint Immunities;

	public AchievementDeleted()
		: base(Opcode.SMSG_ACHIEVEMENT_DELETED)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(AchievementID);
		_worldPacket.WriteUInt32(Immunities);
	}
}
