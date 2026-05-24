using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ConquestFormulaConstants : ServerPacket
{
	public int PvpMinCPPerWeek;

	public int PvpMaxCPPerWeek;

	public float PvpCPBaseCoefficient;

	public float PvpCPExpCoefficient;

	public float PvpCPNumerator;

	public ConquestFormulaConstants()
		: base(Opcode.SMSG_CONQUEST_FORMULA_CONSTANTS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteInt32(this.PvpMinCPPerWeek);
		base._worldPacket.WriteInt32(this.PvpMaxCPPerWeek);
		base._worldPacket.WriteFloat(this.PvpCPBaseCoefficient);
		base._worldPacket.WriteFloat(this.PvpCPExpCoefficient);
		base._worldPacket.WriteFloat(this.PvpCPNumerator);
	}
}
