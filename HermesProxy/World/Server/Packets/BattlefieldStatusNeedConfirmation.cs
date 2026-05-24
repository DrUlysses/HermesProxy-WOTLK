using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BattlefieldStatusNeedConfirmation : ServerPacket
{
	public BattlefieldStatusHeader Hdr = new BattlefieldStatusHeader();

	public uint Mapid;

	public uint Timeout;

	public byte Role;

	public BattlefieldStatusNeedConfirmation()
		: base(Opcode.SMSG_BATTLEFIELD_STATUS_NEED_CONFIRMATION)
	{
	}

	protected override void Write()
	{
		this.Hdr.Write(base._worldPacket);
		base._worldPacket.WriteUInt32(this.Mapid);
		base._worldPacket.WriteUInt32(this.Timeout);
		base._worldPacket.WriteUInt8(this.Role);
	}
}
