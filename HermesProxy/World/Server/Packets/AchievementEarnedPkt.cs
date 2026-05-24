using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AchievementEarnedPkt : ServerPacket
{
	public WowGuid128 Sender;
	public WowGuid128 Earner;
	public uint AchievementID;
	public long Time;
	public uint EarnerNativeRealm;
	public uint EarnerVirtualRealm;
	public bool Initial;

	public AchievementEarnedPkt()
		: base(Opcode.SMSG_ACHIEVEMENT_EARNED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(Sender);
		_worldPacket.WritePackedGuid128(Earner);
		_worldPacket.WriteUInt32(AchievementID);
		_worldPacket.WritePackedTime(Time);
		_worldPacket.WriteUInt32(EarnerNativeRealm);
		_worldPacket.WriteUInt32(EarnerVirtualRealm);
		_worldPacket.WriteBit(Initial);
		_worldPacket.FlushBits();
	}
}
