using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ShowTaxiNodes : ServerPacket
{
	public ShowTaxiNodesWindowInfo WindowInfo;

	public List<byte> CanLandNodes = new List<byte>();

	public List<byte> CanUseNodes = new List<byte>();

	public ShowTaxiNodes()
		: base(Opcode.SMSG_SHOW_TAXI_NODES)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(WindowInfo != null);
		_worldPacket.FlushBits();
		var canLandNodes = new List<byte>(CanLandNodes);
		PadToUInt64Alignment(canLandNodes);
		_worldPacket.WriteInt32(canLandNodes.Count / 8);
		var canUseNodes = new List<byte>(CanUseNodes);
		PadToUInt64Alignment(canUseNodes);
		_worldPacket.WriteInt32(canUseNodes.Count / 8);
		if (WindowInfo != null)
		{
			_worldPacket.WritePackedGuid128(WindowInfo.UnitGUID);
			_worldPacket.WriteUInt32(WindowInfo.CurrentNode);
		}
		foreach (var node in canLandNodes)
		{
			_worldPacket.WriteUInt8(node);
		}
		foreach (var node2 in canUseNodes)
		{
			_worldPacket.WriteUInt8(node2);
		}
	}

	private void PadToUInt64Alignment(List<byte> nodes)
	{
		var remainder = nodes.Count % 8;
		if (remainder != 0)
		{
			for (var i = 0; i < 8 - remainder; i++)
			{
				nodes.Add(0);
			}
		}
	}
}
