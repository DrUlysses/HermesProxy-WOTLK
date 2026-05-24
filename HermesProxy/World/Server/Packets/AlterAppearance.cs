using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class AlterAppearance : ClientPacket
{
	public readonly Array<ChrCustomizationChoice> Customizations = new(50);
	public byte NewSex;
	public int CustomizedRace;
	public int CustomizedChrModelID;

	public AlterAppearance(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		var customizationCount = _worldPacket.ReadUInt32();
		NewSex = _worldPacket.ReadUInt8();
		CustomizedRace = _worldPacket.ReadInt32();
		CustomizedChrModelID = _worldPacket.ReadInt32();
		for (uint i = 0; i < customizationCount; i++)
		{
			Customizations[(int)i] = new ChrCustomizationChoice(_worldPacket.ReadUInt32(), _worldPacket.ReadUInt32());
		}
		Customizations.Sort();
	}
}
