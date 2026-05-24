using System;
using Framework.GameMath;
using Framework.Logging;
using HermesProxy.Enums;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Objects;

public sealed class MovementInfo
{
	public const float DEFAULT_WALK_SPEED = 2.5f;

	public const float DEFAULT_RUN_SPEED = 7f;

	public const float DEFAULT_RUN_BACK_SPEED = 4.5f;

	public const float DEFAULT_SWIM_SPEED = 4.72222f;

	public const float DEFAULT_SWIM_BACK_SPEED = 2.5f;

	public const float DEFAULT_FLY_SPEED = 7f;

	public const float DEFAULT_FLY_BACK_SPEED = 4.5f;

	public const float DEFAULT_TURN_RATE = 3.141593f;

	public const float DEFAULT_PITCH_RATE = 3.141593f;

	public uint Flags;

	public uint FlagsExtra;

	public uint FlagsExtra2;

	public uint MoveTime;

	public float SwimPitch;

	public uint FallTime;

	public float JumpHorizontalSpeed;

	public float JumpVerticalSpeed;

	public float JumpCosAngle;

	public float JumpSinAngle;

	public float SplineElevation;

	public bool HasSplineData;

	public Vector3 Position;

	public float Orientation;

	public float CorpseOrientation;

	public WowGuid128 TransportGuid;

	public Vector3 TransportOffset;

	public float TransportOrientation;

	public uint TransportTime;

	public uint TransportTime2;

	public sbyte TransportSeat = -1;

	public Quaternion Rotation;

	public float WalkSpeed;

	public float RunSpeed;

	public float RunBackSpeed;

	public float SwimSpeed;

	public float SwimBackSpeed;

	public float FlightSpeed;

	public float FlightBackSpeed;

	public float TurnRate;

	public float PitchRate;

	public bool Hover;

	public float VehicleOrientation;

	public uint VehicleId;

	public uint TransportPathTimer;

	public MovementInfo CopyFromMe()
	{
		MovementInfo copy = new MovementInfo();
		copy.Flags = Flags;
		copy.FlagsExtra = FlagsExtra;
		copy.SwimPitch = SwimPitch;
		copy.FallTime = FallTime;
		copy.JumpHorizontalSpeed = JumpHorizontalSpeed;
		copy.JumpVerticalSpeed = JumpVerticalSpeed;
		copy.JumpCosAngle = JumpCosAngle;
		copy.JumpSinAngle = JumpSinAngle;
		copy.SplineElevation = SplineElevation;
		copy.HasSplineData = HasSplineData;
		copy.Position = Position;
		copy.Orientation = Orientation;
		copy.CorpseOrientation = CorpseOrientation;
		copy.TransportGuid = TransportGuid;
		copy.TransportOffset = TransportOffset;
		copy.TransportOrientation = TransportOrientation;
		copy.TransportTime = TransportTime;
		copy.TransportTime2 = TransportTime2;
		copy.TransportSeat = TransportSeat;
		copy.Rotation = Rotation;
		copy.WalkSpeed = WalkSpeed;
		copy.RunSpeed = RunSpeed;
		copy.RunBackSpeed = RunBackSpeed;
		copy.SwimSpeed = SwimSpeed;
		copy.SwimBackSpeed = SwimBackSpeed;
		copy.FlightSpeed = FlightSpeed;
		copy.FlightBackSpeed = FlightBackSpeed;
		copy.TurnRate = TurnRate;
		copy.PitchRate = PitchRate;
		copy.Hover = Hover;
		copy.VehicleId = VehicleId;
		copy.VehicleOrientation = VehicleOrientation;
		copy.TransportPathTimer = TransportPathTimer;
		return copy;
	}

	public void SetMovementFlags(MovementFlagModern f)
	{
		Flags = (uint)f;
	}

	public void AddMovementFlag(MovementFlagModern f)
	{
		Flags |= (uint)f;
	}

	public void RemoveMovementFlag(MovementFlagModern f)
	{
		Flags &= (uint)(~f);
	}

	public bool HasMovementFlag(MovementFlagModern f)
	{
		return (Flags & (uint)f) != 0;
	}

	public void ReadMovementInfoLegacy(WorldPacket packet, GameSessionData gameState)
	{
		bool hasPitch;
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			MovementFlagWotLK flags = (MovementFlagWotLK)(Flags = packet.ReadUInt32());
			FlagsExtra = packet.ReadUInt16();
			hasPitch = flags.HasAnyFlag(MovementFlagWotLK.Swimming | MovementFlagWotLK.Flying) || FlagsExtra.HasAnyFlag(MovementFlagExtra.AlwaysAllowPitching);
		}
		else if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			MovementFlagTBC flags2 = (MovementFlagTBC)packet.ReadUInt32();
			Flags = (uint)flags2.CastFlags<MovementFlagWotLK>();
			FlagsExtra = packet.ReadUInt8();
			hasPitch = flags2.HasAnyFlag(MovementFlagTBC.Swimming | MovementFlagTBC.Flying2);
		}
		else
		{
			MovementFlagVanilla flags3 = (MovementFlagVanilla)packet.ReadUInt32();
			Flags = (uint)flags3.CastFlags<MovementFlagWotLK>();
			hasPitch = flags3.HasAnyFlag(MovementFlagVanilla.Swimming);
			Hover = flags3.HasAnyFlag(MovementFlagVanilla.FixedZ);
		}
		MoveTime = packet.ReadUInt32();
		Position = packet.ReadVector3();
		Orientation = packet.ReadFloat();
		if (Flags.HasAnyFlag(MovementFlagWotLK.OnTransport))
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
			{
				TransportGuid = packet.ReadPackedGuid().To128(gameState);
			}
			else
			{
				TransportGuid = packet.ReadGuid().To128(gameState);
			}
			TransportOffset = packet.ReadVector3();
			TransportOrientation = packet.ReadFloat();
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				TransportTime = packet.ReadUInt32();
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				TransportSeat = packet.ReadInt8();
			}
			if (FlagsExtra.HasAnyFlag(MovementFlagExtra.InterpolateMove))
			{
				TransportTime2 = packet.ReadUInt32();
			}
		}
		if (hasPitch)
		{
			SwimPitch = packet.ReadFloat();
		}
		FallTime = packet.ReadUInt32();
		if (Flags.HasAnyFlag(MovementFlagWotLK.Falling))
		{
			JumpVerticalSpeed = packet.ReadFloat();
			JumpSinAngle = packet.ReadFloat();
			JumpCosAngle = packet.ReadFloat();
			JumpHorizontalSpeed = packet.ReadFloat();
		}
		if (Flags.HasAnyFlag(MovementFlagWotLK.SplineElevation))
		{
			SplineElevation = packet.ReadFloat();
		}
	}

	public void WriteMovementInfoLegacy(WorldPacket data)
	{
		uint flags = (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? ((uint)((MovementFlagModern)Flags).CastFlags<MovementFlagWotLK>()) : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? ((uint)((MovementFlagModern)Flags).CastFlags<MovementFlagVanilla>()) : ((uint)((MovementFlagModern)Flags).CastFlags<MovementFlagTBC>())));
		if (TransportGuid != null)
		{
			flags = (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? (flags | 0x200) : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? (flags | 0x2000000) : (flags | 0x200)));
		}
		data.WriteUInt32(flags);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
		{
			data.WriteUInt16((ushort)FlagsExtra);
		}
		else if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
		{
			data.WriteUInt8((byte)FlagsExtra);
		}
		data.WriteUInt32(MoveTime);
		data.WriteVector3(Position);
		data.WriteFloat(Orientation);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? flags.HasAnyFlag(MovementFlagWotLK.OnTransport) : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? flags.HasAnyFlag(MovementFlagVanilla.OnTransport) : flags.HasAnyFlag(MovementFlagTBC.OnTransport)))
		{
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
			{
				data.WritePackedGuid(TransportGuid.To64());
			}
			else
			{
				data.WriteGuid(TransportGuid.To64());
			}
			data.WriteVector3(TransportOffset);
			data.WriteFloat(TransportOrientation);
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
			{
				data.WriteUInt32(TransportTime);
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
			{
				data.WriteInt8(TransportSeat);
			}
			if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) && FlagsExtra.HasAnyFlag(MovementFlagExtra.InterpolateMove))
			{
				data.WriteUInt32(TransportTime2);
			}
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? (flags.HasAnyFlag(MovementFlagWotLK.Swimming | MovementFlagWotLK.Flying) || FlagsExtra.HasAnyFlag(MovementFlagExtra.AlwaysAllowPitching)) : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? flags.HasAnyFlag(MovementFlagVanilla.Swimming) : flags.HasAnyFlag(MovementFlagTBC.Swimming | MovementFlagTBC.Flying2)))
		{
			data.WriteFloat(SwimPitch);
		}
		data.WriteUInt32(FallTime);
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? flags.HasAnyFlag(MovementFlagWotLK.Falling) : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? flags.HasAnyFlag(MovementFlagVanilla.Falling) : flags.HasAnyFlag(MovementFlagTBC.Falling)))
		{
			data.WriteFloat(JumpVerticalSpeed);
			data.WriteFloat(JumpSinAngle);
			data.WriteFloat(JumpCosAngle);
			data.WriteFloat(JumpHorizontalSpeed);
		}
		if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? flags.HasAnyFlag(MovementFlagWotLK.SplineElevation) : ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? flags.HasAnyFlag(MovementFlagVanilla.SplineElevation) : flags.HasAnyFlag(MovementFlagTBC.SplineElevation)))
		{
			data.WriteFloat(SplineElevation);
		}
	}

	public void ReadMovementInfoModern(WorldPacket data)
	{
		if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
		{
			Flags = data.ReadUInt32();
			FlagsExtra = data.ReadUInt32();
			FlagsExtra2 = data.ReadUInt32();
		}
		MoveTime = data.ReadUInt32();
		Position = data.ReadVector3();
		Orientation = data.ReadFloat();
		SwimPitch = data.ReadFloat();
		SplineElevation = data.ReadFloat();
		uint removeMovementForcesCount = data.ReadUInt32();
		uint moveIndex = data.ReadUInt32();
		for (uint i = 0u; i < removeMovementForcesCount; i++)
		{
			data.ReadPackedGuid128();
		}
		if (!ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
		{
			Flags = data.ReadBits<uint>(30);
			FlagsExtra = data.ReadBits<uint>(18);
		}
		bool hasStandingOnGameObjectGUID = ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3) && data.HasBit();
		bool hasTransport = data.HasBit();
		bool hasFall = data.HasBit();
		bool hasSpline = data.HasBit();
		data.ReadBit(); // HeightChangeFailed
		data.ReadBit(); // RemoteTimeValid
		bool hasInertia = ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3) && data.HasBit();
		bool hasAdvFlying = ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3) && data.HasBit();
		if (hasTransport)
		{
			ReadTransportInfoModern(data);
		}
		if (hasStandingOnGameObjectGUID)
		{
			data.ReadPackedGuid128(); // StandingOnGameObjectGUID
		}
		if (hasInertia)
		{
			data.ReadPackedGuid128();
			data.ReadVector3();
			data.ReadUInt32();
		}
		if (hasAdvFlying)
		{
			data.ReadFloat(); // forwardVelocity
			data.ReadFloat(); // upVelocity
		}
		if (hasFall)
		{
			FallTime = data.ReadUInt32();
			JumpVerticalSpeed = data.ReadFloat();
			if (data.HasBit())
			{
				JumpSinAngle = data.ReadFloat();
				JumpCosAngle = data.ReadFloat();
				JumpHorizontalSpeed = data.ReadFloat();
			}
		}
	}

	public void ReadTransportInfoModern(WorldPacket data)
	{
		TransportGuid = data.ReadPackedGuid128();
		TransportOffset = data.ReadVector3();
		TransportOrientation = data.ReadFloat();
		TransportSeat = data.ReadInt8();
		TransportTime = data.ReadUInt32();
		bool hasPrevTime = data.HasBit();
		bool hasVehicleId = data.HasBit();
		if (hasPrevTime)
		{
			TransportTime2 = data.ReadUInt32();
		}
		if (hasVehicleId)
		{
			VehicleId = data.ReadUInt32();
		}
	}

	public void WriteMovementInfoModern(WorldPacket data, WowGuid128 guid)
	{
		bool hasFallDirection = Flags.HasAnyFlag(MovementFlagModern.Falling | MovementFlagModern.FallingFar);
		bool hasFall = hasFallDirection || FallTime != 0;
		data.WritePackedGuid128(guid);
		if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
		{
			data.WriteUInt32(Flags);
			data.WriteUInt32(FlagsExtra);
			data.WriteUInt32(FlagsExtra2);
		}
		data.WriteUInt32(MoveTime);
		data.WriteFloat(Position.X);
		data.WriteFloat(Position.Y);
		data.WriteFloat(Position.Z);
		data.WriteFloat(Orientation);
		data.WriteFloat(SwimPitch);
		data.WriteFloat(SplineElevation);
		data.WriteUInt32(0u);
		data.WriteUInt32(0u);
		if (!ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
		{
			data.WriteBits(Flags, 30);
			data.WriteBits(FlagsExtra, 18);
		}
		if (ModernVersion.ExpansionVersion >= 3)
		{
			data.WriteBit(bit: false);
			data.WriteBit(TransportGuid != null);
			data.WriteBit(hasFall);
			data.WriteBit(HasSplineData);
			data.WriteBit(bit: false);
			data.WriteBit(bit: false);
			data.WriteBit(bit: false);
			data.WriteBit(bit: false);
		}
		else
		{
			data.WriteBit(TransportGuid != null);
			data.WriteBit(hasFall);
			data.WriteBit(HasSplineData);
			data.WriteBit(bit: false);
			data.WriteBit(bit: false);
			if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
			{
				data.WriteBit(bit: false);
			}
		}
		data.FlushBits();
		if (TransportGuid != null)
		{
			WriteTransportInfoModern(data);
		}
		if (hasFall)
		{
			data.WriteUInt32(FallTime);
			data.WriteFloat(JumpVerticalSpeed);
			data.WriteBit(hasFallDirection);
			data.FlushBits();
			if (hasFallDirection)
			{
				data.WriteFloat(JumpSinAngle);
				data.WriteFloat(JumpCosAngle);
				data.WriteFloat(JumpHorizontalSpeed);
			}
		}
	}

	public void WriteTransportInfoModern(WorldPacket data)
	{
		bool hasPrevTime = false;
		bool hasVehicleId = VehicleId != 0;
		data.WritePackedGuid128(TransportGuid);
		data.WriteFloat(TransportOffset.X);
		data.WriteFloat(TransportOffset.Y);
		data.WriteFloat(TransportOffset.Z);
		data.WriteFloat(TransportOrientation);
		data.WriteInt8(TransportSeat);
		data.WriteUInt32(TransportTime);
		data.WriteBit(hasPrevTime);
		data.WriteBit(hasVehicleId);
		data.FlushBits();
		if (hasPrevTime)
		{
			data.WriteUInt32(0u);
		}
		if (hasVehicleId)
		{
			data.WriteUInt32(VehicleId);
		}
	}

	public static void ClampOrientation(ref float orientation)
	{
		while (orientation < 0f)
		{
			orientation += (float)Math.PI * 2f;
		}
		while (orientation > (float)Math.PI * 2f)
		{
			orientation -= (float)Math.PI * 2f;
		}
	}

	public void ValidateMovementInfo()
	{
		ClampOrientation(ref Orientation);
		ClampOrientation(ref TransportOrientation);
		Action<bool, MovementFlagModern> RemoveViolatingFlags = delegate(bool check, MovementFlagModern maskToRemove)
		{
			if (check)
			{
				Log.Print(LogType.Error, $"Violation of MovementFlags found ({check}). MovementFlags: {Flags}, MovementFlags2: {FlagsExtra}. Mask {maskToRemove} will be removed.", "World\\Objects\\MovementInfo.cs");
				RemoveMovementFlag(maskToRemove);
			}
		};
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.Root) && HasMovementFlag(MovementFlagModern.MaskMoving), MovementFlagModern.MaskMoving);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.Ascending) && HasMovementFlag(MovementFlagModern.Descending), MovementFlagModern.Ascending | MovementFlagModern.Descending);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.TurnLeft) && HasMovementFlag(MovementFlagModern.TurnRight), MovementFlagModern.TurnLeft | MovementFlagModern.TurnRight);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.StrafeLeft) && HasMovementFlag(MovementFlagModern.StrafeRight), MovementFlagModern.StrafeLeft | MovementFlagModern.StrafeRight);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.PitchUp) && HasMovementFlag(MovementFlagModern.PitchDown), MovementFlagModern.PitchUp | MovementFlagModern.PitchDown);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.Forward) && HasMovementFlag(MovementFlagModern.Backward), MovementFlagModern.Forward | MovementFlagModern.Backward);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.DisableGravity | MovementFlagModern.CanFly) && HasMovementFlag(MovementFlagModern.Falling), MovementFlagModern.Falling);
		RemoveViolatingFlags(HasMovementFlag(MovementFlagModern.SplineElevation) && MathFunctions.fuzzyEq(SplineElevation, 0f), MovementFlagModern.SplineElevation);
		if (MathFunctions.fuzzyNe(SplineElevation, 0f))
		{
			AddMovementFlag(MovementFlagModern.SplineElevation);
		}
	}
}
