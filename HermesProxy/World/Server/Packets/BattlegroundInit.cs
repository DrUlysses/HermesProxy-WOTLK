using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BattlegroundInit : ServerPacket
{
	public uint Milliseconds;

	public ushort BattlegroundPoints;

	public BattlegroundInit()
		: base(Opcode.SMSG_BATTLEGROUND_INIT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.Milliseconds);
		base._worldPacket.WriteUInt16(this.BattlegroundPoints);
	}
}
