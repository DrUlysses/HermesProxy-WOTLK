using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SetAllTaskProgress : ServerPacket
{
	public List<TaskProgress> Tasks = new List<TaskProgress>();

	public SetAllTaskProgress()
		: base(Opcode.SMSG_SET_ALL_TASK_PROGRESS, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteInt32(Tasks.Count);
		foreach (var task in Tasks)
		{
			task.Write(_worldPacket);
		}
	}
}
