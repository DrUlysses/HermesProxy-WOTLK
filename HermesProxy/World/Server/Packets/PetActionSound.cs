using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PetActionSound : ServerPacket
{
	public WowGuid128 UnitGUID;

	public uint Action;

	public PetActionSound()
		: base(Opcode.SMSG_PET_ACTION_SOUND)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(UnitGUID);
		_worldPacket.WriteUInt32(Action);
	}
}
