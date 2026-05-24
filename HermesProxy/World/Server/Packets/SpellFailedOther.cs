using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellFailedOther : ServerPacket
{
	public WowGuid128 CasterUnit;

	public WowGuid128 CastID;

	public uint SpellID;

	public uint SpellXSpellVisualID;

	public byte Reason;

	public SpellFailedOther()
		: base(Opcode.SMSG_SPELL_FAILED_OTHER, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.CasterUnit);
		base._worldPacket.WritePackedGuid128(this.CastID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
		base._worldPacket.WriteUInt8(this.Reason);
	}
}
