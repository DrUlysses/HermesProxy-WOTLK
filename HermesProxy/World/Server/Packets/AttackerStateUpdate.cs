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

	public readonly List<SubDamage> SubDmg = new();

	public byte VictimState;

	public int AttackerState = 0;

	public uint MeleeSpellID = 0u;

	public int BlockAmount;

	public int RageGained = 0;

	public UnkAttackerState UnkState;

	public readonly float Unk = 0f;

	public readonly ContentTuningParams ContentTuning = new();

	public SpellCastLogData LogData;

	public AttackerStateUpdate()
		: base(Opcode.SMSG_ATTACKER_STATE_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		var attackRoundInfo = new WorldPacket();
		attackRoundInfo.WriteUInt32((uint)HitInfo);
		attackRoundInfo.WritePackedGuid128(AttackerGUID);
		attackRoundInfo.WritePackedGuid128(VictimGUID);
		attackRoundInfo.WriteInt32(Damage);
		attackRoundInfo.WriteInt32(OriginalDamage);
		attackRoundInfo.WriteInt32(OverDamage);
		attackRoundInfo.WriteUInt8((byte)SubDmg.Count);
		foreach (var subDmg in SubDmg)
		{
			attackRoundInfo.WriteUInt32(subDmg.SchoolMask);
			attackRoundInfo.WriteFloat(subDmg.FloatDamage);
			attackRoundInfo.WriteInt32(subDmg.IntDamage);
			if (HitInfo.HasAnyFlag(HitInfo.FullAbsorb | HitInfo.PartialAbsorb))
			{
				attackRoundInfo.WriteInt32(subDmg.Absorbed);
			}
			if (HitInfo.HasAnyFlag(HitInfo.FullResist | HitInfo.PartialResist))
			{
				attackRoundInfo.WriteInt32(subDmg.Resisted);
			}
		}
		attackRoundInfo.WriteUInt8(VictimState);
		attackRoundInfo.WriteInt32(AttackerState);
		attackRoundInfo.WriteUInt32(MeleeSpellID);
		if (HitInfo.HasAnyFlag(HitInfo.Block))
		{
			attackRoundInfo.WriteInt32(BlockAmount);
		}
		if (HitInfo.HasAnyFlag(HitInfo.RageGain))
		{
			attackRoundInfo.WriteInt32(RageGained);
		}
		if (HitInfo.HasAnyFlag(HitInfo.Unk0))
		{
			attackRoundInfo.WriteUInt32(UnkState.State1);
			attackRoundInfo.WriteFloat(UnkState.State2);
			attackRoundInfo.WriteFloat(UnkState.State3);
			attackRoundInfo.WriteFloat(UnkState.State4);
			attackRoundInfo.WriteFloat(UnkState.State5);
			attackRoundInfo.WriteFloat(UnkState.State6);
			attackRoundInfo.WriteFloat(UnkState.State7);
			attackRoundInfo.WriteFloat(UnkState.State8);
			attackRoundInfo.WriteFloat(UnkState.State9);
			attackRoundInfo.WriteFloat(UnkState.State10);
			attackRoundInfo.WriteFloat(UnkState.State11);
			attackRoundInfo.WriteUInt32(UnkState.State12);
		}
		if (HitInfo.HasAnyFlag(HitInfo.Unk12 | HitInfo.Block))
		{
			attackRoundInfo.WriteFloat(Unk);
		}
		attackRoundInfo.WriteUInt8((byte)ContentTuning.TuningType);
		attackRoundInfo.WriteUInt8(ContentTuning.TargetLevel);
		attackRoundInfo.WriteUInt8(ContentTuning.Expansion);
		attackRoundInfo.WriteInt16(ContentTuning.PlayerLevelDelta);
		attackRoundInfo.WriteInt8(ContentTuning.TargetScalingLevelDelta);
		attackRoundInfo.WriteFloat(ContentTuning.PlayerItemLevel);
		attackRoundInfo.WriteFloat(ContentTuning.TargetItemLevel);
		attackRoundInfo.WriteUInt32(ContentTuning.ScalingHealthItemLevelCurveID);
		attackRoundInfo.WriteUInt32((uint)ContentTuning.Flags);
		attackRoundInfo.WriteInt32(0); // PlayerContentTuningID
		attackRoundInfo.WriteInt32(0); // TargetContentTuningID
		_worldPacket.WriteBit(LogData != null);
		if (LogData != null)
		{
			LogData.Write(_worldPacket);
		}
		_worldPacket.FlushBits();
		_worldPacket.WriteUInt32(attackRoundInfo.GetSize());
		_worldPacket.WriteBytes(attackRoundInfo);
	}
}
