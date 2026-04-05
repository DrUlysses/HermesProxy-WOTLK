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

	public override void Write()
	{
		this.Ticket.Write(base._worldPacket);
		base._worldPacket.WriteUInt8(this.Result);
		base._worldPacket.WriteUInt8(this.ResultDetail);
		base._worldPacket.WriteUInt32((uint)this.BlackList.Count);
		base._worldPacket.WriteUInt32(0u); // BlackListNames count
		foreach (DfJoinBlackList entry in this.BlackList)
		{
			base._worldPacket.WriteBit(entry.PlayerGuid != null);
			base._worldPacket.WriteUInt32((uint)entry.Slots.Count);
			if (entry.PlayerGuid != null)
			{
				base._worldPacket.WritePackedGuid128(entry.PlayerGuid);
			}
			foreach (DfJoinBlackListSlot slot in entry.Slots)
			{
				base._worldPacket.WriteUInt32(slot.Slot);
				base._worldPacket.WriteUInt32(slot.Reason);
				base._worldPacket.WriteInt32(slot.SubReason1);
				base._worldPacket.WriteInt32(slot.SubReason2);
				base._worldPacket.WriteUInt32(slot.SoftLock);
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

	public override void Write()
	{
		this.Ticket.Write(base._worldPacket);
		base._worldPacket.WriteUInt8(this.SubType);
		base._worldPacket.WriteUInt8(this.Reason);
		base._worldPacket.WriteUInt32((uint)this.Slots.Count);
		base._worldPacket.WriteUInt8(this.RequestedRoles);
		base._worldPacket.WriteUInt32((uint)this.SuspendedPlayers.Count);
		base._worldPacket.WriteUInt32(this.QueueMapID);
		foreach (uint slot in this.Slots)
		{
			base._worldPacket.WriteUInt32(slot);
		}
		foreach (WowGuid128 guid in this.SuspendedPlayers)
		{
			base._worldPacket.WritePackedGuid128(guid);
		}
		base._worldPacket.WriteBit(this.IsParty);
		base._worldPacket.WriteBit(this.NotifyUI);
		base._worldPacket.WriteBit(this.Joined);
		base._worldPacket.WriteBit(this.LfgJoined);
		base._worldPacket.WriteBit(this.Queued);
		base._worldPacket.WriteBit(false); // Unused
		base._worldPacket.FlushBits();
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

	public override void Write()
	{
		this.Ticket.Write(base._worldPacket);
		base._worldPacket.WriteUInt64(this.InstanceID);
		base._worldPacket.WriteUInt32(this.ProposalID);
		base._worldPacket.WriteUInt32(this.Slot);
		base._worldPacket.WriteInt8(this.State);
		base._worldPacket.WriteUInt32(this.CompletedMask);
		base._worldPacket.WriteUInt32(this.EncounterMask);
		base._worldPacket.WriteUInt32((uint)this.Players.Count);
		base._worldPacket.WriteUInt8(0); // Unused
		base._worldPacket.WriteBit(this.ValidCompletedMask);
		base._worldPacket.WriteBit(this.ProposalSilent);
		base._worldPacket.WriteBit(this.IsRequeue);
		base._worldPacket.FlushBits();
		foreach (DfProposalPlayer player in this.Players)
		{
			base._worldPacket.WriteUInt8(player.Roles);
			base._worldPacket.WriteBit(player.Me);
			base._worldPacket.WriteBit(player.SameParty);
			base._worldPacket.WriteBit(player.MyParty);
			base._worldPacket.WriteBit(player.Responded);
			base._worldPacket.WriteBit(player.Accepted);
			base._worldPacket.FlushBits();
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

	public override void Write()
	{
		this.Ticket.Write(base._worldPacket);
		base._worldPacket.WriteUInt32(this.Slot);
		base._worldPacket.WriteUInt32(this.AvgWaitTimeMe);
		base._worldPacket.WriteUInt32(this.AvgWaitTime);
		for (int i = 0; i < 3; i++)
		{
			base._worldPacket.WriteUInt32(this.AvgWaitTimeByRole[i]);
			base._worldPacket.WriteUInt8(this.LastNeeded[i]);
		}
		base._worldPacket.WriteUInt32(this.QueuedTime);
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
		this.Ticket.Read(base._worldPacket);
		this.InstanceID = base._worldPacket.ReadUInt64();
		this.ProposalID = base._worldPacket.ReadUInt32();
		this.Accepted = base._worldPacket.HasBit();
	}
}
