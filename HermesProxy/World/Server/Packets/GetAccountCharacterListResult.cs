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
		base._worldPacket.WriteUInt32(this.Token);
		base._worldPacket.WriteUInt32((uint)this.CharacterList.Count);
		base._worldPacket.ResetBitPos();
		base._worldPacket.WriteBit(bit: false);
		foreach (AccountCharacterListEntry entry in this.CharacterList)
		{
			entry.Write(base._worldPacket);
		}
	}
}
