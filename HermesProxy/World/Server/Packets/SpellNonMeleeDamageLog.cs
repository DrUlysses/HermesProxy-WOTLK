using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellNonMeleeDamageLog : ServerPacket
{
	public WowGuid128 TargetGUID;

	public WowGuid128 CasterGUID;

	public WowGuid128 CastID;

	public uint SpellID;

	public uint SpellXSpellVisualID;

	public int Damage;

	public int OriginalDamage;

	public int Overkill = -1;

	public byte SchoolMask;

	public int ShieldBlock;

	public int Resisted;

	public bool Periodic;

	public int Absorbed;

	public SpellHitType Flags;

	public SpellCastLogData LogData;

	public ContentTuningParams ContentTuning;

	public SpellNonMeleeDamageLog()
		: base(Opcode.SMSG_SPELL_NON_MELEE_DAMAGE_LOG, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WritePackedGuid128(this.CastID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
		base._worldPacket.WriteInt32(this.Damage);
		base._worldPacket.WriteInt32(this.OriginalDamage);
		base._worldPacket.WriteInt32(this.Overkill);
		base._worldPacket.WriteUInt8(this.SchoolMask);
		base._worldPacket.WriteInt32(this.Absorbed);
		base._worldPacket.WriteInt32(this.Resisted);
		base._worldPacket.WriteInt32(this.ShieldBlock);
		base._worldPacket.WriteUInt32(0); // WorldTextViewers count
		base._worldPacket.WriteUInt32(0); // Supporters count
		base._worldPacket.WriteBit(this.Periodic);
		base._worldPacket.WriteBits((uint)this.Flags, 7);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(this.LogData != null);
		base._worldPacket.WriteBit(this.ContentTuning != null);
		base._worldPacket.FlushBits();
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
		if (this.ContentTuning != null)
		{
			this.ContentTuning.Write(base._worldPacket);
		}
	}
}
