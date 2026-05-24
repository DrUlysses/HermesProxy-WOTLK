using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class EnvironmentalDamageLog : ServerPacket
{
	public WowGuid128 Victim;

	public EnvironmentalDamage Type;

	public int Amount;

	public int Resisted;

	public int Absorbed;

	public SpellCastLogData LogData;

	public EnvironmentalDamageLog()
		: base(Opcode.SMSG_ENVIRONMENTAL_DAMAGE_LOG)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Victim);
		base._worldPacket.WriteUInt8((byte)this.Type);
		base._worldPacket.WriteInt32(this.Amount);
		base._worldPacket.WriteInt32(this.Resisted);
		base._worldPacket.WriteInt32(this.Absorbed);
		base._worldPacket.WriteBit(this.LogData != null);
		base._worldPacket.FlushBits();
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
	}
}
