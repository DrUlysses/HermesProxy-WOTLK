using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InspectResult : ServerPacket
{
	public PlayerModelDisplayInfo DisplayInfo = new PlayerModelDisplayInfo();

	public List<ushort> Glyphs = new List<ushort>();

	public List<byte> Talents = new List<byte>();

	public InspectGuildData GuildData;

	public Array<PVPBracketData> Bracket = new Array<PVPBracketData>(6, default(PVPBracketData));

	public uint? AzeriteLevel;

	public int ItemLevel;

	public uint LifetimeHK;

	public uint HonorLevel = 1u;

	public ushort TodayHK;

	public ushort YesterdayHK;

	public byte LifetimeMaxRank;

	public InspectResult()
		: base(Opcode.SMSG_INSPECT_RESULT)
	{
	}

	protected override void Write()
	{
		this.DisplayInfo.Write(base._worldPacket);
		base._worldPacket.WriteInt32(this.Glyphs.Count);
		base._worldPacket.WriteInt32(this.Talents.Count);
		base._worldPacket.WriteInt32(this.ItemLevel);
		base._worldPacket.WriteUInt8(this.LifetimeMaxRank);
		base._worldPacket.WriteUInt16(this.TodayHK);
		base._worldPacket.WriteUInt16(this.YesterdayHK);
		base._worldPacket.WriteUInt32(this.LifetimeHK);
		base._worldPacket.WriteUInt32(this.HonorLevel);
		for (int i = 0; i < this.Glyphs.Count; i++)
		{
			base._worldPacket.WriteUInt16(this.Glyphs[i]);
		}
		for (int j = 0; j < this.Talents.Count; j++)
		{
			base._worldPacket.WriteUInt8(this.Talents[j]);
		}
		base._worldPacket.WriteBit(this.GuildData != null);
		base._worldPacket.WriteBit(this.AzeriteLevel.HasValue);
		base._worldPacket.FlushBits();
		foreach (PVPBracketData item in this.Bracket)
		{
			item.Write(base._worldPacket);
		}
		if (this.GuildData != null)
		{
			this.GuildData.Write(base._worldPacket);
		}
		if (this.AzeriteLevel.HasValue)
		{
			base._worldPacket.WriteUInt32(this.AzeriteLevel.Value);
		}
	}
}
