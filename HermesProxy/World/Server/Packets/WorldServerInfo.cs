using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class WorldServerInfo : ServerPacket
{
	public uint DifficultyID;

	public byte IsTournamentRealm;

	public bool XRealmPvpAlert;

	public uint? RestrictedAccountMaxLevel;

	public ulong? RestrictedAccountMaxMoney;

	public uint? InstanceGroupSize;

	public WorldServerInfo()
		: base(Opcode.SMSG_WORLD_SERVER_INFO, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.DifficultyID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteBit(this.IsTournamentRealm != 0);
		}
		else
		{
			base._worldPacket.WriteUInt8(this.IsTournamentRealm);
		}
		base._worldPacket.WriteBit(this.XRealmPvpAlert);
		base._worldPacket.WriteBit(this.RestrictedAccountMaxLevel.HasValue);
		base._worldPacket.WriteBit(this.RestrictedAccountMaxMoney.HasValue);
		base._worldPacket.WriteBit(this.InstanceGroupSize.HasValue);
		if (this.RestrictedAccountMaxLevel.HasValue)
		{
			base._worldPacket.WriteUInt32(this.RestrictedAccountMaxLevel.Value);
		}
		if (this.RestrictedAccountMaxMoney.HasValue)
		{
			base._worldPacket.WriteUInt64(this.RestrictedAccountMaxMoney.Value);
		}
		if (this.InstanceGroupSize.HasValue)
		{
			base._worldPacket.WriteUInt32(this.InstanceGroupSize.Value);
		}
		base._worldPacket.FlushBits();
	}
}
