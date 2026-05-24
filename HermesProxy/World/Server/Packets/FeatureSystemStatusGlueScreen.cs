using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class FeatureSystemStatusGlueScreen : ServerPacket
{
	public bool BpayStoreAvailable;

	public bool BpayStoreDisabledByParentalControls;

	public bool CharUndeleteEnabled;

	public bool BpayStoreEnabled;

	public bool CommerceSystemEnabled;

	public bool Unk14;

	public bool WillKickFromWorld;

	public bool IsExpansionPreorderInStore;

	public bool KioskModeEnabled;

	public bool CompetitiveModeEnabled;

	public bool TrialBoostEnabled;

	public bool TokenBalanceEnabled;

	public bool LiveRegionCharacterListEnabled;

	public bool LiveRegionCharacterCopyEnabled;

	public bool LiveRegionAccountCopyEnabled;

	public bool LiveRegionKeyBindingsCopyEnabled = false;

	public bool Unknown901CheckoutRelated = false;

	public EuropaTicketConfig EuropaTicketSystemStatus;

	public List<int> LiveRegionCharacterCopySourceRegions = new List<int>();

	public uint TokenPollTimeSeconds;

	public long TokenBalanceAmount;

	public int MaxCharactersPerRealm;

	public uint BpayStoreProductDeliveryDelay;

	public int ActiveCharacterUpgradeBoostType;

	public int ActiveClassTrialBoostType;

	public int MinimumExpansionLevel;

	public int MaximumExpansionLevel;

	public int ActiveSeason;

	public List<GameRuleValuePair> GameRuleValues = new List<GameRuleValuePair>();

	public short MaxPlayerNameQueriesPerPacket;

	public short PlayerNameQueryTelemetryInterval;

	public uint KioskSessionMinutes;

	public FeatureSystemStatusGlueScreen()
		: base(Opcode.SMSG_FEATURE_SYSTEM_STATUS_GLUE_SCREEN)
	{
	}

	protected override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			this.WriteWotlk343();
			return;
		}
		base._worldPacket.WriteBit(this.BpayStoreEnabled);
		base._worldPacket.WriteBit(this.BpayStoreAvailable);
		base._worldPacket.WriteBit(this.BpayStoreDisabledByParentalControls);
		base._worldPacket.WriteBit(this.CharUndeleteEnabled);
		base._worldPacket.WriteBit(this.CommerceSystemEnabled);
		base._worldPacket.WriteBit(this.Unk14);
		base._worldPacket.WriteBit(this.WillKickFromWorld);
		base._worldPacket.WriteBit(this.IsExpansionPreorderInStore);
		base._worldPacket.WriteBit(this.KioskModeEnabled);
		base._worldPacket.WriteBit(this.CompetitiveModeEnabled);
		base._worldPacket.WriteBit(this.TrialBoostEnabled);
		base._worldPacket.WriteBit(this.TokenBalanceEnabled);
		base._worldPacket.WriteBit(this.LiveRegionCharacterListEnabled);
		base._worldPacket.WriteBit(this.LiveRegionCharacterCopyEnabled);
		base._worldPacket.WriteBit(this.LiveRegionAccountCopyEnabled);
		base._worldPacket.WriteBit(this.LiveRegionKeyBindingsCopyEnabled);
		base._worldPacket.WriteBit(this.Unknown901CheckoutRelated);
		base._worldPacket.WriteBit(this.EuropaTicketSystemStatus != null);
		base._worldPacket.FlushBits();
		if (this.EuropaTicketSystemStatus != null)
		{
			this.EuropaTicketSystemStatus.Write(base._worldPacket);
		}
		base._worldPacket.WriteUInt32(this.TokenPollTimeSeconds);
		base._worldPacket.WriteUInt32(this.KioskSessionMinutes);
		base._worldPacket.WriteInt64(this.TokenBalanceAmount);
		base._worldPacket.WriteInt32(this.MaxCharactersPerRealm);
		base._worldPacket.WriteInt32(this.LiveRegionCharacterCopySourceRegions.Count);
		base._worldPacket.WriteUInt32(this.BpayStoreProductDeliveryDelay);
		base._worldPacket.WriteInt32(this.ActiveCharacterUpgradeBoostType);
		base._worldPacket.WriteInt32(this.ActiveClassTrialBoostType);
		base._worldPacket.WriteInt32(this.MinimumExpansionLevel);
		base._worldPacket.WriteInt32(this.MaximumExpansionLevel);
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
		}
		foreach (int sourceRegion in this.LiveRegionCharacterCopySourceRegions)
		{
			base._worldPacket.WriteInt32(sourceRegion);
		}
		if (!ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
		{
			return;
		}
		foreach (GameRuleValuePair rulePair in this.GameRuleValues)
		{
			rulePair.Write(base._worldPacket);
		}
	}

	private void WriteWotlk343()
	{
		base._worldPacket.WriteBit(this.BpayStoreEnabled);
		base._worldPacket.WriteBit(this.BpayStoreAvailable);
		base._worldPacket.WriteBit(this.BpayStoreDisabledByParentalControls);
		base._worldPacket.WriteBit(this.CharUndeleteEnabled);
		base._worldPacket.WriteBit(this.CommerceSystemEnabled);
		base._worldPacket.WriteBit(this.Unk14);
		base._worldPacket.WriteBit(this.WillKickFromWorld);
		base._worldPacket.WriteBit(this.IsExpansionPreorderInStore);
		base._worldPacket.WriteBit(this.KioskModeEnabled);
		base._worldPacket.WriteBit(this.CompetitiveModeEnabled);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(this.TrialBoostEnabled);
		base._worldPacket.WriteBit(this.TokenBalanceEnabled);
		base._worldPacket.WriteBit(this.LiveRegionCharacterListEnabled);
		base._worldPacket.WriteBit(this.LiveRegionCharacterCopyEnabled);
		base._worldPacket.WriteBit(this.LiveRegionAccountCopyEnabled);
		base._worldPacket.WriteBit(this.LiveRegionKeyBindingsCopyEnabled);
		base._worldPacket.WriteBit(this.Unknown901CheckoutRelated);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(this.EuropaTicketSystemStatus != null);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.FlushBits();
		if (this.EuropaTicketSystemStatus != null)
		{
			this.EuropaTicketSystemStatus.Write(base._worldPacket);
		}
		base._worldPacket.WriteUInt32(this.TokenPollTimeSeconds);
		base._worldPacket.WriteUInt32(this.KioskSessionMinutes);
		base._worldPacket.WriteInt64(this.TokenBalanceAmount);
		base._worldPacket.WriteInt32(this.MaxCharactersPerRealm);
		base._worldPacket.WriteInt32(this.LiveRegionCharacterCopySourceRegions.Count);
		base._worldPacket.WriteUInt32(this.BpayStoreProductDeliveryDelay);
		base._worldPacket.WriteInt32(this.ActiveCharacterUpgradeBoostType);
		base._worldPacket.WriteInt32(this.ActiveClassTrialBoostType);
		base._worldPacket.WriteInt32(this.MinimumExpansionLevel);
		base._worldPacket.WriteInt32(this.MaximumExpansionLevel);
		base._worldPacket.WriteInt32(this.ActiveSeason);
		base._worldPacket.WriteInt32(this.GameRuleValues.Count);
		base._worldPacket.WriteInt16(this.MaxPlayerNameQueriesPerPacket);
		base._worldPacket.WriteInt16(this.PlayerNameQueryTelemetryInterval);
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteInt32(0);
		foreach (int sourceRegion in this.LiveRegionCharacterCopySourceRegions)
		{
			base._worldPacket.WriteInt32(sourceRegion);
		}
		foreach (GameRuleValuePair rulePair in this.GameRuleValues)
		{
			rulePair.Write(base._worldPacket);
		}
	}
}
