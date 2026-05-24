using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PartyMemberFullState : ServerPacket
{
	public bool ForEnemy;

	public ushort Level;

	public GroupMemberOnlineStatus StatusFlags;

	public int CurrentHealth;

	public int MaxHealth;

	public byte PowerType;

	public ushort CurrentPower;

	public ushort MaxPower;

	public ushort ZoneID;

	public short PositionX;

	public short PositionY;

	public short PositionZ;

	public int VehicleSeat;

	public PartyMemberPhaseStates Phases = new PartyMemberPhaseStates();

	public List<PartyMemberAuraStates> Auras = new List<PartyMemberAuraStates>();

	public PartyMemberPetStats Pet;

	public ushort PowerDisplayID;

	public ushort SpecID;

	public ushort WmoGroupID;

	public int WmoDoodadPlacementID;

	public sbyte[] PartyType = new sbyte[2];

	public CTROptions ChromieTime;

	public WowGuid128 MemberGuid;

	public PartyMemberFullState()
		: base(Opcode.SMSG_PARTY_MEMBER_FULL_STATE)
	{
		this.Phases.PhaseShiftFlags = 8u;
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.ForEnemy);
		base._worldPacket.FlushBits();
		for (byte i = 0; i < 2; i++)
		{
			base._worldPacket.WriteInt8(this.PartyType[i]);
		}
		base._worldPacket.WriteInt16((short)this.StatusFlags);
		base._worldPacket.WriteUInt8(this.PowerType);
		base._worldPacket.WriteInt16((short)this.PowerDisplayID);
		base._worldPacket.WriteInt32(this.CurrentHealth);
		base._worldPacket.WriteInt32(this.MaxHealth);
		base._worldPacket.WriteUInt16(this.CurrentPower);
		base._worldPacket.WriteUInt16(this.MaxPower);
		base._worldPacket.WriteUInt16(this.Level);
		base._worldPacket.WriteUInt16(this.SpecID);
		base._worldPacket.WriteUInt16(this.ZoneID);
		base._worldPacket.WriteUInt16(this.WmoGroupID);
		base._worldPacket.WriteInt32(this.WmoDoodadPlacementID);
		base._worldPacket.WriteInt16(this.PositionX);
		base._worldPacket.WriteInt16(this.PositionY);
		base._worldPacket.WriteInt16(this.PositionZ);
		base._worldPacket.WriteInt32(this.VehicleSeat);
		base._worldPacket.WriteInt32(this.Auras.Count);
		this.Phases.Write(base._worldPacket);
		this.ChromieTime.Write(base._worldPacket);
		foreach (PartyMemberAuraStates aura in this.Auras)
		{
			aura.Write(base._worldPacket);
		}
		base._worldPacket.WriteBit(this.Pet != null);
		base._worldPacket.FlushBits();
		if (this.Pet != null)
		{
			this.Pet.WriteFull(base._worldPacket);
		}
		base._worldPacket.WritePackedGuid128(this.MemberGuid);
	}
}
