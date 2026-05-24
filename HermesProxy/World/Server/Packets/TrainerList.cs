using System;
using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TrainerList : ServerPacket
{
	public WowGuid128 TrainerGUID;

	public int TrainerType;

	public uint TrainerID = 1u;

	public List<TrainerListSpell> Spells = new List<TrainerListSpell>();

	public string Greeting;

	public TrainerList()
		: base(Opcode.SMSG_TRAINER_LIST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(TrainerGUID);
		_worldPacket.WriteInt32(TrainerType);
		_worldPacket.WriteUInt32(TrainerID);
		_worldPacket.WriteInt32(Spells.Count);
		foreach (var spell in Spells)
		{
			_worldPacket.WriteUInt32(spell.SpellID);
			_worldPacket.WriteUInt32(spell.MoneyCost);
			_worldPacket.WriteUInt32(spell.ReqSkillLine);
			_worldPacket.WriteUInt32(spell.ReqSkillRank);
			for (var i = 0u; i < 3; i++)
			{
				_worldPacket.WriteUInt32(spell.ReqAbility[i]);
			}
			_worldPacket.WriteUInt8((byte)spell.Usable);
			_worldPacket.WriteUInt8(spell.ReqLevel);
		}
		_worldPacket.WriteBits(Greeting.GetByteCount(), 11);
		_worldPacket.FlushBits();
		_worldPacket.WriteString(Greeting);
	}
}
