using System;
using Framework.Constants;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

public class QueryCreatureResponse : ServerPacket
{
	public bool Allow;

	public CreatureTemplate Stats;

	public uint CreatureID;

	public QueryCreatureResponse()
		: base(Opcode.SMSG_QUERY_CREATURE_RESPONSE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.CreatureID);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.FlushBits();
		if (!this.Allow)
		{
			return;
		}
		base._worldPacket.WriteBits((!this.Stats.Title.IsEmpty()) ? (this.Stats.Title.GetByteCount() + 1) : 0, 11);
		base._worldPacket.WriteBits((!this.Stats.TitleAlt.IsEmpty()) ? (this.Stats.TitleAlt.GetByteCount() + 1) : 0, 11);
		base._worldPacket.WriteBits((!this.Stats.CursorName.IsEmpty()) ? (this.Stats.CursorName.GetByteCount() + 1) : 0, 6);
		base._worldPacket.WriteBit(this.Stats.Civilian);
		base._worldPacket.WriteBit(this.Stats.Leader);
		for (int i = 0; i < 4; i++)
		{
			base._worldPacket.WriteBits(this.Stats.Name[i].GetByteCount() + 1, 11);
			base._worldPacket.WriteBits(this.Stats.NameAlt[i].GetByteCount() + 1, 11);
		}
		for (int j = 0; j < 4; j++)
		{
			if (!string.IsNullOrEmpty(this.Stats.Name[j]))
			{
				base._worldPacket.WriteCString(this.Stats.Name[j]);
			}
			if (!string.IsNullOrEmpty(this.Stats.NameAlt[j]))
			{
				base._worldPacket.WriteCString(this.Stats.NameAlt[j]);
			}
		}
		for (int k = 0; k < 2; k++)
		{
			base._worldPacket.WriteUInt32(this.Stats.Flags[k]);
		}
		base._worldPacket.WriteInt32(this.Stats.Type);
		base._worldPacket.WriteInt32(this.Stats.Family);
		base._worldPacket.WriteInt32(this.Stats.Classification);
		base._worldPacket.WriteUInt32(this.Stats.PetSpellDataId);
		for (int l = 0; l < 2; l++)
		{
			base._worldPacket.WriteUInt32(this.Stats.ProxyCreatureID[l]);
		}
		base._worldPacket.WriteInt32(this.Stats.Display.CreatureDisplay.Count);
		base._worldPacket.WriteFloat(this.Stats.Display.TotalProbability);
		foreach (CreatureXDisplay display in this.Stats.Display.CreatureDisplay)
		{
			base._worldPacket.WriteUInt32(display.CreatureDisplayID);
			base._worldPacket.WriteFloat(display.Scale);
			base._worldPacket.WriteFloat(display.Probability);
		}
		base._worldPacket.WriteFloat(this.Stats.HpMulti);
		base._worldPacket.WriteFloat(this.Stats.EnergyMulti);
		base._worldPacket.WriteInt32(this.Stats.QuestItems.Count);
		base._worldPacket.WriteUInt32(this.Stats.MovementInfoID);
		base._worldPacket.WriteInt32(this.Stats.HealthScalingExpansion);
		base._worldPacket.WriteUInt32(this.Stats.RequiredExpansion);
		base._worldPacket.WriteUInt32(this.Stats.VignetteID);
		base._worldPacket.WriteInt32(this.Stats.Class);
		base._worldPacket.WriteInt32(this.Stats.DifficultyID);
		base._worldPacket.WriteInt32(this.Stats.WidgetSetID);
		base._worldPacket.WriteInt32(this.Stats.WidgetSetUnitConditionID);
		if (!this.Stats.Title.IsEmpty())
		{
			base._worldPacket.WriteCString(this.Stats.Title);
		}
		if (!this.Stats.TitleAlt.IsEmpty())
		{
			base._worldPacket.WriteCString(this.Stats.TitleAlt);
		}
		if (!this.Stats.CursorName.IsEmpty())
		{
			base._worldPacket.WriteCString(this.Stats.CursorName);
		}
		foreach (uint questItem in this.Stats.QuestItems)
		{
			base._worldPacket.WriteUInt32(questItem);
		}
	}
}
