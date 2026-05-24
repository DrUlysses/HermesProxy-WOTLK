using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class LfgOfferContinue : ServerPacket
{
	public uint Slot;

	public LfgOfferContinue()
		: base(Opcode.SMSG_LFG_OFFER_CONTINUE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Slot);
	}
}
