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
		_worldPacket.WritePackedGuid128(PetGUID);
		_worldPacket.WriteUInt16(CreatureFamily);
		_worldPacket.WriteInt16(Specialization);
		_worldPacket.WriteUInt32(TimeLimit);
		_worldPacket.WriteUInt16((ushort)((byte)CommandState | (Flag << 16)));
		_worldPacket.WriteUInt8((byte)ReactState);
		var actionButtons = ActionButtons;
		foreach (var actionButton in actionButtons)
		{
			_worldPacket.WriteUInt32(actionButton);
		}
		_worldPacket.WriteInt32(Actions.Count);
		_worldPacket.WriteInt32(Cooldowns.Count);
		_worldPacket.WriteInt32(SpellHistory.Count);
		foreach (var action in Actions)
		{
			_worldPacket.WriteUInt32(action);
		}
		foreach (var cooldown in Cooldowns)
		{
			_worldPacket.WriteUInt32(cooldown.SpellID);
			_worldPacket.WriteUInt32(cooldown.Duration);
			_worldPacket.WriteUInt32(cooldown.CategoryDuration);
			_worldPacket.WriteFloat(cooldown.ModRate);
			_worldPacket.WriteUInt16(cooldown.Category);
		}
		foreach (var history in SpellHistory)
		{
			_worldPacket.WriteUInt32(history.CategoryID);
			_worldPacket.WriteUInt32(history.RecoveryTime);
			_worldPacket.WriteFloat(history.ChargeModRate);
			_worldPacket.WriteInt8(history.ConsumedCharges);
		}
	}
}
