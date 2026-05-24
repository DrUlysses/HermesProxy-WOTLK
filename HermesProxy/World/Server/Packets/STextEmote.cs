using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class STextEmote : ServerPacket
{
	public WowGuid128 SourceGUID;

	public WowGuid128 SourceAccountGUID;

	public WowGuid128 TargetGUID;

	public int SoundIndex = -1;

	public int EmoteID;

	public STextEmote()
		: base(Opcode.SMSG_TEXT_EMOTE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.SourceGUID);
		base._worldPacket.WritePackedGuid128(this.SourceAccountGUID);
		base._worldPacket.WriteInt32(this.EmoteID);
		base._worldPacket.WriteInt32(this.SoundIndex);
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
	}
}
