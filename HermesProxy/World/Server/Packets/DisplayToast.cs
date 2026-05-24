using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DisplayToast : ServerPacket
{
	public ulong Quantity;

	public byte DisplayToastMethod = 16;

	public uint QuestID;

	public bool Mailed;

	public byte Type;

	public bool BonusRoll;

	public ItemInstance ItemReward = new ItemInstance();

	public uint SpecializationID;

	public uint ItemQuantity;

	public uint CurrencyID;

	public DisplayToast()
		: base(Opcode.SMSG_DISPLAY_TOAST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt64(this.Quantity);
		base._worldPacket.WriteUInt8(this.DisplayToastMethod);
		base._worldPacket.WriteUInt32(this.QuestID);
		base._worldPacket.WriteBit(this.Mailed);
		base._worldPacket.WriteBits(this.Type, 2);
		if (this.Type == 0)
		{
			base._worldPacket.WriteBit(this.BonusRoll);
			base._worldPacket.FlushBits();
			this.ItemReward.Write(base._worldPacket);
			base._worldPacket.WriteUInt32(this.SpecializationID);
			base._worldPacket.WriteUInt32(this.ItemQuantity);
		}
		else
		{
			base._worldPacket.FlushBits();
		}
		if (this.Type == 1)
		{
			base._worldPacket.WriteUInt32(this.CurrencyID);
		}
	}
}
