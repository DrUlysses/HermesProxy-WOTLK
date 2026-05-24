using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootReleaseResponse : ServerPacket
{
	public WowGuid128 LootObj;

	public WowGuid128 Owner;

	public LootReleaseResponse()
		: base(Opcode.SMSG_LOOT_RELEASE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WritePackedGuid128(this.Owner);
	}
}
