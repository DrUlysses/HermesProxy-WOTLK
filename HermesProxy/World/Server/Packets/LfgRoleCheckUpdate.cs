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
	public readonly List<uint> JoinSlots = new();
	public int GroupFinderActivityID;
	public readonly List<LfgRoleCheckMember> Members = new();
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
		foreach (var slot in JoinSlots)
		{
			_worldPacket.WriteUInt32(slot);
		}
		_worldPacket.WriteBit(IsBeginning);
		_worldPacket.WriteBit(IsRequeue);
		_worldPacket.FlushBits();
		foreach (var member in Members)
		{
			_worldPacket.WritePackedGuid128(member.Guid);
			_worldPacket.WriteUInt32(member.RolesDesired);
			_worldPacket.WriteUInt8(member.Level);
			_worldPacket.WriteBit(member.RoleCheckComplete);
			_worldPacket.FlushBits();
		}
	}
}
