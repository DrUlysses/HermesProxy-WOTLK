using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class StandStateUpdate : ServerPacket
{
	public uint AnimKitID;

	public byte StandState;

	public StandStateUpdate()
		: base(Opcode.SMSG_STAND_STATE_UPDATE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.AnimKitID);
		base._worldPacket.WriteUInt8(this.StandState);
	}
}
