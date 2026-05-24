using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class UpdateActionButtons : ServerPacket
{
	public List<int> ActionButtons = new();

	public byte Reason;

	public UpdateActionButtons()
		: base(Opcode.SMSG_UPDATE_ACTION_BUTTONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		for (var i = 0; i < 180; i++)
		{
			_worldPacket.WriteInt64(i < ActionButtons.Count ? ActionButtons[i] : 0);
		}
		_worldPacket.WriteUInt8(Reason);
	}
}
