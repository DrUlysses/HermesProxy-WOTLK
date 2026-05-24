using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class BattlePetJournalLockAcquired : ServerPacket
{
	public BattlePetJournalLockAcquired()
		: base(Opcode.SMSG_BATTLE_PET_JOURNAL_LOCK_ACQUIRED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
	}
}
