using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PartyCommandResult : ServerPacket
{
	public string Name;

	public byte Command;

	public byte Result;

	public uint ResultData;

	public WowGuid128 ResultGUID = WowGuid128.Empty;

	public PartyCommandResult()
		: base(Opcode.SMSG_PARTY_COMMAND_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
		base._worldPacket.WriteBits(this.Command, 4);
		base._worldPacket.WriteBits(this.Result, 6);
		base._worldPacket.WriteUInt32(this.ResultData);
		base._worldPacket.WritePackedGuid128(this.ResultGUID);
		base._worldPacket.WriteString(this.Name);
	}
}
