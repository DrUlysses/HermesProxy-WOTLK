using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class MinimapPing : ServerPacket
{
	public WowGuid128 SenderGUID;

	public Vector2 Position;

	public MinimapPing()
		: base(Opcode.SMSG_MINIMAP_PING)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.SenderGUID);
		base._worldPacket.WriteVector2(this.Position);
	}
}
