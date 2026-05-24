using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GenerateRandomCharacterNameResult : ServerPacket
{
	public bool Success;

	public string Name = "";

	public GenerateRandomCharacterNameResult()
		: base(Opcode.SMSG_GENERATE_RANDOM_CHARACTER_NAME_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBool(this.Success);
		base._worldPacket.WriteBits(this.Name.Length, 6);
		base._worldPacket.WriteString(this.Name);
	}
}
