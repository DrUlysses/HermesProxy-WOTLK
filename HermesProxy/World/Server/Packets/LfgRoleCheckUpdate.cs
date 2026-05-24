using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LfgRoleCheckMember
{
	public WowGuid128 Guid;
	public uint RolesDesired;
	public byte Level;
	public bool RoleCheckComplete;
}

public class LfgRoleCheckUpdate : ServerPacket
{
	public byte PartyIndex;
	public byte RoleCheckStatus;
	public List<uint> JoinSlots = new List<uint>();
	public int GroupFinderActivityID;
	public List<LfgRoleCheckMember> Members = new List<LfgRoleCheckMember>();
	public bool IsBeginning;
	public bool IsRequeue;

	public LfgRoleCheckUpdate()
		: base(Opcode.SMSG_LFG_ROLE_CHECK_UPDATE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.PartyIndex);
		base._worldPacket.WriteUInt8(this.RoleCheckStatus);
		base._worldPacket.WriteUInt32((uint)this.JoinSlots.Count);
		base._worldPacket.WriteUInt32(0); // BgQueueIDs count
		base._worldPacket.WriteInt32(this.GroupFinderActivityID);
		base._worldPacket.WriteUInt32((uint)this.Members.Count);
		foreach (uint slot in this.JoinSlots)
		{
			base._worldPacket.WriteUInt32(slot);
		}
		base._worldPacket.WriteBit(this.IsBeginning);
		base._worldPacket.WriteBit(this.IsRequeue);
		base._worldPacket.FlushBits();
		foreach (LfgRoleCheckMember member in this.Members)
		{
			base._worldPacket.WritePackedGuid128(member.Guid);
			base._worldPacket.WriteUInt32(member.RolesDesired);
			base._worldPacket.WriteUInt8(member.Level);
			base._worldPacket.WriteBit(member.RoleCheckComplete);
			base._worldPacket.FlushBits();
		}
	}
}
