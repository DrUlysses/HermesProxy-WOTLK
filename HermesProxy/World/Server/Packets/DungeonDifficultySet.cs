using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DungeonDifficultySet : ServerPacket
{
	public int DifficultyID;

	public DungeonDifficultySet()
		: base(Opcode.SMSG_SET_DUNGEON_DIFFICULTY)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.DifficultyID);
	}
}
