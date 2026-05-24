using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TalentInfoData
{
	public uint TalentID;
	public byte Rank;
}

public class TalentGroupInfoData
{
	public byte SpecID;
	public List<TalentInfoData> Talents = new List<TalentInfoData>();
	public List<ushort> GlyphIDs = new List<ushort>();
}

public class UpdateTalentData : ServerPacket
{
	public uint UnspentTalentPoints;
	public byte ActiveGroup;
	public bool IsPetTalents;
	public List<TalentGroupInfoData> TalentGroups = new List<TalentGroupInfoData>();

	public UpdateTalentData()
		: base(Opcode.SMSG_UPDATE_TALENT_DATA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(UnspentTalentPoints);
		_worldPacket.WriteUInt8(ActiveGroup);
		_worldPacket.WriteUInt32((uint)TalentGroups.Count);

		foreach (var group in TalentGroups)
		{
			_worldPacket.WriteUInt8((byte)group.Talents.Count);
			_worldPacket.WriteUInt32((uint)group.Talents.Count);

			_worldPacket.WriteUInt8((byte)group.GlyphIDs.Count);
			_worldPacket.WriteUInt32((uint)group.GlyphIDs.Count);

			_worldPacket.WriteUInt8(group.SpecID);

			foreach (var talent in group.Talents)
			{
				_worldPacket.WriteUInt32(talent.TalentID);
				_worldPacket.WriteUInt8(talent.Rank);
			}

			foreach (var glyphId in group.GlyphIDs)
			{
				_worldPacket.WriteUInt16(glyphId);
			}
		}

		_worldPacket.WriteBit(IsPetTalents);
		_worldPacket.FlushBits();
	}
}
