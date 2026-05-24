using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PlayObjectSound : ServerPacket
{
	public uint SoundEntryID;

	public WowGuid128 SourceObjectGUID;

	public WowGuid128 TargetObjectGUID;

	public Vector3 Position = default;

	public int BroadcastTextID;

	public PlayObjectSound()
		: base(Opcode.SMSG_PLAY_OBJECT_SOUND)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(SoundEntryID);
		_worldPacket.WritePackedGuid128(SourceObjectGUID);
		_worldPacket.WritePackedGuid128(TargetObjectGUID);
		_worldPacket.WriteVector3(Position);
		_worldPacket.WriteInt32(BroadcastTextID);
	}
}
