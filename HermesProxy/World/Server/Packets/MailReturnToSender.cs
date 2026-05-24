namespace HermesProxy.World.Server.Packets;

public class MailReturnToSender : ClientPacket
{
	public uint MailID;

	public WowGuid128 SenderGUID;

	public MailReturnToSender(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		MailID = (uint)_worldPacket.ReadUInt64();
		SenderGUID = _worldPacket.ReadPackedGuid128();
	}
}
