namespace HermesProxy.World.Server.Packets;

public struct PowerUpdatePower
{
	public readonly int Power;

	public readonly byte PowerType;

	public PowerUpdatePower(int power, byte powerType)
	{
		Power = power;
		PowerType = powerType;
	}
}
