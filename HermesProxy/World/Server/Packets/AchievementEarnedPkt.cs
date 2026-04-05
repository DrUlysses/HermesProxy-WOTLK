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

	public override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Sender);
		base._worldPacket.WritePackedGuid128(this.Earner);
		base._worldPacket.WriteUInt32(this.AchievementID);
		base._worldPacket.WritePackedTime(this.Time);
		base._worldPacket.WriteUInt32(this.EarnerNativeRealm);
		base._worldPacket.WriteUInt32(this.EarnerVirtualRealm);
		base._worldPacket.WriteBit(this.Initial);
		base._worldPacket.FlushBits();
	}
}
