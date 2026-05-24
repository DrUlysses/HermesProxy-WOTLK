using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PartyMemberPartialState : ServerPacket
{
	public class PartyTypeChange
	{
		public byte PartyType1;

		public byte PartyType2;
	}

	public class Vector3_UInt16
	{
		public short X;

		public short Y;

		public short Z;
	}

	public class UnkStruct901_2
	{
		public uint Unk902_3;

		public uint Unk902_4;

		public uint Unk902_5;

		public void Write(WorldPacket data)
		{
			data.WriteUInt32(this.Unk902_3);
			data.WriteUInt32(this.Unk902_4);
			data.WriteUInt32(this.Unk902_5);
		}
	}

	public WowGuid128 AffectedGUID;

	public bool ForEnemyChanged;

	public bool SetPvPInactive;

	public bool Unk901_1;

	public PartyTypeChange PartyType;

	public ushort? StatusFlags;

	public byte? PowerType;

	public ushort? OverrideDisplayPower;

	public uint? CurrentHealth;

	public uint? MaxHealth;

	public ushort? CurrentPower;

	public ushort? MaxPower;

	public ushort? Level;

	public ushort? Spec;

	public ushort? ZoneID;

	public ushort? WmoGroupID;

	public uint? WmoDoodadPlacementID;

	public Vector3_UInt16 Position;

	public uint? VehicleSeatRecID;

	public List<PartyMemberAuraStates> Auras;

	public PartyMemberPetStats Pet;

	public PartyMemberPhaseStates Phase;

	public UnkStruct901_2 Unk901_2;

	public PartyMemberPartialState()
		: base(Opcode.SMSG_PARTY_MEMBER_PARTIAL_STATE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.ForEnemyChanged);
		base._worldPacket.WriteBit(this.SetPvPInactive);
		base._worldPacket.WriteBit(this.Unk901_1);
		base._worldPacket.WriteBit(this.PartyType != null);
		base._worldPacket.WriteBit(this.StatusFlags.HasValue);
		base._worldPacket.WriteBit(this.PowerType.HasValue);
		base._worldPacket.WriteBit(this.OverrideDisplayPower.HasValue);
		base._worldPacket.WriteBit(this.CurrentHealth.HasValue);
		base._worldPacket.WriteBit(this.MaxHealth.HasValue);
		base._worldPacket.WriteBit(this.CurrentPower.HasValue);
		base._worldPacket.WriteBit(this.MaxPower.HasValue);
		base._worldPacket.WriteBit(this.Level.HasValue);
		base._worldPacket.WriteBit(this.Spec.HasValue);
		base._worldPacket.WriteBit(this.ZoneID.HasValue);
		base._worldPacket.WriteBit(this.WmoGroupID.HasValue);
		base._worldPacket.WriteBit(this.WmoDoodadPlacementID.HasValue);
		base._worldPacket.WriteBit(this.Position != null);
		base._worldPacket.WriteBit(this.VehicleSeatRecID.HasValue);
		base._worldPacket.WriteBit(this.Auras != null);
		base._worldPacket.WriteBit(this.Pet != null);
		base._worldPacket.WriteBit(this.Phase != null);
		base._worldPacket.WriteBit(this.Unk901_2 != null);
		base._worldPacket.FlushBits();
		if (this.Pet != null)
		{
			this.Pet.WritePartial(base._worldPacket);
		}
		base._worldPacket.WritePackedGuid128(this.AffectedGUID);
		if (this.PartyType != null)
		{
			base._worldPacket.WriteUInt8(this.PartyType.PartyType1);
			base._worldPacket.WriteUInt8(this.PartyType.PartyType2);
		}
		if (this.StatusFlags.HasValue)
		{
			base._worldPacket.WriteUInt16(this.StatusFlags.Value);
		}
		if (this.PowerType.HasValue)
		{
			base._worldPacket.WriteUInt8(this.PowerType.Value);
		}
		if (this.OverrideDisplayPower.HasValue)
		{
			base._worldPacket.WriteUInt16(this.OverrideDisplayPower.Value);
		}
		if (this.CurrentHealth.HasValue)
		{
			base._worldPacket.WriteUInt32(this.CurrentHealth.Value);
		}
		if (this.MaxHealth.HasValue)
		{
			base._worldPacket.WriteUInt32(this.MaxHealth.Value);
		}
		if (this.CurrentPower.HasValue)
		{
			base._worldPacket.WriteUInt16(this.CurrentPower.Value);
		}
		if (this.MaxPower.HasValue)
		{
			base._worldPacket.WriteUInt16(this.MaxPower.Value);
		}
		if (this.Level.HasValue)
		{
			base._worldPacket.WriteUInt16(this.Level.Value);
		}
		if (this.Spec.HasValue)
		{
			base._worldPacket.WriteUInt16(this.Spec.Value);
		}
		if (this.ZoneID.HasValue)
		{
			base._worldPacket.WriteUInt16(this.ZoneID.Value);
		}
		if (this.WmoGroupID.HasValue)
		{
			base._worldPacket.WriteUInt16(this.WmoGroupID.Value);
		}
		if (this.WmoDoodadPlacementID.HasValue)
		{
			base._worldPacket.WriteUInt32(this.WmoDoodadPlacementID.Value);
		}
		if (this.Position != null)
		{
			base._worldPacket.WriteInt16(this.Position.X);
			base._worldPacket.WriteInt16(this.Position.Y);
			base._worldPacket.WriteInt16(this.Position.Z);
		}
		if (this.VehicleSeatRecID.HasValue)
		{
			base._worldPacket.WriteUInt32(this.VehicleSeatRecID.Value);
		}
		if (this.Auras != null)
		{
			base._worldPacket.WriteInt32(this.Auras.Count);
			foreach (PartyMemberAuraStates aura in this.Auras)
			{
				aura.Write(base._worldPacket);
			}
		}
		if (this.Phase != null)
		{
			this.Phase.Write(base._worldPacket);
		}
		if (this.Unk901_2 != null)
		{
			this.Unk901_2.Write(base._worldPacket);
		}
	}
}
