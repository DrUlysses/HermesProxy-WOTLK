using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyEquipmentSetList : ServerPacket
{
	public EmptyEquipmentSetList()
		: base(Opcode.SMSG_LOAD_EQUIPMENT_SET, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32(0u);
	}
}
