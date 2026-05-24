using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ReadItemResultOK : ServerPacket
{
	public WowGuid128 ItemGUID;

	public ReadItemResultOK()
		: base(Opcode.SMSG_READ_ITEM_RESULT_OK)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ItemGUID);
	}
}
