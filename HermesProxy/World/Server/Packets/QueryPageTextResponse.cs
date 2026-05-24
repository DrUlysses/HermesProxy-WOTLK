using System;
using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class QueryPageTextResponse : ServerPacket
{
	public struct PageTextInfo
	{
		public uint Id;

		public uint NextPageID;

		public int PlayerConditionID;

		public byte Flags;

		public string Text;

		public void Write(WorldPacket data)
		{
			data.WriteUInt32(this.Id);
			data.WriteUInt32(this.NextPageID);
			data.WriteInt32(this.PlayerConditionID);
			data.WriteUInt8(this.Flags);
			data.WriteBits(this.Text.GetByteCount(), 12);
			data.FlushBits();
			data.WriteString(this.Text);
		}
	}

	public uint PageTextID;

	public bool Allow;

	public List<PageTextInfo> Pages = new List<PageTextInfo>();

	public QueryPageTextResponse()
		: base(Opcode.SMSG_QUERY_PAGE_TEXT_RESPONSE)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.PageTextID);
		base._worldPacket.WriteBit(this.Allow);
		base._worldPacket.FlushBits();
		if (!this.Allow)
		{
			return;
		}
		base._worldPacket.WriteInt32(this.Pages.Count);
		foreach (PageTextInfo page in this.Pages)
		{
			page.Write(base._worldPacket);
		}
	}
}
