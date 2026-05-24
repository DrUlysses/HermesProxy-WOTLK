using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class SummonRequest : ServerPacket
{
	public enum SummonReason
	{
		Spell = 0,
		Scenario = 1
	}

	public WowGuid128 SummonerGUID;

	public uint SummonerVirtualRealmAddress;

	public int AreaID;

	public SummonReason Reason;

	public bool SkipStartingArea;

	public SummonRequest()
		: base(Opcode.SMSG_SUMMON_REQUEST, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.SummonerGUID);
		base._worldPacket.WriteUInt32(this.SummonerVirtualRealmAddress);
		base._worldPacket.WriteInt32(this.AreaID);
		base._worldPacket.WriteUInt8((byte)this.Reason);
		base._worldPacket.WriteBit(this.SkipStartingArea);
		base._worldPacket.FlushBits();
	}
}
