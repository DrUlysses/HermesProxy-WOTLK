using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DfJoinResult : ServerPacket
{
	public RideTicket Ticket = new RideTicket();
	public byte Result;
	public byte ResultDetail;
	public List<DfJoinBlackList> BlackList = new List<DfJoinBlackList>();

	public DfJoinResult()
		: base(Opcode.SMSG_LFG_JOIN_RESULT)
	{
	}

	protected override void Write()
	{
		Ticket.Write(_worldPacket);
		_worldPacket.WriteUInt8(Result);
		_worldPacket.WriteUInt8(ResultDetail);
		_worldPacket.WriteUInt32((uint)BlackList.Count);
		_worldPacket.WriteUInt32(0u); // BlackListNames count
		foreach (DfJoinBlackList entry in BlackList)
		{
			_worldPacket.WriteBit(entry.PlayerGuid != null);
			_worldPacket.WriteUInt32((uint)entry.Slots.Count);
			if (entry.PlayerGuid != null)
			{
				_worldPacket.WritePackedGuid128(entry.PlayerGuid);
			}
			foreach (DfJoinBlackListSlot slot in entry.Slots)
			{
				_worldPacket.WriteUInt32(slot.Slot);
				_worldPacket.WriteUInt32(slot.Reason);
				_worldPacket.WriteInt32(slot.SubReason1);
				_worldPacket.WriteInt32(slot.SubReason2);
				_worldPacket.WriteUInt32(slot.SoftLock);
			}
		}
	}
}

public class DfJoinBlackList
{
	public WowGuid128 PlayerGuid;
	public List<DfJoinBlackListSlot> Slots = new List<DfJoinBlackListSlot>();
}

public class DfJoinBlackListSlot
{
	public uint Slot;
	public uint Reason;
	public int SubReason1;
	public int SubReason2;
	public uint SoftLock;
}

public class DfUpdateStatus : ServerPacket
{
	public RideTicket Ticket = new RideTicket();
	public byte SubType;
	public byte Reason;
	public List<uint> Slots = new List<uint>();
	public byte RequestedRoles;
	public List<WowGuid128> SuspendedPlayers = new List<WowGuid128>();
	public uint QueueMapID;
	public bool IsParty;
	public bool NotifyUI;
	public bool Joined;
	public bool LfgJoined;
	public bool Queued;

	public DfUpdateStatus()
		: base(Opcode.SMSG_LFG_UPDATE_STATUS)
	{
	}

	protected override void Write()
	{
		Ticket.Write(_worldPacket);
		_worldPacket.WriteUInt8(SubType);
		_worldPacket.WriteUInt8(Reason);
		_worldPacket.WriteUInt32((uint)Slots.Count);
		_worldPacket.WriteUInt8(RequestedRoles);
		_worldPacket.WriteUInt32((uint)SuspendedPlayers.Count);
		_worldPacket.WriteUInt32(QueueMapID);
		foreach (uint slot in Slots)
		{
			_worldPacket.WriteUInt32(slot);
		}
		foreach (WowGuid128 guid in SuspendedPlayers)
		{
			_worldPacket.WritePackedGuid128(guid);
		}
		_worldPacket.WriteBit(IsParty);
		_worldPacket.WriteBit(NotifyUI);
		_worldPacket.WriteBit(Joined);
		_worldPacket.WriteBit(LfgJoined);
		_worldPacket.WriteBit(Queued);
		_worldPacket.WriteBit(false); // Unused
		_worldPacket.FlushBits();
	}
}

public class DfProposalUpdate : ServerPacket
{
	public RideTicket Ticket = new RideTicket();
	public ulong InstanceID;
	public uint ProposalID;
	public uint Slot;
	public sbyte State;
	public uint CompletedMask;
	public uint EncounterMask;
	public List<DfProposalPlayer> Players = new List<DfProposalPlayer>();
	public bool ValidCompletedMask;
	public bool ProposalSilent;
	public bool IsRequeue;

	public DfProposalUpdate()
		: base(Opcode.SMSG_LFG_PROPOSAL_UPDATE)
	{
	}

	protected override void Write()
	{
		Ticket.Write(_worldPacket);
		_worldPacket.WriteUInt64(InstanceID);
		_worldPacket.WriteUInt32(ProposalID);
		_worldPacket.WriteUInt32(Slot);
		_worldPacket.WriteInt8(State);
		_worldPacket.WriteUInt32(CompletedMask);
		_worldPacket.WriteUInt32(EncounterMask);
		_worldPacket.WriteUInt32((uint)Players.Count);
		_worldPacket.WriteUInt8(0); // Unused
		_worldPacket.WriteBit(ValidCompletedMask);
		_worldPacket.WriteBit(ProposalSilent);
		_worldPacket.WriteBit(IsRequeue);
		_worldPacket.FlushBits();
		foreach (DfProposalPlayer player in Players)
		{
			_worldPacket.WriteUInt8(player.Roles);
			_worldPacket.WriteBit(player.Me);
			_worldPacket.WriteBit(player.SameParty);
			_worldPacket.WriteBit(player.MyParty);
			_worldPacket.WriteBit(player.Responded);
			_worldPacket.WriteBit(player.Accepted);
			_worldPacket.FlushBits();
		}
	}
}

public class DfProposalPlayer
{
	public byte Roles;
	public bool Me;
	public bool SameParty;
	public bool MyParty;
	public bool Responded;
	public bool Accepted;
}

public class DfQueueStatus : ServerPacket
{
	public RideTicket Ticket = new RideTicket();
	public uint Slot;
	public uint AvgWaitTimeMe;
	public uint AvgWaitTime;
	public uint[] AvgWaitTimeByRole = new uint[3]; // Tank, Healer, DPS
	public byte[] LastNeeded = new byte[3];
	public uint QueuedTime;

	public DfQueueStatus()
		: base(Opcode.SMSG_LFG_QUEUE_STATUS)
	{
	}

	protected override void Write()
	{
		Ticket.Write(_worldPacket);
		_worldPacket.WriteUInt32(Slot);
		_worldPacket.WriteUInt32(AvgWaitTimeMe);
		_worldPacket.WriteUInt32(AvgWaitTime);
		for (int i = 0; i < 3; i++)
		{
			_worldPacket.WriteUInt32(AvgWaitTimeByRole[i]);
			_worldPacket.WriteUInt8(LastNeeded[i]);
		}
		_worldPacket.WriteUInt32(QueuedTime);
	}
}

public class DfProposalResponsePkt : ClientPacket
{
	public RideTicket Ticket = new RideTicket();
	public ulong InstanceID;
	public uint ProposalID;
	public bool Accepted;

	public DfProposalResponsePkt(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Ticket.Read(_worldPacket);
		InstanceID = _worldPacket.ReadUInt64();
		ProposalID = _worldPacket.ReadUInt32();
		Accepted = _worldPacket.HasBit();
	}
}
