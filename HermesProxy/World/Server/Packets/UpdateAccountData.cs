using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class UpdateAccountData : ServerPacket
{
	public WowGuid128 Player;

	public long Time;

	public uint Size;

	public uint DataType;

	public byte[] CompressedData;

	public UpdateAccountData(AccountData data)
		: base(Opcode.SMSG_UPDATE_ACCOUNT_DATA)
	{
		this.Player = data.Guid;
		this.Time = data.Timestamp;
		this.Size = data.UncompressedSize;
		this.DataType = data.Type;
		this.CompressedData = data.CompressedData;
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Player);
		base._worldPacket.WriteInt64(this.Time);
		base._worldPacket.WriteUInt32(this.Size);
		if (ModernVersion.GetAccountDataCount() <= 8)
		{
			base._worldPacket.WriteBits(this.DataType, 3);
		}
		else
		{
			base._worldPacket.WriteBits(this.DataType, 4);
		}
		if (this.CompressedData == null)
		{
			base._worldPacket.WriteUInt32(0u);
			return;
		}
		base._worldPacket.WriteInt32(this.CompressedData.Length);
		base._worldPacket.WriteBytes(this.CompressedData);
	}
}
