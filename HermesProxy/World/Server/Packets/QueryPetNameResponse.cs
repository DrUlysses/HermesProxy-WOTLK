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
		base._worldPacket.WritePackedGuid128(this.UnitGUID);
		base._worldPacket.WriteBit(this.Allow);
		if (this.Allow)
		{
			base._worldPacket.WriteBits(this.Name.GetByteCount(), 8);
			base._worldPacket.WriteBit(this.HasDeclined);
			for (byte i = 0; i < 5; i++)
			{
				base._worldPacket.WriteBits(this.DeclinedNames.name[i].GetByteCount(), 7);
			}
			for (byte i2 = 0; i2 < 5; i2++)
			{
				base._worldPacket.WriteString(this.DeclinedNames.name[i2]);
			}
			base._worldPacket.WriteInt64(this.Timestamp);
			base._worldPacket.WriteString(this.Name);
		}
		base._worldPacket.FlushBits();
	}
}
