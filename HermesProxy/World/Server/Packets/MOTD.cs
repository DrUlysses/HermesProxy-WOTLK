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
		_worldPacket.WriteBits(Text.Count, 4);
		_worldPacket.FlushBits();
		foreach (var line in Text)
		{
			_worldPacket.WriteBits(line.GetByteCount(), 7);
			_worldPacket.FlushBits();
			_worldPacket.WriteString(line);
		}
	}
}
