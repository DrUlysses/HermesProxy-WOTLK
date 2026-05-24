using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CharacterRenameResult : ServerPacket
{
	public string Name = "";

	public byte Result = 0;

	public WowGuid128 Guid;

	public CharacterRenameResult()
		: base(Opcode.SMSG_CHARACTER_RENAME_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.Result);
		base._worldPacket.WriteBit(this.Guid != null);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
		base._worldPacket.FlushBits();
		if (this.Guid != null)
		{
			base._worldPacket.WritePackedGuid128(this.Guid);
		}
		base._worldPacket.WriteString(this.Name);
	}
}
