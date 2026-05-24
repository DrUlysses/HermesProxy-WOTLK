using System;
using System.Runtime.InteropServices;

namespace HermesProxy.World.Client;

[StructLayout(LayoutKind.Explicit)]
public struct UpdateField : IEquatable<UpdateField>
{
	[FieldOffset(0)]
	public readonly uint UInt32Value;

	[FieldOffset(0)]
	public readonly int Int32Value;

	[FieldOffset(0)]
	public readonly float FloatValue;

	public UpdateField(uint val)
	{
		this = default;
		UInt32Value = val;
	}

	public UpdateField(int val)
	{
		this = default;
		Int32Value = val;
	}

	public UpdateField(float val)
	{
		this = default;
		FloatValue = val;
	}

	public override bool Equals(object? obj)
	{
		return obj is UpdateField field && Equals(field);
	}

	public bool Equals(UpdateField other)
	{
		if (UInt32Value == other.UInt32Value)
		{
			return true;
		}
		return Math.Abs(FloatValue - other.FloatValue) < float.Epsilon;
	}

	public static bool operator ==(UpdateField first, UpdateField other)
	{
		return first.Equals(other);
	}

	public static bool operator !=(UpdateField first, UpdateField other)
	{
		return !(first == other);
	}

	public override int GetHashCode()
	{
		return UInt32Value.GetHashCode();
	}
}
