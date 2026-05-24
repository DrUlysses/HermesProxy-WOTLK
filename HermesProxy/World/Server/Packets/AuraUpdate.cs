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
		this.UnitGUID = guid;
		this.UpdateAll = all;
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.UpdateAll);
		base._worldPacket.WriteBits(this.Auras.Count, 9);
		foreach (AuraInfo aura2 in this.Auras)
		{
			aura2.Write(base._worldPacket);
		}
		base._worldPacket.WritePackedGuid128(this.UnitGUID);
	}
}
