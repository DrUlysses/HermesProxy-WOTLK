namespace HermesProxy.World.Server.Packets;

public class GuildBankUpdateTab : ClientPacket
{
	public WowGuid128 BankGuid;

	public byte BankTab;

	public string Name;

	public string Icon;

	public GuildBankUpdateTab(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		BankGuid = _worldPacket.ReadPackedGuid128();
		BankTab = _worldPacket.ReadUInt8();
		_worldPacket.ResetBitPos();
		var nameLen = _worldPacket.ReadBits<uint>(7);
		var iconLen = _worldPacket.ReadBits<uint>(9);
		Name = _worldPacket.ReadString(nameLen);
		Icon = _worldPacket.ReadString(iconLen);
	}
}
