using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class LootMoneyNotify : ServerPacket
{
	public ulong Money;

	public ulong MoneyMod;

	public bool SoleLooter;

	public LootMoneyNotify()
		: base(Opcode.SMSG_LOOT_MONEY_NOTIFY)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt64(this.Money);
		base._worldPacket.WriteUInt64(this.MoneyMod);
		base._worldPacket.WriteBit(this.SoleLooter);
		base._worldPacket.FlushBits();
	}
}
