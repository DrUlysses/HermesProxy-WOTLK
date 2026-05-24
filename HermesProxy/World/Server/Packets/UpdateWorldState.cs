using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class UpdateWorldState : ServerPacket
{
	public uint VariableID;

	public int Value;

	public bool Hidden;

	public UpdateWorldState()
		: base(Opcode.SMSG_UPDATE_WORLD_STATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.VariableID);
		base._worldPacket.WriteInt32(this.Value);
		base._worldPacket.WriteBit(this.Hidden);
		base._worldPacket.FlushBits();
	}
}
