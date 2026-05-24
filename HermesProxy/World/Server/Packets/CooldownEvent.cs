using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CooldownEvent : ServerPacket
{
	public bool IsPet;

	public uint SpellID;

	public CooldownEvent()
		: base(Opcode.SMSG_COOLDOWN_EVENT, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteBit(this.IsPet);
		base._worldPacket.FlushBits();
	}
}
