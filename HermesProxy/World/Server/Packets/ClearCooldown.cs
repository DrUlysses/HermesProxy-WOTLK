using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ClearCooldown : ServerPacket
{
	public bool IsPet;

	public uint SpellID;

	public bool ClearOnHold;

	public ClearCooldown()
		: base(Opcode.SMSG_CLEAR_COOLDOWN, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteBit(this.ClearOnHold);
		base._worldPacket.WriteBit(this.IsPet);
		base._worldPacket.FlushBits();
	}
}
