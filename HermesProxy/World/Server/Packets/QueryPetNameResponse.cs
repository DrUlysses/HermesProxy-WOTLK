using System;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class QueryPetNameResponse : ServerPacket
{
	public WowGuid128 UnitGUID;

	public bool Allow;

	public bool HasDeclined;

	public DeclinedName DeclinedNames = new DeclinedName();

	public long Timestamp;

	public string Name = "";

	public QueryPetNameResponse()
		: base(Opcode.SMSG_QUERY_PET_NAME_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(UnitGUID);
		_worldPacket.WriteBit(Allow);
		if (Allow)
		{
			_worldPacket.WriteBits(Name.GetByteCount(), 8);
			_worldPacket.WriteBit(HasDeclined);
			for (byte i = 0; i < 5; i++)
			{
				_worldPacket.WriteBits(DeclinedNames.name[i].GetByteCount(), 7);
			}
			for (byte i2 = 0; i2 < 5; i2++)
			{
				_worldPacket.WriteString(DeclinedNames.name[i2]);
			}
			_worldPacket.WriteInt64(Timestamp);
			_worldPacket.WriteString(Name);
		}
		_worldPacket.FlushBits();
	}
}
