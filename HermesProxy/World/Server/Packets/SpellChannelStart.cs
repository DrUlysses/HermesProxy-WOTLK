using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellChannelStart : ServerPacket
{
	public WowGuid128 CasterGUID;

	public uint SpellID;

	public uint SpellXSpellVisualID;

	public uint Duration;

	public SpellChannelStartInterruptImmunities InterruptImmunities;

	public SpellTargetedHealPrediction HealPrediction;

	public SpellChannelStart()
		: base(Opcode.SMSG_SPELL_CHANNEL_START, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
		base._worldPacket.WriteUInt32(this.Duration);
		base._worldPacket.WriteBit(this.InterruptImmunities != null);
		base._worldPacket.WriteBit(this.HealPrediction != null);
		base._worldPacket.FlushBits();
		if (this.InterruptImmunities != null)
		{
			this.InterruptImmunities.Write(base._worldPacket);
		}
		if (this.HealPrediction != null)
		{
			this.HealPrediction.Write(base._worldPacket);
		}
	}
}
