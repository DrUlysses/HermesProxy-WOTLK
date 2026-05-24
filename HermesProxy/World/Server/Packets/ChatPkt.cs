using System;
using System.Collections.Generic;
using System.Linq;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ChatPkt : ServerPacket
{
	public ChatMessageTypeModern SlashCmd = ChatMessageTypeModern.System;

	public uint _Language = 0u;

	public WowGuid128 SenderGUID;

	public WowGuid128 SenderGuildGUID;

	public WowGuid128 SenderAccountGUID;

	public WowGuid128 TargetGUID;

	public WowGuid128 ChannelGUID;

	public uint SenderVirtualAddress;

	public uint TargetVirtualAddress;

	public int SpellID;

	public string SenderName = "";

	public string TargetName = "";

	public string Prefix = "";

	public string Channel = "";

	public string ChatText = "";

	public uint AchievementID;

	public ChatFlags _ChatFlags = ChatFlags.None;

	public float DisplayTime = 0f;

	public uint? Unused_801;

	public bool HideChatLog = false;

	public bool FakeSenderName = false;

	public ChatPkt(GlobalSessionData globalSession, ChatMessageTypeModern chatType, string message, uint language = 0u, WowGuid128 sender = null, string senderName = "", WowGuid128 receiver = null, string receiverName = "", string channelName = "", ChatFlags chatFlags = ChatFlags.None, string addonPrefix = "", uint achievementId = 0u)
		: base(Opcode.SMSG_CHAT)
	{
		this.SlashCmd = chatType;
		this._Language = language;
		this._ChatFlags = chatFlags;
		this.ChatText = message;
		this.Channel = channelName;
		this.AchievementID = achievementId;
		this.Prefix = addonPrefix;
		this.SenderGUID = ((sender != null) ? sender : WowGuid128.Empty);
		if (string.IsNullOrEmpty(senderName) && sender != null)
		{
			this.SenderName = globalSession.GameState.GetPlayerName(sender);
		}
		else
		{
			this.SenderName = senderName;
		}
		this.SenderAccountGUID = ((sender != null) ? globalSession.GetGameAccountGuidForPlayer(sender) : WowGuid128.Empty);
		this.SenderGuildGUID = WowGuid128.Empty;
		this.TargetGUID = ((receiver != null) ? receiver : WowGuid128.Empty);
		if (string.IsNullOrEmpty(receiverName) && receiver != null)
		{
			this.TargetName = globalSession.GameState.GetPlayerName(receiver);
		}
		else
		{
			this.TargetName = receiverName;
		}
		if (!this.SenderGUID.IsEmpty())
		{
			this.SenderVirtualAddress = globalSession.RealmId.GetAddress();
		}
		if (!this.TargetGUID.IsEmpty())
		{
			this.TargetVirtualAddress = globalSession.RealmId.GetAddress();
		}
	}

	public static bool CheckAddonPrefix(HashSet<string> registeredPrefixes, ref uint language, ref string text, ref string addonPrefix)
	{
		if (language == uint.MaxValue)
		{
			language = 183u;
			char tab = '\t';
			if (!text.Contains(tab))
			{
				return false;
			}
			string[] parts = text.Split(tab);
			addonPrefix = parts[0];
			text = string.Join(" ", parts.Skip(1).ToList());
			if (!registeredPrefixes.Contains(addonPrefix))
			{
				return false;
			}
		}
		return true;
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt8((byte)this.SlashCmd);
		base._worldPacket.WriteUInt32(this._Language);
		base._worldPacket.WritePackedGuid128(this.SenderGUID);
		base._worldPacket.WritePackedGuid128(this.SenderGuildGUID);
		base._worldPacket.WritePackedGuid128(this.SenderAccountGUID);
		base._worldPacket.WritePackedGuid128(this.TargetGUID);
		base._worldPacket.WriteUInt32(this.TargetVirtualAddress);
		base._worldPacket.WriteUInt32(this.SenderVirtualAddress);
		base._worldPacket.WriteInt32((int)this.AchievementID);
		base._worldPacket.WriteFloat(this.DisplayTime);
		base._worldPacket.WriteInt32(this.SpellID);
		base._worldPacket.WriteBits(this.SenderName.GetByteCount(), 11);
		base._worldPacket.WriteBits(this.TargetName.GetByteCount(), 11);
		base._worldPacket.WriteBits(this.Prefix.GetByteCount(), 5);
		base._worldPacket.WriteBits(this.Channel.GetByteCount(), 7);
		base._worldPacket.WriteBits(this.ChatText.GetByteCount(), 12);
		base._worldPacket.WriteBits((uint)this._ChatFlags, 15);
		base._worldPacket.WriteBit(this.HideChatLog);
		base._worldPacket.WriteBit(this.FakeSenderName);
		base._worldPacket.WriteBit(this.Unused_801.HasValue);
		base._worldPacket.WriteBit(this.ChannelGUID != null);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteString(this.SenderName);
		base._worldPacket.WriteString(this.TargetName);
		base._worldPacket.WriteString(this.Prefix);
		base._worldPacket.WriteString(this.Channel);
		base._worldPacket.WriteString(this.ChatText);
		if (this.Unused_801.HasValue)
		{
			base._worldPacket.WriteUInt32(this.Unused_801.Value);
		}
		if (this.ChannelGUID != null)
		{
			base._worldPacket.WritePackedGuid128(this.ChannelGUID);
		}
	}
}
