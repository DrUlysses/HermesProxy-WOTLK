using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CriteriaUpdatePkt : ServerPacket
{
	public uint CriteriaID;
	public ulong Quantity;
	public WowGuid128 PlayerGUID;
	public uint Flags;
	public long CurrentTime;
	public long ElapsedTime;
	public uint CreationTime;

	public CriteriaUpdatePkt()
		: base(Opcode.SMSG_CRITERIA_UPDATE, ConnectionType.Instance)
	{
	}

	public override void Write()
	{
		base._worldPacket.WriteUInt32(this.CriteriaID);
		base._worldPacket.WriteUInt64(this.Quantity);
		base._worldPacket.WritePackedGuid128(this.PlayerGUID);
		base._worldPacket.WriteUInt32(0); // Unused_10_1_5
		base._worldPacket.WriteUInt32(this.Flags);
		base._worldPacket.WritePackedTime(this.CurrentTime);
		base._worldPacket.WriteInt64(this.ElapsedTime); // Duration<Seconds> = int64
		base._worldPacket.WriteUInt32(this.CreationTime); // Timestamp<> = uint32
		base._worldPacket.WriteBit(false); // RafAcceptanceID
		base._worldPacket.FlushBits();
	}
}
