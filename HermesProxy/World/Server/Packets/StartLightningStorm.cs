using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class StartLightningStorm : ServerPacket
{
	public uint LightningStormId;

	public StartLightningStorm()
		: base(Opcode.SMSG_START_LIGHTNING_STORM, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.LightningStormId);
	}
}
