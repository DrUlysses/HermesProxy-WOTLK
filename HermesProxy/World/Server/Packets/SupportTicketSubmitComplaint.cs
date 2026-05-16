using System;
using System.Collections.Generic;
using Framework.GameMath;
using Framework.Logging;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class SupportTicketSubmitComplaint : ClientPacket
{
	public class HeaderInfo
	{
		public uint SelfPlayerMapId;

		public Vector3 SelfPlayerPos;

		public float SelfPlayerOrientation;

		public void Read(WorldPacket worldPacket)
		{
			this.SelfPlayerMapId = worldPacket.ReadUInt32();
			this.SelfPlayerPos = worldPacket.ReadVector3();
			this.SelfPlayerOrientation = worldPacket.ReadFloat();
		}
	}

	public class ChatLogInfo
	{
		public class ChatLine
		{
			public DateTime Time;

			public string Text;
		}

		public List<ChatLine> ChatLines = new List<ChatLine>();

		public uint? ReportedLineIdx;

		public void Read(WorldPacket worldPacket)
		{
			uint chatLogLineCount = worldPacket.ReadUInt32();
			bool hasReportedLineIndex = worldPacket.ReadBool();
			for (int i = 0; i < chatLogLineCount; i++)
			{
				DateTime time = worldPacket.ReadTime64();
				uint textLength = worldPacket.ReadBits<uint>(12);
				worldPacket.ResetBitPos();
				string text = worldPacket.ReadString(textLength);
				this.ChatLines.Add(new ChatLine
				{
					Time = time,
					Text = text
				});
			}
			if (hasReportedLineIndex)
			{
				this.ReportedLineIdx = worldPacket.ReadUInt32();
			}
		}
	}

	public class MailInfo
	{
		public uint MailId;

		public string MailTextBody;

		public string MailSubject;

		public void Read(WorldPacket worldPacket)
		{
			this.MailId = worldPacket.ReadUInt32();
			uint textBodyLength = worldPacket.ReadBits<uint>(13);
			uint subjectLength = worldPacket.ReadBits<uint>(9);
			worldPacket.ResetBitPos();
			this.MailTextBody = worldPacket.ReadString(textBodyLength);
			this.MailSubject = worldPacket.ReadString(subjectLength);
		}
	}

	public HeaderInfo Header = new HeaderInfo();

	public WowGuid128 TargetCharacterGuid;

	public ChatLogInfo ChatLog = new ChatLogInfo();

	public MailInfo? SelectedMailInfo = null;

	public GmTicketComplaintType ComplaintType;

	public string TextNote;

	public SupportTicketSubmitComplaint(WorldPacket packet)
		: base(packet)
	{
	}

	public override void Read()
	{
		this.Header.Read(base._worldPacket);
		this.TargetCharacterGuid = base._worldPacket.ReadPackedGuid128();
		this.ChatLog.Read(base._worldPacket);
		this.ComplaintType = (GmTicketComplaintType)base._worldPacket.ReadBits<uint>(5);
		uint noteLength = base._worldPacket.ReadBits<uint>(10);
		bool hasMailInfo = base._worldPacket.ReadBit();
		bool unk2 = base._worldPacket.ReadBit();
		bool unk3 = base._worldPacket.ReadBit();
		bool hasGuildInfo = base._worldPacket.ReadBit();
		bool unk5 = base._worldPacket.ReadBit();
		bool unk6 = base._worldPacket.ReadBit();
		bool hasClubMessage = base._worldPacket.ReadBit();
		bool unk8 = base._worldPacket.ReadBit();
		bool unk9 = base._worldPacket.ReadBit();
		base._worldPacket.ResetBitPos();
		if (hasClubMessage)
		{
			bool isUsingVoice = base._worldPacket.ReadBit();
			base._worldPacket.ResetBitPos();
		}
		if (base._worldPacket.ReadUInt32() != 0)
		{
			Log.Print(LogType.Error, "You reported something that we do not handle (?)", "Read", "Packets\\SupportTicketPackets.cs");
			Log.Print(LogType.Error, "Please create a new issue on GitHub and tell us what you did", "Read", "Packets\\SupportTicketPackets.cs");
			return;
		}
		if (hasMailInfo)
		{
			this.SelectedMailInfo = new MailInfo();
			this.SelectedMailInfo.Read(base._worldPacket);
		}
		this.TextNote = base._worldPacket.ReadString(noteLength);
	}
}
