using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellFailure : ServerPacket
{
	public WowGuid128 CasterUnit;

	public WowGuid128 CastID;

	public uint SpellID;

	public uint SpellXSpellVisualID;

	public ushort Reason;

	public SpellFailure()
		: base(Opcode.SMSG_SPELL_FAILURE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.CasterUnit);
		base._worldPacket.WritePackedGuid128(this.CastID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
		base._worldPacket.WriteUInt16(this.Reason);
	}
}
