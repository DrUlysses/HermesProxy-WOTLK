namespace HermesProxy.World.Server.Packets;

public class AddOnInfo
{
	public string Name;

	public string Version;

	public bool Loaded;

	public bool Disabled;

	public void Read(WorldPacket data)
	{
		data.ResetBitPos();
		var nameLength = data.ReadBits<uint>(10);
		var versionLength = data.ReadBits<uint>(10);
		Loaded = data.HasBit();
		Disabled = data.HasBit();
		if (nameLength > 1)
		{
			Name = data.ReadString(nameLength - 1);
			data.ReadUInt8();
		}
		if (versionLength > 1)
		{
			Version = data.ReadString(versionLength - 1);
			data.ReadUInt8();
		}
	}
}
