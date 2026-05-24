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
			this.ClassID = classId;
			this.ActiveExpansionLevel = activeExpLevel;
			this.AccountExpansionLevel = accountExpLevel;
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
			this.FactionGroup = factionGroup;
			this.ClassID = classID;
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
		base._worldPacket.WriteUInt32((uint)this.Result);
		base._worldPacket.WriteBit(this.SuccessInfo != null);
		base._worldPacket.WriteBit(this.WaitInfo != null);
		base._worldPacket.FlushBits();
		if (this.SuccessInfo != null)
		{
			base._worldPacket.WriteUInt32(this.SuccessInfo.VirtualRealmAddress);
			base._worldPacket.WriteInt32(this.SuccessInfo.VirtualRealms.Count);
			base._worldPacket.WriteUInt32(this.SuccessInfo.TimeRested);
			base._worldPacket.WriteUInt8(this.SuccessInfo.ActiveExpansionLevel);
			base._worldPacket.WriteUInt8(this.SuccessInfo.AccountExpansionLevel);
			base._worldPacket.WriteUInt32(this.SuccessInfo.TimeSecondsUntilPCKick);
			base._worldPacket.WriteInt32(this.SuccessInfo.AvailableClasses.Count);
			base._worldPacket.WriteInt32(this.SuccessInfo.Templates.Count);
			base._worldPacket.WriteUInt32(this.SuccessInfo.CurrencyID);
			base._worldPacket.WriteInt64(this.SuccessInfo.Time);
			foreach (RaceClassAvailability raceClassAvailability in this.SuccessInfo.AvailableClasses)
			{
				base._worldPacket.WriteUInt8(raceClassAvailability.RaceID);
				base._worldPacket.WriteInt32(raceClassAvailability.Classes.Count);
				foreach (ClassAvailability classAvailability in raceClassAvailability.Classes)
				{
					base._worldPacket.WriteUInt8(classAvailability.ClassID);
					base._worldPacket.WriteUInt8(classAvailability.ActiveExpansionLevel);
					base._worldPacket.WriteUInt8(classAvailability.AccountExpansionLevel);
					if (ModernVersion.ExpansionVersion >= 3)
					{
						base._worldPacket.WriteUInt8(0);
					}
				}
			}
			base._worldPacket.WriteBit(this.SuccessInfo.IsExpansionTrial);
			base._worldPacket.WriteBit(this.SuccessInfo.ForceCharacterTemplate);
			base._worldPacket.WriteBit(this.SuccessInfo.NumPlayersHorde.HasValue);
			base._worldPacket.WriteBit(this.SuccessInfo.NumPlayersAlliance.HasValue);
			base._worldPacket.WriteBit(this.SuccessInfo.ExpansionTrialExpiration.HasValue);
			if (ModernVersion.ExpansionVersion >= 3)
			{
				base._worldPacket.WriteBit(bit: false);
			}
			base._worldPacket.FlushBits();
			base._worldPacket.WriteUInt32(this.SuccessInfo.GameTimeInfo.BillingPlan);
			base._worldPacket.WriteUInt32(this.SuccessInfo.GameTimeInfo.TimeRemain);
			base._worldPacket.WriteUInt32(this.SuccessInfo.GameTimeInfo.Unknown735);
			base._worldPacket.WriteBit(this.SuccessInfo.GameTimeInfo.InGameRoom);
			base._worldPacket.WriteBit(this.SuccessInfo.GameTimeInfo.InGameRoom);
			base._worldPacket.WriteBit(this.SuccessInfo.GameTimeInfo.InGameRoom);
			base._worldPacket.FlushBits();
			if (this.SuccessInfo.NumPlayersHorde.HasValue)
			{
				base._worldPacket.WriteUInt16(this.SuccessInfo.NumPlayersHorde.Value);
			}
			if (this.SuccessInfo.NumPlayersAlliance.HasValue)
			{
				base._worldPacket.WriteUInt16(this.SuccessInfo.NumPlayersAlliance.Value);
			}
			if (this.SuccessInfo.ExpansionTrialExpiration.HasValue)
			{
				base._worldPacket.WriteInt32(this.SuccessInfo.ExpansionTrialExpiration.Value);
			}
			foreach (VirtualRealmInfo virtualRealm2 in this.SuccessInfo.VirtualRealms)
			{
				virtualRealm2.Write(base._worldPacket);
			}
			foreach (CharacterTemplate templat in this.SuccessInfo.Templates)
			{
				base._worldPacket.WriteUInt32(templat.TemplateSetId);
				base._worldPacket.WriteInt32(templat.Classes.Count);
				foreach (CharacterTemplateClass templateClass in templat.Classes)
				{
					base._worldPacket.WriteUInt8(templateClass.ClassID);
					base._worldPacket.WriteUInt8((byte)templateClass.FactionGroup);
				}
				base._worldPacket.WriteBits(templat.Name.GetByteCount(), 7);
				base._worldPacket.WriteBits(templat.Description.GetByteCount(), 10);
				base._worldPacket.FlushBits();
				base._worldPacket.WriteString(templat.Name);
				base._worldPacket.WriteString(templat.Description);
			}
		}
		if (this.WaitInfo != null)
		{
			this.WaitInfo.Write(base._worldPacket);
		}
	}
}
