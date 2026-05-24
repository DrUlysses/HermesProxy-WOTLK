using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellDamageShield : ServerPacket
{
	public WowGuid128 VictimGUID;

	public WowGuid128 CasterGUID;

	public uint SpellID;

	public int Damage;

	public int OriginalDamage;

	public uint OverKill;

	public uint SchoolMask;

	public uint LogAbsorbed;

	public SpellCastLogData LogData;

	public SpellDamageShield()
		: base(Opcode.SMSG_SPELL_DAMAGE_SHIELD, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.VictimGUID);
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteInt32(this.Damage);
		base._worldPacket.WriteInt32(this.OriginalDamage);
		base._worldPacket.WriteUInt32(this.OverKill);
		base._worldPacket.WriteUInt32(this.SchoolMask);
		base._worldPacket.WriteUInt32(this.LogAbsorbed);
		base._worldPacket.WriteBit(this.LogData != null);
		base._worldPacket.FlushBits();
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
	}
}
