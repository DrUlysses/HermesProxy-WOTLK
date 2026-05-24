using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

/// <summary>
/// In 3.4.3, SMSG_SHOW_BANK is replaced by SMSG_NPC_INTERACTION_OPEN_RESULT
/// with InteractionType = Banker (8). Opcode 0x288A (10378).
/// Format: PackedGuid128 Npc, int32 InteractionType, WriteBit Success.
/// </summary>
public class ShowBank : ServerPacket
{
	public WowGuid128 Guid;

	public int InteractionType = 8; // PlayerInteractionType::Banker

	public ShowBank()
		: base(Opcode.SMSG_SHOW_BANK, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Guid);
		base._worldPacket.WriteInt32(this.InteractionType);
		base._worldPacket.WriteBit(true); // Success
		base._worldPacket.FlushBits();
	}
}
