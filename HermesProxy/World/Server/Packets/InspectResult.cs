using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InspectResult : ServerPacket
{
	public readonly PlayerModelDisplayInfo DisplayInfo = new();

	public readonly List<ushort> Glyphs = new();

	public readonly List<byte> Talents = new();

	public InspectGuildData GuildData;

	public readonly Array<PVPBracketData> Bracket = new(6, default);

	public uint? AzeriteLevel;

	public int ItemLevel;

	public uint LifetimeHK;

	public readonly uint HonorLevel = 1u;

	public ushort TodayHK;

	public ushort YesterdayHK;

	public byte LifetimeMaxRank;

	public InspectResult()
		: base(Opcode.SMSG_INSPECT_RESULT)
	{
	}

	protected override void Write()
	{
		DisplayInfo.Write(_worldPacket);
		_worldPacket.WriteInt32(Glyphs.Count);
		_worldPacket.WriteInt32(Talents.Count);
		_worldPacket.WriteInt32(ItemLevel);
		_worldPacket.WriteUInt8(LifetimeMaxRank);
		_worldPacket.WriteUInt16(TodayHK);
		_worldPacket.WriteUInt16(YesterdayHK);
		_worldPacket.WriteUInt32(LifetimeHK);
		_worldPacket.WriteUInt32(HonorLevel);
		for (var i = 0; i < Glyphs.Count; i++)
		{
			_worldPacket.WriteUInt16(Glyphs[i]);
		}
		for (var j = 0; j < Talents.Count; j++)
		{
			_worldPacket.WriteUInt8(Talents[j]);
		}
		_worldPacket.WriteBit(GuildData != null);
		_worldPacket.WriteBit(AzeriteLevel.HasValue);
		_worldPacket.FlushBits();
		foreach (var item in Bracket)
		{
			item.Write(_worldPacket);
		}
		if (GuildData != null)
		{
			GuildData.Write(_worldPacket);
		}
		if (AzeriteLevel.HasValue)
		{
			_worldPacket.WriteUInt32(AzeriteLevel.Value);
		}
	}
}
