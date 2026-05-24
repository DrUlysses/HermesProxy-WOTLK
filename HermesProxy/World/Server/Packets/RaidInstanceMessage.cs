using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class RaidInstanceMessage : ServerPacket
{
	public InstanceResetWarningType Type;

	public uint MapID;

	public DifficultyModern DifficultyID;

	public bool Locked;

	public bool Extended;

	public RaidInstanceMessage()
		: base(Opcode.SMSG_RAID_INSTANCE_MESSAGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8((byte)this.Type);
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteUInt32((uint)this.DifficultyID);
		base._worldPacket.WriteBit(this.Locked);
		base._worldPacket.WriteBit(this.Extended);
		base._worldPacket.FlushBits();
	}
}
