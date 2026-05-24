using Framework.IO;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DBReply : ServerPacket
{
	public DB2Hash TableHash;

	public uint Timestamp;

	public uint RecordID;

	public HotfixStatus Status = HotfixStatus.Invalid;

	public ByteBuffer Data = new ByteBuffer();

	public DBReply()
		: base(Opcode.SMSG_DB_REPLY)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.TableHash);
		base._worldPacket.WriteUInt32(this.RecordID);
		base._worldPacket.WriteUInt32(this.Timestamp);
		base._worldPacket.WriteBits((byte)this.Status, 3);
		base._worldPacket.WriteUInt32(this.Data.GetSize());
		base._worldPacket.WriteBytes(this.Data.GetData());
	}
}
