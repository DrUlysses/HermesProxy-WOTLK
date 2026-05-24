using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

// Plural/batch version of name query response for 3.4.3 (SMSG_QUERY_PLAYER_NAMES_RESPONSE)
// TC343 format: uint32 count + per-entry { Result, Guid, WriteBit(hasData), WriteBit(hasUnused920), FlushBits, [Data] }
public class QueryPlayerNamesResponse : ServerPacket
{
	public class NameCacheLookupResult
	{
		public WowGuid128 Player;
		public byte Result;
		public PlayerGuidLookupData? Data;
	}

	public List<NameCacheLookupResult> Players = new();

	public QueryPlayerNamesResponse()
		: base(Opcode.SMSG_QUERY_PLAYER_NAMES_RESPONSE)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32((uint)Players.Count);
		foreach (var result in Players)
		{
			_worldPacket.WriteUInt8(result.Result);
			_worldPacket.WritePackedGuid128(result.Player);
			_worldPacket.WriteBit(result.Result == 0 && result.Data != null); // hasData
			_worldPacket.WriteBit(false); // hasUnused920
			_worldPacket.FlushBits();
			if (result.Result == 0 && result.Data != null)
			{
				result.Data.Write(_worldPacket);
			}
		}
	}
}
