using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class MOTD : ServerPacket
{
	public List<string> Text = new List<string>();

	public MOTD()
		: base(Opcode.SMSG_MOTD)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.Text.Count, 4);
		base._worldPacket.FlushBits();
		foreach (string line in this.Text)
		{
			base._worldPacket.WriteBits(line.GetByteCount(), 7);
			base._worldPacket.FlushBits();
			base._worldPacket.WriteString(line);
		}
	}
}
