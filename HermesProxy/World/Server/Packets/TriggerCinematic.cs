using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TriggerCinematic : ServerPacket
{
	public uint CinematicID;

	public readonly WowGuid128 ConversationGuid = WowGuid128.Empty;

	public TriggerCinematic()
		: base(Opcode.SMSG_TRIGGER_CINEMATIC)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(CinematicID);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WritePackedGuid128(ConversationGuid);
		}
	}
}
