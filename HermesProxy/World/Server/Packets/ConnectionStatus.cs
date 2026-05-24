using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class ConnectionStatus : ServerPacket
{
	public byte State;

	public bool SuppressNotification;

	public ConnectionStatus()
		: base(Opcode.SMSG_BATTLE_NET_CONNECTION_STATUS)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBits(this.State, 2);
		base._worldPacket.WriteBit(this.SuppressNotification);
		base._worldPacket.FlushBits();
	}
}
