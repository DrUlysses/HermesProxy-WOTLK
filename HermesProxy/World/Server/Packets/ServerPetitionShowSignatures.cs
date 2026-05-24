using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ServerPetitionShowSignatures : ServerPacket
{
	public struct PetitionSignature
	{
		public WowGuid128 Signer;

		public int Choice;
	}

	public WowGuid128 Item;

	public WowGuid128 Owner;

	public WowGuid128 OwnerAccountID;

	public int PetitionID = 0;

	public List<PetitionSignature> Signatures;

	public ServerPetitionShowSignatures()
		: base(Opcode.SMSG_PETITION_SHOW_SIGNATURES)
	{
		this.Signatures = new List<PetitionSignature>();
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.Item);
		base._worldPacket.WritePackedGuid128(this.Owner);
		base._worldPacket.WritePackedGuid128(this.OwnerAccountID);
		base._worldPacket.WriteInt32(this.PetitionID);
		base._worldPacket.WriteInt32(this.Signatures.Count);
		foreach (PetitionSignature signature in this.Signatures)
		{
			base._worldPacket.WritePackedGuid128(signature.Signer);
			base._worldPacket.WriteInt32(signature.Choice);
		}
	}
}
