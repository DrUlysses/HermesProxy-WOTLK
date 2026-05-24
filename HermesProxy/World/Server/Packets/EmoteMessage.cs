using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmoteMessage : ServerPacket
{
	public WowGuid128 Guid;

	public uint EmoteID;

	public int SequenceVariation;

	public List<uint> SpellVisualKitIDs = new List<uint>();

	public EmoteMessage()
		: base(Opcode.SMSG_EMOTE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(Guid);
		_worldPacket.WriteUInt32(EmoteID);
		_worldPacket.WriteInt32(SpellVisualKitIDs.Count);
		_worldPacket.WriteInt32(SequenceVariation);
		foreach (var id in SpellVisualKitIDs)
		{
			_worldPacket.WriteUInt32(id);
		}
	}
}
