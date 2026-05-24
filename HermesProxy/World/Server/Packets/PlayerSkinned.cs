using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PlayerSkinned : ServerPacket
{
	public bool FreeRepop;

	public PlayerSkinned()
		: base(Opcode.SMSG_PLAYER_SKINNED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.FreeRepop);
		base._worldPacket.FlushBits();
	}
}
