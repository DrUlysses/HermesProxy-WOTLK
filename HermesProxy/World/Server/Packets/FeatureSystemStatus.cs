using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class FeatureSystemStatus : ServerPacket
{
	public class SessionAlertConfig
	{
		public int Delay;

		public int Period;

		public int DisplayTime;
	}

	public struct SocialQueueConfig
	{
		public bool ToastsDisabled;

		public float ToastDuration;

		public float DelayDuration;

		public float QueueMultiplier;

		public float PlayerMultiplier;

		public float PlayerFriendValue;

		public float PlayerGuildValue;

		public float ThrottleInitialThreshold;

		public float ThrottleDecayTime;

		public float ThrottlePrioritySpike;

		public float ThrottleMinThreshold;

		public float ThrottlePvPPriorityNormal;

		public float ThrottlePvPPriorityLow;

		public float ThrottlePvPHonorThreshold;

		public float ThrottleLfgListPriorityDefault;

		public float ThrottleLfgListPriorityAbove;

		public float ThrottleLfgListPriorityBelow;

		public float ThrottleLfgListIlvlScalingAbove;

		public float ThrottleLfgListIlvlScalingBelow;

		public float ThrottleRfPriorityAbove;

		public float ThrottleRfIlvlScalingAbove;

		public float ThrottleDfMaxItemLevel;

		public float ThrottleDfBestPriority;
	}

	public struct SquelchInfo
	{
		public bool IsSquelched;

		public WowGuid128 BnetAccountGuid;

		public WowGuid128 GuildGuid;
	}

	public struct RafSystemFeatureInfo
	{
		public bool Enabled;

		public bool RecruitingEnabled;

		public uint MaxRecruits;

		public uint MaxRecruitMonths;

		public uint MaxRecruitmentUses;

		public uint DaysInCycle;
	}

	public bool VoiceEnabled;

	public bool BrowserEnabled;

	public bool BpayStoreAvailable;

	public bool BpayStoreEnabled;

	public SessionAlertConfig SessionAlert;

	public uint ScrollOfResurrectionMaxRequestsPerDay;

	public bool ScrollOfResurrectionEnabled;

	public EuropaTicketConfig EuropaTicketSystemStatus;

	public uint ScrollOfResurrectionRequestsRemaining;

	public uint CfgRealmID;

	public byte ComplaintStatus;

	public int CfgRealmRecID;

	public uint TwitterPostThrottleLimit;

	public uint TwitterPostThrottleCooldown;

	public uint TokenPollTimeSeconds;

	public long TokenBalanceAmount;

	public uint BpayStoreProductDeliveryDelay;

	public uint ClubsPresenceUpdateTimer;

	public uint HiddenUIClubsPresenceUpdateTimer;

	public int ActiveSeason;

	public List<GameRuleValuePair> GameRuleValues = new List<GameRuleValuePair>();

	public short MaxPlayerNameQueriesPerPacket;

	public short PlayerNameQueryTelemetryInterval;

	public uint KioskSessionMinutes;

	public bool ItemRestorationButtonEnabled;

	public bool CharUndeleteEnabled;

	public bool BpayStoreDisabledByParentalControls;

	public bool TwitterEnabled;

	public bool CommerceSystemEnabled;

	public bool Unk67;

	public bool WillKickFromWorld;

	public bool RestrictedAccount;

	public bool TutorialsEnabled;

	public bool KioskModeEnabled;

	public bool CompetitiveModeEnabled;

	public bool TokenBalanceEnabled;

	public bool WarModeFeatureEnabled;

	public bool ClubsEnabled;

	public bool ClubsBattleNetClubTypeAllowed;

	public bool ClubsCharacterClubTypeAllowed;

	public bool ClubsPresenceUpdateEnabled;

	public bool VoiceChatDisabledByParentalControl;

	public bool VoiceChatMutedByParentalControl;

	public bool QuestSessionEnabled;

	public bool IsMuted;

	public bool ClubFinderEnabled;

	public bool Unknown901CheckoutRelated;

	public bool TextToSpeechFeatureEnabled;

	public bool ChatDisabledByDefault;

	public bool ChatDisabledByPlayer;

	public bool LFGListCustomRequiresAuthenticator;

	public bool BattlegroundsEnabled;

	public List<byte> RaceClassExpansionLevels;

	public SocialQueueConfig QuickJoinConfig;

	public SquelchInfo Squelch;

	public RafSystemFeatureInfo RAFSystem;

	public FeatureSystemStatus()
		: base(Opcode.SMSG_FEATURE_SYSTEM_STATUS)
	{
	}

	protected override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			this.WriteWotLK();
			return;
		}
		base._worldPacket.WriteUInt8(this.ComplaintStatus);
		base._worldPacket.WriteUInt32(this.ScrollOfResurrectionRequestsRemaining);
		base._worldPacket.WriteUInt32(this.ScrollOfResurrectionMaxRequestsPerDay);
		base._worldPacket.WriteUInt32(this.CfgRealmID);
		base._worldPacket.WriteInt32(this.CfgRealmRecID);
		base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruits);
		base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruitMonths);
		base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruitmentUses);
		base._worldPacket.WriteUInt32(this.RAFSystem.DaysInCycle);
		base._worldPacket.WriteUInt32(this.TwitterPostThrottleLimit);
		base._worldPacket.WriteUInt32(this.TwitterPostThrottleCooldown);
		base._worldPacket.WriteUInt32(this.TokenPollTimeSeconds);
		base._worldPacket.WriteUInt32(this.KioskSessionMinutes);
		base._worldPacket.WriteInt64(this.TokenBalanceAmount);
		base._worldPacket.WriteUInt32(this.BpayStoreProductDeliveryDelay);
		base._worldPacket.WriteUInt32(this.ClubsPresenceUpdateTimer);
		base._worldPacket.WriteUInt32(this.HiddenUIClubsPresenceUpdateTimer);
		if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
		{
			base._worldPacket.WriteInt32(this.ActiveSeason);
			base._worldPacket.WriteInt32(this.GameRuleValues.Count);
			if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 2, 2, 5, 3))
			{
				base._worldPacket.WriteInt16(this.MaxPlayerNameQueriesPerPacket);
			}
			if (ModernVersion.AddedInVersion(9, 2, 7, 1, 14, 4, 3, 4, 0))
			{
				base._worldPacket.WriteInt16(this.PlayerNameQueryTelemetryInterval);
			}
			foreach (GameRuleValuePair rulePair in this.GameRuleValues)
			{
				rulePair.Write(base._worldPacket);
			}
		}
		base._worldPacket.WriteBit(this.VoiceEnabled);
		base._worldPacket.WriteBit(this.EuropaTicketSystemStatus != null);
		base._worldPacket.WriteBit(this.ScrollOfResurrectionEnabled);
		base._worldPacket.WriteBit(this.BpayStoreEnabled);
		base._worldPacket.WriteBit(this.BpayStoreAvailable);
		base._worldPacket.WriteBit(this.BpayStoreDisabledByParentalControls);
		base._worldPacket.WriteBit(this.ItemRestorationButtonEnabled);
		base._worldPacket.WriteBit(this.BrowserEnabled);
		base._worldPacket.WriteBit(this.SessionAlert != null);
		base._worldPacket.WriteBit(this.RAFSystem.Enabled);
		base._worldPacket.WriteBit(this.RAFSystem.RecruitingEnabled);
		base._worldPacket.WriteBit(this.CharUndeleteEnabled);
		base._worldPacket.WriteBit(this.RestrictedAccount);
		base._worldPacket.WriteBit(this.CommerceSystemEnabled);
		base._worldPacket.WriteBit(this.TutorialsEnabled);
		base._worldPacket.WriteBit(this.TwitterEnabled);
		base._worldPacket.WriteBit(this.Unk67);
		base._worldPacket.WriteBit(this.WillKickFromWorld);
		base._worldPacket.WriteBit(this.KioskModeEnabled);
		base._worldPacket.WriteBit(this.CompetitiveModeEnabled);
		base._worldPacket.WriteBit(this.TokenBalanceEnabled);
		base._worldPacket.WriteBit(this.WarModeFeatureEnabled);
		base._worldPacket.WriteBit(this.ClubsEnabled);
		base._worldPacket.WriteBit(this.ClubsBattleNetClubTypeAllowed);
		base._worldPacket.WriteBit(this.ClubsCharacterClubTypeAllowed);
		base._worldPacket.WriteBit(this.ClubsPresenceUpdateEnabled);
		base._worldPacket.WriteBit(this.VoiceChatDisabledByParentalControl);
		base._worldPacket.WriteBit(this.VoiceChatMutedByParentalControl);
		base._worldPacket.WriteBit(this.QuestSessionEnabled);
		base._worldPacket.WriteBit(this.IsMuted);
		base._worldPacket.WriteBit(this.ClubFinderEnabled);
		base._worldPacket.WriteBit(this.Unknown901CheckoutRelated);
		if (ModernVersion.AddedInVersion(9, 1, 5, 1, 14, 1, 2, 5, 3))
		{
			base._worldPacket.WriteBit(this.TextToSpeechFeatureEnabled);
			base._worldPacket.WriteBit(this.ChatDisabledByDefault);
			base._worldPacket.WriteBit(this.ChatDisabledByPlayer);
			base._worldPacket.WriteBit(this.LFGListCustomRequiresAuthenticator);
		}
		if (ModernVersion.IsClassicVersionBuild())
		{
			base._worldPacket.WriteBit(this.BattlegroundsEnabled);
			base._worldPacket.WriteBit(this.RaceClassExpansionLevels != null);
		}
		base._worldPacket.FlushBits();
		base._worldPacket.WriteBit(this.QuickJoinConfig.ToastsDisabled);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ToastDuration);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.DelayDuration);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.QueueMultiplier);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.PlayerMultiplier);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.PlayerFriendValue);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.PlayerGuildValue);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleInitialThreshold);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleDecayTime);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePrioritySpike);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleMinThreshold);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePvPPriorityNormal);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePvPPriorityLow);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePvPHonorThreshold);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityDefault);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityBelow);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListIlvlScalingAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListIlvlScalingBelow);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleRfPriorityAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleRfIlvlScalingAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleDfMaxItemLevel);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleDfBestPriority);
		if (this.SessionAlert != null)
		{
			base._worldPacket.WriteInt32(this.SessionAlert.Delay);
			base._worldPacket.WriteInt32(this.SessionAlert.Period);
			base._worldPacket.WriteInt32(this.SessionAlert.DisplayTime);
		}
		if (ModernVersion.IsClassicVersionBuild() && this.RaceClassExpansionLevels != null)
		{
			base._worldPacket.WriteInt32(this.RaceClassExpansionLevels.Count);
			for (int i = 0; i < this.RaceClassExpansionLevels.Count; i++)
			{
				base._worldPacket.WriteUInt8(this.RaceClassExpansionLevels[i]);
			}
		}
		base._worldPacket.WriteBit(this.Squelch.IsSquelched);
		base._worldPacket.WritePackedGuid128(this.Squelch.BnetAccountGuid);
		base._worldPacket.WritePackedGuid128(this.Squelch.GuildGuid);
		if (this.EuropaTicketSystemStatus != null)
		{
			this.EuropaTicketSystemStatus.Write(base._worldPacket);
		}
	}

	private void WriteWotLK()
	{
		base._worldPacket.WriteUInt8(this.ComplaintStatus);
		base._worldPacket.WriteUInt32(this.CfgRealmID);
		base._worldPacket.WriteInt32(this.CfgRealmRecID);
		base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruits);
		base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruitMonths);
		base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruitmentUses);
		base._worldPacket.WriteUInt32(this.RAFSystem.DaysInCycle);
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WriteUInt32(this.TokenPollTimeSeconds);
		base._worldPacket.WriteUInt32(this.KioskSessionMinutes);
		base._worldPacket.WriteInt64(this.TokenBalanceAmount);
		base._worldPacket.WriteUInt32(this.BpayStoreProductDeliveryDelay);
		base._worldPacket.WriteUInt32(this.ClubsPresenceUpdateTimer);
		base._worldPacket.WriteUInt32(this.HiddenUIClubsPresenceUpdateTimer);
		base._worldPacket.WriteInt32(this.ActiveSeason);
		base._worldPacket.WriteInt32(this.GameRuleValues.Count);
		base._worldPacket.WriteInt16(this.MaxPlayerNameQueriesPerPacket);
		base._worldPacket.WriteInt16(this.PlayerNameQueryTelemetryInterval);
		base._worldPacket.WriteInt32(0);
		foreach (GameRuleValuePair rulePair in this.GameRuleValues)
		{
			rulePair.Write(base._worldPacket);
		}
		base._worldPacket.WriteBit(this.VoiceEnabled);
		base._worldPacket.WriteBit(this.EuropaTicketSystemStatus != null);
		base._worldPacket.WriteBit(this.BpayStoreEnabled);
		base._worldPacket.WriteBit(this.BpayStoreAvailable);
		base._worldPacket.WriteBit(this.BpayStoreDisabledByParentalControls);
		base._worldPacket.WriteBit(this.ItemRestorationButtonEnabled);
		base._worldPacket.WriteBit(this.BrowserEnabled);
		base._worldPacket.WriteBit(this.SessionAlert != null);
		base._worldPacket.WriteBit(this.RAFSystem.Enabled);
		base._worldPacket.WriteBit(this.RAFSystem.RecruitingEnabled);
		base._worldPacket.WriteBit(this.CharUndeleteEnabled);
		base._worldPacket.WriteBit(this.RestrictedAccount);
		base._worldPacket.WriteBit(this.CommerceSystemEnabled);
		base._worldPacket.WriteBit(this.TutorialsEnabled);
		base._worldPacket.WriteBit(this.Unk67);
		base._worldPacket.WriteBit(this.WillKickFromWorld);
		base._worldPacket.WriteBit(this.KioskModeEnabled);
		base._worldPacket.WriteBit(this.CompetitiveModeEnabled);
		base._worldPacket.WriteBit(this.TokenBalanceEnabled);
		base._worldPacket.WriteBit(this.WarModeFeatureEnabled);
		base._worldPacket.WriteBit(this.ClubsEnabled);
		base._worldPacket.WriteBit(this.ClubsBattleNetClubTypeAllowed);
		base._worldPacket.WriteBit(this.ClubsCharacterClubTypeAllowed);
		base._worldPacket.WriteBit(this.ClubsPresenceUpdateEnabled);
		base._worldPacket.WriteBit(this.VoiceChatDisabledByParentalControl);
		base._worldPacket.WriteBit(this.VoiceChatMutedByParentalControl);
		base._worldPacket.WriteBit(this.QuestSessionEnabled);
		base._worldPacket.WriteBit(this.IsMuted);
		base._worldPacket.WriteBit(this.ClubFinderEnabled);
		base._worldPacket.WriteBit(this.Unknown901CheckoutRelated);
		base._worldPacket.WriteBit(this.TextToSpeechFeatureEnabled);
		base._worldPacket.WriteBit(this.ChatDisabledByDefault);
		base._worldPacket.WriteBit(this.ChatDisabledByPlayer);
		base._worldPacket.WriteBit(this.LFGListCustomRequiresAuthenticator);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.WriteBit(bit: true);
		base._worldPacket.FlushBits();
		base._worldPacket.WriteBit(this.QuickJoinConfig.ToastsDisabled);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ToastDuration);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.DelayDuration);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.QueueMultiplier);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.PlayerMultiplier);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.PlayerFriendValue);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.PlayerGuildValue);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleInitialThreshold);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleDecayTime);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePrioritySpike);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleMinThreshold);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePvPPriorityNormal);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePvPPriorityLow);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottlePvPHonorThreshold);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityDefault);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityBelow);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListIlvlScalingAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleLfgListIlvlScalingBelow);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleRfPriorityAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleRfIlvlScalingAbove);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleDfMaxItemLevel);
		base._worldPacket.WriteFloat(this.QuickJoinConfig.ThrottleDfBestPriority);
		if (this.SessionAlert != null)
		{
			base._worldPacket.WriteInt32(this.SessionAlert.Delay);
			base._worldPacket.WriteInt32(this.SessionAlert.Period);
			base._worldPacket.WriteInt32(this.SessionAlert.DisplayTime);
		}
		base._worldPacket.WriteBit(this.Squelch.IsSquelched);
		base._worldPacket.WritePackedGuid128(this.Squelch.BnetAccountGuid);
		base._worldPacket.WritePackedGuid128(this.Squelch.GuildGuid);
		if (this.EuropaTicketSystemStatus != null)
		{
			this.EuropaTicketSystemStatus.Write(base._worldPacket);
		}
	}
}
