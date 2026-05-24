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
		List<byte> canLandNodes = new List<byte>(CanLandNodes);
		PadToUInt64Alignment(canLandNodes);
		_worldPacket.WriteInt32(canLandNodes.Count / 8);
		List<byte> canUseNodes = new List<byte>(CanUseNodes);
		PadToUInt64Alignment(canUseNodes);
		_worldPacket.WriteInt32(canUseNodes.Count / 8);
		if (WindowInfo != null)
		{
			_worldPacket.WritePackedGuid128(WindowInfo.UnitGUID);
			_worldPacket.WriteUInt32(WindowInfo.CurrentNode);
		}
		foreach (byte node in canLandNodes)
		{
			_worldPacket.WriteUInt8(node);
		}
		foreach (byte node2 in canUseNodes)
		{
			_worldPacket.WriteUInt8(node2);
		}
	}

	private void PadToUInt64Alignment(List<byte> nodes)
	{
		int remainder = nodes.Count % 8;
		if (remainder != 0)
		{
			for (int i = 0; i < 8 - remainder; i++)
			{
				nodes.Add(0);
			}
		}
	}
}
