using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpellGo : ServerPacket
{
	public SpellCastData Cast = new SpellCastData();

	public SpellCastLogData LogData;

	public SpellGo()
		: base(Opcode.SMSG_SPELL_GO, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		this.Cast.Write(base._worldPacket);
		base._worldPacket.WriteBit(this.LogData != null);
		if (this.LogData != null)
		{
			this.LogData.Write(base._worldPacket);
		}
		base._worldPacket.FlushBits();
	}
}
