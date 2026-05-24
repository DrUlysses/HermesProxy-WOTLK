using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class GuildBankItemInfo
{
	public readonly ItemInstance Item = new();

	public int Slot;

	public int Count;

	public int EnchantmentID;

	public int Charges;

	public int OnUseEnchantmentID;

	public uint Flags;

	public bool Locked;

	public readonly List<ItemGemData> SocketEnchant = new();
}
