using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class SpellCastData
{
	public WowGuid128 CasterGUID;

	public WowGuid128 CasterUnit;

	public WowGuid128 CastID = WowGuid128.Empty;

	public readonly WowGuid128 OriginalCastID = WowGuid128.Empty;

	public int SpellID;

	public uint SpellXSpellVisualID;

	public uint CastFlags;

	public uint CastFlagsEx;

	public uint CastTime;

	public readonly List<WowGuid128> HitTargets = new();

	public readonly List<WowGuid128> MissTargets = new();

	public readonly List<SpellMissStatus> MissStatus = new();

	public readonly SpellTargetData Target = new();

	public readonly List<SpellPowerData> RemainingPower = new();

	public RuneData RemainingRunes;

	public MissileTrajectoryResult MissileTrajectory;

	public int? AmmoDisplayId;

	public int? AmmoInventoryType;

	public byte DestLocSpellCastIndex;

	public readonly List<TargetLocation> TargetPoints = new();

	public CreatureImmunities Immunities;

	public readonly SpellHealPrediction Predict = new();

	public void Write(WorldPacket data)
	{
		data.WritePackedGuid128(CasterGUID);
		data.WritePackedGuid128(CasterUnit);
		data.WritePackedGuid128(CastID);
		data.WritePackedGuid128(OriginalCastID);
		data.WriteInt32(SpellID);
		data.WriteUInt32(SpellXSpellVisualID);
		data.WriteUInt32(CastFlags);
		data.WriteUInt32(CastFlagsEx);
		data.WriteUInt32(CastTime);
		MissileTrajectory.Write(data);
		data.WriteUInt8(DestLocSpellCastIndex);
		Immunities.Write(data);
		Predict.Write(data);
		data.WriteBits(HitTargets.Count, 16);
		data.WriteBits(MissTargets.Count, 16);
		data.WriteBits(MissStatus.Count, 16);
		data.WriteBits(RemainingPower.Count, 9);
		data.WriteBit(RemainingRunes != null);
		data.WriteBits(TargetPoints.Count, 16);
		data.WriteBit(AmmoDisplayId.HasValue);
		data.WriteBit(AmmoInventoryType.HasValue);
		data.FlushBits();
		Target.Write(data);
		foreach (var hitTarget in HitTargets)
		{
			data.WritePackedGuid128(hitTarget);
		}
		foreach (var missTarget in MissTargets)
		{
			data.WritePackedGuid128(missTarget);
		}
		foreach (var item in MissStatus)
		{
			item.Write(data);
		}
		foreach (var item2 in RemainingPower)
		{
			item2.Write(data);
		}
		if (RemainingRunes != null)
		{
			RemainingRunes.Write(data);
		}
		foreach (var targetLoc in TargetPoints)
		{
			targetLoc.Write(data);
		}
		if (AmmoDisplayId.HasValue)
		{
			data.WriteInt32(AmmoDisplayId.Value);
		}
		if (AmmoInventoryType.HasValue)
		{
			data.WriteInt32(AmmoInventoryType.Value);
		}
	}
}
