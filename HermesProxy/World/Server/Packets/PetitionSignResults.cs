using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PetitionSignResults : ServerPacket
{
	public WowGuid128 Item;

	public WowGuid128 Player;

	public PetitionSignResult Error = PetitionSignResult.Ok;

	public PetitionSignResults()
		: base(Opcode.SMSG_PETITION_SIGN_RESULTS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Item);
		base._worldPacket.WritePackedGuid128(this.Player);
		base._worldPacket.WriteBits(this.Error, 4);
		base._worldPacket.FlushBits();
	}
}
