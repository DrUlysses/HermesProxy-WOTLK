using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GetAccountCharacterListResult : ServerPacket
{
	public uint Token = 0u;

	public List<AccountCharacterListEntry> CharacterList = new List<AccountCharacterListEntry>();

	public GetAccountCharacterListResult()
		: base(Opcode.SMSG_GET_ACCOUNT_CHARACTER_LIST_RESULT)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(Token);
		_worldPacket.WriteUInt32((uint)CharacterList.Count);
		_worldPacket.ResetBitPos();
		_worldPacket.WriteBit(bit: false);
		foreach (AccountCharacterListEntry entry in CharacterList)
		{
			entry.Write(_worldPacket);
		}
	}
}
