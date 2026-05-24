using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PlaySpellVisualKit : ServerPacket
{
	public WowGuid128 Unit;

	public uint KitRecID;

	public uint KitType;

	public uint Duration;

	public bool MountedVisual = false;

	public PlaySpellVisualKit()
		: base(Opcode.SMSG_PLAY_SPELL_VISUAL_KIT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Unit);
		base._worldPacket.WriteUInt32(this.KitRecID);
		base._worldPacket.WriteUInt32(this.KitType);
		base._worldPacket.WriteUInt32(this.Duration);
		base._worldPacket.WriteBit(this.MountedVisual);
		base._worldPacket.FlushBits();
	}
}
