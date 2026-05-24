using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootList : ServerPacket
{
	public WowGuid128 Owner;

	public WowGuid128 LootObj;

	public WowGuid128 Master;

	public WowGuid128 RoundRobinWinner;

	public LootList()
		: base(Opcode.SMSG_LOOT_LIST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Owner);
		base._worldPacket.WritePackedGuid128(this.LootObj);
		base._worldPacket.WriteBit(this.Master != null);
		base._worldPacket.WriteBit(this.RoundRobinWinner != null);
		base._worldPacket.FlushBits();
		if (this.Master != null)
		{
			base._worldPacket.WritePackedGuid128(this.Master);
		}
		if (this.RoundRobinWinner != null)
		{
			base._worldPacket.WritePackedGuid128(this.RoundRobinWinner);
		}
	}
}
