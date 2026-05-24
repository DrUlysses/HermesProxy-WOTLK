using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class GameObjectCustomAnim : ServerPacket
{
	public WowGuid128 ObjectGUID;

	public uint CustomAnim;

	public bool PlayAsDespawn;

	public GameObjectCustomAnim()
		: base(Opcode.SMSG_GAME_OBJECT_CUSTOM_ANIM, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ObjectGUID);
		base._worldPacket.WriteUInt32(this.CustomAnim);
		base._worldPacket.WriteBit(this.PlayAsDespawn);
		base._worldPacket.FlushBits();
	}
}
