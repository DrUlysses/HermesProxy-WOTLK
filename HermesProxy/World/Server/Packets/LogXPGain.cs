using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LogXPGain : ServerPacket
{
	public WowGuid128 Victim;

	public int Original;

	public PlayerLogXPReason Reason;

	public int Amount;

	public float GroupBonus = 1f;

	public byte RAFBonus;

	public LogXPGain()
		: base(Opcode.SMSG_LOG_XP_GAIN)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Victim);
		base._worldPacket.WriteInt32(this.Original);
		base._worldPacket.WriteUInt8((byte)this.Reason);
		base._worldPacket.WriteInt32(this.Amount);
		base._worldPacket.WriteFloat(this.GroupBonus);
		base._worldPacket.WriteUInt8(this.RAFBonus);
	}
}
