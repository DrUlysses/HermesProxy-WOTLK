using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InitializeFactions : ServerPacket
{
	private const ushort MaxFactionCount = 1000;

	public int[] FactionStandings = new int[1000];

	public bool[] FactionHasBonus = new bool[1000];

	public ReputationFlags[] FactionFlags = new ReputationFlags[1000];

	public static ushort GetFactionCount()
	{
		return (ushort)((ModernVersion.ExpansionVersion >= 3) ? 1000u : 400u);
	}

	public InitializeFactions()
		: base(Opcode.SMSG_INITIALIZE_FACTIONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		ushort count = GetFactionCount();
		for (ushort i = 0; i < count; i++)
		{
			if (ModernVersion.ExpansionVersion >= 3)
			{
				_worldPacket.WriteUInt16((ushort)FactionFlags[i]);
			}
			else
			{
				_worldPacket.WriteUInt8((byte)(FactionFlags[i] & (ReputationFlags.Visible | ReputationFlags.AtWar | ReputationFlags.Hidden | ReputationFlags.Header | ReputationFlags.Peaceful | ReputationFlags.Inactive | ReputationFlags.ShowPropagated | ReputationFlags.HeaderShowsBar)));
			}
			_worldPacket.WriteInt32(FactionStandings[i]);
		}
		for (ushort i2 = 0; i2 < count; i2++)
		{
			_worldPacket.WriteBit(FactionHasBonus[i2]);
		}
		_worldPacket.FlushBits();
	}
}
