using Framework.GameMath;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class TransferPending : ServerPacket
{
	public class ShipTransferPending
	{
		public uint Id;

		public int OriginMapID;
	}

	public uint MapID;

	public Vector3 OldMapPosition;

	public ShipTransferPending Ship;

	public int? TransferSpellID;

	public TransferPending()
		: base(Opcode.SMSG_TRANSFER_PENDING)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteVector3(this.OldMapPosition);
		base._worldPacket.WriteBit(this.Ship != null);
		base._worldPacket.WriteBit(this.TransferSpellID.HasValue);
		if (this.Ship != null)
		{
			base._worldPacket.WriteUInt32(this.Ship.Id);
			base._worldPacket.WriteInt32(this.Ship.OriginMapID);
		}
		if (this.TransferSpellID.HasValue)
		{
			base._worldPacket.WriteInt32(this.TransferSpellID.Value);
		}
		base._worldPacket.FlushBits();
	}
}
