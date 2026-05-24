namespace HermesProxy.World.Server.Packets;

public class MailTakeMoney : ClientPacket
{
	public WowGuid128 Mailbox;

	public uint MailID;

	public long Money;

	public MailTakeMoney(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Mailbox = _worldPacket.ReadPackedGuid128();
		MailID = (uint)_worldPacket.ReadUInt64();
		Money = _worldPacket.ReadInt64();
	}
}
