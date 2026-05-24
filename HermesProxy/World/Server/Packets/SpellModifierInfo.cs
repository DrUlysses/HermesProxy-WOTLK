using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class SpellModifierInfo
{
	public byte ModIndex;

	public readonly List<SpellModifierData> ModifierData = new();

	public void Write(WorldPacket data)
	{
		data.WriteUInt8(ModIndex);
		data.WriteInt32(ModifierData.Count);
		foreach (var modifierDatum in ModifierData)
		{
			modifierDatum.Write(data);
		}
	}
}
