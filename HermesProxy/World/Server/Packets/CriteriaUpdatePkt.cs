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

	protected override void Write()
	{
		_worldPacket.WriteUInt32(CriteriaID);
		_worldPacket.WriteUInt64(Quantity);
		_worldPacket.WritePackedGuid128(PlayerGUID);
		_worldPacket.WriteUInt32(0); // Unused_10_1_5
		_worldPacket.WriteUInt32(Flags);
		_worldPacket.WritePackedTime(CurrentTime);
		_worldPacket.WriteInt64(ElapsedTime); // Duration<Seconds> = int64
		_worldPacket.WriteUInt32(CreationTime); // Timestamp<> = uint32
		_worldPacket.WriteBit(false); // RafAcceptanceID
		_worldPacket.FlushBits();
	}
}
