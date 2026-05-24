using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public struct InvUpdate
{
	public struct InvItem
	{
		public byte ContainerSlot;

		public byte Slot;
	}

	public List<InvItem> Items;

	public InvUpdate(WorldPacket data)
	{
		Items = new List<InvItem>();
		var size = data.ReadBits<int>(2);
		data.ResetBitPos();
		for (var i = 0; i < size; i++)
		{
			var item = new InvItem
			{
				ContainerSlot = data.ReadUInt8(),
				Slot = data.ReadUInt8()
			};
			Items.Add(item);
		}
	}
}
