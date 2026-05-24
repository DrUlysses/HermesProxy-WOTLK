using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class OverrideLight : ServerPacket
{
	public int AreaLightID;
	public int OverrideLightID;
	public int TransitionMilliseconds;

	public OverrideLight()
		: base(Opcode.SMSG_OVERRIDE_LIGHT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.AreaLightID);
		base._worldPacket.WriteInt32(this.OverrideLightID);
		base._worldPacket.WriteInt32(this.TransitionMilliseconds);
	}
}
