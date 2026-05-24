using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EmptyAccountMountUpdate : ServerPacket
{
	public EmptyAccountMountUpdate()
		: base(Opcode.SMSG_ACCOUNT_MOUNT_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(bit: true);
		_worldPacket.WriteUInt32(0u);
		_worldPacket.FlushBits();
	}
}

public class AccountMountUpdate : ServerPacket
{
	public readonly List<uint> MountSpellIDs = new();

	public AccountMountUpdate()
		: base(Opcode.SMSG_ACCOUNT_MOUNT_UPDATE, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(true); // IsFullUpdate
		_worldPacket.WriteUInt32((uint)MountSpellIDs.Count);
		foreach (var spellId in MountSpellIDs)
		{
			_worldPacket.WriteInt32((int)spellId);
			_worldPacket.WriteBits(0u, 4); // flags: none
		}
		_worldPacket.FlushBits();
	}
}
