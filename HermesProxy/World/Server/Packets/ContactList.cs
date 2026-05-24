using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ContactList : ServerPacket
{
	public List<ContactInfo> Contacts;

	public SocialFlag Flags;

	public ContactList()
		: base(Opcode.SMSG_CONTACT_LIST)
	{
		this.Contacts = new List<ContactInfo>();
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32((uint)this.Flags);
		base._worldPacket.WriteBits(this.Contacts.Count, 8);
		base._worldPacket.FlushBits();
		foreach (ContactInfo contact in this.Contacts)
		{
			contact.Write(base._worldPacket);
		}
	}
}
