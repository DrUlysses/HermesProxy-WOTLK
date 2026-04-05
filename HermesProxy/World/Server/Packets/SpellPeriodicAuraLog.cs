using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellPeriodicAuraLog : ServerPacket
{
	public class PeriodicalAuraLogEffectDebugInfo
	{
		public float CritRollMade { get; set; }

		public float CritRollNeeded { get; set; }
	}

	public class SpellLogEffect
	{
		public uint Effect;

		public int Amount;

		public int OriginalDamage;

		public uint OverHealOrKill;

		public uint SchoolMaskOrPower;

		public uint AbsorbedOrAmplitude;

		public uint Resisted;

		public bool Crit;

		public PeriodicalAuraLogEffectDebugInfo DebugInfo;

		public ContentTuningParams ContentTuning;

		public void Write(WorldPacket data)
		{
			data.WriteUInt32(this.Effect);
			data.WriteInt32(this.Amount);
			data.WriteInt32(this.OriginalDamage);
			data.WriteUInt32(this.OverHealOrKill);
			data.WriteUInt32(this.SchoolMaskOrPower);
			data.WriteUInt32(this.AbsorbedOrAmplitude);
			data.WriteUInt32(this.Resisted);
			data.WriteUInt32(0u); // Supporters.size() = 0
			data.WriteBit(this.Crit);
			data.WriteBit(this.DebugInfo != null);
			data.WriteBit(this.ContentTuning != null);
			data.FlushBits();
			if (this.ContentTuning != null)
			{
				this.ContentTuning.Write(data);
			}
			if (this.DebugInfo != null)
			{
				data.WriteFloat(this.DebugInfo.CritRollMade);
				data.WriteFloat(this.DebugInfo.CritRollNeeded);
			}
		}
	}

	public WowGuid128 TargetGUID;

	public WowGuid128 CasterGUID;

	public uint SpellID;

	public SpellCastLogData LogData;

	public List<SpellLogEffect> Effects = new List<SpellLogEffect>();

	public SpellPeriodicAuraLog()
		: base(Opcode.SMSG_SPELL_PERIODIC_AURA_LOG, ConnectionType.Instance)
	{
	}

	public override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteInt32(this.Effects.Count);
		base._worldPacket.WriteBit(this.LogData != null);
		base._worldPacket.FlushBits();
		foreach (SpellLogEffect effect in this.Effects)
		{
			effect.Write(base._worldPacket);
		}
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
	}
}
