using System;
using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AttackerStateUpdate : ServerPacket
{
	public HitInfo HitInfo;

	public WowGuid128 AttackerGUID;

	public WowGuid128 VictimGUID;

	public int Damage;

	public int OriginalDamage;

	public int OverDamage = -1;

	public List<SubDamage> SubDmg = new List<SubDamage>();

	public byte VictimState;

	public int AttackerState = 0;

	public uint MeleeSpellID = 0u;

	public int BlockAmount;

	public int RageGained = 0;

	public UnkAttackerState UnkState;

	public float Unk = 0f;

	public ContentTuningParams ContentTuning = new ContentTuningParams();

	public SpellCastLogData LogData;

	public AttackerStateUpdate()
		: base(Opcode.SMSG_ATTACKER_STATE_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		WorldPacket attackRoundInfo = new WorldPacket();
		attackRoundInfo.WriteUInt32((uint)this.HitInfo);
		attackRoundInfo.WritePackedGuid128(this.AttackerGUID);
		attackRoundInfo.WritePackedGuid128(this.VictimGUID);
		attackRoundInfo.WriteInt32(this.Damage);
		attackRoundInfo.WriteInt32(this.OriginalDamage);
		attackRoundInfo.WriteInt32(this.OverDamage);
		attackRoundInfo.WriteUInt8((byte)this.SubDmg.Count);
		foreach (SubDamage subDmg in this.SubDmg)
		{
			attackRoundInfo.WriteUInt32(subDmg.SchoolMask);
			attackRoundInfo.WriteFloat(subDmg.FloatDamage);
			attackRoundInfo.WriteInt32(subDmg.IntDamage);
			if (this.HitInfo.HasAnyFlag(HitInfo.FullAbsorb | HitInfo.PartialAbsorb))
			{
				attackRoundInfo.WriteInt32(subDmg.Absorbed);
			}
			if (this.HitInfo.HasAnyFlag(HitInfo.FullResist | HitInfo.PartialResist))
			{
				attackRoundInfo.WriteInt32(subDmg.Resisted);
			}
		}
		attackRoundInfo.WriteUInt8(this.VictimState);
		attackRoundInfo.WriteInt32(this.AttackerState);
		attackRoundInfo.WriteUInt32(this.MeleeSpellID);
		if (this.HitInfo.HasAnyFlag(HitInfo.Block))
		{
			attackRoundInfo.WriteInt32(this.BlockAmount);
		}
		if (this.HitInfo.HasAnyFlag(HitInfo.RageGain))
		{
			attackRoundInfo.WriteInt32(this.RageGained);
		}
		if (this.HitInfo.HasAnyFlag(HitInfo.Unk0))
		{
			attackRoundInfo.WriteUInt32(this.UnkState.State1);
			attackRoundInfo.WriteFloat(this.UnkState.State2);
			attackRoundInfo.WriteFloat(this.UnkState.State3);
			attackRoundInfo.WriteFloat(this.UnkState.State4);
			attackRoundInfo.WriteFloat(this.UnkState.State5);
			attackRoundInfo.WriteFloat(this.UnkState.State6);
			attackRoundInfo.WriteFloat(this.UnkState.State7);
			attackRoundInfo.WriteFloat(this.UnkState.State8);
			attackRoundInfo.WriteFloat(this.UnkState.State9);
			attackRoundInfo.WriteFloat(this.UnkState.State10);
			attackRoundInfo.WriteFloat(this.UnkState.State11);
			attackRoundInfo.WriteUInt32(this.UnkState.State12);
		}
		if (this.HitInfo.HasAnyFlag(HitInfo.Unk12 | HitInfo.Block))
		{
			attackRoundInfo.WriteFloat(this.Unk);
		}
		attackRoundInfo.WriteUInt8((byte)this.ContentTuning.TuningType);
		attackRoundInfo.WriteUInt8(this.ContentTuning.TargetLevel);
		attackRoundInfo.WriteUInt8(this.ContentTuning.Expansion);
		attackRoundInfo.WriteInt16(this.ContentTuning.PlayerLevelDelta);
		attackRoundInfo.WriteInt8(this.ContentTuning.TargetScalingLevelDelta);
		attackRoundInfo.WriteFloat(this.ContentTuning.PlayerItemLevel);
		attackRoundInfo.WriteFloat(this.ContentTuning.TargetItemLevel);
		attackRoundInfo.WriteUInt32(this.ContentTuning.ScalingHealthItemLevelCurveID);
		attackRoundInfo.WriteUInt32((uint)this.ContentTuning.Flags);
		attackRoundInfo.WriteInt32(0); // PlayerContentTuningID
		attackRoundInfo.WriteInt32(0); // TargetContentTuningID
		base._worldPacket.WriteBit(this.LogData != null);
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
		base._worldPacket.FlushBits();
		base._worldPacket.WriteUInt32(attackRoundInfo.GetSize());
		base._worldPacket.WriteBytes(attackRoundInfo);
	}
}
