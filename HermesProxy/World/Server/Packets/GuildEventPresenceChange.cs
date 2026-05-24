using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildEventPresenceChange : ServerPacket
{
	public WowGuid128 Guid;

	public uint VirtualRealmAddress;

	public bool LoggedOn;

	public bool Mobile;

	public string Name;

	public GuildEventPresenceChange()
		: base(Opcode.SMSG_GUILD_EVENT_PRESENCE_CHANGE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
		base._worldPacket.WriteBit(this.LoggedOn);
		base._worldPacket.WriteBit(this.Mobile);
		base._worldPacket.WriteString(this.Name);
	}
}
