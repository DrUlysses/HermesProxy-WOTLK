namespace HermesProxy.World.Server.Packets;

public class MailTakeItem : ClientPacket
{
	public WowGuid128 Mailbox;

	public uint MailID;

	public uint AttachID;

	public MailTakeItem(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		Mailbox = _worldPacket.ReadPackedGuid128();
		MailID = (uint)_worldPacket.ReadUInt64();
		AttachID = (uint)_worldPacket.ReadUInt64();
	}
}
