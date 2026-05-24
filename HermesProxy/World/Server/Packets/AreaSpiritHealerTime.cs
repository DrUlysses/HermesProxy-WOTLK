using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AreaSpiritHealerTime : ServerPacket
{
	public WowGuid128 HealerGuid;

	public uint TimeLeft;

	public AreaSpiritHealerTime()
		: base(Opcode.SMSG_AREA_SPIRIT_HEALER_TIME)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.HealerGuid);
		base._worldPacket.WriteUInt32(this.TimeLeft);
	}
}
