using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class FriendStatusPkt : ServerPacket
{
	public FriendsResult FriendResult;

	public WowGuid128 Guid;

	public WowGuid128 WowAccountGuid;

	public uint VirtualRealmAddress;

	public FriendStatus Status;

	public uint AreaID;

	public uint Level;

	public Class ClassID = Class.None;

	public string Notes;

	public bool Mobile;

	public FriendStatusPkt()
		: base(Opcode.SMSG_FRIEND_STATUS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8((byte)this.FriendResult);
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WritePackedGuid128(this.WowAccountGuid);
		base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
		base._worldPacket.WriteUInt8((byte)this.Status);
		base._worldPacket.WriteUInt32(this.AreaID);
		base._worldPacket.WriteUInt32(this.Level);
		base._worldPacket.WriteUInt32((uint)this.ClassID);
		base._worldPacket.WriteBits(this.Notes.GetByteCount(), 10);
		base._worldPacket.WriteBit(this.Mobile);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Notes);
	}
}
