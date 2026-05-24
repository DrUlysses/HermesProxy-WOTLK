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
		base._worldPacket.WriteUInt32(this.SoundEntryID);
		base._worldPacket.WritePackedGuid128(this.SourceObjectGuid);
		base._worldPacket.WriteInt32(this.BroadcastTextId);
	}
}
