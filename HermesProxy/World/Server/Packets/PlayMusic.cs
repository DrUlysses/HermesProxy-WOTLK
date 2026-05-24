using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PlayMusic : ServerPacket
{
	public uint SoundEntryID;

	public PlayMusic()
		: base(Opcode.SMSG_PLAY_MUSIC)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.SoundEntryID);
	}
}
