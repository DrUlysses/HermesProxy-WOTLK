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
		_worldPacket.WriteUInt8(PartyIndex);
		_worldPacket.WriteUInt8(RoleCheckStatus);
		_worldPacket.WriteUInt32((uint)JoinSlots.Count);
		_worldPacket.WriteUInt32(0); // BgQueueIDs count
		_worldPacket.WriteInt32(GroupFinderActivityID);
		_worldPacket.WriteUInt32((uint)Members.Count);
		foreach (uint slot in JoinSlots)
		{
			_worldPacket.WriteUInt32(slot);
		}
		_worldPacket.WriteBit(IsBeginning);
		_worldPacket.WriteBit(IsRequeue);
		_worldPacket.FlushBits();
		foreach (LfgRoleCheckMember member in Members)
		{
			_worldPacket.WritePackedGuid128(member.Guid);
			_worldPacket.WriteUInt32(member.RolesDesired);
			_worldPacket.WriteUInt8(member.Level);
			_worldPacket.WriteBit(member.RoleCheckComplete);
			_worldPacket.FlushBits();
		}
	}
}
