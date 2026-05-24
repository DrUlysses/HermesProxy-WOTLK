using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class PartyInvite : ServerPacket
{
	public bool CanAccept = true;

	public bool MightCRZYou;

	public bool IsXRealm;

	public bool MustBeBNetFriend;

	public bool AllowMultipleRoles;

	public bool QuestSessionActive;

	public ushort Unk1 = 4904;

	public VirtualRealmInfo InviterRealm;

	public WowGuid128 InviterGUID;

	public WowGuid128 InviterBNetAccountId;

	public string InviterName;

	public uint ProposedRoles;

	public int LfgCompletedMask;

	public List<int> LfgSlots = new List<int>();

	public PartyInvite()
		: base(Opcode.SMSG_PARTY_INVITE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.CanAccept);
		base._worldPacket.WriteBit(this.MightCRZYou);
		base._worldPacket.WriteBit(this.IsXRealm);
		base._worldPacket.WriteBit(this.MustBeBNetFriend);
		base._worldPacket.WriteBit(this.AllowMultipleRoles);
		base._worldPacket.WriteBit(this.QuestSessionActive);
		base._worldPacket.WriteBits(this.InviterName.GetByteCount(), 6);
		this.InviterRealm.Write(base._worldPacket);
		base._worldPacket.WritePackedGuid128(this.InviterGUID);
		base._worldPacket.WritePackedGuid128(this.InviterBNetAccountId);
		base._worldPacket.WriteUInt16(this.Unk1);
		base._worldPacket.WriteUInt32(this.ProposedRoles);
		base._worldPacket.WriteInt32(this.LfgSlots.Count);
		base._worldPacket.WriteInt32(this.LfgCompletedMask);
		base._worldPacket.WriteString(this.InviterName);
		foreach (int LfgSlot in this.LfgSlots)
		{
			base._worldPacket.WriteInt32(LfgSlot);
		}
	}
}
