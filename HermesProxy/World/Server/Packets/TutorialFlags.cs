using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TutorialFlags : ServerPacket
{
	public readonly uint[] TutorialData = new uint[8];

	public TutorialFlags()
		: base(Opcode.SMSG_TUTORIAL_FLAGS)
	{
	}

	protected override void Write()
	{
		for (byte i = 0; i < 8; i++)
		{
			_worldPacket.WriteUInt32(TutorialData[i]);
		}
	}
}
