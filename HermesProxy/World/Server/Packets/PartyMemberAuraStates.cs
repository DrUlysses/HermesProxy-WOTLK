using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class PartyMemberAuraStates
{
	public uint SpellId;

	public ushort AuraFlags;

	public uint ActiveFlags;

	public readonly List<float> Points = new();

	public void Write(WorldPacket data)
	{
		data.WriteUInt32(SpellId);
		data.WriteUInt16(AuraFlags);
		data.WriteUInt32(ActiveFlags);
		data.WriteInt32(Points.Count);
		foreach (var point in Points)
		{
			data.WriteFloat(point);
		}
	}
}
