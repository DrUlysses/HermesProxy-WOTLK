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
		base._worldPacket.WritePackedGuid128(this.TrainerGUID);
		base._worldPacket.WriteInt32(this.TrainerType);
		base._worldPacket.WriteUInt32(this.TrainerID);
		base._worldPacket.WriteInt32(this.Spells.Count);
		foreach (TrainerListSpell spell in this.Spells)
		{
			base._worldPacket.WriteUInt32(spell.SpellID);
			base._worldPacket.WriteUInt32(spell.MoneyCost);
			base._worldPacket.WriteUInt32(spell.ReqSkillLine);
			base._worldPacket.WriteUInt32(spell.ReqSkillRank);
			for (uint i = 0u; i < 3; i++)
			{
				base._worldPacket.WriteUInt32(spell.ReqAbility[i]);
			}
			base._worldPacket.WriteUInt8((byte)spell.Usable);
			base._worldPacket.WriteUInt8(spell.ReqLevel);
		}
		base._worldPacket.WriteBits(this.Greeting.GetByteCount(), 11);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Greeting);
	}
}
