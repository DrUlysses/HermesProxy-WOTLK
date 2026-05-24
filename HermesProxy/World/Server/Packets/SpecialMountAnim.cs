using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SpecialMountAnim : ServerPacket
{
	public WowGuid128 UnitGUID;

	public List<int> SpellVisualKitIDs = new List<int>();

	public int SequenceVariation;

	public SpecialMountAnim()
		: base(Opcode.SMSG_SPECIAL_MOUNT_ANIM, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WritePackedGuid128(UnitGUID);
		_worldPacket.WriteInt32(SpellVisualKitIDs.Count);
		_worldPacket.WriteInt32(SequenceVariation);
		foreach (var id in SpellVisualKitIDs)
		{
			_worldPacket.WriteInt32(id);
		}
	}
}
