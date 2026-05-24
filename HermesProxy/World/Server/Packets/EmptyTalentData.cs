using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyTalentData : ServerPacket
{
	public EmptyTalentData()
		: base(Opcode.SMSG_UPDATE_TALENT_DATA, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(0u);        // UnspentTalentPoints
		_worldPacket.WriteUInt8(0);          // ActiveGroup
		_worldPacket.WriteUInt32(1u);        // GroupCount = 1
		_worldPacket.WriteUInt8(0);          // TalentCount (byte)
		_worldPacket.WriteUInt32(0u);        // TalentCount (dword)
		_worldPacket.WriteUInt8(0);          // GlyphCount (byte)
		_worldPacket.WriteUInt32(0u);        // GlyphCount (dword)
		_worldPacket.WriteUInt8(4);          // SpecID = MAX_SPECIALIZATIONS (no spec)
		_worldPacket.WriteBit(bit: false);   // IsPetTalents
		_worldPacket.FlushBits();
	}
}
