using System;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DuelWinner : ServerPacket
{
	public string BeatenName;

	public string WinnerName;

	public uint BeatenVirtualRealmAddress;

	public uint WinnerVirtualRealmAddress;

	public bool Fled;

	public DuelWinner()
		: base(Opcode.SMSG_DUEL_WINNER, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.BeatenName.GetByteCount(), 6);
		base._worldPacket.WriteBits(this.WinnerName.GetByteCount(), 6);
		base._worldPacket.WriteBit(this.Fled);
		base._worldPacket.WriteUInt32(this.BeatenVirtualRealmAddress);
		base._worldPacket.WriteUInt32(this.WinnerVirtualRealmAddress);
		base._worldPacket.WriteString(this.BeatenName);
		base._worldPacket.WriteString(this.WinnerName);
	}
}
