using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellHealLog : ServerPacket
{
	public WowGuid128 CasterGUID;

	public WowGuid128 TargetGUID;

	public uint SpellID;

	public int HealAmount;

	public int OriginalHealAmount;

	public uint OverHeal;

	public uint Absorbed;

	public bool Crit;

	public float? CritRollMade;

	public float? CritRollNeeded;

	public SpellCastLogData LogData;

	public ContentTuningParams ContentTuning;

	public SpellHealLog()
		: base(Opcode.SMSG_SPELL_HEAL_LOG, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteInt32(this.HealAmount);
		base._worldPacket.WriteInt32(this.OriginalHealAmount);
		base._worldPacket.WriteUInt32(this.OverHeal);
		base._worldPacket.WriteUInt32(this.Absorbed);
		base._worldPacket.WriteUInt32(0); // Supporters count
		base._worldPacket.WriteBit(this.Crit);
		base._worldPacket.WriteBit(this.CritRollMade.HasValue);
		base._worldPacket.WriteBit(this.CritRollNeeded.HasValue);
		base._worldPacket.WriteBit(this.LogData != null);
		base._worldPacket.WriteBit(this.ContentTuning != null);
		base._worldPacket.FlushBits();
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
		if (this.CritRollMade.HasValue)
		{
			base._worldPacket.WriteFloat(this.CritRollMade.Value);
		}
		if (this.CritRollNeeded.HasValue)
		{
			base._worldPacket.WriteFloat(this.CritRollNeeded.Value);
		}
		if (this.ContentTuning != null)
		{
			this.ContentTuning.Write(base._worldPacket);
		}
	}
}
