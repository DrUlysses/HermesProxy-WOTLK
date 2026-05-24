using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class BattlefieldStatusHeader
{
	public readonly RideTicket Ticket = new();

	public readonly List<uint> BattlefieldListIDs = new();

	public byte Unk254;

	public byte RangeMin;

	public byte RangeMax = 70;

	public byte ArenaTeamSize;

	public uint InstanceID;

	public bool IsArena;

	public bool TournamentRules;

	public void Write(WorldPacket data)
	{
		Ticket.Write(data);
		if (ModernVersion.AddedInClassicVersion(1, 14, 3, 2, 5, 4))
		{
			data.WriteUInt8(Unk254);
		}
		data.WriteInt32(BattlefieldListIDs.Count);
		data.WriteUInt8(RangeMin);
		data.WriteUInt8(RangeMax);
		data.WriteUInt8(ArenaTeamSize);
		data.WriteUInt32(InstanceID);
		foreach (var battlefieldListID in BattlefieldListIDs)
		{
			ulong bgId = battlefieldListID;
			var queueID = bgId | 0x1F10000000000000L;
			data.WriteUInt64(queueID);
		}
		data.WriteBit(IsArena);
		data.WriteBit(TournamentRules);
		data.FlushBits();
	}
}
