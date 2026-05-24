using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SpellDelayed : ServerPacket
{
	public WowGuid128 CasterGUID;

	public int Delay;

	public SpellDelayed()
		: base(Opcode.SMSG_SPELL_DELAYED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteInt32(this.Delay);
	}
}
