using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventPlayerLeft : ServerPacket
{
	public bool Removed;

	public WowGuid128 RemoverGUID;

	public uint RemoverVirtualRealmAddress;

	public string RemoverName;

	public WowGuid128 LeaverGUID;

	public uint LeaverVirtualRealmAddress;

	public string LeaverName;

	public GuildEventPlayerLeft()
		: base(Opcode.SMSG_GUILD_EVENT_PLAYER_LEFT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Removed);
		base._worldPacket.WriteBits(this.LeaverName.GetByteCount(), 6);
		if (this.Removed)
		{
			base._worldPacket.WriteBits(this.RemoverName.GetByteCount(), 6);
			base._worldPacket.WritePackedGuid128(this.RemoverGUID);
			base._worldPacket.WriteUInt32(this.RemoverVirtualRealmAddress);
			base._worldPacket.WriteString(this.RemoverName);
		}
		base._worldPacket.WritePackedGuid128(this.LeaverGUID);
		base._worldPacket.WriteUInt32(this.LeaverVirtualRealmAddress);
		base._worldPacket.WriteString(this.LeaverName);
	}
}
