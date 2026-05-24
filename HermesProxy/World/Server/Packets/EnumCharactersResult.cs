using System;
using System.Collections.Generic;
using Framework.GameMath;
using Framework.Logging;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class EnumCharactersResult : ServerPacket
{
	public class CharacterInfo
	{
		public struct VisualItemInfo
		{
			public uint DisplayId;

			public uint DisplayEnchantId;

			public uint SecondaryItemModifiedAppearanceID;

			public byte InvType;

			public byte Subclass;

			public void Write(WorldPacket data)
			{
				data.WriteUInt32(this.DisplayId);
				data.WriteUInt32(this.DisplayEnchantId);
				data.WriteUInt32(this.SecondaryItemModifiedAppearanceID);
				data.WriteUInt8(this.InvType);
				data.WriteUInt8(this.Subclass);
			}
		}

		public struct PetInfo
		{
			public uint CreatureDisplayId;

			public uint Level;

			public uint CreatureFamily;
		}

		public WowGuid128 Guid;

		public ulong GuildClubMemberID;

		public string Name;

		public byte ListPosition;

		public Race RaceId;

		public Class ClassId;

		public Gender SexId;

		public Array<ChrCustomizationChoice> Customizations;

		public byte ExperienceLevel;

		public uint ZoneId;

		public uint MapId;

		public Vector3 PreloadPos;

		public WowGuid128 GuildGuid;

		public CharacterFlags Flags;

		public uint Flags2;

		public uint Flags3;

		public uint Flags4;

		public bool FirstLogin;

		public byte unkWod61x;

		public bool ExpansionChosen;

		public ulong LastPlayedTime;

		public ushort SpecID;

		public uint Unknown703;

		public uint LastLoginVersion;

		public uint OverrideSelectScreenFileDataID;

		public uint PetCreatureDisplayId;

		public uint PetExperienceLevel;

		public uint PetCreatureFamilyId;

		public bool BoostInProgress;

		public uint[] ProfessionIds = new uint[2];

		public VisualItemInfo[] VisualItems = new VisualItemInfo[23];

		public List<string> MailSenders = new List<string>();

		public List<uint> MailSenderTypes = new List<uint>();

		public void Write(WorldPacket data)
		{
			long startPos = data.GetSize();
			data.WritePackedGuid128(this.Guid);
			data.WriteUInt64(this.GuildClubMemberID);
			data.WriteUInt8(this.ListPosition);
			data.WriteUInt8((byte)this.RaceId);
			data.WriteUInt8((byte)this.ClassId);
			data.WriteUInt8((byte)this.SexId);
			data.WriteInt32(this.Customizations.Count);
			data.WriteUInt8(this.ExperienceLevel);
			data.WriteUInt32(this.ZoneId);
			data.WriteUInt32(this.MapId);
			data.WriteVector3(this.PreloadPos);
			data.WritePackedGuid128(this.GuildGuid);
			data.WriteUInt32((uint)this.Flags);
			data.WriteUInt32(this.Flags2);
			data.WriteUInt32(this.Flags3);
			data.WriteUInt32(this.PetCreatureDisplayId);
			data.WriteUInt32(this.PetExperienceLevel);
			data.WriteUInt32(this.PetCreatureFamilyId);
			data.WriteUInt32(this.ProfessionIds[0]);
			data.WriteUInt32(this.ProfessionIds[1]);
			int visualItemCount = ((ModernVersion.ExpansionVersion >= 3) ? 34 : this.VisualItems.Length);
			for (int vi = 0; vi < visualItemCount; vi++)
			{
				if (vi < this.VisualItems.Length)
				{
					this.VisualItems[vi].Write(data);
				}
				else
				{
					default(VisualItemInfo).Write(data);
				}
			}
			data.WriteUInt64(this.LastPlayedTime);
			data.WriteUInt16(this.SpecID);
			if (ModernVersion.ExpansionVersion >= 3)
			{
				data.WriteInt32(0);
				data.WriteInt32((int)this.LastLoginVersion);
			}
			else
			{
				data.WriteUInt32(this.Unknown703);
				data.WriteUInt32(this.LastLoginVersion);
			}
			data.WriteUInt32(this.Flags4);
			data.WriteInt32(this.MailSenders.Count);
			data.WriteInt32(this.MailSenderTypes.Count);
			data.WriteUInt32(this.OverrideSelectScreenFileDataID);
			foreach (ChrCustomizationChoice customization in this.Customizations)
			{
				data.WriteUInt32(customization.ChrCustomizationOptionID);
				data.WriteUInt32(customization.ChrCustomizationChoiceID);
			}
			foreach (uint mailSenderType in this.MailSenderTypes)
			{
				data.WriteUInt32(mailSenderType);
			}
			data.WriteBits(this.Name.GetByteCount(), 6);
			data.WriteBit(this.FirstLogin);
			data.WriteBit(this.BoostInProgress);
			data.WriteBits(this.unkWod61x, 5);
			if (ModernVersion.ExpansionVersion >= 3)
			{
				data.WriteBits(0, 2);
				data.WriteBit(bit: false);
				data.WriteBit(bit: false);
			}
			else
			{
				data.WriteBit(bit: false);
				data.WriteBit(this.ExpansionChosen);
			}
			foreach (string str in this.MailSenders)
			{
				data.WriteBits(str.GetByteCount() + 1, 6);
			}
			data.FlushBits();
			foreach (string str2 in this.MailSenders)
			{
				if (!str2.IsEmpty())
				{
					data.WriteCString(str2);
				}
			}
			data.WriteString(this.Name);
			long totalSize = data.GetSize() - startPos;
			byte[] allData = data.GetData();
			int dumpStart = (int)startPos;
			int dumpLen = Math.Min(40, (int)totalSize);
			string hex = BitConverter.ToString(allData, dumpStart, dumpLen);
			int lastStart = Math.Max(0, (int)totalSize - 30);
			string lastHex = BitConverter.ToString(allData, dumpStart + lastStart, (int)totalSize - lastStart);
			Log.Print(LogType.Debug, $"CharacterInfo: name={this.Name} race={this.RaceId} class={this.ClassId} level={this.ExperienceLevel} visItems={visualItemCount} totalBytes={totalSize}", "Write", "CharacterPackets.cs");
			Log.Print(LogType.Debug, "CharacterInfo LAST 30 bytes: " + lastHex, "Write", "CharacterPackets.cs");
		}
	}

	public struct RaceUnlock
	{
		public int RaceID;

		public bool HasExpansion;

		public bool HasAchievement;

		public bool HasHeritageArmor;

		public RaceUnlock(int raceId, bool hasExpansion, bool hasAchievement, bool hasHeritageArmor)
		{
			this.RaceID = raceId;
			this.HasExpansion = hasExpansion;
			this.HasAchievement = hasAchievement;
			this.HasHeritageArmor = hasHeritageArmor;
		}

		public void Write(WorldPacket data)
		{
			data.WriteInt32(this.RaceID);
			data.WriteBit(this.HasExpansion);
			data.WriteBit(this.HasAchievement);
			data.WriteBit(this.HasHeritageArmor);
			data.FlushBits();
		}
	}

	public struct UnlockedConditionalAppearance
	{
		public int AchievementID;

		public int Unused;

		public void Write(WorldPacket data)
		{
			data.WriteInt32(this.AchievementID);
			data.WriteInt32(this.Unused);
		}
	}

	public bool Success;

	public bool IsDeletedCharacters;

	public bool IsNewPlayerRestrictionSkipped;

	public bool IsNewPlayerRestricted;

	public bool IsNewPlayer;

	public bool IsAlliedRacesCreationAllowed;

	public int MaxCharacterLevel = 1;

	public uint? DisabledClassesMask = 0u;

	public List<CharacterInfo> Characters = new List<CharacterInfo>();

	public List<RaceUnlock> RaceUnlockData = new List<RaceUnlock>();

	public List<UnlockedConditionalAppearance> UnlockedConditionalAppearances = new List<UnlockedConditionalAppearance>();

	public EnumCharactersResult()
		: base(Opcode.SMSG_ENUM_CHARACTERS_RESULT)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteBit(this.Success);
		base._worldPacket.WriteBit(this.IsDeletedCharacters);
		base._worldPacket.WriteBit(this.IsNewPlayerRestrictionSkipped);
		base._worldPacket.WriteBit(this.IsNewPlayerRestricted);
		base._worldPacket.WriteBit(this.IsNewPlayer);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			base._worldPacket.WriteBit(bit: false);
			base._worldPacket.WriteBit(this.DisabledClassesMask.HasValue);
			base._worldPacket.WriteUInt32((uint)this.Characters.Count);
			base._worldPacket.WriteInt32(this.MaxCharacterLevel);
			base._worldPacket.WriteUInt32((uint)this.RaceUnlockData.Count);
			base._worldPacket.WriteUInt32((uint)this.UnlockedConditionalAppearances.Count);
			base._worldPacket.WriteUInt32(0u);
			if (this.DisabledClassesMask.HasValue)
			{
				base._worldPacket.WriteUInt32(this.DisabledClassesMask.Value);
			}
			foreach (UnlockedConditionalAppearance unlockedConditionalAppearance2 in this.UnlockedConditionalAppearances)
			{
				unlockedConditionalAppearance2.Write(base._worldPacket);
			}
			foreach (CharacterInfo charInfo in this.Characters)
			{
				charInfo.Write(base._worldPacket);
			}
			{
				foreach (RaceUnlock raceUnlockDatum in this.RaceUnlockData)
				{
					raceUnlockDatum.Write(base._worldPacket);
				}
				return;
			}
		}
		base._worldPacket.WriteBit(this.DisabledClassesMask.HasValue);
		base._worldPacket.WriteBit(this.IsAlliedRacesCreationAllowed);
		base._worldPacket.WriteInt32(this.Characters.Count);
		base._worldPacket.WriteInt32(this.MaxCharacterLevel);
		base._worldPacket.WriteInt32(this.RaceUnlockData.Count);
		base._worldPacket.WriteInt32(this.UnlockedConditionalAppearances.Count);
		if (this.DisabledClassesMask.HasValue)
		{
			base._worldPacket.WriteUInt32(this.DisabledClassesMask.Value);
		}
		foreach (UnlockedConditionalAppearance unlockedConditionalAppearance3 in this.UnlockedConditionalAppearances)
		{
			unlockedConditionalAppearance3.Write(base._worldPacket);
		}
		foreach (CharacterInfo charInfo2 in this.Characters)
		{
			charInfo2.Write(base._worldPacket);
		}
		foreach (RaceUnlock raceUnlockDatum2 in this.RaceUnlockData)
		{
			raceUnlockDatum2.Write(base._worldPacket);
		}
	}
}
