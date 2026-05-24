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
				data.WriteUInt32(DisplayId);
				data.WriteUInt32(DisplayEnchantId);
				data.WriteUInt32(SecondaryItemModifiedAppearanceID);
				data.WriteUInt8(InvType);
				data.WriteUInt8(Subclass);
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

		public List<string> MailSenders = new();

		public List<uint> MailSenderTypes = new();

		public void Write(WorldPacket data)
		{
			long startPos = data.GetSize();
			data.WritePackedGuid128(Guid);
			data.WriteUInt64(GuildClubMemberID);
			data.WriteUInt8(ListPosition);
			data.WriteUInt8((byte)RaceId);
			data.WriteUInt8((byte)ClassId);
			data.WriteUInt8((byte)SexId);
			data.WriteInt32(Customizations.Count);
			data.WriteUInt8(ExperienceLevel);
			data.WriteUInt32(ZoneId);
			data.WriteUInt32(MapId);
			data.WriteVector3(PreloadPos);
			data.WritePackedGuid128(GuildGuid);
			data.WriteUInt32((uint)Flags);
			data.WriteUInt32(Flags2);
			data.WriteUInt32(Flags3);
			data.WriteUInt32(PetCreatureDisplayId);
			data.WriteUInt32(PetExperienceLevel);
			data.WriteUInt32(PetCreatureFamilyId);
			data.WriteUInt32(ProfessionIds[0]);
			data.WriteUInt32(ProfessionIds[1]);
			var visualItemCount = ModernVersion.ExpansionVersion >= 3 ? 34 : VisualItems.Length;
			for (var vi = 0; vi < visualItemCount; vi++)
			{
				if (vi < VisualItems.Length)
				{
					VisualItems[vi].Write(data);
				}
				else
				{
					default(VisualItemInfo).Write(data);
				}
			}
			data.WriteUInt64(LastPlayedTime);
			data.WriteUInt16(SpecID);
			if (ModernVersion.ExpansionVersion >= 3)
			{
				data.WriteInt32(0);
				data.WriteInt32((int)LastLoginVersion);
			}
			else
			{
				data.WriteUInt32(Unknown703);
				data.WriteUInt32(LastLoginVersion);
			}
			data.WriteUInt32(Flags4);
			data.WriteInt32(MailSenders.Count);
			data.WriteInt32(MailSenderTypes.Count);
			data.WriteUInt32(OverrideSelectScreenFileDataID);
			foreach (var customization in Customizations)
			{
				data.WriteUInt32(customization.ChrCustomizationOptionID);
				data.WriteUInt32(customization.ChrCustomizationChoiceID);
			}
			foreach (var mailSenderType in MailSenderTypes)
			{
				data.WriteUInt32(mailSenderType);
			}
			data.WriteBits(Name.GetByteCount(), 6);
			data.WriteBit(FirstLogin);
			data.WriteBit(BoostInProgress);
			data.WriteBits(unkWod61x, 5);
			if (ModernVersion.ExpansionVersion >= 3)
			{
				data.WriteBits(0, 2);
				data.WriteBit(bit: false);
				data.WriteBit(bit: false);
			}
			else
			{
				data.WriteBit(bit: false);
				data.WriteBit(ExpansionChosen);
			}
			foreach (var str in MailSenders)
			{
				data.WriteBits(str.GetByteCount() + 1, 6);
			}
			data.FlushBits();
			foreach (var str2 in MailSenders)
			{
				if (!str2.IsEmpty())
				{
					data.WriteCString(str2);
				}
			}
			data.WriteString(Name);
			var totalSize = data.GetSize() - startPos;
			var allData = data.GetData();
			var dumpStart = (int)startPos;
			var dumpLen = Math.Min(40, (int)totalSize);
			var hex = BitConverter.ToString(allData, dumpStart, dumpLen);
			var lastStart = Math.Max(0, (int)totalSize - 30);
			var lastHex = BitConverter.ToString(allData, dumpStart + lastStart, (int)totalSize - lastStart);
			Log.Print(LogType.Debug, $"CharacterInfo: name={Name} race={RaceId} class={ClassId} level={ExperienceLevel} visItems={visualItemCount} totalBytes={totalSize}", "CharacterPackets.cs");
			Log.Print(LogType.Debug, "CharacterInfo LAST 30 bytes: " + lastHex, "CharacterPackets.cs");
		}
	}

	public struct RaceUnlock
	{
		public readonly int RaceID;

		public readonly bool HasExpansion;

		public readonly bool HasAchievement;

		public readonly bool HasHeritageArmor;

		public RaceUnlock(int raceId, bool hasExpansion, bool hasAchievement, bool hasHeritageArmor)
		{
			RaceID = raceId;
			HasExpansion = hasExpansion;
			HasAchievement = hasAchievement;
			HasHeritageArmor = hasHeritageArmor;
		}

		public void Write(WorldPacket data)
		{
			data.WriteInt32(RaceID);
			data.WriteBit(HasExpansion);
			data.WriteBit(HasAchievement);
			data.WriteBit(HasHeritageArmor);
			data.FlushBits();
		}
	}

	public struct UnlockedConditionalAppearance
	{
		public int AchievementID;

		public int Unused;

		public void Write(WorldPacket data)
		{
			data.WriteInt32(AchievementID);
			data.WriteInt32(Unused);
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

	public List<CharacterInfo> Characters = new();

	public List<RaceUnlock> RaceUnlockData = new();

	public List<UnlockedConditionalAppearance> UnlockedConditionalAppearances = new();

	public EnumCharactersResult()
		: base(Opcode.SMSG_ENUM_CHARACTERS_RESULT)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteBit(Success);
		_worldPacket.WriteBit(IsDeletedCharacters);
		_worldPacket.WriteBit(IsNewPlayerRestrictionSkipped);
		_worldPacket.WriteBit(IsNewPlayerRestricted);
		_worldPacket.WriteBit(IsNewPlayer);
		if (ModernVersion.ExpansionVersion >= 3)
		{
			_worldPacket.WriteBit(bit: false);
			_worldPacket.WriteBit(DisabledClassesMask.HasValue);
			_worldPacket.WriteUInt32((uint)Characters.Count);
			_worldPacket.WriteInt32(MaxCharacterLevel);
			_worldPacket.WriteUInt32((uint)RaceUnlockData.Count);
			_worldPacket.WriteUInt32((uint)UnlockedConditionalAppearances.Count);
			_worldPacket.WriteUInt32(0u);
			if (DisabledClassesMask.HasValue)
			{
				_worldPacket.WriteUInt32(DisabledClassesMask.Value);
			}
			foreach (var unlockedConditionalAppearance2 in UnlockedConditionalAppearances)
			{
				unlockedConditionalAppearance2.Write(_worldPacket);
			}
			foreach (var charInfo in Characters)
			{
				charInfo.Write(_worldPacket);
			}
			{
				foreach (var raceUnlockDatum in RaceUnlockData)
				{
					raceUnlockDatum.Write(_worldPacket);
				}
				return;
			}
		}
		_worldPacket.WriteBit(DisabledClassesMask.HasValue);
		_worldPacket.WriteBit(IsAlliedRacesCreationAllowed);
		_worldPacket.WriteInt32(Characters.Count);
		_worldPacket.WriteInt32(MaxCharacterLevel);
		_worldPacket.WriteInt32(RaceUnlockData.Count);
		_worldPacket.WriteInt32(UnlockedConditionalAppearances.Count);
		if (DisabledClassesMask.HasValue)
		{
			_worldPacket.WriteUInt32(DisabledClassesMask.Value);
		}
		foreach (var unlockedConditionalAppearance3 in UnlockedConditionalAppearances)
		{
			unlockedConditionalAppearance3.Write(_worldPacket);
		}
		foreach (var charInfo2 in Characters)
		{
			charInfo2.Write(_worldPacket);
		}
		foreach (var raceUnlockDatum2 in RaceUnlockData)
		{
			raceUnlockDatum2.Write(_worldPacket);
		}
	}
}
