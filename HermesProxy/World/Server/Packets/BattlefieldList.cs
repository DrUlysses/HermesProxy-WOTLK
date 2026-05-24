using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class BattlefieldList : ServerPacket
{
	public WowGuid128 BattlemasterGuid;

	public int Verification = 121761856;

	public uint BattlemasterListID;

	public byte MinLevel = 70;

	public byte MaxLevel = 70;

	public List<int> BattlefieldInstances = new List<int>();

	public bool PvpAnywhere;

	public bool HasRandomWinToday;

	public BattlefieldList()
		: base(Opcode.SMSG_BATTLEFIELD_LIST)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.BattlemasterGuid);
		base._worldPacket.WriteInt32(this.Verification);
		base._worldPacket.WriteUInt32(this.BattlemasterListID);
		base._worldPacket.WriteUInt8(this.MinLevel);
		base._worldPacket.WriteUInt8(this.MaxLevel);
		base._worldPacket.WriteInt32(this.BattlefieldInstances.Count);
		foreach (int field in this.BattlefieldInstances)
		{
			base._worldPacket.WriteInt32(field);
		}
		base._worldPacket.WriteBit(this.PvpAnywhere);
		base._worldPacket.WriteBit(this.HasRandomWinToday);
		base._worldPacket.FlushBits();
	}
}
