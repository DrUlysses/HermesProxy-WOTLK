using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class PetSpells : ServerPacket
{
	public WowGuid128 PetGUID;

	public ushort CreatureFamily;

	public short Specialization = -1;

	public uint TimeLimit;

	public ReactStates ReactState;

	public CommandStates CommandState;

	public byte Flag;

	public uint[] ActionButtons = new uint[10];

	public List<uint> Actions = new List<uint>();

	public List<PetSpellCooldown> Cooldowns = new List<PetSpellCooldown>();

	public List<PetSpellHistory> SpellHistory = new List<PetSpellHistory>();

	public PetSpells()
		: base(Opcode.SMSG_PET_SPELLS_MESSAGE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.PetGUID);
		base._worldPacket.WriteUInt16(this.CreatureFamily);
		base._worldPacket.WriteInt16(this.Specialization);
		base._worldPacket.WriteUInt32(this.TimeLimit);
		base._worldPacket.WriteUInt16((ushort)((byte)this.CommandState | (this.Flag << 16)));
		base._worldPacket.WriteUInt8((byte)this.ReactState);
		uint[] actionButtons = this.ActionButtons;
		foreach (uint actionButton in actionButtons)
		{
			base._worldPacket.WriteUInt32(actionButton);
		}
		base._worldPacket.WriteInt32(this.Actions.Count);
		base._worldPacket.WriteInt32(this.Cooldowns.Count);
		base._worldPacket.WriteInt32(this.SpellHistory.Count);
		foreach (uint action in this.Actions)
		{
			base._worldPacket.WriteUInt32(action);
		}
		foreach (PetSpellCooldown cooldown in this.Cooldowns)
		{
			base._worldPacket.WriteUInt32(cooldown.SpellID);
			base._worldPacket.WriteUInt32(cooldown.Duration);
			base._worldPacket.WriteUInt32(cooldown.CategoryDuration);
			base._worldPacket.WriteFloat(cooldown.ModRate);
			base._worldPacket.WriteUInt16(cooldown.Category);
		}
		foreach (PetSpellHistory history in this.SpellHistory)
		{
			base._worldPacket.WriteUInt32(history.CategoryID);
			base._worldPacket.WriteUInt32(history.RecoveryTime);
			base._worldPacket.WriteFloat(history.ChargeModRate);
			base._worldPacket.WriteInt8(history.ConsumedCharges);
		}
	}
}
