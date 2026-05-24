using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellPrepare : ServerPacket
{
	public WowGuid128 ClientCastID;

	public WowGuid128 ServerCastID;

	public SpellPrepare()
		: base(Opcode.SMSG_SPELL_PREPARE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.ClientCastID);
		base._worldPacket.WritePackedGuid128(this.ServerCastID);
	}
}
