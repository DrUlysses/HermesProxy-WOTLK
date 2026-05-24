using System;
using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PetStableList : ServerPacket
{
	public WowGuid128 StableMaster;

	public byte NumStableSlots;

	public List<PetStableInfo> Pets = new List<PetStableInfo>();

	public PetStableList()
		: base(Opcode.SMSG_PET_STABLE_LIST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.StableMaster);
		base._worldPacket.WriteInt32(this.Pets.Count);
		base._worldPacket.WriteUInt8(this.NumStableSlots);
		foreach (PetStableInfo pet in this.Pets)
		{
			base._worldPacket.WriteUInt32(pet.PetNumber);
			base._worldPacket.WriteUInt32(pet.CreatureID);
			base._worldPacket.WriteUInt32(pet.DisplayID);
			base._worldPacket.WriteUInt32(pet.ExperienceLevel);
			base._worldPacket.WriteUInt8(pet.LoyaltyLevel);
			base._worldPacket.WriteUInt8(pet.PetFlags);
			base._worldPacket.WriteBits(pet.PetName.GetByteCount(), 8);
			base._worldPacket.WriteString(pet.PetName);
		}
	}
}
