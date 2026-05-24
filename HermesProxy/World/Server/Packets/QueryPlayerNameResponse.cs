using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryPlayerNameResponse : ServerPacket
{
	public WowGuid128 Player;

	public byte Result;

	public PlayerGuidLookupData Data;

	public QueryPlayerNameResponse()
		: base(Opcode.SMSG_QUERY_PLAYER_NAME_RESPONSE)
	{
		this.Data = new PlayerGuidLookupData();
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt8((sbyte)this.Result);
		base._worldPacket.WritePackedGuid128(this.Player);
		if (this.Result == 0)
		{
			this.Data.Write(base._worldPacket);
		}
	}
}
