using System;
using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class AuthResponse : ServerPacket
{
	public enum FactionMasks : byte
	{
		Player = 1,
		Alliance = 2,
		Horde = 4,
		Monster = 8
	}

	public class ClassAvailability
	{
		public byte ClassID;

		public byte ActiveExpansionLevel;

		public byte AccountExpansionLevel;

		public ClassAvailability(byte classId, byte activeExpLevel, byte accountExpLevel)
		{
			ClassID = classId;
			ActiveExpansionLevel = activeExpLevel;
			AccountExpansionLevel = accountExpLevel;
		}
	}

	public class RaceClassAvailability
	{
		public byte RaceID;

		public List<ClassAvailability> Classes = new List<ClassAvailability>();
	}

	public struct CharacterTemplateClass
	{
		public FactionMasks FactionGroup;

		public byte ClassID;

		public CharacterTemplateClass(FactionMasks factionGroup, byte classID)
		{
			FactionGroup = factionGroup;
			ClassID = classID;
		}
	}

	public class CharacterTemplate
	{
		public uint TemplateSetId;

		public List<CharacterTemplateClass> Classes;

		public string Name;

		public string Description;

		public byte Level;
	}

	public class AuthSuccessInfo
	{
		public struct GameTime
		{
			public uint BillingPlan;

			public uint TimeRemain;

			public uint Unknown735;

			public bool InGameRoom;
		}

		public byte ActiveExpansionLevel;

		public byte AccountExpansionLevel;

		public uint TimeRested;

		public uint VirtualRealmAddress;

		public uint TimeSecondsUntilPCKick;

		public uint CurrencyID;

		public long Time;

		public GameTime GameTimeInfo;

		public List<VirtualRealmInfo> VirtualRealms = new List<VirtualRealmInfo>();

		public List<CharacterTemplate> Templates = new List<CharacterTemplate>();

		public List<RaceClassAvailability> AvailableClasses;

		public bool IsExpansionTrial;

		public bool ForceCharacterTemplate;

		public ushort? NumPlayersHorde;

		public ushort? NumPlayersAlliance;

		public int? ExpansionTrialExpiration;
	}

	public AuthSuccessInfo SuccessInfo;

	public AuthWaitInfo WaitInfo;

	public BattlenetRpcErrorCode Result;

	public AuthResponse()
		: base(Opcode.SMSG_AUTH_RESPONSE)
	{
	}

	protected override void Write()
	{
		_worldPacket.WriteUInt32((uint)Result);
		_worldPacket.WriteBit(SuccessInfo != null);
		_worldPacket.WriteBit(WaitInfo != null);
		_worldPacket.FlushBits();
		if (SuccessInfo != null)
		{
			_worldPacket.WriteUInt32(SuccessInfo.VirtualRealmAddress);
			_worldPacket.WriteInt32(SuccessInfo.VirtualRealms.Count);
			_worldPacket.WriteUInt32(SuccessInfo.TimeRested);
			_worldPacket.WriteUInt8(SuccessInfo.ActiveExpansionLevel);
			_worldPacket.WriteUInt8(SuccessInfo.AccountExpansionLevel);
			_worldPacket.WriteUInt32(SuccessInfo.TimeSecondsUntilPCKick);
			_worldPacket.WriteInt32(SuccessInfo.AvailableClasses.Count);
			_worldPacket.WriteInt32(SuccessInfo.Templates.Count);
			_worldPacket.WriteUInt32(SuccessInfo.CurrencyID);
			_worldPacket.WriteInt64(SuccessInfo.Time);
			foreach (RaceClassAvailability raceClassAvailability in SuccessInfo.AvailableClasses)
			{
				_worldPacket.WriteUInt8(raceClassAvailability.RaceID);
				_worldPacket.WriteInt32(raceClassAvailability.Classes.Count);
				foreach (ClassAvailability classAvailability in raceClassAvailability.Classes)
				{
					_worldPacket.WriteUInt8(classAvailability.ClassID);
					_worldPacket.WriteUInt8(classAvailability.ActiveExpansionLevel);
					_worldPacket.WriteUInt8(classAvailability.AccountExpansionLevel);
					if (ModernVersion.ExpansionVersion >= 3)
					{
						_worldPacket.WriteUInt8(0);
					}
				}
			}
			_worldPacket.WriteBit(SuccessInfo.IsExpansionTrial);
			_worldPacket.WriteBit(SuccessInfo.ForceCharacterTemplate);
			_worldPacket.WriteBit(SuccessInfo.NumPlayersHorde.HasValue);
			_worldPacket.WriteBit(SuccessInfo.NumPlayersAlliance.HasValue);
			_worldPacket.WriteBit(SuccessInfo.ExpansionTrialExpiration.HasValue);
			if (ModernVersion.ExpansionVersion >= 3)
			{
				_worldPacket.WriteBit(bit: false);
			}
			_worldPacket.FlushBits();
			_worldPacket.WriteUInt32(SuccessInfo.GameTimeInfo.BillingPlan);
			_worldPacket.WriteUInt32(SuccessInfo.GameTimeInfo.TimeRemain);
			_worldPacket.WriteUInt32(SuccessInfo.GameTimeInfo.Unknown735);
			_worldPacket.WriteBit(SuccessInfo.GameTimeInfo.InGameRoom);
			_worldPacket.WriteBit(SuccessInfo.GameTimeInfo.InGameRoom);
			_worldPacket.WriteBit(SuccessInfo.GameTimeInfo.InGameRoom);
			_worldPacket.FlushBits();
			if (SuccessInfo.NumPlayersHorde.HasValue)
			{
				_worldPacket.WriteUInt16(SuccessInfo.NumPlayersHorde.Value);
			}
			if (SuccessInfo.NumPlayersAlliance.HasValue)
			{
				_worldPacket.WriteUInt16(SuccessInfo.NumPlayersAlliance.Value);
			}
			if (SuccessInfo.ExpansionTrialExpiration.HasValue)
			{
				_worldPacket.WriteInt32(SuccessInfo.ExpansionTrialExpiration.Value);
			}
			foreach (VirtualRealmInfo virtualRealm2 in SuccessInfo.VirtualRealms)
			{
				virtualRealm2.Write(_worldPacket);
			}
			foreach (CharacterTemplate templat in SuccessInfo.Templates)
			{
				_worldPacket.WriteUInt32(templat.TemplateSetId);
				_worldPacket.WriteInt32(templat.Classes.Count);
				foreach (CharacterTemplateClass templateClass in templat.Classes)
				{
					_worldPacket.WriteUInt8(templateClass.ClassID);
					_worldPacket.WriteUInt8((byte)templateClass.FactionGroup);
				}
				_worldPacket.WriteBits(templat.Name.GetByteCount(), 7);
				_worldPacket.WriteBits(templat.Description.GetByteCount(), 10);
				_worldPacket.FlushBits();
				_worldPacket.WriteString(templat.Name);
				_worldPacket.WriteString(templat.Description);
			}
		}
		if (WaitInfo != null)
		{
			WaitInfo.Write(_worldPacket);
		}
	}
}
