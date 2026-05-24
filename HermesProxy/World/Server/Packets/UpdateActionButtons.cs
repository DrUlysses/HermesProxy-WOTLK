using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class UpdateActionButtons : ServerPacket
{
	public List<int> ActionButtons = new List<int>();

	public byte Reason;

	public UpdateActionButtons()
		: base(Opcode.SMSG_UPDATE_ACTION_BUTTONS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		for (int i = 0; i < 180; i++)
		{
			base._worldPacket.WriteInt64((i < this.ActionButtons.Count) ? this.ActionButtons[i] : 0);
		}
		base._worldPacket.WriteUInt8(this.Reason);
	}
}
