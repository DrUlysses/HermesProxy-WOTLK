using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootRemoved : ServerPacket
{
	public WowGuid128 Owner;

	public WowGuid128 LootObj;

	public byte LootListID;

	public LootRemoved()
		: base(Opcode.SMSG_LOOT_REMOVED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Owner);
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WriteUInt8(this.LootListID);
	}
}
