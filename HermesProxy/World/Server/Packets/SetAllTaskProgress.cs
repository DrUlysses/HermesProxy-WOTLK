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
		base._worldPacket.WriteInt32(this.Tasks.Count);
		foreach (TaskProgress task in this.Tasks)
		{
			task.Write(base._worldPacket);
		}
	}
}
