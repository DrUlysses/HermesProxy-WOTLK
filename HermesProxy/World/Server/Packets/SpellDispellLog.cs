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
		_worldPacket.WriteBit(IsSteal);
		_worldPacket.WriteBit(IsBreak);
		_worldPacket.WritePackedGuid128(TargetGUID);
		_worldPacket.WritePackedGuid128(CasterGUID);
		_worldPacket.WriteUInt32(DispelledBySpellID);
		_worldPacket.WriteInt32(DispellData.Count);
		foreach (var data in DispellData)
		{
			_worldPacket.WriteUInt32(data.SpellID);
			_worldPacket.WriteBit(data.Harmful);
			_worldPacket.WriteBit(data.Rolled.HasValue);
			_worldPacket.WriteBit(data.Needed.HasValue);
			if (data.Rolled.HasValue)
			{
				_worldPacket.WriteInt32(data.Rolled.Value);
			}
			if (data.Needed.HasValue)
			{
				_worldPacket.WriteInt32(data.Needed.Value);
			}
			_worldPacket.FlushBits();
		}
	}
}
