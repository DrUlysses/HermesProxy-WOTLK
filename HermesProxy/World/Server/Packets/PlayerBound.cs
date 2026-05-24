using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PlayerBound : ServerPacket
{
	public WowGuid128 BinderGUID;

	public uint AreaID;

	public PlayerBound()
		: base(Opcode.SMSG_PLAYER_BOUND)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.BinderGUID);
		base._worldPacket.WriteUInt32(this.AreaID);
	}
}
