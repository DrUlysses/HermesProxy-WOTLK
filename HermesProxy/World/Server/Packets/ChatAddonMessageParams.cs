using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ChatAddonMessageParams
{
	public string Prefix;

	public string Text;

	public ChatMessageTypeModern Type;

	public bool IsLogged;

	public void Read(WorldPacket data)
	{
		data.ResetBitPos();
		uint prefixLen = data.ReadBits<uint>(5);
		uint textLen = data.ReadBits<uint>(8);
		IsLogged = data.HasBit();
		Type = (ChatMessageTypeModern)data.ReadInt32();
		Prefix = data.ReadString(prefixLen);
		Text = data.ReadString(textLen);
	}
}
