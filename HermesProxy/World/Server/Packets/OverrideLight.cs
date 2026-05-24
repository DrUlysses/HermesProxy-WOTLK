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
		_worldPacket.WriteInt32(AreaLightID);
		_worldPacket.WriteInt32(OverrideLightID);
		_worldPacket.WriteInt32(TransitionMilliseconds);
	}
}
