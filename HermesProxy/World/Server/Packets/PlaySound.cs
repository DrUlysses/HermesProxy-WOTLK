using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PlaySound : ServerPacket
{
	public uint SoundEntryID;

	public WowGuid128 SourceObjectGuid;

	public int BroadcastTextId;

	public PlaySound()
		: base(Opcode.SMSG_PLAY_SOUND)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(SoundEntryID);
		_worldPacket.WritePackedGuid128(SourceObjectGuid);
		_worldPacket.WriteInt32(BroadcastTextId);
	}
}
