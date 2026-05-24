using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PetitionRenameGuildResponse : ServerPacket
{
	public WowGuid128 PetitionGuid;

	public string NewGuildName;

	public PetitionRenameGuildResponse()
		: base(Opcode.SMSG_PETITION_RENAME_GUILD_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PetitionGuid);
		base._worldPacket.WriteBits(this.NewGuildName.GetByteCount(), 7);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.NewGuildName);
	}
}
