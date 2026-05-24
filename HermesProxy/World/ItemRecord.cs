namespace HermesProxy.World;

public class ItemRecord
{
	public int Id;

	public byte ClassId;

	public byte SubclassId;

	public byte Material;

	public sbyte InventoryType;

	public int RequiredLevel;

	public byte SheatheType;

	public ushort RandomProperty;

	public ushort ItemRandomSuffixGroupId;

	public sbyte SoundOverrideSubclassId;

	public ushort ScalingStatDistributionId;

	public int IconFileDataId;

	public byte ItemGroupSoundsId;

	public int ContentTuningId;

	public uint MaxDurability;

	public byte AmmoType;

	public readonly byte[] DamageType = new byte[5];

	public readonly short[] Resistances = new short[7];

	public readonly ushort[] MinDamage = new ushort[5];

	public readonly ushort[] MaxDamage = new ushort[5];
}
