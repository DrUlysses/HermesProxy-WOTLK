using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ExplorationExperience : ServerPacket
{
	public uint AreaID;

	public uint Experience;

	public ExplorationExperience()
		: base(Opcode.SMSG_EXPLORATION_EXPERIENCE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.AreaID);
		base._worldPacket.WriteUInt32(this.Experience);
	}
}
