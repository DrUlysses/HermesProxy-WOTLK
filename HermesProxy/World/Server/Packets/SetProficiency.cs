using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SetProficiency : ServerPacket
{
	public uint ProficiencyMask;

	public byte ProficiencyClass;

	public SetProficiency()
		: base(Opcode.SMSG_SET_PROFICIENCY, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.ProficiencyMask);
		base._worldPacket.WriteUInt8(this.ProficiencyClass);
	}
}
