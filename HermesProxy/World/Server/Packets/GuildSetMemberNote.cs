namespace HermesProxy.World.Server.Packets;

public class GuildSetMemberNote : ClientPacket
{
	public WowGuid128 NoteeGUID;

	public bool IsPublic;

	public string Note;

	public GuildSetMemberNote(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		NoteeGUID = _worldPacket.ReadPackedGuid128();
		uint noteLen = _worldPacket.ReadBits<uint>(8);
		IsPublic = _worldPacket.HasBit();
		Note = _worldPacket.ReadString(noteLen);
	}
}
