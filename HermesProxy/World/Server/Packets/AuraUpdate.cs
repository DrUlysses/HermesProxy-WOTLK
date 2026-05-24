using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AuraUpdate : ServerPacket
{
	public bool UpdateAll;

	public List<AuraInfo> Auras = new List<AuraInfo>();

	public WowGuid128 UnitGUID;

	public AuraUpdate(WowGuid128 guid, bool all)
		: base(Opcode.SMSG_AURA_UPDATE, ConnectionType.Instance)
	{
		UnitGUID = guid;
		UpdateAll = all;
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(UpdateAll);
		_worldPacket.WriteBits(Auras.Count, 9);
		foreach (var aura2 in Auras)
		{
			aura2.Write(_worldPacket);
		}
		_worldPacket.WritePackedGuid128(UnitGUID);
	}
}
