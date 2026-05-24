using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellDispellLog : ServerPacket
{
	public bool IsSteal;

	public bool IsBreak;

	public WowGuid128 TargetGUID;

	public WowGuid128 CasterGUID;

	public uint DispelledBySpellID;

	public List<SpellDispellData> DispellData = new List<SpellDispellData>();

	public SpellDispellLog()
		: base(Opcode.SMSG_SPELL_DISPELL_LOG, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.IsSteal);
		base._worldPacket.WriteBit(this.IsBreak);
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.DispelledBySpellID);
		base._worldPacket.WriteInt32(this.DispellData.Count);
		foreach (SpellDispellData data in this.DispellData)
		{
			base._worldPacket.WriteUInt32(data.SpellID);
			base._worldPacket.WriteBit(data.Harmful);
			base._worldPacket.WriteBit(data.Rolled.HasValue);
			base._worldPacket.WriteBit(data.Needed.HasValue);
			if (data.Rolled.HasValue)
			{
				base._worldPacket.WriteInt32(data.Rolled.Value);
			}
			if (data.Needed.HasValue)
			{
				base._worldPacket.WriteInt32(data.Needed.Value);
			}
			base._worldPacket.FlushBits();
		}
	}
}
