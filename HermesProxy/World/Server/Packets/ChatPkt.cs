using System;
using System.Collections.Generic;
using System.Linq;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ChatPkt : ServerPacket
{
	public ChatMessageTypeModern SlashCmd = ChatMessageTypeModern.System;

	public uint _Language;

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
		SlashCmd = chatType;
		_Language = language;
		_ChatFlags = chatFlags;
		ChatText = message;
		Channel = channelName;
		AchievementID = achievementId;
		Prefix = addonPrefix;
		SenderGUID = ((sender != null) ? sender : WowGuid128.Empty);
		if (string.IsNullOrEmpty(senderName) && sender != null)
		{
			SenderName = globalSession.GameState.GetPlayerName(sender);
		}
		else
		{
			SenderName = senderName;
		}
		SenderAccountGUID = ((sender != null) ? globalSession.GetGameAccountGuidForPlayer(sender) : WowGuid128.Empty);
		SenderGuildGUID = WowGuid128.Empty;
		TargetGUID = ((receiver != null) ? receiver : WowGuid128.Empty);
		if (string.IsNullOrEmpty(receiverName) && receiver != null)
		{
			TargetName = globalSession.GameState.GetPlayerName(receiver);
		}
		else
		{
			TargetName = receiverName;
		}
		if (!SenderGUID.IsEmpty())
		{
			SenderVirtualAddress = globalSession.RealmId.GetAddress();
		}
		if (!TargetGUID.IsEmpty())
		{
			TargetVirtualAddress = globalSession.RealmId.GetAddress();
		}
	}

	public static bool CheckAddonPrefix(HashSet<string> registeredPrefixes, ref uint language, ref string text, ref string addonPrefix)
	{
		if (language == uint.MaxValue)
		{
			language = 183u;
			var tab = '\t';
			if (!text.Contains(tab))
			{
				return false;
			}
			var parts = text.Split(tab);
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
		_worldPacket.WriteUInt8((byte)SlashCmd);
		_worldPacket.WriteUInt32(_Language);
		_worldPacket.WritePackedGuid128(SenderGUID);
		_worldPacket.WritePackedGuid128(SenderGuildGUID);
		_worldPacket.WritePackedGuid128(SenderAccountGUID);
		_worldPacket.WritePackedGuid128(TargetGUID);
		_worldPacket.WriteUInt32(TargetVirtualAddress);
		_worldPacket.WriteUInt32(SenderVirtualAddress);
		_worldPacket.WriteInt32((int)AchievementID);
		_worldPacket.WriteFloat(DisplayTime);
		_worldPacket.WriteInt32(SpellID);
		_worldPacket.WriteBits(SenderName.GetByteCount(), 11);
		_worldPacket.WriteBits(TargetName.GetByteCount(), 11);
		_worldPacket.WriteBits(Prefix.GetByteCount(), 5);
		_worldPacket.WriteBits(Channel.GetByteCount(), 7);
		_worldPacket.WriteBits(ChatText.GetByteCount(), 12);
		_worldPacket.WriteBits((uint)_ChatFlags, 15);
		_worldPacket.WriteBit(HideChatLog);
		_worldPacket.WriteBit(FakeSenderName);
		_worldPacket.WriteBit(Unused_801.HasValue);
		_worldPacket.WriteBit(ChannelGUID != null);
		_worldPacket.FlushBits();
		_worldPacket.WriteString(SenderName);
		_worldPacket.WriteString(TargetName);
		_worldPacket.WriteString(Prefix);
		_worldPacket.WriteString(Channel);
		_worldPacket.WriteString(ChatText);
		if (Unused_801.HasValue)
		{
			_worldPacket.WriteUInt32(Unused_801.Value);
		}
		if (ChannelGUID != null)
		{
			_worldPacket.WritePackedGuid128(ChannelGUID);
		}
	}
}
