using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryPetitionResponse : ServerPacket
{
	public uint PetitionID = 0u;

	public bool Allow = false;

	public PetitionInfo Info;

	public QueryPetitionResponse()
		: base(Opcode.SMSG_QUERY_PETITION_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.PetitionID);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.FlushBits();
		if (this.Allow)
		{
			this.Info.Write(base._worldPacket);
		}
	}
}
