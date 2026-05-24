using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class WhoResponsePkt : ServerPacket
{
	public uint RequestID;

	public List<WhoEntry> Players = new List<WhoEntry>();

	public WhoResponsePkt()
		: base(Opcode.SMSG_WHO)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.RequestID);
		base._worldPacket.WriteBits(this.Players.Count, 6);
		base._worldPacket.FlushBits();
		this.Players.ForEach(delegate(WhoEntry p)
		{
			p.Write(base._worldPacket);
		});
	}
}
