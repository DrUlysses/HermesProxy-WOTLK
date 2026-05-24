using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InitialSetup : ServerPacket
{
	public byte ServerExpansionLevel;

	public byte ServerExpansionTier;

	public InitialSetup()
		: base(Opcode.SMSG_INITIAL_SETUP, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8(this.ServerExpansionLevel);
		base._worldPacket.WriteUInt8(this.ServerExpansionTier);
	}
}
