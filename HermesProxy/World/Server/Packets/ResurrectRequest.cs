using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ResurrectRequest : ServerPacket
{
	public WowGuid128 CasterGUID;

	public uint CasterVirtualRealmAddress;

	public uint PetNumber;

	public uint SpellID;

	public bool UseTimer = false;

	public bool Sickness;

	public string Name;

	public ResurrectRequest()
		: base(Opcode.SMSG_RESURRECT_REQUEST)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.CasterGUID);
		base._worldPacket.WriteUInt32(this.CasterVirtualRealmAddress);
		base._worldPacket.WriteUInt32(this.PetNumber);
		base._worldPacket.WriteUInt32(this.SpellID);
		base._worldPacket.WriteBits(this.Name.GetByteCount(), 11);
		base._worldPacket.WriteBit(this.UseTimer);
		base._worldPacket.WriteBit(this.Sickness);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.Name);
	}
}
