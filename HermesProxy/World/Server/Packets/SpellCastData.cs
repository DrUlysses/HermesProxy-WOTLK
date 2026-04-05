using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class SpellCastData
{
	public WowGuid128 CasterGUID;

	public WowGuid128 CasterUnit;

	public WowGuid128 CastID = WowGuid128.Empty;

	public WowGuid128 OriginalCastID = WowGuid128.Empty;

	public int SpellID;

	public uint SpellXSpellVisualID;

	public uint CastFlags;

	public uint CastFlagsEx;

	public uint CastTime;

	public List<WowGuid128> HitTargets = new List<WowGuid128>();

	public List<WowGuid128> MissTargets = new List<WowGuid128>();

	public List<SpellMissStatus> MissStatus = new List<SpellMissStatus>();

	public SpellTargetData Target = new SpellTargetData();

	public List<SpellPowerData> RemainingPower = new List<SpellPowerData>();

	public RuneData RemainingRunes;

	public MissileTrajectoryResult MissileTrajectory;

	public int? AmmoDisplayId;

	public int? AmmoInventoryType;

	public byte DestLocSpellCastIndex;

	public List<TargetLocation> TargetPoints = new List<TargetLocation>();

	public CreatureImmunities Immunities;

	public SpellHealPrediction Predict = new SpellHealPrediction();

	public void Write(WorldPacket data)
	{
		data.WritePackedGuid128(this.CasterGUID);
		data.WritePackedGuid128(this.CasterUnit);
		data.WritePackedGuid128(this.CastID);
		data.WritePackedGuid128(this.OriginalCastID);
		data.WriteInt32(this.SpellID);
		data.WriteUInt32(this.SpellXSpellVisualID);
		data.WriteUInt32(this.CastFlags);
		data.WriteUInt32(this.CastFlagsEx);
		data.WriteUInt32(this.CastTime);
		this.MissileTrajectory.Write(data);
		data.WriteUInt8(this.DestLocSpellCastIndex);
		this.Immunities.Write(data);
		this.Predict.Write(data);
		data.WriteBits(this.HitTargets.Count, 16);
		data.WriteBits(this.MissTargets.Count, 16);
		data.WriteBits(this.MissStatus.Count, 16);
		data.WriteBits(this.RemainingPower.Count, 9);
		data.WriteBit(this.RemainingRunes != null);
		data.WriteBits(this.TargetPoints.Count, 16);
		data.WriteBit(this.AmmoDisplayId.HasValue);
		data.WriteBit(this.AmmoInventoryType.HasValue);
		data.FlushBits();
		this.Target.Write(data);
		foreach (WowGuid128 hitTarget in this.HitTargets)
		{
			data.WritePackedGuid128(hitTarget);
		}
		foreach (WowGuid128 missTarget in this.MissTargets)
		{
			data.WritePackedGuid128(missTarget);
		}
		foreach (SpellMissStatus item in this.MissStatus)
		{
			item.Write(data);
		}
		foreach (SpellPowerData item2 in this.RemainingPower)
		{
			item2.Write(data);
		}
		if (this.RemainingRunes != null)
		{
			this.RemainingRunes.Write(data);
		}
		foreach (TargetLocation targetLoc in this.TargetPoints)
		{
			targetLoc.Write(data);
		}
		if (this.AmmoDisplayId.HasValue)
		{
			data.WriteInt32(this.AmmoDisplayId.Value);
		}
		if (this.AmmoInventoryType.HasValue)
		{
			data.WriteInt32(this.AmmoInventoryType.Value);
		}
	}
}
