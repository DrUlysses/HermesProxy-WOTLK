using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TriggerCinematic : ServerPacket
{
	public uint CinematicID;

	public WowGuid128 ConversationGuid = WowGuid128.Empty;

	public TriggerCinematic()
		: base(Opcode.SMSG_TRIGGER_CINEMATIC)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.CinematicID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WritePackedGuid128(this.ConversationGuid);
		}
	}
}
