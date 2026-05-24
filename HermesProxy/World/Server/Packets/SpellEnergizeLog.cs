using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellEnergizeLog : ServerPacket
{
	public WowGuid128 TargetGUID;

	public WowGuid128 CasterGUID;

	public uint SpellID;

	public PowerType Type;

	public int Amount;

	public int OverEnergize;

	public SpellCastLogData LogData;

	public SpellEnergizeLog()
		: base(Opcode.SMSG_SPELL_ENERGIZE_LOG, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteUInt32((uint)this.Type);
		base._worldPacket.WriteInt32(this.Amount);
		base._worldPacket.WriteInt32(this.OverEnergize);
		base._worldPacket.WriteBit(this.LogData != null);
		base._worldPacket.FlushBits();
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
	}
}
