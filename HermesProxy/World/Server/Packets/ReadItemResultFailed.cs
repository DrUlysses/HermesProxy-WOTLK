using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ReadItemResultFailed : ServerPacket
{
	public WowGuid128 ItemGUID;

	public uint Delay;

	public byte Subcode;

	public ReadItemResultFailed()
		: base(Opcode.SMSG_READ_ITEM_RESULT_FAILED)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ItemGUID);
		base._worldPacket.WriteUInt32(this.Delay);
		base._worldPacket.WriteBits(this.Subcode, 2);
		base._worldPacket.FlushBits();
	}
}
