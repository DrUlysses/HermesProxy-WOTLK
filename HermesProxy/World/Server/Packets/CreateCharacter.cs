using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class CreateCharacter : ClientPacket
{
	public CharacterCreateInfo CreateInfo;

	public CreateCharacter(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		CreateInfo = new CharacterCreateInfo();
		var nameLength = _worldPacket.ReadBits<uint>(6);
		var hasTemplateSet = _worldPacket.HasBit();
		CreateInfo.IsTrialBoost = _worldPacket.HasBit();
		CreateInfo.UseNPE = _worldPacket.HasBit();
		CreateInfo.RaceId = (Race)_worldPacket.ReadUInt8();
		CreateInfo.ClassId = (Class)_worldPacket.ReadUInt8();
		CreateInfo.Sex = (Gender)_worldPacket.ReadUInt8();
		var customizationCount = _worldPacket.ReadUInt32();
		CreateInfo.Name = _worldPacket.ReadString(nameLength);
		if (hasTemplateSet)
		{
			CreateInfo.TemplateSet = _worldPacket.ReadUInt32();
		}
		for (var i = 0; i < customizationCount; i++)
		{
			CreateInfo.Customizations[i] = new ChrCustomizationChoice(_worldPacket.ReadUInt32(), _worldPacket.ReadUInt32());
		}
		CreateInfo.Customizations.Sort();
	}
}
