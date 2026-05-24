using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InspectHonorStatsResultTBC : ServerPacket
{
	public WowGuid128 PlayerGUID;

	public byte LifetimeHighestRank;

	public ushort Unused1;

	public ushort YesterdayHonorableKills;

	public ushort Unused3;

	public ushort LifetimeHonorableKills;

	public uint Unused4;

	public uint Unused5;

	public uint Unused6;

	public uint Unused7;

	public uint Unused8;

	public byte Unused9;

	public InspectHonorStatsResultTBC()
		: base(Opcode.SMSG_INSPECT_HONOR_STATS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PlayerGUID);
		base._worldPacket.WriteUInt8(this.LifetimeHighestRank);
		base._worldPacket.WriteUInt16(this.Unused1);
		base._worldPacket.WriteUInt16(this.YesterdayHonorableKills);
		base._worldPacket.WriteUInt16(this.Unused3);
		base._worldPacket.WriteUInt16(this.LifetimeHonorableKills);
		base._worldPacket.WriteUInt32(this.Unused4);
		base._worldPacket.WriteUInt32(this.Unused5);
		base._worldPacket.WriteUInt32(this.Unused6);
		base._worldPacket.WriteUInt32(this.Unused7);
		base._worldPacket.WriteUInt32(this.Unused8);
		base._worldPacket.WriteUInt8(this.Unused9);
	}
}
